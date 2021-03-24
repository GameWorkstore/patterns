using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class EventService : IService
    {
        private EventServiceMonobehaviour _behaviour;

        public Signal Update;
        public Signal LateUpdate;
        public Signal ApplicationQuit;
        public Signal<bool> ApplicationFocus;
        public Queue<Action> ActionsPerFrame = new Queue<Action>();

        public override void Preprocess()
        {
            _behaviour = new GameObject("EventService").AddComponent<EventServiceMonobehaviour>();
            _behaviour.EventService = this;
            if (!Application.isPlaying) return;
            UnityEngine.Object.DontDestroyOnLoad(_behaviour.gameObject);
        }

        public override void Postprocess()
        {
            UnityEngine.Object.Destroy(_behaviour.gameObject);
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return _behaviour.StartCoroutine(coroutine);
        }

        public Coroutine QueueDelayedAction(Action action, float time)
        {
            return _behaviour.StartCoroutine(DelayedAction(action, time));
        }

        private IEnumerator DelayedAction(Action action, float time)
        {
            yield return new WaitForSecondsRealtime(time);
            action?.Invoke();
        }

        public void QueueAction(Action action)
        {
            ActionsPerFrame.Enqueue(action);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            _behaviour.StopCoroutine(coroutine);
        }

        internal void ExecuteUpdate()
        {
            Update.Invoke();
            if (ActionsPerFrame.Count > 0)
            {
                Action action = ActionsPerFrame.Dequeue();
                try
                {
                    action();
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message + ":\n" + e.StackTrace);
                }
            }
        }

        internal void ExecuteLateUpdate()
        {
            LateUpdate.Invoke();
        }

        internal void ExecuteApplicationQuit()
        {
            ApplicationQuit.Invoke();
        }

        internal void ExecuteApplicationFocus(bool focus)
        {
            ApplicationFocus.Invoke(focus);
        }
    }

    public class EventServiceMonobehaviour : MonoBehaviour
    {
        internal EventService EventService;

        public void Update()
        {
            EventService.ExecuteUpdate();
        }

        public void LateUpdate()
        {
            EventService.ExecuteLateUpdate();
        }

        public void OnApplicationFocus(bool focus)
        {
            EventService.ExecuteApplicationFocus(focus);
        }

        public void OnApplicationQuit()
        {
            EventService.ExecuteApplicationQuit();
        }

        public void OnDestroy()
        {
            ServiceProvider.ShutdownServices();
        }
    }
}