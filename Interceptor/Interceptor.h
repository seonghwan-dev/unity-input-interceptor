#pragma once

#define INTERCEPTOR_API extern "C" __declspec(dllexport)

using EVENT_HANDLER = bool(__stdcall* )(int vk);

INTERCEPTOR_API bool Install();
INTERCEPTOR_API void Uninstall();
INTERCEPTOR_API bool RegisterHandler(int eventType, EVENT_HANDLER handler);
INTERCEPTOR_API bool UnregisterHandler(int eventType);