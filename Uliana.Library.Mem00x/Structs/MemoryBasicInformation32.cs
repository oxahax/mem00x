using System;

namespace Uliana.Library.Mem00x.Structs
{
    /// <summary>
    /// Contém informações sobre um intervalo de páginas no espaço de endereço virtual de um processo de 32 bits.
    /// </summary>
    public struct MemoryBasicInformation32
    {
        /// <summary>
        /// Um ponteiro para o endereço base da região das páginas.
        /// </summary>
        public IntPtr BaseAddress;

        /// <summary>
        /// Um ponteiro para o endereço base de um intervalo de páginas alocadas pela
        /// função VirtualAlloc <inheritdoc cref="https://msdn.microsoft.com/pt-br/library/windows/desktop/aa366775(v=vs.85).aspx"/>.
        /// </summary>
        public IntPtr AllocationBase;

        /// <summary>
        /// A opção de proteção de memória quando a região foi inicialmente alocada.
        /// </summary>
        public uint AllocationProtect;

        /// <summary>
        /// O tamanho da região que começa no endereço base no qual todas as páginas
        /// têm atributos idênticos.
        /// </summary>
        public uint RegionSize;

        /// <summary>
        /// O estado das páginas na região.
        /// </summary>
        public uint State;

        /// <summary>
        /// A proteção de acesso das páginas na região.
        /// </summary>
        public uint Protect;

        /// <summary>
        /// O tipo de páginas na região.
        /// </summary>
        public uint Type;
    }
}