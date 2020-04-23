namespace PointBlank.BO.Database
{
    using Base;
    using IBO.Database;
    using NHibernate;
    using OR.Database;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='GameChannelBo'/>
    /// </summary>
    public class GameChannelBo : BaseBo<GameChannel>, IGameChannelBo
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        public GameChannelBo()
            : this(null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        /// <param name="sessaoControle">Sessão de controle a ser utilizada</param>
        public GameChannelBo(ISession sessaoControle)
            : base(sessaoControle)
        {
        }
        #endregion
    }
}