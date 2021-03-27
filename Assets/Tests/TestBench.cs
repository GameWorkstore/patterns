using System.Collections;
using System.Collections.Generic;
using GameWorkstore.Patterns;
using NUnit.Framework;
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
        Assert.AreEqual(concrete,abstracted);
    }
}
