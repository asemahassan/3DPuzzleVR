using UnityEngine;
using Valve.VR;

namespace Wacki {

    public class ViveUILaserPointer : IUILaserPointer {

        public EVRButtonId button = EVRButtonId.k_EButton_SteamVR_Trigger;

        private SteamVR_TrackedObject _trackedObject;
        private bool _connected = false;
        private int _index;

        protected override void Initialize()
        {
            InitializeLaser();
            var trackedObject = GetComponent<SteamVR_TrackedObject>();

            if(trackedObject != null) {
                _index = (int)trackedObject.index;
                _connected = true;
            }
        }

        public void InitializeLaser()
        {
            base.Initialize();
            Debug.Log("Initialize");

        }

        public override bool ButtonDown()
        {
            if(!_connected)
                return false;

            var device = SteamVR_Controller.Input(_index);
            if(device != null) {
                var result = device.GetPressDown(button);
                return result;
            }

            return false;
        }

        public override bool ButtonUp()
        {
            if(!_connected)
                return false;

            var device = SteamVR_Controller.Input(_index);
            if(device != null)
                return device.GetPressUp(button);

            return false;
        }
        
        public override void OnEnterControl(GameObject control)
        {
            var device = SteamVR_Controller.Input(_index);
            Debug.Log("On enter object; " + control.name);
           // device.TriggerHapticPulse(1000);
        }

        public override void OnExitControl(GameObject control)
        {
            var device = SteamVR_Controller.Input(_index);
            Debug.Log("On exit object; " + control.name);
            // device.TriggerHapticPulse(600);
        }
    }

}