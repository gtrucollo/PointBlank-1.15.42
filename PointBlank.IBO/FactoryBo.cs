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
                return Network.CriarNovoCanalWcf<IContaBo>("ContaBo", false, null);
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
                Network.FecharCanalWcf((IClientChannel)bo);
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
                Network.FecharCanalWcf((IClientChannel)bo);
            }
        }
        #endregion
    }
}
