using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace MemoryIO
{
    // By Apoc of Mmowned
    internal static class Native
    {
        private static IntPtr _procHandle;

        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
                                                      int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll"), SuppressUnmanagedCodeSecurity]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
                                                       uint nSize,
                                                       out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        private static extern bool CloseHandle(IntPtr hHandle);

        public static int WriteBytes(IntPtr address, byte[] val)
        {
            if (_procHandle == IntPtr.Zero)
            {
                throw new Exception("There's no current process handle... are you sure you did everything right?");
            }
            int written;
            if (WriteProcessMemory(_procHandle, address, val, (uint) val.Length, out written))
            {
                return written;
            }
            throw new Exception(
                string.Format(
                    "Could not write the specified bytes! {0} [{1}]",
                    address.ToString("X8"),
                    Marshal.GetLastWin32Error()));
        }

        public static byte[] ReadBytes(IntPtr address, int count)
        {
            if (_procHandle == IntPtr.Zero)
            {
                throw new Exception("There's no current process handle... are you sure you did everything right?");
            }
            var ret = new byte[count];
            int numRead;
            if (ReadProcessMemory(_procHandle, address, ret, count, out numRead) && numRead == count)
            {
                return ret;
            }
            return new byte[count];
        }

        public static void OpenProcessHandle(int processId)
        {
            if (_procHandle != IntPtr.Zero)
                CloseProcessHandle();
            Process.EnterDebugMode();
            // We really don't want to inherit a handle here.
            _procHandle = OpenProcess(0x38/*0x001F0FFF*/, false, processId);
            if (_procHandle != IntPtr.Zero)
            {
                Memory.ImageBase = Process.GetProcessById(processId).MainModule.BaseAddress;
            }
        }

        public static void CloseProcessHandle()
        {
            if (_procHandle == IntPtr.Zero)
                return;

            CloseHandle(_procHandle);
            Process.LeaveDebugMode();
        }
    }
}