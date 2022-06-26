using System;
using System.Collections.Generic;

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
                if (handler == null)
                    handler = new DefaultKeyCodeHandler();
                
                pWindowsHook = User32.SetWindowsHookEx(
                    EHookId.WH_KEYBOARD,
                    HookProc,
                    IntPtr.Zero,
                    currentThreadId
                );
            }

            return pWindowsHook != IntPtr.Zero;
        }

        /// <summary>
        /// WindowsHook을 해제합니다.
        /// </summary>
        public static bool Unhook()
        {
            if (pWindowsHook == IntPtr.Zero) return false;

            User32.UnhookWindowsHookEx(pWindowsHook);
            pWindowsHook = IntPtr.Zero;
            handler = null;
            
            keys.Clear();
            cachedConsumePolicy.Clear();
            
            bIsHooked = false;
            return true;
        }

        public static void SetKeyDownCallback(Func<int, bool> keyDown)
        {
            if (bIsHooked)
                throw new HookException("You can't change options after installed hook. ");
            
            OnKeyDown = keyDown;
        }

        public static void SetKeyUpCallback(Func<int, bool> keyUp)
        {
            if (bIsHooked)
                throw new HookException("You can't change options after installed hook. ");
            
            OnKeyUp = keyUp;
        }

        public static void SetManagePressedKey(bool manage)
        {
            if (bIsHooked)
                throw new HookException("You can't change options after installed hook. ");
            
            bManagePressedKey = manage;
        }

        public static void SetKeyCodeHandler(IKeyCodeHandler keyCodeHandler)
        {
            if (bIsHooked)
                throw new HookException("You can't change options after installed hook. ");
                
            handler = keyCodeHandler;
        }
        
        #region Internal Fields 
        
        private static bool bIsHooked;
        private static bool bManagePressedKey;
        
        private static IntPtr pWindowsHook = IntPtr.Zero;

        private static Func<int, bool> OnKeyDown;
        private static Func<int, bool> OnKeyUp;

        private static readonly HashSet<ushort> keys = new HashSet<ushort>(8);
        private static readonly Dictionary<ushort, bool> cachedConsumePolicy = new Dictionary<ushort, bool>(8);
        
        private static IKeyCodeHandler handler;

#if UNITY_EDITOR
        public static void Reset()
        {
            bIsHooked = false;
            bManagePressedKey = false;
            
            pWindowsHook = IntPtr.Zero;
            
            OnKeyDown = null;
            OnKeyUp = null;

            handler = null;
            
            keys.Clear();
            cachedConsumePolicy.Clear();
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
            int keyCode = GetKey(nativeKeyCode);

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

        private static int GetKey(ushort key)
        {
            return handler.Handle(key);
        }

        public sealed class HookException : Exception
        {
            public HookException(string message) : base(message)
            {
            }
        }
        
        #endregion
    }
}