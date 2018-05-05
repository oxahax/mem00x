using System;

namespace Uliana.Library.Mem00x.Structs
{
    /// <summary>
    /// Indexa informações nativas sobre o host que consome a API.
    /// </summary>
    public struct SystemInfo
    {
        /// <summary>
        /// A arquitetura do processador do sistema operacional instalado.
        /// </summary>
        public ushort processorArchitecture;

        /// <summary>
        /// Se a estrutura é reservada para uso futuro.
        /// </summary>
        ushort reserved;

        /// <summary>
        /// O tamanho da página e a granularidade da proteção e compromisso da página.
        /// </summary>
        public uint pageSize;

        /// <summary>
        /// Um ponteiro para o menor endereço de memória acessível para aplicativos e DLLs.
        /// </summary>
        public IntPtr minimumApplicationAddress;

        /// <summary>
        /// Um ponteiro para o maior endereço de memória acessível para aplicativos e DLLs.
        /// </summary>
        public IntPtr maximumApplicationAddress;

        /// <summary>
        /// Uma máscara representando o conjunto de processadores configurados no sistema.
        /// </summary>
        public IntPtr activeProcessorMask;

        /// <summary>
        /// O número de processadores lógicos no grupo atual.
        /// </summary>
        public uint numberOfProcessors;

        /// <summary>
        /// Tipo primitivo para enumeração do tipo do processador.
        /// </summary>
        public uint processorType;

        /// <summary>
        /// A granularidade do endereço inicial no qual a memória virtual pode ser alocada.
        /// </summary>
        public uint allocationGranularity;

        /// <summary>
        /// O nível do processador corespondente da arquitetura.
        /// </summary>
        public ushort processorLevel;

        /// <summary>
        /// A revisão do processador corespondente da arquitetura.
        /// </summary>
        public ushort processorRevision;
    }
}