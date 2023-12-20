using GameWorkstore.Patterns;
using NUnit.Framework;

public class PropertyTest
{
    [Test]
    public void SetPropertyForNonNullValues()
    {
        int targetValue = 0;
        void OnPropertyChanged(int v) => Assert.AreEqual(v, targetValue);
        var property = new Property<int>();

        //check if property triggers upon register
        property.Register(OnPropertyChanged);

        //check if property triggers upon set
        targetValue = 1;
        property.Set(targetValue);
    }

    internal class TestClass { public int internalV; }

    [Test]
    public void SetPropertyForNullableValues()
    {
        TestClass targetValue = null;
        void onPropertyChanged(TestClass v) => Assert.AreEqual(v, targetValue);
        var property = new PropertyNullable<TestClass>();

        //check if property triggers upon register
        property.Register(onPropertyChanged);

        //check if property triggers upon set
        targetValue = new TestClass();
        property.Set(targetValue);

        //check if property triggers upon set null
        targetValue = null;
        property.Set(targetValue);
    }

    [Test]
    public void ForceSync()
    {
        int targetValue = 1;
        void onPropertyChanged(TestClass v) => Assert.AreEqual(targetValue, v.internalV);
        var targetProperty = new TestClass() { internalV = 1 };
        var property = new PropertyNullable<TestClass>();
        property.Set(targetProperty);

        //check if property triggers upon register
        property.Register(onPropertyChanged);

        targetValue = 2;
        property.Value.internalV = 2;
        property.ForceSync();
    }
}
