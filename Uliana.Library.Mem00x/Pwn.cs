using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Uliana.Library.Mem00x.Enums;
using Uliana.Library.Mem00x.Libraries;
using static System.Diagnostics.Process;
using static Uliana.Library.Mem00x.Libraries.Kernel32;
using static Uliana.Library.Mem00x.InternalHelpers;

namespace Uliana.Library.Mem00x {
    /// <summary>
    /// Principal objeto de manuseio da API.
    /// </summary>
    public class Pwn {
        // Construtor implícito.
        #region Propriedades

        internal static IntPtr Handle;
        internal static ProcessModule Module;
        internal static ProcessModule MainModule;
        internal static Process InternalProcess;
        internal static Dictionary<string, IntPtr> Modules
            = new Dictionary<string, IntPtr>();

        #endregion

        /// <summary>
        /// Realiza a abertura remota do processo na instânia do Mem00x.
        /// </summary>
        /// <param name="pid">Identificador único do processo.</param>
        public void OpenProcess(int pid)
        {
            if (!IsAdminRole())
                throw new Exception("Usuário não administrativo!");

            InternalProcess = GetProcessById(pid);

            Handle = Kernel32.OpenProcess(0x1F0FFF, true, pid);
            EnterDebugMode();

            if (Handle == IntPtr.Zero)
                Marshal.GetLastWin32Error();

            Module = InternalProcess.MainModule;

            GetModules();
        }

        /// <summary>
        /// Realiza uma busca nas insâncias de processos de primeiro e
        /// segundo plano em execução.
        /// </summary>
        /// <param name="name">Nome do processo que deseja encontrar.</param>
        /// <returns>Caso não encontrado, retorna-se zero.</returns>
        public int GetProcessIdByName(string name) =>
            GetProcessesByName(name ?? throw new ArgumentNullException(nameof(name)))
                .Where(p => p.ProcessName == name)
                .Select(p => p.Id)
                .FirstOrDefault();

        /// <summary>
        /// Realiza a escrita diretamente em um endereço de memória especificado pelo
        /// arquivo de configuração.
        /// </summary>
        /// <param name="code">Pode ter várias entradas, onde são válidas:
        /// endereço absoluto,
        /// módulo + pointer + offset,
        /// módulo + offset,
        /// alias para arquivo de configuração.
        /// </param>
        /// <param name="type">Tipo da estrtura que ira ser escrita.</param>
        /// <param name="write">Valor para ser escrito.</param>
        /// <param name="file">Caminho até o arquivo de configuração.</param>
        /// <returns>Caso nenhum erro aconteça, é retornado verdadeiro.</returns>
        public bool WriteToMemmory(string code, DataType type, string write, string file = null)
        {
            byte[] memory;
            int size;

            var theCode = GetCode(code, file);

            switch (type)
            {
                case DataType.Float:
                {
                    memory = BitConverter.GetBytes(Convert.ToSingle(write));
                    size = 4;
                }
                    break;
                case DataType.Int:
                {
                    memory = BitConverter.GetBytes(Convert.ToInt32(write));
                    size = 4;
                }
                    break;
                case DataType.Byte:
                {
                    memory = new byte[1];
                    memory[0] = Convert.ToByte(write, 16);
                    size = 1;
                }
                    break;
                case DataType.Bytes:
                {
                    if (write.Contains(",") || write.Contains(" ")) {
                        var stringBytes = write.Split(write.Contains(",") ? ',' : ' ');

                        var c = stringBytes.Count();
                        memory = new byte[c];
                        for (var i = 0; i < c; i++) {
                            memory[i] = Convert.ToByte(stringBytes[i], 16);
                        }
                        size = stringBytes.Count();
                    } else {
                        memory = new byte[1];
                        memory[0] = Convert.ToByte(write, 16);
                        size = 1;
                    }
                }
                    break;
                case DataType.DByte:
                {
                    memory = new byte[2];
                    memory[0] = (byte)(Convert.ToInt32(write) % 256);
                    memory[1] = (byte)(Convert.ToInt32(write) / 256);
                    size = 2;
                }
                    break;
                case DataType.Double:
                {
                    memory = BitConverter.GetBytes(Convert.ToDouble(write));
                    size = 8;
                }
                    break;
                case DataType.Long:
                {
                    memory = BitConverter.GetBytes(Convert.ToInt64(write));
                    size = 8;
                }
                    break;
                case DataType.String:
                {
                    memory = System.Text.Encoding.UTF8.GetBytes(write);
                    size = write.Length;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            return WriteProcessMemory(Handle, theCode, memory, (UIntPtr)size, IntPtr.Zero);
        }


        /// <summary>
        /// Suspende o processo carregado pelo Mem00x.
        /// </summary>
        public void SuspendProcess() =>
            InternalProcess
                .Threads
                .Cast<ProcessThread>()
                .ToList()
                .ForEach(t => {
                    var opened = OpenThread(ThreadAccess.SuspendResume, false, (uint)t.Id);
                    SuspendThread(opened);
                    CloseHandle(opened);
                });

        /// <summary>
        /// Suspende o processo carregado pelo Mem00x.
        /// </summary>
        public void ResumeProcess() =>
            InternalProcess
                .Threads
                .Cast<ProcessThread>()
                .ToList()
                .ForEach(t => {
                    var opened = OpenThread(ThreadAccess.SuspendResume, false, (uint)t.Id);
                    int toResume;
                    do {
                        toResume = ResumeThread(opened);
                    } while (toResume > 0);
                    CloseHandle(opened);
                });
    }
}
