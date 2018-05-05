using System.Security.Principal;

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
    }
}
