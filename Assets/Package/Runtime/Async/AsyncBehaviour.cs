using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public interface IAsyncBehaviour
    {
        int UpdateIndex { get; set; }
        int LateUpdateIndex { get; set; }
        int FixedUpdateIndex { get; set; }

        bool Update();
        bool LateUpdate();
        bool FixedUpdate();
    }

    public class AsyncBehaviour : MonoBehaviour, IAsyncBehaviour
    {
        private abstract class ContinuationQueue
        {
            public abstract void Update();
            public abstract bool IsEmpty { get; }
        }

        private class ContinuationQueue<T> : ContinuationQueue where T : IContinuation
        {
            private static void RemoveWhileIterating(List<T> list, ref int index)
            {
                int count = list.Count;

                if (count > 1)
                {
                    list[index] = list[count - 1];
                    list.RemoveAt(count - 1);
                    --index; // decrement index so swapped item is next
                }
                else
                    list.RemoveAt(index);
            }

            protected readonly List<T> buffer;

            public ContinuationQueue(int capacity)
            {
                buffer = new List<T>(capacity);
            }

            public override void Update()
            {
                for (int i = 0; i < buffer.Count; ++i)
                {
                    var cont = buffer[i];

                    if (cont.IsCompleted())
                        lock (buffer)
                            RemoveWhileIterating(buffer, ref i); // remove without rearranging list
                    else
                        buffer[i] = cont; // reassign as we are dealing with value type and IsCompleted has side effects
                }
            }

            public override bool IsEmpty => buffer.Count == 0;

            public void Add(T c)
            {
                lock (buffer)
                    buffer.Add(c);
            }
        }

        public abstract class UnityAwaiter : INotifyCompletion
        {
            public abstract bool IsCompleted { get; }
            public abstract void OnCompleted(Action continuation);
            public void GetResult() { }
            public UnityAwaiter GetAwaiter() => this;
        }

        private class UnityAwaiter<T> : UnityAwaiter where T : IContinuation
        {
            readonly ContinuationQueue<T> buffer;
            public UnityAwaiter(ContinuationQueue<T> buffer) { this.buffer = buffer; }
            public UnityAwaiter(ContinuationQueue buffer) { this.buffer = buffer as ContinuationQueue<T>; }

            public override bool IsCompleted => false;
            public override void OnCompleted(Action continuation)
            {
                var c = continuations.Dequeue();
                c.Set(continuation);
                buffer.Add(c);
            }

            public void AddContinuation(T continuation) => continuations.Enqueue(continuation);
            protected Queue<T> continuations = new Queue<T>(128);
        }

        interface IContinuation
        {
            void Set(Action continuation);
            bool IsCompleted();
        }

        struct FrameContinuation : IContinuation
        {
            int waitForFrame;
            Action continuation;

            public FrameContinuation(int waitForFrame)
            {
                this.waitForFrame = waitForFrame;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (UnityAsyncManager.FrameCount >= waitForFrame)
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct FixedContinuation : IContinuation
        {
            uint waitForStep;
            Action continuation;

            public FixedContinuation(uint waitForStep)
            {
                this.waitForStep = waitForStep;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (UnityAsyncManager.FixedStepCount >= waitForStep)
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct TimeContinuation : IContinuation
        {
            float waitForTime;
            Action continuation;

            public TimeContinuation(float waitForTime)
            {
                this.waitForTime = waitForTime;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (UnityAsyncManager.Time >= waitForTime)
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct CustomContinuation : IContinuation
        {
            CustomYieldInstruction operation;
            Action continuation;

            public CustomContinuation(CustomYieldInstruction operation)
            {
                this.operation = operation;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (!operation.MoveNext())
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct AsyncOpContinuation : IContinuation
        {
            AsyncOperation operation;
            Action continuation;

            public AsyncOpContinuation(AsyncOperation operation)
            {
                this.operation = operation;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (operation == null || operation != null && operation.isDone)
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct UnscaledTimeContinuation : IContinuation
        {
            float waitForTime;
            Action continuation;

            public UnscaledTimeContinuation(float waitForTime)
            {
                this.waitForTime = waitForTime;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (UnityAsyncManager.UnscaledTime >= waitForTime)
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        struct ConditionContinuation : IContinuation
        {
            Func<bool> condition;
            Action continuation;

            public ConditionContinuation(Func<bool> condition)
            {
                this.condition = condition;
                continuation = null;
            }

            public void Set(Action cont) => continuation = cont;
            public bool IsCompleted()
            {
                if (condition())
                {
                    continuation();
                    return true;
                }

                return false;
            }
        }

        int IAsyncBehaviour.UpdateIndex { get; set; }
        int IAsyncBehaviour.LateUpdateIndex { get; set; }
        int IAsyncBehaviour.FixedUpdateIndex { get; set; }

        bool hasUpdates, hasLateUpdates, hasFixedUpdates;

        UnityAwaiter<FrameContinuation> updateAwaiter;
        UnityAwaiter<FixedContinuation> fixedUpdateAwaiter;
        UnityAwaiter<FrameContinuation> lateUpdateAwaiter;
        UnityAwaiter<ConditionContinuation> conditionAwaiter;
        UnityAwaiter<UnscaledTimeContinuation> unscaledTimeAwaiter;
        UnityAwaiter<TimeContinuation> timeAwaiter;
        UnityAwaiter<CustomContinuation> customAwaiter;
        UnityAwaiter<AsyncOpContinuation> asyncOpAwaiter;

        List<ContinuationQueue> updateQueues;
        List<ContinuationQueue> lateUpdateQueues;
        List<ContinuationQueue> fixedUpdateQueues;

        protected virtual void Awake()
        {
            updateQueues = new List<ContinuationQueue>
            {
                new ContinuationQueue<FrameContinuation>(128),
                new ContinuationQueue<TimeContinuation>(64),
                new ContinuationQueue<UnscaledTimeContinuation>(32),
                new ContinuationQueue<ConditionContinuation>(32),
                new ContinuationQueue<CustomContinuation>(16),
                new ContinuationQueue<AsyncOpContinuation>(16)
            };

            lateUpdateQueues = new List<ContinuationQueue> { new ContinuationQueue<FrameContinuation>(32) };
            fixedUpdateQueues = new List<ContinuationQueue> { new ContinuationQueue<FixedContinuation>(32) };

            updateAwaiter = new UnityAwaiter<FrameContinuation>(updateQueues[0]);
            lateUpdateAwaiter = new UnityAwaiter<FrameContinuation>(lateUpdateQueues[0]);
            fixedUpdateAwaiter = new UnityAwaiter<FixedContinuation>(fixedUpdateQueues[0]);

            timeAwaiter = new UnityAwaiter<TimeContinuation>(updateQueues[1]);
            unscaledTimeAwaiter = new UnityAwaiter<UnscaledTimeContinuation>(updateQueues[2]);

            conditionAwaiter = new UnityAwaiter<ConditionContinuation>(updateQueues[3]);
            customAwaiter = new UnityAwaiter<CustomContinuation>(updateQueues[4]);
            asyncOpAwaiter = new UnityAwaiter<AsyncOpContinuation>(updateQueues[5]);
        }

        bool IAsyncBehaviour.Update()
        {
            bool notEmpty = false;

            foreach (var q in updateQueues)
            {
                if (q.IsEmpty)
                    continue;

                q.Update();
                notEmpty = true;
            }

            hasUpdates = notEmpty;

            return notEmpty;
        }

        bool IAsyncBehaviour.LateUpdate()
        {
            bool notEmpty = false;

            foreach (var q in lateUpdateQueues)
            {
                if (q.IsEmpty)
                    continue;

                q.Update();
                notEmpty = true;
            }

            hasLateUpdates = notEmpty;

            return notEmpty;
        }

        bool IAsyncBehaviour.FixedUpdate()
        {
            bool notEmpty = false;

            foreach (var q in fixedUpdateQueues)
            {
                if (q.IsEmpty)
                    continue;

                q.Update();
                notEmpty = true;
            }

            hasFixedUpdates = notEmpty;

            return notEmpty;
        }

        public UnityAwaiter NextUpdate()
        {
            RegisterUpdates();

            updateAwaiter.AddContinuation(new FrameContinuation(UnityAsyncManager.FrameCount + 1));
            return updateAwaiter;
        }

        public UnityAwaiter NextLateUpdate()
        {
            RegisterLateUpdates();

            lateUpdateAwaiter.AddContinuation(new FrameContinuation(UnityAsyncManager.FrameCount + 1));
            return lateUpdateAwaiter;
        }

        public UnityAwaiter NextFixedUpdate()
        {
            RegisterFixedUpdates();

            fixedUpdateAwaiter.AddContinuation(new FixedContinuation(UnityAsyncManager.FixedStepCount + 1));
            return fixedUpdateAwaiter;
        }

        public UnityAwaiter Updates(int framesToWait)
        {
            RegisterUpdates();

            updateAwaiter.AddContinuation(new FrameContinuation(UnityAsyncManager.FrameCount + framesToWait));
            return updateAwaiter;
        }

        public UnityAwaiter LateUpdates(int framesToWait)
        {
            RegisterLateUpdates();

            lateUpdateAwaiter.AddContinuation(new FrameContinuation(UnityAsyncManager.FrameCount + framesToWait));
            return lateUpdateAwaiter;
        }

        public UnityAwaiter FixedUpdates(int stepsToWait)
        {
            RegisterFixedUpdates();

            fixedUpdateAwaiter.AddContinuation(new FixedContinuation(UnityAsyncManager.FixedStepCount + (uint)stepsToWait));
            return fixedUpdateAwaiter;
        }

        public UnityAwaiter Seconds(float secondsToWait)
        {
            RegisterUpdates();

            timeAwaiter.AddContinuation(new TimeContinuation(UnityAsyncManager.Time + secondsToWait));
            return timeAwaiter;
        }

        public UnityAwaiter SecondsUnscaled(float secondsToWait)
        {
            RegisterUpdates();

            unscaledTimeAwaiter.AddContinuation(new UnscaledTimeContinuation(UnityAsyncManager.UnscaledTime + secondsToWait));
            return unscaledTimeAwaiter;
        }

        public UnityAwaiter Until(Func<bool> condition)
        {
            RegisterUpdates();

            conditionAwaiter.AddContinuation(new ConditionContinuation(condition));
            return conditionAwaiter;
        }

        // please just use WaitUntil...
        public UnityAwaiter While(Func<bool> condition)
        {
            RegisterUpdates();

            // eek!
            conditionAwaiter.AddContinuation(new ConditionContinuation(() => !condition()));
            return conditionAwaiter;
        }

        public UnityAwaiter Custom(CustomYieldInstruction instruction)
        {
            RegisterUpdates();

            customAwaiter.AddContinuation(new CustomContinuation(instruction));
            return customAwaiter;
        }

        public UnityAwaiter AsyncOp(AsyncOperation op)
        {
            RegisterUpdates();

            asyncOpAwaiter.AddContinuation(new AsyncOpContinuation(op));
            return asyncOpAwaiter;
        }

        void RegisterUpdates()
        {
            if (hasUpdates)
                return;

            hasUpdates = true;
            UnityAsyncManager.RegisterUpdate(this);
        }

        void RegisterLateUpdates()
        {
            if (hasLateUpdates)
                return;

            hasLateUpdates = true;
            UnityAsyncManager.RegisterLateUpdate(this);
        }

        void RegisterFixedUpdates()
        {
            if (hasFixedUpdates)
                return;

            hasFixedUpdates = true;
            UnityAsyncManager.RegisterFixedUpdate(this);
        }

        protected virtual void OnEnable()
        {
            if (hasUpdates)
                UnityAsyncManager.RegisterUpdate(this);

            if (hasLateUpdates)
                UnityAsyncManager.RegisterLateUpdate(this);

            if (hasFixedUpdates)
                UnityAsyncManager.RegisterFixedUpdate(this);
        }

        protected virtual void OnDisable()
        {
            if (hasUpdates)
                UnityAsyncManager.UnregisterUpdate(this);

            if (hasLateUpdates)
                UnityAsyncManager.UnregisterLateUpdate(this);

            if (hasFixedUpdates)
                UnityAsyncManager.UnregisterFixedUpdate(this);
        }
    }
}