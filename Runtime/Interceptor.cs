using System;
using System.Collections.Generic;
using Windows.Utils;
using UnityEngine;

namespace Calci
{
    using Windows.API;
    using Windows.Enums;
    
    public static class Interceptor
    {
        /// <summary>
        /// WindowsHook을 등록합니다. 
        /// </summary>
        /// <returns></returns>
        public static bool Hook()
        {
            if (bIsHooked) return false;

            int currentThreadId = (int)Kernel32.GetCurrentThreadId();
            if (currentThreadId == 0) return false;
            
            if (pWindowsHook == IntPtr.Zero)
            {
                if (bUseKeyMap)
                {
                    unityKeyMap.Clear();
                    VirtualKeyMapping.SetMappings(unityKeyMap);
                }
                
                pWindowsHook = User32.SetWindowsHookEx(
                    EHookId.WH_KEYBOARD,
                    HookProc,
                    IntPtr.Zero,
                    currentThreadId
                );
                
#if UNITY_EDITOR
                Debug.Log("[Interceptor] Successfully hooks <color=green>installed</color>. ");
#endif
            }

            return pWindowsHook != IntPtr.Zero;
        }

        /// <summary>
        /// WindowsHook을 해제합니다.
        /// </summary>
        public static void Unhook()
        {
            if (pWindowsHook == IntPtr.Zero) return;

            User32.UnhookWindowsHookEx(pWindowsHook);
            pWindowsHook = IntPtr.Zero;

            keys.Clear();
            cachedConsumePolicy.Clear();
            unityKeyMap.Clear();
            
            bIsHooked = false;
            
#if UNITY_EDITOR
            Debug.Log("[Interceptor] Successfully hooks <color=yellow>uninstalled</color>. ");
#endif
        }

        public static void SetKeyDownCallback(Func<KeyCode, bool> keyDown)
        {
            OnKeyDown = keyDown;
        }

        public static void SetKeyUpCallback(Func<KeyCode, bool> keyUp)
        {
            OnKeyUp = keyUp;
        }

        public static void SetManagePressedKey(bool manage)
        {
            bManagePressedKey = manage;
        }

        public static void SetUnityKeyCodeMap(bool useKeyMap)
        {
            bUseKeyMap = useKeyMap;
        }
        
        #region Internal Fields 
        
        private static bool bIsHooked = false;
        private static bool bManagePressedKey = false;
        private static bool bUseKeyMap = false;
        
        private static IntPtr pWindowsHook = IntPtr.Zero;

        private static Func<KeyCode, bool> OnKeyDown;
        private static Func<KeyCode, bool> OnKeyUp;

        private static readonly HashSet<ushort> keys = new HashSet<ushort>(8);
        private static readonly Dictionary<ushort, bool> cachedConsumePolicy = new Dictionary<ushort, bool>(8);
        
        private static readonly Dictionary<ushort, KeyCode> unityKeyMap = new Dictionary<ushort, KeyCode>();

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            bIsHooked = false;
            bManagePressedKey = false;
            
            pWindowsHook = IntPtr.Zero;
            
            OnKeyDown = null;
            OnKeyUp = null;
            
            keys.Clear();
            cachedConsumePolicy.Clear();
            unityKeyMap.Clear();
            
            Debug.Log("[Interceptor] SubsystemRegistration - Cleaned up! ");
        }
#endif
        
        #endregion
        
        #region Implementations
        
        [AOT.MonoPInvokeCallback(typeof(User32.HOOKPROC))]
        private static int HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                // Skip this event
                return User32.CallNextHookEx(
                    pWindowsHook, nCode, wParam, lParam);
            }

            bool consume = HandleKey(wParam, lParam);
            return consume ? 1 : User32.CallNextHookEx(pWindowsHook, 0, wParam, lParam);
        }

        private static bool HandleKey(IntPtr wParam, IntPtr lParam)
        {
            int nativeKeyDown = (int)lParam;
            ushort nativeKeyCode = (ushort)wParam;
            
            bool isKeyDown = IsKeyDown(nativeKeyDown);
            KeyCode keyCode = GetKey(nativeKeyCode);

            if (isKeyDown)
            {
                if (OnKeyDown == null)
                {
                    return false;
                }

                if (bManagePressedKey)
                {
                    bool nowAdded = keys.Add(nativeKeyCode);
                    if (nowAdded)
                    {
                        bool consume = OnKeyDown(keyCode);
                        cachedConsumePolicy[nativeKeyCode] = consume;

                        return consume;
                    }
                    else
                    {
                        return cachedConsumePolicy[nativeKeyCode];
                    }
                }
                else
                {
                    return OnKeyDown(keyCode);
                }
            }
            else
            {
                if (OnKeyUp == null)
                {
                    return false;
                }

                if (bManagePressedKey)
                {
                    keys.Remove(nativeKeyCode);
                    cachedConsumePolicy.Remove(nativeKeyCode);
                }
                
                return OnKeyUp(keyCode);
            }
        }

        private static bool IsKeyDown(int ptr)
        {
            return (ptr & (1 << 31)) == 0;
        }

        private static KeyCode GetKey(ushort key)
        {
            if (bUseKeyMap)
            {
                if (unityKeyMap.TryGetValue(key, out var keyCode))
                {
                    return keyCode;
                }

                return KeyCode.None;
            }
            
            // alphabets
            if (key >= 0x41 && key <= 0x5A)
            {
                return (KeyCode)(32 + key);
            }
            
            // num pads
            
            // numbers
            
            // functions
            
            // combinations

            return KeyCode.None;
        }
        
        #endregion
    }
}