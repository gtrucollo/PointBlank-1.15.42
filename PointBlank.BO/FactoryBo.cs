namespace PointBlank.BO
{
    using NHibernate;
    using System;

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
        }
        #endregion
        #endregion
    }
}