using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using static Uliana.Library.Mem00x.Libraries.Kernel32;
using static Uliana.Library.Mem00x.Pwn;

namespace Uliana.Library.Mem00x {
    /// <summary>
    /// Implementa funções genéricas e estátias para facilitar
    /// o funcionamento interno da API.
    /// </summary>
    internal static class InternalHelpers {

        /// <summary>
        /// Válida se o usuário atual é administrador.
        /// </summary>
        /// <returns>Caso sim, retorna verdadeiro.</returns>
        public static bool IsAdminRole() {
            using (var identity = WindowsIdentity.GetCurrent()) {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static UIntPtr GetCode(string name, string path, int size = 8) {
            if (Is64Bit()) {
                if (size == 8) size = 16;
                return Get64BitCode(name, path, size);
            }

            var localCode = LoadCode(name, path);

            if (string.IsNullOrEmpty(localCode)) return UIntPtr.Zero;

            if (!localCode.Contains("+") && !localCode.Contains(",")) return new UIntPtr(Convert.ToUInt32(localCode, 16));

            var newOffsets = localCode;

            if (localCode.Contains("+"))
                newOffsets = localCode.Substring(localCode.IndexOf('+') + 1);

            var memoryAddress = new byte[size];

            if (newOffsets.Contains(",")) {
                var offsetsList = new List<int>();

                newOffsets
                    .Split(',')
                    .ToList()
                    .ForEach(oldOffsets => {
                        var local = oldOffsets;
                        if (oldOffsets.Contains("0x")) local = oldOffsets.Replace("0x", string.Empty);
                        offsetsList
                            .Add(int.Parse(local, NumberStyles.HexNumber));
                    });

                var offsets = offsetsList
                    .ToArray();

                if (localCode.Contains("base") || localCode.Contains("main"))
                    ReadProcessMemory(Handle,
                        (UIntPtr)((int)MainModule.BaseAddress + offsets[0]),
                        memoryAddress, (UIntPtr)size, IntPtr.Zero);

                else if (!localCode.Contains("base") &&
                         !localCode.Contains("main") &&
                         localCode.Contains("+")) {
                    var moduleName = localCode
                        .Split('+');
                    var altModule = IntPtr.Zero;
                    if (!moduleName[0].Contains(".dll") && !moduleName[0].Contains(".exe")) {
                        var theAddr = moduleName[0];
                        if (theAddr.Contains("0x")) theAddr = theAddr.Replace("0x", "");
                        altModule = (IntPtr)int.Parse(theAddr, NumberStyles.HexNumber);
                    } else {
                        try {
                            altModule = Modules[moduleName[0]];
                        } catch {
                            // ignored
                        }
                    }
                    ReadProcessMemory(Handle, (UIntPtr)((int)altModule + offsets[0]), memoryAddress, (UIntPtr)size, IntPtr.Zero);
                } else
                    ReadProcessMemory(Handle, (UIntPtr)(offsets[0]), memoryAddress, (UIntPtr)size, IntPtr.Zero);

                var num1 = BitConverter.ToUInt32(memoryAddress, 0); 
                var base1 = (UIntPtr)0;

                for (var i = 1; i < offsets.Length; i++) {
                    base1 = new UIntPtr(num1 + Convert.ToUInt32(offsets[i]));
                    ReadProcessMemory(Handle, base1, memoryAddress, (UIntPtr)size, IntPtr.Zero);
                    num1 = BitConverter.ToUInt32(memoryAddress, 0); 
                }
                return base1;
            } else {
                var trueCode  = Convert.ToInt32(newOffsets, 16);
                var altModule = IntPtr.Zero;

                if (localCode.Contains("base") || localCode.Contains("main"))
                    altModule = MainModule.BaseAddress;
                else if (!localCode.Contains("base") && !localCode.Contains("main") && localCode.Contains("+")) {
                    var moduleName = localCode.Split('+');
                    if (!moduleName[0].Contains(".dll") && !moduleName[0].Contains(".exe")) {
                        var theAddr = moduleName[0];
                        if (theAddr.Contains("0x")) theAddr = theAddr.Replace("0x", "");
                        altModule = (IntPtr)int.Parse(theAddr, NumberStyles.HexNumber);
                    } else {
                        try {
                            altModule = Modules[moduleName[0]];
                        } catch {
                            // ignored
                        }
                    }
                } else
                    altModule = Modules[localCode.Split('+')[0]];
                return (UIntPtr)((int)altModule + trueCode);
            }
        }

        private static UIntPtr Get64BitCode(string name, string path, int size = 16) {
            var theCode = LoadCode(name, path);

            if (string.IsNullOrEmpty(theCode))
                return UIntPtr.Zero;

            var newOffsets = theCode;

            if (theCode.Contains("+"))
                newOffsets = theCode.Substring(theCode.IndexOf('+') + 1);

            var memoryAddress = new byte[size];

            if (!theCode.Contains("+") && !theCode.Contains(",")) return new UIntPtr(Convert.ToUInt64(theCode, 16));

            if (newOffsets.Contains(",")) {
                var offsetsList = new List<long>();

                var newerOffsets = newOffsets.Split(',');
                foreach (var oldOffsets in newerOffsets) {
                    var test = oldOffsets;
                    if (oldOffsets.Contains("0x")) test = oldOffsets.Replace("0x", "");
                    offsetsList.Add(long.Parse(test, NumberStyles.HexNumber));
                }
                var offsets = offsetsList.ToArray();

                if (theCode.Contains("base") || theCode.Contains("main"))
                    ReadProcessMemory(Handle, (UIntPtr)((long)MainModule.BaseAddress + offsets[0]), memoryAddress, (UIntPtr)size, IntPtr.Zero);
                else if (!theCode.Contains("base") && !theCode.Contains("main") && theCode.Contains("+")) {
                    var moduleName = theCode.Split('+');
                    var altModule = IntPtr.Zero;
                    if (!moduleName[0].Contains(".dll") && !moduleName[0].Contains(".exe"))
                        altModule = (IntPtr)long
                            .Parse(moduleName[0], NumberStyles.HexNumber);
                    else {
                        try {
                            altModule = Modules[moduleName[0]];
                        } catch {
                            Debug.WriteLine("Module " + moduleName[0] + " was not found in module list!");
                            Debug.WriteLine("Modules: " + string.Join(",", Modules));
                        }
                    }
                    ReadProcessMemory(Handle, (UIntPtr)((long)altModule + offsets[0]), memoryAddress, (UIntPtr)size, IntPtr.Zero);
                } else
                    ReadProcessMemory(Handle, (UIntPtr)(offsets[0]), memoryAddress, (UIntPtr)size, IntPtr.Zero);

                var num1 = BitConverter.ToUInt64(memoryAddress, 0);

                var base1 = (UIntPtr)0;

                for (var i = 1; i < offsets.Length; i++) {
                    base1 = new UIntPtr(num1 + Convert.ToUInt64(offsets[i]));
                    ReadProcessMemory(Handle, base1, memoryAddress, (UIntPtr)size, IntPtr.Zero);
                    num1 = BitConverter.ToUInt64(memoryAddress, 0);
                }
                return base1;
            } else {
                var trueCode = Convert.ToInt64(newOffsets, 16);
                var altModule = IntPtr.Zero;
                if (theCode.Contains("base") || theCode.Contains("main"))
                    altModule = MainModule.BaseAddress;
                else if (!theCode.Contains("base") && !theCode.Contains("main") && theCode.Contains("+")) {
                    var moduleName = theCode.Split('+');
                    if (!moduleName[0].Contains(".dll") && !moduleName[0].Contains(".exe")) {
                        var theAddr = moduleName[0];
                        if (theAddr.Contains("0x")) theAddr = theAddr.Replace("0x", "");
                        altModule = (IntPtr)long.Parse(theAddr, NumberStyles.HexNumber);
                    } else {
                        try {
                            altModule = Modules[moduleName[0]];
                        } catch {
                            // ignored
                        }
                    }
                } else
                    altModule = Modules[theCode.Split('+')[0]];
                return (UIntPtr)((long)altModule + trueCode);
            }
        }

        /// <summary>
        /// Get code from ini file.
        /// </summary>
        /// <param name="name">label for address or code</param>
        /// <param name="file">path and name of ini file</param>
        /// <returns></returns>
        public static string LoadCode(string name, string file) {
            var returnCode = new StringBuilder(1024);

            if (!string.IsNullOrEmpty(file))
                GetPrivateProfileString("codes", name, "", returnCode, (uint)returnCode.Capacity, file);
            else
                returnCode.Append(name);

            return returnCode
                .ToString();
        }

        public static void GetModules() {
            if (Modules.Any())
                Modules.Clear();

            if (!string.IsNullOrEmpty(Module.ModuleName) &&
                !Modules.ContainsKey(Module.ModuleName))
                Modules
                    .Cast<ProcessModule>()
                    .ToList()
                    .ForEach(m => Modules.Add(m.ModuleName,
                        m.BaseAddress));
        }
    }
}
