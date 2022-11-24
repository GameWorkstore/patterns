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
}
