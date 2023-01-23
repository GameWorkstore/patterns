using System;
using GameWorkstore.Patterns;
using NSubstitute;
using NUnit.Framework;

public class SignalTest
{
    [Test]
    public void IsSubstituible()
    {
        var substituible = Substitute.For<ISignal>();

        substituible.Register(null);
        substituible.Unregister(null);
        substituible.Invoke();
        substituible.Empty();

        substituible.Received(1).Empty();
        substituible.Received(1).Invoke();
        substituible.Received(1).Register(Arg.Any<Action>());
        substituible.Received(1).Unregister(Arg.Any<Action>());
    }

    [Test]
    public void RegisteredFunctionIsCalled()
    {
        var signal = new Signal<int>();
        var hasCalled = false;
        void SomeFunction(int i)
        {
            hasCalled = true;
        }

        signal.Register(SomeFunction);
        signal.Invoke(1);
        Assert.True(hasCalled);
    }

    [Test]
    public void UnregisteredFunctionIsNotCalled()
    {
        var signal = new Signal<int>();
        var hasCalled = false;
        void SomeFunction(int i)
        {
            hasCalled = true;
        }

        signal.Register(SomeFunction);
        signal.Unregister(SomeFunction);
        signal.Invoke(1);
        Assert.False(hasCalled);
    }
}
