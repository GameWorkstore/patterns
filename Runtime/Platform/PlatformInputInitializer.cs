#if UNITY_INPUTSYSTEM_PRESENT
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameWorkstore.Patterns
{
    public class PlatformInputInitializer : MonoBehaviour
    {
        public InputActionReference MobileInputObserver;
        public InputActionReference PcInputObserver;
        public InputActionReference ConsoleInputObserver;

        public void Awake()
        {
            ServiceProvider.GetService<PlatformInputUpdaterService>().SetInputListener(
                MobileInputObserver,
                PcInputObserver,
                ConsoleInputObserver
            );
        }
    }
}
#endif