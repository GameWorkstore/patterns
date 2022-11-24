using System;
using System.Collections.Generic;
using GameWorkstore.Patterns;
using NSubstitute;
using NUnit.Framework;

public class HighSpeedArrayTests
{
    [Test]
    public void Add()
    {
        var array = new HighSpeedArray<int>(4);
        Assert.AreEqual(0, array.Count);

        array.Add(1);
        Assert.AreEqual(1, array.Count);
    }

    [Test]
    public void RemoveAt()
    {
        var array = new HighSpeedArray<int>(4);
        array.Add(1);
        array.Add(2);
        array.Add(3);
        array.Add(4);
        Assert.AreEqual(4, array.Count);

        array.RemoveAt(1);

        Assert.AreEqual(4, array[1]);
    }

    [Test]
    public void Remove()
    {
        var array = new HighSpeedArray<int>(4);
        array.Add(1);
        array.Add(2);
        array.Add(3);
        array.Add(4);
        Assert.AreEqual(4, array.Count);

        array.Remove(2);

        Assert.AreEqual(4, array[1]);
    }

    [Test]
    public void RemoveAll()
    {
        var array = new HighSpeedArray<int>(4);
        array.Add(1);
        array.Add(2);
        array.Add(2);
        array.Add(2);
        array.Add(3);
        array.Add(4);
        Assert.AreEqual(6, array.Count);

        array.RemoveAll(2);

        Assert.AreEqual(3, array.Count);
        Assert.AreEqual(4, array[1]);
        Assert.AreEqual(3, array[2]);
    }

    public class ComparerInt : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

    [Test]
    public void Sort()
    {
        var array = new HighSpeedArray<int>(4);
        array.Add(4);
        array.Add(1);
        array.Add(2);
        array.Add(3);
        Assert.AreEqual(4, array.Count);

        array.Sort(new ComparerInt());

        Assert.AreEqual(1, array[0]);
        Assert.AreEqual(2, array[1]);
        Assert.AreEqual(3, array[2]);
        Assert.AreEqual(4, array[3]);
    }

    [Test]
    public void IEnumerator()
    {
        var array = new HighSpeedArray<int>(4);
        array.Add(0);
        array.Add(1);
        array.Add(2);
        array.Add(3);
        Assert.AreEqual(4, array.Count);

        int i = 0;
        foreach(var t in array)
        {
            Assert.AreEqual(i, t);
            i++;
        }
    }
}
