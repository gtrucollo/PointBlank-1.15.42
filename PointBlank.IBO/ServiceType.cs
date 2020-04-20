namespace PointBlank.IBO
{
    using System;

    public class ServiceType
    {
        #region Campos
        /// <summary>
        /// Controle para o tipo do objeto
        /// </summary>
        private readonly Type tipoObjeto;

        /// <summary>
        /// Controle pra o tipo interface do objeto
        /// </summary>
        private readonly Type tipoObjetoInterface;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ServiceType"/>
        /// </summary>
        /// <param name="tipoObjeto">Tipo referente a implementação</param>
        /// <param name="tipoObjetoInterface">Tipo referente a interface</param>
        public ServiceType(Type tipoObjeto, Type tipoObjetoInterface)
        {
            this.tipoObjeto = tipoObjeto;
            this.tipoObjetoInterface = tipoObjetoInterface;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Retorna o tipo do serviço
        /// </summary>
        /// <param name="objetoInterface">Se true indica que é da interface</param>
        /// <returns>O tipo</returns>
        public Type GetServiceType(bool objetoInterface)
        {
            return objetoInterface ? this.tipoObjetoInterface : this.tipoObjeto;
        }
        #endregion
    }
}