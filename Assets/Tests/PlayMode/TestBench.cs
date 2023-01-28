using System.Collections;
using System.Collections.Generic;
using GameWorkstore.Patterns;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

public abstract class AbstractService : IService
{
    public abstract void PlatformFunction();
}

public class ConcreteService : AbstractService
{
    public override void Postprocess()
    {
    }

    public override void Preprocess()
    {
    }

    public override void PlatformFunction()
    {
    }
}

public class TestBench
{
    [Test]
    public void GetConcreteService()
    {
        ServiceProvider.Shutdown();
        Assert.NotNull(ServiceProvider.GetService<ConcreteService>());
    }

    [Test]
    public void GetAbstractService()
    {
        ServiceProvider.Shutdown();
        var concrete = ServiceProvider.GetService<ConcreteService>();
        var abstracted = ServiceProvider.GetAbstract<AbstractService>();
        Assert.NotNull(concrete);
        Assert.NotNull(abstracted);
        Assert.AreEqual(concrete, abstracted);
    }

    [UnityTest]
    public IEnumerator FunctionCrashesDontPreventOtherFunctionsToExecute()
    {
        var evt = ServiceProvider.GetService<EventService>();
        int i = 0;
        //function works
        evt.QueueAction(() =>
        {
            i++;
        });
        //function works partially
        bool exploded = false;
        evt.QueueAction(() =>
        {
            i++;
            try
            {
                Gate gate = null;
                gate.Release();
            }
            catch
            {
                exploded = true;
            }
        });
        //function will not work at all
        evt.QueueAction(null);

        while (evt.ActionsPerFrame.Count > 0)
        {
            yield return null;
        }
        Assert.AreEqual(true, exploded);
        Assert.AreEqual(2, i);
    }

    [Test]
    public void HighSpeedArrayIsValidICollection()
    {
        var array = new HighSpeedArray<int>(10) { 1 };
        Assert.Contains(1, array);
    }

    [Test]
    public void HighSpeedArraySetCapacityAndCountIsInSync()
    {
        var value = new HighSpeedArray<int>(3);
        value.AddRange(new[] { 1, 2, 3, 4, 5 });
        value.SetCapacity(3);
        Assert.AreEqual(3, value.Capacity);
        Assert.AreEqual(3, value.Count);
    }
}
