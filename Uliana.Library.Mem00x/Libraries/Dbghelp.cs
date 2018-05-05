using System;
using System.Runtime.InteropServices;
using Uliana.Library.Mem00x.Enums;

namespace Uliana.Library.Mem00x.Libraries {

    /// <summary>
    /// Implementa todos as importações necessárias dentro da
    /// DLL "dbghelp32.dll" necessárias.
    /// </summary>
    internal static class Dbghelp {
        [DllImport("dbghelp.dll")]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            int ProcessId,
            IntPtr hFile,
            MiniDump DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallackParam);
    }
}
