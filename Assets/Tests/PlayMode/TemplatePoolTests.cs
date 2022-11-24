using GameWorkstore.Patterns;
using NUnit.Framework;
using UnityEngine;

public class TemplatePoolTests
{
    [Test]
    public void Instantiate()
    {
        var pool = new TemplatePool<Transform>();
        pool.Template = new GameObject().transform;
        Assert.AreEqual(0, pool.Count);

        pool.Instantiate();
        Assert.AreEqual(1, pool.Count);
    }

    [Test]
    public void Dispose()
    {
        var pool = new TemplatePool<Transform>();
        pool.Template = new GameObject().transform;
        Assert.AreEqual(0, pool.Count);

        var t0 = pool.Instantiate();
        var t1 = pool.Instantiate();
        Assert.AreEqual(2, pool.Count);

        pool.Dispose(t0);
        Assert.AreEqual(1, pool.Count);

        pool.Dispose(t1);
        Assert.AreEqual(0, pool.Count);
    }

    [Test]
    public void FastDispose()
    {
        var pool = new TemplatePool<Transform>();
        pool.Template = new GameObject().transform;
        Assert.AreEqual(0, pool.Count);

        var t0 = pool.FastInstantiate();
        var t1 = pool.FastInstantiate();
        pool.FastDispose(t0);
        Assert.AreEqual(1, pool.Available);

        pool.FastDispose(t1);
        Assert.AreEqual(2, pool.Available);
    }
}
