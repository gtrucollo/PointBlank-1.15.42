namespace PointBlank.BO.Database
{
    using System.Collections.Generic;
    using Base;
    using IBO.Database;
    using NHibernate;
    using OR.Database;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='GameServerBo'/>
    /// </summary>
    public class GameServerBo : BaseBo<GameServer>, IGameServerBo
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        public GameServerBo()
            : this(null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        /// <param name="sessaoControle">Sessão de controle a ser utilizada</param>
        public GameServerBo(ISession sessaoControle)
            : base(sessaoControle)
        {
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Selecionar todos os registros do banco de dados
        /// </summary>
        /// <returns>A lista com os registros selecionados</returns>
        public IList<GameServer> ObterRelacaoTodos()
        {
            return this.SessaoControle.QueryOver<GameServer>()
                .OrderBy(x => x.IdGameServer).Asc
                .List<GameServer>();
        }

        /// <summary>
        /// Criar a estrutura padrão dos servidores
        /// </summary>
        /// <param name="enderecoIp">Endereço de IP atual</param>
        /// <param name="porta">Porta de conexão atual</param>
        public void CriarEstruturaPadrao(string enderecoIp, int porta)
        {
            // Validar
            if (this.ObterRelacaoTotal() > 0)
            {
                return;
            }

            try
            {
                // Iniciar transação
                this.IniciarTransacao();

                // Criar servidor hidden
                GameServer serverHidde =
                    new GameServer()
                    {
                        Disponivel = (int)GameServer.DisponivelEnum.Sim,
                        MaximoJogadores = 50,
                        Nome = "Hidden",
                        EnderecoIp = enderecoIp,
                        Porta = porta
                    };

                // Gravar
                this.InsertUpdate(serverHidde);

                // Criar servidor comum
                GameServer serverComum =
                    new GameServer()
                    {
                        Disponivel = (int)GameServer.DisponivelEnum.Sim,
                        MaximoJogadores = 50,
                        Nome = "First",
                        EnderecoIp = enderecoIp,
                        Porta = porta + 1,
                    };

                // Criar canais
                for (int contChannel = 1; contChannel <= 10; contChannel++)
                {
                    serverComum.ListaGameChannel.Add(
                        new GameChannel()
                        {
                            IdGameChannel = contChannel,
                            IdGameServer = serverComum,
                            IP = enderecoIp,
                            Porta = serverComum.Porta,
                            Nome = string.Format("Livre - {0}", contChannel),
                            MaximoJogadores = serverComum.MaximoJogadores / 10,
                            MensagemTopo = string.Format("Bem vindo ao servidor Livre - {0}", contChannel),
                        });
                }

                // Gravar
                this.InsertUpdate(serverComum);

                // Gravar
                this.GravarTransacao();
            }
            catch
            {
                // Cancelar transação
                this.CancelarTransacao();
                throw;
            }
        }
        #endregion
    }
}