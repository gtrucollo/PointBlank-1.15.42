﻿namespace PointBlank.BO.Database
{
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
    }
}