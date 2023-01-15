#if UNITY_INPUTSYSTEM_PRESENT
using UnityEngine.InputSystem;

namespace GameWorkstore.Patterns
{
    public class PlatformInputUpdaterService : IService
    {
        private PlatformInputService _platformService;
        private InputActionReference _inputActionPc = null;
        private InputActionReference _inputActionConsole = null;
        private InputActionReference _inputActionMobile = null;

        public override void Postprocess()
        {
        }

        public override void Preprocess()
        {
            _platformService = ServiceProvider.GetService<PlatformInputService>();
        }

        public void SetInputListener(
            InputActionReference mobile,
            InputActionReference pc,
            InputActionReference console
        )
        {
            if (_inputActionMobile == null && mobile != null)
            {
                _inputActionMobile = mobile;
                _inputActionMobile.action.Enable();
                _inputActionMobile.action.performed += OnMobileInput;
            }
            if(_inputActionPc == null && pc != null)
            {
                _inputActionPc = pc;
                _inputActionPc.action.Enable();
                _inputActionPc.action.performed += OnPcInput;
            }
            if(_inputActionConsole == null && console != null)
            {
                _inputActionConsole = console;
                _inputActionConsole.action.Enable();
                _inputActionConsole.action.performed += OnConsoleInput;
            }
        }

        private void OnConsoleInput(InputAction.CallbackContext obj)
        {
            _platformService.UpdatePlatform(PlatformInputService.InputPlatform.PC);
        }

        private void OnPcInput(InputAction.CallbackContext obj)
        {
            _platformService.UpdatePlatform(PlatformInputService.InputPlatform.CONSOLE);
        }

        private void OnMobileInput(InputAction.CallbackContext contect)
        {
            _platformService.UpdatePlatform(PlatformInputService.InputPlatform.MOBILE);
        }
    }
}
#endif