using System;
using UnityEngine;

namespace GameWorkstore.Patterns
{
	// convenience methods for awaiting without access to an AsyncBehaviour
	public static class Await
	{
		public static AsyncBehaviour.UnityAwaiter NextUpdate() => UnityAsyncManager.Behaviour.NextUpdate();
		public static AsyncBehaviour.UnityAwaiter NextLateUpdate() => UnityAsyncManager.Behaviour.NextLateUpdate();
		public static AsyncBehaviour.UnityAwaiter NextFixedUpdate() => UnityAsyncManager.Behaviour.NextFixedUpdate();
		public static AsyncBehaviour.UnityAwaiter Updates(int framesToWait) => UnityAsyncManager.Behaviour.Updates(framesToWait);
		public static AsyncBehaviour.UnityAwaiter LateUpdates(int framesToWait) => UnityAsyncManager.Behaviour.LateUpdates(framesToWait);
		public static AsyncBehaviour.UnityAwaiter FixedUpdates(int stepsToWait) => UnityAsyncManager.Behaviour.FixedUpdates(stepsToWait);
		public static AsyncBehaviour.UnityAwaiter Seconds(float secondsToWait) => UnityAsyncManager.Behaviour.Seconds(secondsToWait);
		public static AsyncBehaviour.UnityAwaiter SecondsUnscaled(float secondsToWait) => UnityAsyncManager.Behaviour.SecondsUnscaled(secondsToWait);
		public static AsyncBehaviour.UnityAwaiter Until(Func<bool> condition) => UnityAsyncManager.Behaviour.Until(condition);
		public static AsyncBehaviour.UnityAwaiter While(Func<bool> condition) => UnityAsyncManager.Behaviour.While(condition);
		public static AsyncBehaviour.UnityAwaiter AsyncOp(AsyncOperation op) => UnityAsyncManager.Behaviour.AsyncOp(op);
		public static AsyncBehaviour.UnityAwaiter Custom(CustomYieldInstruction instruction) => UnityAsyncManager.Behaviour.Custom(instruction);
	}

	public static class AwaitExtensions
	{
		public static AsyncBehaviour.UnityAwaiter GetAwaiter(this AsyncOperation @this) => Await.AsyncOp(@this);
		public static AsyncBehaviour.UnityAwaiter GetAwaiter(this CustomYieldInstruction @this) => Await.Custom(@this);
	}
}