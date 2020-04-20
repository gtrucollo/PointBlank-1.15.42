namespace PointBlank.IBO
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using OR.Library.Exceptions;

    public static class Network
    {
        #region Constantes
        /// <summary>
        /// Chave de segurança
        /// </summary>
        public const string CHAVE_SEGURANCA = "point_blank_best_emulator_ever";

        /// <summary>
        /// Porta padrão de conexão
        /// </summary>
        public const int PORTA_CONEXAO = 5900;
        #endregion

        #region Campos
        /// <summary>
        /// Controle para o endereço de conexão
        /// </summary>
        private static string enderecoConexao = "localhost";

        /// <summary>
        /// Controle ConexaoPorta
        /// </summary>
        private static int conexaoPorta = Network.PORTA_CONEXAO;

        /// <summary>
        /// Controle para o time-out de conexão
        /// </summary>
        private static TimeSpan timeOut = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Controle para a lista de host's
        /// </summary>
        private static List<ServiceHost> listaHost;
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém ou define EnderecoConexao
        /// </summary>
        public static string ConexaoEndereco
        {
            get
            {
                return Network.enderecoConexao;
            }

            set
            {
                Network.enderecoConexao = value;
            }
        }

        /// <summary>
        /// Obtém ou define ConexaoPorta
        /// </summary>
        public static int ConexaoPorta
        {
            get
            {
                return Network.conexaoPorta;
            }

            set
            {
                Network.conexaoPorta = value;
            }
        }

        /// <summary>
        /// Obtém ou define ConexaoChaveSeguranca
        /// </summary>
        public static string ConexaoChaveSeguranca { get; set; }

        /// <summary>
        /// Obtém ou define TimeOutEnvio
        /// </summary>
        public static TimeSpan TimeOut
        {
            get
            {
                return Network.timeOut;
            }

            set
            {
                Network.timeOut = value;
            }
        }

        /// <summary>
        /// Obtém o valor de ListaServiceHost
        /// </summary>
        public static List<ServiceHost> ListaServiceHost
        {
            get
            {
                if (Network.listaHost == null)
                {
                    Network.listaHost = new List<ServiceHost>();
                }

                return Network.listaHost;
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Gerar um novo objeto NetTcpBinding
        /// </summary>
        /// <param name="conexaoSegura">Identifica se utiliza conexão segura</param>
        /// <returns>Objeto NetTcpBinding</returns>
        public static Binding ObterNovoNetTcpBinding()
        {
            // Opções padrões
            NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None, true);

            // ReaderQuotas
            netTcpBinding.ReaderQuotas.MaxDepth = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            // Opções Diversas
            netTcpBinding.MaxBufferSize = int.MaxValue;
            netTcpBinding.MaxBufferPoolSize = int.MaxValue;
            netTcpBinding.MaxReceivedMessageSize = int.MaxValue;
            netTcpBinding.MaxConnections = int.MaxValue;
            netTcpBinding.ListenBacklog = int.MaxValue;

            // Tempo de espera para receber os dados das requisições
            netTcpBinding.SendTimeout = Network.TimeOut;

            // Tempo máximo que o canal permaneçe aberto sem nenhuma atividade (Requisições)
            switch (Network.TimeOut > TimeSpan.FromMinutes(30))
            {
                case true:
                    netTcpBinding.ReceiveTimeout = Network.TimeOut; ;
                    netTcpBinding.ReliableSession.InactivityTimeout = Network.TimeOut;
                    break;

                default:
                    // Tempo mínimo
                    netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(30);
                    netTcpBinding.ReliableSession.InactivityTimeout = TimeSpan.FromMinutes(30);
                    break;
            }

            // Retorno
            return new CustomBinding(netTcpBinding);
        }

        /// <summary>
        /// Gerar um novo objeto HttpBinding
        /// </summary>
        /// <returns>Objeto HttpBinding</returns>
        public static BasicHttpBinding ObterNovoHttpBinding()
        {
            // Opções padrões
            BasicHttpBinding httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            // ReaderQuotas
            httpBinding.ReaderQuotas.MaxDepth = int.MaxValue;
            httpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            httpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            httpBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            httpBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            // Opções Diversas
            httpBinding.MaxBufferSize = int.MaxValue;
            httpBinding.MaxBufferPoolSize = int.MaxValue;
            httpBinding.MaxReceivedMessageSize = int.MaxValue;

            // Tempo máximo que o canal permaneçe aberto sem nenhuma atividade (Requisições)
            switch (Network.TimeOut > TimeSpan.FromMinutes(30))
            {
                case true:
                    httpBinding.SendTimeout = Network.TimeOut;
                    httpBinding.ReceiveTimeout = Network.TimeOut;
                    break;

                default:
                    // Tempo mínimo
                    httpBinding.SendTimeout = TimeSpan.FromMinutes(30);
                    httpBinding.ReceiveTimeout = TimeSpan.FromMinutes(30);
                    break;
            }

            // Retorno
            return httpBinding;
        }

        /// <summary>
        /// Incluir um serviço (Wcf)
        /// </summary>
        /// <param name="servico">Serviço a ser incluído</param>
        public static void IncluirCanalWcfHost(ServiceType servico)
        {
            try
            {
                // Endereço
                string enderecoTmp = string.Format(
                    "net.tcp://{0}:{1}/PointBlankCore/{2}",
                    Network.ConexaoEndereco,
                    Network.ConexaoPorta,
                    servico.GetServiceType(false).Name);

                // Instânciar um host para o serviço
                ServiceHost host = new ServiceHost(servico.GetServiceType(false), new Uri[] { new Uri(enderecoTmp) });
                ServiceEndpoint endpoint = host.AddServiceEndpoint(servico.GetServiceType(true), Network.ObterNovoNetTcpBinding(), enderecoTmp);

                // Opção para repassar ao cliente a mensagem do erro
                ServiceBehaviorAttribute debuggingBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                debuggingBehavior.IncludeExceptionDetailInFaults = true;

                // Total de conexões
                host.Description.Behaviors.Add(new ServiceThrottlingBehavior() { MaxConcurrentSessions = int.MaxValue, MaxConcurrentCalls = int.MaxValue, MaxConcurrentInstances = int.MaxValue });

                // Abrir o serviço
                host.Open();

                // Atualizar lista de controle
                Network.ListaServiceHost.Add(host);
            }
            catch (AddressAlreadyInUseException)
            {
                throw new PointBlankException(string.Format("A porta {0} já está em uso. Verifique senão foi iniciado um outro serviço na mesma", Network.ConexaoPorta));
            }
        }

        /// <summary>
        /// Inicializa as configurações
        /// </summary>
        /// <param name="conexaoEndereco">Endereço de conexão</param>
        /// <param name="conexaoPorta">Porta de conexão</param>
        /// <param name="chaveSeguranca">Chave de segurança</param>
        public static void Inicializar(string conexaoEndereco, int conexaoPorta, string chaveSeguranca)
        {
            Network.ConexaoEndereco = conexaoEndereco;
            Network.ConexaoPorta = conexaoPorta;
            Network.ConexaoChaveSeguranca = chaveSeguranca;
        }
        #endregion
    }
}
