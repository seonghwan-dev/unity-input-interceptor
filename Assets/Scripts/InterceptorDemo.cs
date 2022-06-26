using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Calci.Demo
{
    public class InterceptorDemo : MonoBehaviour
    {
        public bool consumeEvent = true;
        public bool managedPressedKey = true;

        private List<KeyCode> keyCodes;

        public TextMeshProUGUI native;
        public TextMeshProUGUI unity;
        public Button button;

        public Toggle consumeToggle;
        public Toggle managedPressedKeyToggle;

        private void OnEnable()
        {
            // set default values
            consumeToggle.SetIsOnWithoutNotify(consumeEvent);
            managedPressedKeyToggle.SetIsOnWithoutNotify(managedPressedKey);

            // bind callbacks
            consumeToggle.onValueChanged.AddListener(b => { consumeEvent = b; });

            managedPressedKeyToggle.onValueChanged.AddListener(b => { managedPressedKey = b; });

            button.onClick.AddListener(() =>
            {
                unity.text = "button clicked";
                native.text = "button clicked";
            });

            // set up details
            if (keyCodes == null || keyCodes.Count < 1)
            {
                keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
            }

            IKeyCodeHandler handler = new MyKeyCodeHandler();

            Interceptor.SetKeyDownCallback(OnKeyDown);
            Interceptor.SetKeyUpCallback(OnKeyUp);

            Interceptor.SetManagePressedKey(managedPressedKey);
            Interceptor.SetKeyCodeHandler(handler);

            bool success = Interceptor.Hook();
            if (success)
            {
                Debug.Log("[Interceptor] Successfully hooks <color=green>installed</color>. ");
            }
        }

        private void OnDisable()
        {
            bool success = Interceptor.Unhook();
            if (success)
            {
                Debug.Log("[Interceptor] Successfully hooks <color=yellow>uninstalled</color>. ");
            }
        }

        private void Update()
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (keyCode.ToString().Contains("Mouse")) continue;

                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log("<color=red>KeyDown</color> - " + keyCode);

                    unity.text = "KeyDown: " + keyCode;
                }
            }
        }

        private bool OnKeyDown(int keyCode)
        {
            KeyCode key = (KeyCode)keyCode;
            string txt = $"KeyDown: {key}";
            Debug.Log(txt);

            native.text = txt;

            return consumeEvent;
        }

        private bool OnKeyUp(int keyCode)
        {
            KeyCode key = (KeyCode)keyCode;
            string txt = $"KeyUp: {key}";
            Debug.Log(txt);

            return consumeEvent;
        }

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            Interceptor.Reset();
        }
#endif
    }

    public class MyKeyCodeHandler : DefaultKeyCodeHandler
    {
        public override int Handle(ushort VirtualKey)
        {
            // respect parents implementation
            int result = base.Handle(VirtualKey);
            if (result > 0) return result;
            
            // my codes
            if (VirtualKey == 0x1B)
            {
                return (int)KeyCode.Escape;
            }
            
            return 0;
        }
    }
}