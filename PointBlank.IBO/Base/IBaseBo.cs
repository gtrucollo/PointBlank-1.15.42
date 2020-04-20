namespace PointBlank.IBO.Base
{
    using System.Collections.Generic;
    using OR.Base;

    public interface IBaseBo<TType> where TType : BaseOr
    {
        #region Insert e Update
        /// <summary>
        /// Insere ou altera um objeto no banco de dados
        /// </summary>      
        /// <param name="objeto">Parâmetro Objeto</param>
        /// <returns>O objeto após a persistência</returns>
        TType InsertUpdate(TType objeto);

        /// <summary>
        /// Insere ou altera vários objetos no banco de dados
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="listaObjetos">Lista com os Objetos</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        void InsertUpdate(List<TType> listaObjetos);
        #endregion

        #region Delete
        /// <summary>
        /// Exclui um objeto do banco de dados.
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="objeto">Parâmetro objeto</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        void Delete(TType objeto);
        #endregion
    }
}
