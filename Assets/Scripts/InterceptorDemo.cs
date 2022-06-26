using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Calci;
using TMPro;
using UnityEngine.UI;

public class InterceptorDemo : MonoBehaviour
{
    public bool consumeEvent = true;
    public bool managedPressedKey = true;
    public bool useUnityKeyCodeMap = true;

    private List<KeyCode> keyCodes;

    public TextMeshProUGUI native;
    public TextMeshProUGUI unity;
    public Button button;

    public Toggle consumeToggle;
    public Toggle managedPressedKeyToggle;
    public Toggle keyCodeMapToggle;

    private void OnEnable()
    {
        // set default values
        consumeToggle.SetIsOnWithoutNotify(consumeEvent);
        managedPressedKeyToggle.SetIsOnWithoutNotify(managedPressedKey);
        keyCodeMapToggle.SetIsOnWithoutNotify(useUnityKeyCodeMap);
        
        // bind callbacks
        consumeToggle.onValueChanged.AddListener(b =>
        {
            consumeEvent = b;
        });
        
        managedPressedKeyToggle.onValueChanged.AddListener(b =>
        {
            managedPressedKey = b;
        });
        
        keyCodeMapToggle.onValueChanged.AddListener(b =>
        {
            useUnityKeyCodeMap = b;
        });
        
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

        Interceptor.SetKeyDownCallback(OnKeyDown);
        Interceptor.SetKeyUpCallback(OnKeyUp);
        
        Interceptor.SetManagePressedKey(managedPressedKey);
        Interceptor.SetUnityKeyCodeMap(useUnityKeyCodeMap);

        Interceptor.Hook();
    }

    private void OnDisable()
    {
        Interceptor.Unhook();
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

    private bool OnKeyDown(KeyCode key)
    {
        string txt = $"KeyDown: {key}";
        Debug.Log(txt);
        
        native.text = txt;
        
        return consumeEvent;
    }

    private bool OnKeyUp(KeyCode key)
    {
        string txt = $"KeyUp: {key}";
        Debug.Log(txt);

        // native.text = txt;
        
        return consumeEvent;
    }
}