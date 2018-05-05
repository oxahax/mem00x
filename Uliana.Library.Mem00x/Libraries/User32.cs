using System;
using System.Runtime.InteropServices;

namespace Uliana.Library.Mem00x.Libraries {
    /// <summary>
    /// Implementa todos as importações necessárias dentro da
    /// DLL "user32.dll" necessárias.
    /// </summary>
    internal static class User32 {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
    }
}
