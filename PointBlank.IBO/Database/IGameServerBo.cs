namespace PointBlank.IBO.Database
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Base;
    using OR.Database;

    [ServiceContract]
    [ServiceKnownType(typeof(GameServer))]
    [ServiceKnownType(typeof(GameChannel))]
    public interface IGameServerBo : IBaseBo<GameServer>
    {
        #region Controle WCF
        /// <summary>
        /// Método para identificar se o serviço está disponível
        /// </summary>
        [OperationContract]
        new void ValidarServicoWcf();
        #endregion

        #region Implementações
        /// <summary>
        /// Selecionar todos os registros do banco de dados
        /// </summary>
        /// <returns>A lista com os registros selecionados</returns>
        [OperationContract]
        IList<GameServer> ObterRelacaoTodos();

        /// <summary>
        /// Criar a estrutura padrão dos servidores
        /// </summary>
        /// <param name="enderecoIp">Endereço de IP atual</param>
        /// <param name="porta">Porta de conexão atual</param>
        [OperationContract]
        void CriarEstruturaPadrao(string enderecoIp, int porta);
        #endregion
    }
}