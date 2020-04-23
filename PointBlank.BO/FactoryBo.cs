namespace PointBlank.BO
{
    using System;
    using Database;
    using NHibernate;

    /// <summary>
    /// Classe que expõe os BO's.
    /// </summary>
    public class FactoryBo : IDisposable
    {
        #region Campos
        /// <summary>
        /// Sessão de controle
        /// </summary>
        private readonly ISession sessaoControle;

        /// <summary>
        /// Controle para contaBo
        /// </summary>
        private ContaBo contaBo;

        /// <summary>
        /// Controle para gameServerBo
        /// </summary>
        private GameServerBo gameServerBo;

        /// <summary>
        /// Controle para GameChannelBo
        /// </summary>
        private GameChannelBo gameChannelBo;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FactoryBo"/>
        /// </summary>
        public FactoryBo()
            : this(null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FactoryBo"/>
        /// </summary>
        /// <param name="sessaoControle">Sessão de controle a ser utilizada</param>
        public FactoryBo(ISession sessaoControle)
        {
            if (sessaoControle == null)
            {
                this.sessaoControle = SessionManager.ObterNovaSessao();
                return;
            }

            this.sessaoControle = sessaoControle;
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de ContaBo
        /// </summary>
        public ContaBo ContaBo
        {
            get
            {
                if (this.contaBo == null)
                {
                    this.contaBo = new ContaBo(this.sessaoControle);
                }

                return this.contaBo;
            }
        }

        /// <summary>
        /// Obtém o valor de GameServerBo
        /// </summary>
        public GameServerBo GameServerBo
        {
            get
            {
                if (this.gameServerBo == null)
                {
                    this.gameServerBo = new GameServerBo(this.sessaoControle);
                }

                return this.gameServerBo;
            }
        }

        /// <summary>
        /// Obtém o valor de GameChannelBo
        /// </summary>
        public GameChannelBo GameChannelBo
        {
            get
            {
                if (this.gameChannelBo == null)
                {
                    this.gameChannelBo = new GameChannelBo(this.sessaoControle);
                }

                return this.gameChannelBo;
            }
        }
        #endregion

        #region Métodos
        #region IDisposable Members
        /// <summary>
        /// Método Dispose
        /// </summary>
        public void Dispose()
        {
            if (this.sessaoControle != null)
            {
                this.sessaoControle.Dispose();
            }

            // Dispose dos BO's
            if (this.contaBo != null)
            {
                this.contaBo.Dispose();
                this.contaBo = null;
            }

            if (this.gameServerBo != null)
            {
                this.gameServerBo.Dispose();
                this.gameServerBo = null;
            }

            if (this.gameChannelBo != null)
            {
                this.gameChannelBo.Dispose();
                this.gameChannelBo = null;
            }

            // Formar limpeza da memoria
            GC.Collect();
        }
        #endregion
        #endregion
    }
}