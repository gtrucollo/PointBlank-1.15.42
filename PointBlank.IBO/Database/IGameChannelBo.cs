namespace PointBlank.IBO.Database
{
    using System.ServiceModel;
    using IBO.Base;
    using OR.Database;

    [ServiceContract]
    [ServiceKnownType(typeof(GameChannel))]
    public interface IGameChannelBo : IBaseBo<GameChannel>
    {
        #region Controle WCF
        /// <summary>
        /// Método para identificar se o serviço está disponível
        /// </summary>
        [OperationContract]
        new void ValidarServicoWcf();
        #endregion
    }
}