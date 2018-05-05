using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Uliana.Library.Mem00x.Enums;
using Uliana.Library.Mem00x.Structs;

namespace Uliana.Library.Mem00x.Libraries {
    /// <summary>
    /// Implementa todos as importações necessárias dentro da
    /// DLL "kernel.32" necessárias.
    /// </summary>
    internal static class Kernel32 {

        /// <summary>
        /// Abre um identificador para outro processo em execução no sistema.
        /// Esse identificador pode ser usado para ler e gravar na outra memória de processo
        /// ou para injetar código no outro processo.
        /// </summary>
        /// <param name="dwDesiredAccess">O acesso ao objeto do processo.
        /// Esse direito de acesso é verificado em relação ao descritor de segurança do processo.</param>
        /// <param name="bInheritHandle">Caso <see cref="true"/>, os processos criados por esse processo herdarão o identificador.</param>
        /// <param name="dwProcessId">O identificador do processo local a ser aberto.</param>
        /// <returns>Se a função for bem-sucedida, o valor de retorno será um identificador aberto para o processo especificado.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwProcessId
        );

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

#if WINXP
#else
        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress,
            out MemoryBasicInformation32 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress,
            out MemoryBasicInformation64 lpBuffer, UIntPtr dwLength);

        public static UIntPtr VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress,
            out MemoryBasicInformation lpBuffer) {
            UIntPtr retVal;

            if (Is64Bit()) {
                var tmp64 = new MemoryBasicInformation64();
                retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp64, new UIntPtr((uint)Marshal.SizeOf(tmp64)));

                lpBuffer.BaseAddress = tmp64.BaseAddress;
                lpBuffer.AllocationBase = tmp64.AllocationBase;
                lpBuffer.AllocationProtect = tmp64.AllocationProtect;
                lpBuffer.RegionSize = (long)tmp64.RegionSize;
                lpBuffer.State = tmp64.State;
                lpBuffer.Protect = tmp64.Protect;
                lpBuffer.Type = tmp64.Type;

                return retVal;
            }

            var tmp32 = new MemoryBasicInformation32();

            retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp32, new UIntPtr((uint)Marshal.SizeOf(tmp32)));

            lpBuffer.BaseAddress = tmp32.BaseAddress;
            lpBuffer.AllocationBase = tmp32.AllocationBase;
            lpBuffer.AllocationProtect = tmp32.AllocationProtect;
            lpBuffer.RegionSize = tmp32.RegionSize;
            lpBuffer.State = tmp32.State;
            lpBuffer.Protect = tmp32.Protect;
            lpBuffer.Type = tmp32.Type;

            return retVal;
        }

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SystemInfo lpSystemInfo);
#endif

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        #region Funções internas

        private static bool Is64Bit() {
            if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) &&
                Environment.OSVersion.Version.Major < 6) return false;

            using (var p = Process.GetCurrentProcess())
                return IsWow64Process(p.Handle, out var retVal) && retVal;
        }

        #endregion
    }
}
