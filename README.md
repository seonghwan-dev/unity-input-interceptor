# unity input interceptor
This plugin allows your script to determine when to intercept a player's keyboard input on Windows.

## usage

```csharp
using Calci;

...

bool consumeEvent;

void OnEnable() {
    
    IKeyCodeHandler handler = new MyKeyCodeHandler();
    
    Interceptor.SetKeyCodeHandler(handler);
    Interceptor.SetManagePressedKey(true);
    
    Interceptor.SetKeyDownCallback(OnKeyDown);
    Interceptor.SetKeyUpCallback(OnKeyUp);
    
    bool result = Interceptor.Hook();
}

void OnDisable() {
    bool result = Interceptor.Unhook();
}

bool OnKeyDown(int keyCode)
{
    KeyCode key = (KeyCode)keyCode;
    string txt = $"KeyDown: {key}";

    Debug.Log(txt);

    return consumeEvent;
}

bool OnKeyUp(int keyCode)
{
    KeyCode key = (KeyCode)keyCode;
    string txt = $"KeyUp: {key}";

    Debug.Log(txt);

    return consumeEvent;
}
```

## install
copy `Packages/com.seonghwan.windows.interceptor` into `Packages/`in your project.

