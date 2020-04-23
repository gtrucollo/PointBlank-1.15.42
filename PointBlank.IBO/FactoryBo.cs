namespace PointBlank.IBO
{
    using System;
    using System.ServiceModel;
    using Database;

    public static class FactoryBo
    {
        #region Propriedades
        /// <summary>
        /// Obtém ContaBo
        /// </summary>
        private static IContaBo ContaBo
        {
            get
            {
                return WcfNetwork.CriarNovoCanalWcf<IContaBo>("ContaBo", false, null);
            }
        }

        /// <summary>
        /// Obtém GameServerBo
        /// </summary>
        private static IGameServerBo GameServerBo
        {
            get
            {
                return WcfNetwork.CriarNovoCanalWcf<IGameServerBo>("GameServerBo", false, null);
            }
        }

        /// <summary>
        /// Obtém GameChannelBo
        /// </summary>
        private static IGameChannelBo GameChannelBo
        {
            get
            {
                return WcfNetwork.CriarNovoCanalWcf<IGameChannelBo>("GameChannelBo", false, null);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Obter IContaBo
        /// </summary>
        /// <typeparam name="TReturn">Tipo do retorno</typeparam>
        /// <param name="code">Ação a ser executada</param>
        /// <returns>Retorno da ação</returns>
        public static TReturn Conta<TReturn>(Func<IContaBo, TReturn> code)
        {
            IContaBo bo = FactoryBo.ContaBo;
            try
            {
                return code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }

        /// <summary>
        /// Obter IContaBo
        /// </summary>
        /// <param name="code">Ação a ser executada</param>
        public static void Conta(Action<IContaBo> code)
        {
            IContaBo bo = FactoryBo.ContaBo;
            try
            {
                code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }

        /// <summary>
        /// Obter IGameServerBo
        /// </summary>
        /// <typeparam name="TReturn">Tipo do retorno</typeparam>
        /// <param name="code">Ação a ser executada</param>
        /// <returns>Retorno da ação</returns>
        public static TReturn GameServer<TReturn>(Func<IGameServerBo, TReturn> code)
        {
            IGameServerBo bo = FactoryBo.GameServerBo;
            try
            {
                return code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }

        /// <summary>
        /// Obter IGameServerBo
        /// </summary>
        /// <param name="code">Ação a ser executada</param>
        public static void GameServer(Action<IGameServerBo> code)
        {
            IGameServerBo bo = FactoryBo.GameServerBo;
            try
            {
                code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }

        /// <summary>
        /// Obter IGameChannelBo
        /// </summary>
        /// <typeparam name="TReturn">Tipo do retorno</typeparam>
        /// <param name="code">Ação a ser executada</param>
        /// <returns>Retorno da ação</returns>
        public static TReturn GameChannel<TReturn>(Func<IGameChannelBo, TReturn> code)
        {
            IGameChannelBo bo = FactoryBo.GameChannelBo;
            try
            {
                return code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }

        /// <summary>
        /// Obter IGameChannelBo
        /// </summary>
        /// <param name="code">Ação a ser executada</param>
        public static void GameChannel(Action<IGameChannelBo> code)
        {
            IGameChannelBo bo = FactoryBo.GameChannelBo;
            try
            {
                code(bo);
            }
            finally
            {
                WcfNetwork.FecharCanalWcf((IClientChannel)bo);
            }
        }
        #endregion
    }
}
