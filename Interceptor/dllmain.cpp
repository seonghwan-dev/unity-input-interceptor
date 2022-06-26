#include "pch.h"
#include <Windows.h>
#include <iostream>
#include <shellapi.h>
#include "Interceptor.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
#if _DEBUG
        AllocConsole();

        FILE* file;
        freopen_s(&file, "CONIN$", "r", stdin);
        freopen_s(&file, "CONOUT$", "w", stdout);
        freopen_s(&file, "CONOUT$", "w", stderr);
#endif
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    default:
        break;
    }
    
    return TRUE;
}

HHOOK pHookProc;
bool bIsInstalled;

LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam);

bool Install()
{
    if (bIsInstalled)
    {
        return false;
    }

    const auto threadId = GetCurrentThreadId();

    if (threadId == 0) return false;
    if (pHookProc != nullptr) return false;

    const auto hModule = GetModuleHandle(nullptr);
    pHookProc = SetWindowsHookEx(WH_KEYBOARD, HookProc, hModule, threadId);

    return pHookProc != nullptr;
}

void Uninstall()
{
    if (pHookProc != nullptr)
    {
        UnhookWindowsHookEx(pHookProc);
    }

    pHookProc = nullptr;
    bIsInstalled = false;
}

bool RegisterHandler(int eventType, EVENT_HANDLER handler)
{
    return true;
}

LRESULT HookProc(int code, WPARAM wParam, LPARAM lParam)
{
    
    return TRUE;
}