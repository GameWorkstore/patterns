namespace GameWorkstore.Patterns
{
    public class PlatformInputService : IService
    {
        public Signal<InputPlatform> OnPlatformChange = new Signal<InputPlatform>();

        public enum InputPlatform
        {
            MOBILE,
            CONSOLE,
            PC
        }

        public InputPlatform CurrentPlatform { get; private set; }

        public override void Preprocess()
        {
            CurrentPlatform = InputPlatform.PC;
        }

        public void UpdatePlatform(InputPlatform platform)
        {
            if (platform == CurrentPlatform) return;
            CurrentPlatform = platform;
            OnPlatformChange.Invoke(platform);
        }

        public override void Postprocess()
        {
        }
    }

}