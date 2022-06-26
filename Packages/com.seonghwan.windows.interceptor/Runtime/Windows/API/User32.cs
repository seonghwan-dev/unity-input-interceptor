using System;
using System.Runtime.InteropServices;
using Windows.Enums;

namespace Windows.API
{
    public static class User32
    {
        private const string DLL = "User32";

        public delegate int HOOKPROC(
            int code,
            IntPtr wParam,
            IntPtr lParam
        );

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hmod"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport(DLL)]
        public static extern IntPtr SetWindowsHookEx(
            EHookId idHook,
            HOOKPROC lpfn,
            IntPtr hmod,
            int dwThreadId
        );

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unhookwindowshookex
        /// </summary>
        /// <param name="hhk"></param>
        /// <returns></returns>
        [DllImport(DLL)]
        public static extern int UnhookWindowsHookEx(
            IntPtr hhk
        );

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callnexthookex
        /// </summary>
        /// <param name="hhk"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport(DLL)]
        public static extern int CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );
    }
}