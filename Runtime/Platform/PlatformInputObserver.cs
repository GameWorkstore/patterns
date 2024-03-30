using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class PlatformInputObserver : MonoBehaviour
    {
        [Header("Mobile")]
        public GameObject[] Mobile;
        [Header("Console")]
        public GameObject[] Console;
        [Header("Computer")]
        public GameObject[] Computer;

        private PlatformInputService _platformService;

        public void Awake()
        {
            _platformService = ServiceProvider.GetService<PlatformInputService>();
            _platformService.OnPlatformChange.Register(OnPlatformChange);
            UpdateVisuals(_platformService.CurrentPlatform);
        }

        private void OnPlatformChange(PlatformInputService.InputPlatform platform)
        {
            UpdateVisuals(platform);
        }

        public void OnDestroy()
        {
            _platformService.OnPlatformChange.Unregister(OnPlatformChange);
        }

        public void UpdateVisuals(PlatformInputService.InputPlatform platform)
        {
            bool mobile = platform == PlatformInputService.InputPlatform.MOBILE;
            bool console = platform == PlatformInputService.InputPlatform.CONSOLE;
            bool pc = platform == PlatformInputService.InputPlatform.PC;

            foreach (var g in Mobile) g.SetActive(mobile);
            foreach (var g in Console) g.SetActive(console);
            foreach (var g in Computer) g.SetActive(pc);
        }
    }
}