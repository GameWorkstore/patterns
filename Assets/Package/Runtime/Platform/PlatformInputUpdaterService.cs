#if UNITY_INPUTSYSTEM_PRESENT
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameWorkstore.Patterns
{
    public class PlatformInputUpdaterService : IService
    {
        private PlatformInputService _platformService;
        private InputActionReference _inputActionPc;
        private InputActionReference _inputActionConsole;
        private bool _isInputSet = false;
        private InputActionReference _inputActionMobile;

        public override void Postprocess()
        {
        }

        public override void Preprocess()
        {
            _platformService = ServiceProvider.GetService<PlatformInputService>();
            ServiceProvider.GetService<EventService>().Update.Register(OnUpdate);
        }

        private void OnUpdate()
        {
            if (_inputActionMobile && _inputActionMobile.action.WasPerformedThisFrame())
            {
                _platformService.UpdatePlatform(PlatformInputService.InputPlatform.MOBILE);
                return;
            }
            if (_inputActionPc && _inputActionPc.action.WasPerformedThisFrame())
            {
                _platformService.UpdatePlatform(PlatformInputService.InputPlatform.CONSOLE);
                return;
            }
            if (_inputActionConsole && _inputActionConsole.action.WasPerformedThisFrame())
            {
                _platformService.UpdatePlatform(PlatformInputService.InputPlatform.PC);
                return;
            }
        }

        public void SetInputListener(
            InputActionReference mobile,
            InputActionReference pc,
            InputActionReference console
        )
        {
            _inputActionMobile = mobile;
            _inputActionMobile.action.Enable();
            _inputActionPc = pc;
            _inputActionPc.action.Enable();
            _inputActionConsole = console;
            _inputActionConsole.action.Enable();
            _isInputSet = true;
        }
    }
}
#endif