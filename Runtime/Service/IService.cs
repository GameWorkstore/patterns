
namespace GameWorkstore.Patterns
{
    public abstract class IService
    {
        public abstract void Preprocess();
        public abstract void Postprocess();
        public virtual int Priority() { return 0; }
    }
}