namespace PointBlank.IBO.Base
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using OR.Base;

    [ServiceContract]
    [ServiceKnownType(typeof(BaseOr))]
    public interface IBaseBo<TType> where TType : BaseOr
    {
        #region Insert e Update
        /// <summary>
        /// Insere ou altera um objeto no banco de dados
        /// </summary>      
        /// <param name="objeto">Parâmetro Objeto</param>
        /// <returns>O objeto após a persistência</returns>
        [OperationContract(Name = "InsertUpdade")]
        TType InsertUpdate(TType objeto);

        /// <summary>
        /// Insere ou altera vários objetos no banco de dados
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="listaObjetos">Lista com os Objetos</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        [OperationContract(Name = "InsertUpdade2")]
        void InsertUpdate(List<TType> listaObjetos);
        #endregion

        #region Delete
        /// <summary>
        /// Exclui um objeto do banco de dados.
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="objeto">Parâmetro objeto</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        [OperationContract(Name = "Delete")]
        void Delete(TType objeto);
        #endregion

        #region Controle WCF
        /// <summary>
        /// Método para identificar se o serviço está disponível
        /// </summary>
        [OperationContract(Name = "ValidarServicoWcf_Base")]
        void ValidarServicoWcf();
        #endregion
    }
}
