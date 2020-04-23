namespace PointBlank.IBO
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Threading;
    using OR.Library;
    using OR.Library.Exceptions;

    public static class WcfNetwork
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
        private static int conexaoPorta = WcfNetwork.PORTA_CONEXAO;

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
                return WcfNetwork.enderecoConexao;
            }

            set
            {
                WcfNetwork.enderecoConexao = value;
            }
        }

        /// <summary>
        /// Obtém ou define ConexaoPorta
        /// </summary>
        public static int ConexaoPorta
        {
            get
            {
                return WcfNetwork.conexaoPorta;
            }

            set
            {
                WcfNetwork.conexaoPorta = value;
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
                return WcfNetwork.timeOut;
            }

            set
            {
                WcfNetwork.timeOut = value;
            }
        }

        /// <summary>
        /// Obtém o valor de ListaServiceHost
        /// </summary>
        public static List<ServiceHost> ListaServiceHost
        {
            get
            {
                if (WcfNetwork.listaHost == null)
                {
                    WcfNetwork.listaHost = new List<ServiceHost>();
                }

                return WcfNetwork.listaHost;
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Gerar um novo objeto NetTcpBinding
        /// </summary>
        /// <param name="conexaoSegura">Identifica se utiliza conexão segura</param>
        /// <returns>Objeto NetTcpBinding</returns>
        public static BasicHttpBinding ObterNovoNetTcpBinding()
        {
            // Opções padrões
            BasicHttpBinding netTcpBinding = new BasicHttpBinding();

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

            // Tempo de espera para receber os dados das requisições
            netTcpBinding.SendTimeout = WcfNetwork.TimeOut;

            // Tempo máximo que o canal permaneçe aberto sem nenhuma atividade (Requisições)
            netTcpBinding.ReceiveTimeout = WcfNetwork.TimeOut;

            // Retorno
            return netTcpBinding;
        }

        /// <summary>
        /// Validar a conexão
        /// </summary>
        /// <param name="communicationObject">Objeto ICommunicationObject</param>
        /// <param name="enderecoUrl">Endereço url</param>
        /// <param name="enderecoPorta">Endereço Porta</param>
        /// <param name="validarMetodoBo">Identifica se será chamado o método 'ValidarServicoWcf' do BO</param>
        public static void ValidarConexao(string enderecoUrl, int enderecoPorta)
        {
            WcfNetwork.ValidarConexao(null, enderecoUrl, enderecoPorta, false);
        }

        /// <summary>
        /// Validar a conexão
        /// </summary>
        /// <param name="communicationObject">Objeto ICommunicationObject</param>
        /// <param name="enderecoUrl">Endereço url</param>
        /// <param name="enderecoPorta">Endereço Porta</param>
        /// <param name="validarMetodoBo">Identifica se será chamado o método 'ValidarServicoWcf' do BO</param>
        private static void ValidarConexao(object communicationObject, string enderecoUrl, int enderecoPorta, bool validarMetodoBo)
        {
            // Verificar por Socket - Execução mais rápida
            int contTentativa = 0;
            while (true)
            {
                try
                {
                    using (TcpClient clienteTcp = new TcpClient())
                    {
                        IAsyncResult conexao = clienteTcp.BeginConnect(Dns.GetHostAddresses(enderecoUrl), enderecoPorta, null, null);
                        conexao.AsyncWaitHandle.WaitOne(1000, false);
                        if (clienteTcp.Connected)
                        {
                            // OK
                            break;
                        }
                    }

                    // Não conectado
                    throw new PointBlankException("Não conectado!");
                }
                catch
                {
                    contTentativa++;
                    if (contTentativa > 4)
                    {
                        throw new PointBlankException("Não foi possível conectar no servidor de aplicação");
                    }

                    Thread.Sleep(100);
                }
            }

            // Identifica se deve ser chamado a função de validação do BO (Normalmente utilizada em canais que não são inicializados toda vez)
            if (!validarMetodoBo)
            {
                return;
            }

            // Localizar método de validação adicional - Usa time-out do WCF
            MethodInfo metodoTmp = communicationObject.GetType().GetMethod("ValidarServicoWcf");
            if (metodoTmp == null)
            {
                throw new PointBlankException(string.Format("Não foi localizada a opção de validação para o canal wcf {0}", communicationObject.GetType().Name));
            }

            // Executar o método de validação
            metodoTmp.Invoke(communicationObject, null);
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
                    "http://{0}:{1}/PointBlankCore/{2}",
                    WcfNetwork.ConexaoEndereco,
                    WcfNetwork.ConexaoPorta,
                    servico.GetServiceType(false).Name);

                // Instânciar um host para o serviço
                ServiceHost host = new ServiceHost(servico.GetServiceType(false), new Uri[] { new Uri(enderecoTmp) });

                // Controle endpoint
                ServiceEndpoint endpoint = host.AddServiceEndpoint(servico.GetServiceType(true), WcfNetwork.ObterNovoNetTcpBinding(), enderecoTmp);

                // Opção para repassar ao cliente a mensagem do erro
                ServiceBehaviorAttribute debuggingBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                debuggingBehavior.IncludeExceptionDetailInFaults = true;

                // Total de conexões
                host.Description.Behaviors.Add(new ServiceThrottlingBehavior() { MaxConcurrentSessions = int.MaxValue, MaxConcurrentCalls = int.MaxValue, MaxConcurrentInstances = int.MaxValue });

                // Abrir o serviço
                host.Open();

                // Atualizar lista de controle
                WcfNetwork.ListaServiceHost.Add(host);
            }
            catch (AddressAlreadyInUseException)
            {
                throw new PointBlankException(string.Format("A porta {0} já está em uso. Verifique senão foi iniciado um outro serviço na mesma", WcfNetwork.ConexaoPorta));
            }
        }

        /// <summary>
        /// Criar canal de comunicação Wcf
        /// </summary>
        /// <typeparam name="TChannel">Contrato wcf</typeparam>
        /// <param name="enderecoUrl">Endereço url</param>
        /// <param name="enderecoPorta">Endereço Porta</param>
        /// <param name="chaveSeguranca">Chave de segurança (Descriptografada)</param>
        /// <param name="netTcpBinding">Objeto NetTcpBinding</param>
        /// <param name="nomeServico">Nome do serviço</param>
        /// <param name="duplex">Identifica se é um canal duplex</param>
        /// <param name="callBack">Eventos de callback para canal duplex</param>
        /// <returns>Canal criado</returns>
        public static TChannel CriarNovoCanalWcf<TChannel>(
            string enderecoUrl,
            int enderecoPorta,
            Binding netTcpBinding,
            string nomeServico,
            bool duplex,
            object callBack)
        {
            try
            {
                // Controle
                EndpointAddress endPoint = new EndpointAddress(
                    new Uri(string.Format("http://{0}:{1}/PointBlankCore/{2}", enderecoUrl, enderecoPorta, nomeServico)),
                    EndpointIdentity.CreateDnsIdentity("localhost"),
                    new AddressHeaderCollection());

                // ChannelFactory
                ChannelFactory<TChannel> scf = null;
                switch (duplex)
                {
                    case true:
                        scf = new DuplexChannelFactory<TChannel>(new InstanceContext(callBack), netTcpBinding, endPoint);
                        break;

                    default:
                        scf = new ChannelFactory<TChannel>(netTcpBinding, endPoint);
                        break;
                }

                // Criar o canal de comunicação
                TChannel canalRetorno = scf.CreateChannel();

                // Validar conexão
                try
                {
                    WcfNetwork.ValidarConexao(canalRetorno, enderecoUrl, enderecoPorta, false);
                }
                catch
                {
                    try
                    {
                        scf.Abort();
                    }
                    catch
                    {
                        Logger.Error("[WcfNetWork] - Erro ao Abortar o Canal WCF");
                    }

                    throw;
                }

                // Retorno
                return canalRetorno;
            }
            catch (PointBlankException)
            {
                throw;
            }
            catch (Exception exp)
            {
                // Erro gerado ao acionar o invoke do método de validação
                if ((exp is TargetInvocationException) && (exp.InnerException != null))
                {
                    // Erro de segurança
                    if ((exp.InnerException is MessageSecurityException) && (exp.InnerException.InnerException != null))
                    {
                        throw new PointBlankException(PointBlankException.GetMessageFull(exp.InnerException.InnerException, true));
                    }

                    // Identificar se foi erro de time-out
                    if (exp.InnerException is TimeoutException)
                    {
                        throw new PointBlankException("Não foi possível conectar no servidor de aplicação (Erro de Timeout)");
                    }

                    throw new PointBlankException("Não foi possível conectar no servidor de aplicação.", exp.InnerException);
                }

                // Alterar tipo do exception para o padrão
                throw new PointBlankException("Não foi possível conectar no servidor de aplicação!", exp);
            }
        }

        /// <summary>
        /// Criar canal de comunicação Wcf - Utilizando os dados padrões já informados
        /// </summary>
        /// <typeparam name="TChannel">Contrato wcf</typeparam>
        /// <param name="nomeServico">Nome do serviço</param>
        /// <param name="duplex">Identifica se é um canal duplex</param>
        /// <param name="callBack">Eventos de callback para canal duplex</param>
        /// <returns>Canal criado</returns>
        public static TChannel CriarNovoCanalWcf<TChannel>(string nomeServico, bool duplex, object callBack)
        {
            return CriarNovoCanalWcf<TChannel>(
                WcfNetwork.ConexaoEndereco,
                WcfNetwork.ConexaoPorta,
                WcfNetwork.ObterNovoNetTcpBinding(),
                nomeServico,
                duplex,
                callBack);
        }

        /// <summary>
        /// Fechar um canal de comunicação Wcf
        /// </summary>
        /// <param name="canal">Canal wcf a ser fechado</param>
        public static void FecharCanalWcf(IClientChannel canal)
        {
            try
            {
                canal.Abort();
            }
            catch
            {
                Logger.Info(string.Format("Não foi possível fechar o canal WCF ({0})", nameof(canal)));
            }

            try
            {
                canal.BeginClose(null, null);
            }
            catch
            {
                return;
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
            WcfNetwork.ConexaoEndereco = conexaoEndereco;
            WcfNetwork.ConexaoPorta = conexaoPorta;
            WcfNetwork.ConexaoChaveSeguranca = chaveSeguranca;
        }
        #endregion
    }
}
