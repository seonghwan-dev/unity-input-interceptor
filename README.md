# unity input interceptor
This plugin allows your script to determine when to intercept a player's keyboard input on Windows.

## usage

```csharp
using Calci;

...

bool consumeEvent;

void OnEnable() {
    Interceptor.SetKeyDownCallback(OnKeyDown);
    Interceptor.SetKeyUpCallback(OnKeyUp);
            
    Interceptor.SetManagePressedKey(true);
    Interceptor.SetUnityKeyCodeMap(true);
    
    Interceptor.Hook();
}

void OnDisable() {
    Interceptor.Unhook();
}

bool OnKeyDown(KeyCode key)
{
    string txt = $"KeyDown: {key}";
    Debug.Log(txt);
    
    return consumeEvent;
}

bool OnKeyUp(KeyCode key)
{
    string txt = $"KeyUp: {key}";
    Debug.Log(txt);

    return consumeEvent;
}
```

## install
copy `Packages/com.seonghwan.windows.interceptor` into `Packages/`in your project.

