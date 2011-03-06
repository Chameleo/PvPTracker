using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// by gauravklr at http://wphone7.wordpress.com/2008/08/23/easy-way-global-keyboard-hooking-in-net-application/
namespace HookApplication
{

    public static class KeyboardHook
    {
        #region ExternMethods

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        public enum BeginKeys
        {
            Alt_Control = Keys.Alt | Keys.Control,
            Alt_Shift = Keys.Alt | Keys.Shift,
            Control_Shift = Keys.Control | Keys.Shift,
            Alt_Control_Shift = Keys.Alt | Keys.Control | Keys.Shift,
        } ;

        public enum EndKey
        {
            A = Keys.A, B = Keys.B, C = Keys.C, D = Keys.D, E = Keys.E, F = Keys.F, G = Keys.G, H = Keys.H,
            I = Keys.I, J = Keys.J, K = Keys.K, L = Keys.L, M = Keys.M, N = Keys.N, O = Keys.O, P = Keys.P,
            Q = Keys.Q, R = Keys.R, S = Keys.S, T = Keys.T, U = Keys.U, V = Keys.V, W = Keys.W, X = Keys.X,
            Y = Keys.Y, Z = Keys.Z
        } ;

        public static void SetHook(Action act,BeginKeys b, EndKey e)
        {
            _beginKeys = b;
            _endKey = e;
            _action = act;

            _hookID = SetHook(_proc);
        }

        public static void UnhookWindowsHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr LowLevelKeyboardProc(
                        int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static BeginKeys _beginKeys;
        private static EndKey _endKey;
        private static Action _action;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                                             GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(
                            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if ((Keys)(_endKey) == (Keys)vkCode && (Keys)_beginKeys == Control.ModifierKeys)
                {
                    _action();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
