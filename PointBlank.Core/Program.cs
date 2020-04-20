namespace PointBlank.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Files;
    using IBO;
    using OR.Library;

    /// <summary>
    /// Classe Program
    /// </summary>
    public class Program
    {
        #region Métodos
        /// <summary>
        /// Método Main
        /// </summary>
        public static void Main()
        {
            try
            {
                // Log de inicio
                Console.Title = "Point Blank - Core";

                // Configurações
                Logger.Info("Carregando arquivo de configurações");
                ConfigFile configFile = new ConfigFile();

                // Atualizar dados a partir da configuração
                Network.Inicializar(configFile.NetworkHost, configFile.NetworkPort, configFile.NetworkKey);

                // Não finalizar o servidor
                Process.GetCurrentProcess().WaitForExit();
            }
            catch (ThreadAbortException)
            {
                Logger.Error("Ocorreu um erro não tratado, todas as conexões serão finalizadas");
                throw;
            }
            catch (Exception exp)
            {
                Logger.Error(exp, "Ocorreu um erro e com isso as conexões serão finalizadas", true);
            }
        }
        #endregion
    }
}