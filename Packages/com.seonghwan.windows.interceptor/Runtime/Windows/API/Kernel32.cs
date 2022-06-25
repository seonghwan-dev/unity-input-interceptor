using System.Runtime.InteropServices;

namespace Windows.API
{
    public static class Kernel32
    {
        private const string DLL = "Kernel32";

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL)]
        public static extern uint GetCurrentThreadId();
    }
}