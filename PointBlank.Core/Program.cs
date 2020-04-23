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

                // Banco de dados
                Logger.Info("Atualizando configurações do banco de dados");
                SessionManager.Servidor = configFile.DatabaseHost;
                SessionManager.Porta = configFile.DatabasePort;
                SessionManager.NomeUsuario = configFile.DatabaseUser;
                SessionManager.Senha = configFile.DatabasePassword;
                SessionManager.NomeBanco = configFile.DatabaseName;

                // Obter nova sessão com banco de dados
                Logger.Info("Inicializando sessão com o banco de dados");
                try
                {
                    SessionManager.ObterNovaSessao();
                }
                catch (Exception exp)
                {
                    Logger.Error(exp, "Verifique as configurações de conexão com o banco de dados", false);
                    return;
                }

                // Atualizar dados a partir da configuração
                Logger.Info("Atualizando configurações de conexões");
                WcfNetwork.Inicializar(configFile.NetworkHost, configFile.NetworkPort);

                // Inicializar serviços (WCF)
                Logger.Info("Inicializando canais WCF");
                foreach (ServiceType servico in ServiceList.ListaServicos)
                {
                    WcfNetwork.IncluirCanalWcfHost(servico);
                }

                Logger.Info("Servidor Inicializado");
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
            finally
            {
                // Não finalizar o servidor
                Process.GetCurrentProcess().WaitForExit();
            }
        }
        #endregion
    }
}