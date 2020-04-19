namespace PointBlank.OR.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Library;
    using Library.Exceptions;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='BaseNetwork'/>
    /// </summary>
    public abstract class BaseNetwork : IDisposable
    {
        #region Campos
        /// <summary>
        /// Controle para a conexão (Servidor)
        /// </summary>
        public TcpListener server;

        /// <summary>
        /// Controle para o ultimo identificador de conexao
        /// </summary>
        private static int lastSessionId;

        /// <summary>
        /// Controle para a lista de clientes conectados
        /// </summary>
        private IList<BaseClient> listaClientes;

        /// <summary>
        /// Endereço de conexão (Servidor)
        /// </summary>
        private readonly string endereco;

        /// <summary>
        /// Porta de conexão (Servidor)
        /// </summary>
        private readonly int porta;

        /// <summary>
        /// controle para mostrar os hex recebidos
        /// </summary>
        private readonly bool showHex;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia de <see cref="BaseNetwork"/>
        /// </summary>
        /// <param name="endereco">Endereço de conexão (Servidor)</param>
        /// <param name="porta">Porta de conexão (Servidor)</param>
        public BaseNetwork(string endereco, int porta, bool showHex)
        {
            // Atualizar informações
            this.endereco = endereco;
            this.porta = porta;
            this.showHex = showHex;

            // Iniciar conexões
            this.server = new TcpListener(IPAddress.Parse(this.endereco), this.porta);
            this.server.Start();
            this.server.BeginAcceptTcpClient(this.AcceptCallback, this.server);

            // Log
            Logger.Info(string.Format("Informações da conexão: {0}:{1}", endereco, porta));
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Controle para a lista de clientes conectados
        /// </summary>
        public IList<BaseClient> ListaClientes
        {
            get
            {
                if (this.listaClientes == null)
                {
                    this.listaClientes = new List<BaseClient>();
                }

                return this.listaClientes;
            }

            private set
            {
                this.listaClientes = value;
            }
        }
        #endregion

        #region Métodos
        #region Públicos
        /// <summary>
        /// Envia um pacote para todos os jogadores conectados
        /// </summary>
        /// <param name="packet">Pacote a ser enviada</param>
        public void EnviarPacoteTodosJogadores(BaseSendPacket packet)
        {
            if (this.ListaClientes.Count <= 0)
            {
                throw new PointBlankException("Nenhum cliente conectado atualmente");
            }

            // Enviar pacote individualmente
            IList<Task> listaTarefas = new List<Task>();
            foreach (BaseClient client in this.ListaClientes)
            {
                try
                {
                    Task taskTmp = new Task(() => client.SendPacket(packet));
                    listaTarefas.Add(taskTmp);
                    taskTmp.Start();
                }
                catch (Exception exp)
                {
                    Logger.Error(exp, "Envio de pacote para todos os jogadores", !(exp is PointBlankException));
                }
            }

            // Aguardar execução
            Task.WaitAll(listaTarefas.ToArray());
        }
        #endregion

        #region Protegidos
        /// <summary>
        /// Criar conexão do cliente
        /// </summary>
        /// <param name="tcpClient">TcpClient atual</param>
        /// <param name="sessionId">Session id do cliente</param>
        /// <param name="showHex">Se true indica que é para mostrar os hex recebidos</param>
        /// <returns>O novo cliente</returns>
        protected abstract BaseClient CriarCliente(TcpClient tcpClient, uint sessionId, bool showHex);

        /// <summary>
        /// Adicionar o novo cliente a lista de clientes conectados
        /// </summary>
        /// <param name="novoCliente">Novo cliente conectado</param>
        protected virtual bool AdicionarNovoCliente(BaseClient novoCliente)
        {
            try
            {
                // Validar
                if ((novoCliente == null) || (!novoCliente.tcpClient.Connected))
                {
                    return false;
                }

                BaseClient oldClient = this.ListaClientes.Where(x => x.sessionId == novoCliente.sessionId).FirstOrDefault();
                switch (oldClient == null)
                {
                    case true:
                        this.ListaClientes.Add(novoCliente);
                        break;

                    default:
                        // Log de informação
                        Logger.Info(string.Format("SessionId: {0} já conectado a conexão antiga foi finalizada!", novoCliente.sessionId));

                        // Remover cliente antiga
                        this.ListaClientes.Remove(oldClient);

                        // Dispose da conexão antiga
                        oldClient.Dispose();

                        // Adicionar nova cliente
                        this.ListaClientes.Add(novoCliente);
                        break;
                }

                // Log de informação
                Logger.Info(string.Format("Novo cliente conectado: {0}{1}", Environment.NewLine, novoCliente.ToString()));
                return true;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                return false;
            }
        }

        /// <summary>
        /// Accept new Connections
        /// </summary>
        /// <param name="result">IAsyncResult</param>
        protected virtual void AcceptCallback(IAsyncResult result)
        {
            try
            {
                TcpListener server = (result.AsyncState as TcpListener);
                if (server == null)
                {
                    return;
                }

                TcpClient client = server.EndAcceptTcpClient(result);
                if (client == null)
                {
                    return;
                }

                // Incrementar
                Interlocked.Increment(ref lastSessionId);

                // Adicionar um novo cliente para conexão
                if (!this.AdicionarNovoCliente(this.CriarCliente(client, (uint)lastSessionId, this.showHex)))
                {
                    // Retornar o contador
                    Interlocked.Decrement(ref lastSessionId);
                }

                // Aceitar novas conexões
                this.server.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), this.server);
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("Erro ao aceitar nova conexão", exp));
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Método Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        #region Outros
        /// <summary>
        /// Método Dispose
        /// </summary>
        /// <param name="disposing">Parâmetro disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            // Finalizar instancias
            try
            {
                if (this.server != null)
                {
                    this.server.Stop();
                    this.server = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[Dispose] Erro ao finalizar a conexão do servidor", exp));
            }


            // Finalizar instancias (Clientes)
            foreach (BaseClient client in this.ListaClientes.ToList())
            {
                try
                {
                    this.ListaClientes.Remove(client);
                    client.Dispose();
                }
                catch (Exception exp)
                {
                    Logger.Error(new PointBlankException("[Dispose] Erro ao finalizar conexão do cliente", exp));
                }
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }
        #endregion
        #endregion
    }
}