using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    // manages all AsyncBehaviours including low overhead update calls
    public class UnityAsyncManager : MonoBehaviour
    {
        public static int FrameCount;
        public static uint FixedStepCount;
        public static float Time;
        public static float UnscaledTime;

        public static AsyncBehaviour Behaviour { get; private set; }

        private static List<IAsyncBehaviour> _updates;
        private static List<IAsyncBehaviour> _lateUpdates;
        private static List<IAsyncBehaviour> _fixedUpdates;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            var asyncManager = new GameObject("AsyncManager").AddComponent<UnityAsyncManager>();
            asyncManager.gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(asyncManager.gameObject);

            FrameCount = 1;
            FixedStepCount = 1;
            Time = UnityEngine.Time.time;
            UnscaledTime = UnityEngine.Time.unscaledTime;

            _updates = new List<IAsyncBehaviour>(128);
            _lateUpdates = new List<IAsyncBehaviour>(32);
            _fixedUpdates = new List<IAsyncBehaviour>(32);

            Behaviour = asyncManager.gameObject.AddComponent<AsyncBehaviour>();
        }

        public static void RegisterUpdate(IAsyncBehaviour b)
        {
            b.UpdateIndex = _updates.Count;
            _updates.Add(b);
        }

        public static void RegisterLateUpdate(IAsyncBehaviour b)
        {
            b.LateUpdateIndex = _lateUpdates.Count;
            _lateUpdates.Add(b);
        }

        public static void RegisterFixedUpdate(IAsyncBehaviour b)
        {
            b.FixedUpdateIndex = _fixedUpdates.Count;
            _fixedUpdates.Add(b);
        }

        public static void UnregisterUpdate(IAsyncBehaviour b)
        {
            int count = _updates.Count;
            int i = b.UpdateIndex;

            if (count > 1)
            {
                var toSwap = _updates[count - 1];
                _updates[i] = toSwap;
                toSwap.UpdateIndex = i;
                _updates.RemoveAt(count - 1);
            }
            else
            {
                _updates.RemoveAt(i);
            }
        }

        public static void UnregisterLateUpdate(IAsyncBehaviour b)
        {
            int count = _fixedUpdates.Count;
            int i = b.LateUpdateIndex;

            if (count > 1)
            {
                var toSwap = _lateUpdates[count - 1];
                _lateUpdates[i] = toSwap;
                toSwap.LateUpdateIndex = i;
                _lateUpdates.RemoveAt(count - 1);
            }
            else
            {
                _lateUpdates.RemoveAt(i);
            }
        }

        public static void UnregisterFixedUpdate(IAsyncBehaviour b)
        {
            int count = _fixedUpdates.Count;
            int i = b.FixedUpdateIndex;

            if (count > 1)
            {
                var toSwap = _fixedUpdates[count - 1];
                _fixedUpdates[i] = toSwap;
                toSwap.FixedUpdateIndex = i;
                _fixedUpdates.RemoveAt(count - 1);
            }
            else
            {
                _fixedUpdates.RemoveAt(i);
            }
        }

        void Update()
        {
            // = Time.frameCount;
            Time = UnityEngine.Time.time;
            UnscaledTime = UnityEngine.Time.unscaledTime;

            for (int i = 0; i < _updates.Count; ++i)
            {
                if (!_updates[i].Update())
                {
                    int count = _updates.Count;

                    if (count > 1)
                    {
                        var toSwap = _updates[count - 1];
                        _updates[i] = toSwap;
                        toSwap.UpdateIndex = i;
                        _updates.RemoveAt(count - 1);
                        --i;
                    }
                    else
                    {
                        _updates.RemoveAt(i);
                    }
                }
            }

            ++FrameCount;
        }

        void LateUpdate()
        {
            for (int i = 0; i < _lateUpdates.Count; ++i)
            {
                if (!_lateUpdates[i].LateUpdate())
                {
                    int count = _lateUpdates.Count;

                    if (count > 1)
                    {
                        var toSwap = _lateUpdates[count - 1];
                        _lateUpdates[i] = toSwap;
                        toSwap.LateUpdateIndex = i;
                        _lateUpdates.RemoveAt(count - 1);
                        --i;
                    }
                    else
                    {
                        _lateUpdates.RemoveAt(i);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            for (int i = 0; i < _fixedUpdates.Count; ++i)
            {
                if (!_fixedUpdates[i].FixedUpdate())
                {
                    int count = _fixedUpdates.Count;

                    if (count > 1)
                    {
                        var toSwap = _fixedUpdates[count - 1];
                        _fixedUpdates[i] = toSwap;
                        toSwap.FixedUpdateIndex = i;
                        _fixedUpdates.RemoveAt(count - 1);
                        --i;
                    }
                    else
                    {
                        _fixedUpdates.RemoveAt(i);
                    }
                }
            }

            ++FixedStepCount;
        }
    }
}