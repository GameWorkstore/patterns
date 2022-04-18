using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class EventService : IService
    {
        private EventServiceMonobehaviour _behaviour;

        public Signal Update { get; } = new Signal();
        public Signal LateUpdate { get; } = new Signal();
        public Signal ApplicationQuit { get; } = new Signal();
        public Signal FixedUpdate { get; } = new Signal();
        public Signal<bool> ApplicationFocus { get; } = new Signal<bool>();
        public Queue<Action> ActionsPerFrame { get; } = new Queue<Action>();

        public bool IsExitApplicationStarted = false;

        public override void Preprocess()
        {
            if (IsExitApplicationStarted) return;
            _behaviour = new GameObject("EventService").AddComponent<EventServiceMonobehaviour>();
            _behaviour.EventService = this;
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
            if (ActionsPerFrame.Count <= 0) return;
            var action = ActionsPerFrame.Dequeue();
            action?.Invoke();
        }

        internal void ExecuteLateUpdate()
        {
            LateUpdate.Invoke();
        }

        internal void ExecuteFixedUpdate()
        {
            FixedUpdate.Invoke();
        }

        internal void ExecuteApplicationQuit()
        {
            ApplicationQuit.Invoke();
            IsExitApplicationStarted = true;
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

		public void FixedUpdate()
		{
            EventService.ExecuteFixedUpdate();
		}

		public void OnDestroy()
        {
            ServiceProvider.Shutdown();
        }
    }
}