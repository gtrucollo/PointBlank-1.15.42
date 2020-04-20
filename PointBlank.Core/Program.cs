namespace PointBlank.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Files;
    using IBO;
    using OR.Library;
    using PointBlank.BO;

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
                Logger.Info("Atualizando configurações de conexões");
                Network.Inicializar(configFile.NetworkHost, configFile.NetworkPort, configFile.NetworkKey);

                // Inicializar serviços (WCF)
                Logger.Info("Inicializando canais WCF");
                foreach (ServiceType servico in ServiceList.ListaServicos)
                {
                    Network.IncluirCanalWcfHost(servico);
                }

                // Não finalizar o servidor
                Logger.Info("Servidor Inicializado");
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