namespace Uliana.Library.Mem00x.Enums
{
    /// <summary>
    /// Enumera os tipos de minidespejo causado pelo modo usuário.
    /// </summary>
    public enum MiniDump
    {
        /// <summary>
        /// Inclui apenas as informações necessárias para capturar
        /// rastreamentos de pilha para todos os segmentos existentes
        /// em um processo.
        /// </summary>
        MiniDumpNormal = 0x00000000,

        /// <summary>
        /// Inclui as seções de dados de todos os módulos carregados.
        /// </summary>
        MiniDumpWithDataSegs = 0x00000001,

        /// <summary>
        /// Inclui toda a memória acessível no processo. Os dados brutos da
        /// memória são incluídos no final, para que as estruturas iniciais
        /// possam ser mapeadas diretamente sem as informações brutas da memória.
        /// <para>
        /// Aviso: Esta opção pode resultar em um arquivo muito grande.
        /// </para>
        /// </summary>
        MiniDumpWithFullMemory = 0x00000002,

        /// <summary>
        /// Inclui informações de alto nível sobre os handler do sistema operacional
        /// que estão ativas quando o minidespejo é feito.
        /// </summary>
        MiniDumpWithHandleData = 0x00000004,

        /// <summary>
        /// Memória de armazenamento de pilha e de backup gravada no arquivo de minidespejo
        /// deve ser filtrada para remover todos os valores de ponteiro, exceto os necessários,
        /// para reconstruir um rastreamento de pilha.
        /// </summary>
        MiniDumpFilterMemory = 0x00000008,

        /// <summary>
        /// A memória de armazenamento de pilha e de apoio deve ser verificada quanto a referências
        /// de ponteiro para módulos na lista de módulos.
        /// </summary>
        MiniDumpScanMemory = 0x00000010,

        /// <summary>
        /// Inclui informações da lista de módulos que foram recentemente descarregados, e se essas
        /// informações forem mantidas pelo sistema operacional.
        /// </summary>
        MiniDumpWithUnloadedModules = 0x00000020,

        /// <summary>
        /// Inclui páginas com dados referenciados por locais ou outra memória de pilha.
        /// <para>
        /// Aviso: Esta opção pode aumentar significativamente o tamanho do arquivo de minidespejo.
        /// </para>
        /// </summary>
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,

        /// <summary>
        /// Filtra caminhos do módulo para informações como nomes de usuário ou diretórios importantes.
        /// <para>
        /// Essa opção pode impedir que o sistema localize o arquivo de imagem e deve ser usado somente
        /// em situações especiais.
        /// </para>
        /// </summary>
        MiniDumpFilterModulePaths = 0x00000080,

        /// <summary>
        /// Inclui informações completas por processo e por thread do sistema operacional.
        /// </summary>
        MiniDumpWithProcessThreadData = 0x00000100,


        /// <summary>
        /// Faz a varedura do espaço de endereço virtual para a memória
        /// "PAGE_READWRITE" a ser incluída.
        /// </summary>
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,

        /// <summary>
        /// Reduz os dados que são despejados, eliminando regiões de memória que não são essenciais
        /// para atender aos critérios especificados para o despejo. Isso pode evitar o despejo de
        /// memória que pode conter dados privados para o usuário. No entanto, não é garantia de que
        /// nenhuma informação privada esteja presente.
        /// </summary>
        MiniDumpWithoutOptionalData = 0x00000400,

        /// <summary>
        /// Inclui informações da região de memória.
        /// </summary>
        MiniDumpWithFullMemoryInfo = 0x00000800,

        /// <summary>
        /// Inclui informações do estado da thread.
        /// </summary>
        MiniDumpWithThreadInfo = 0x00001000,

        /// <summary>
        /// Incluir todas as seções relacionadas a código e código dos módulos carregados para capturar
        /// o conteúdo executável.
        /// </summary>
        MiniDumpWithCodeSegs = 0x00002000
    }
}