namespace PointBlank.OR.Base
{
    using System;
    using System.Collections.Generic;
    using NHibernate.Collection.Generic;

    /// <summary>
    /// Classe base para objetos OR's
    /// </summary>
    public abstract class BaseOr : ICloneable
    {
        #region Campos
        /// <summary>
        /// Identificador da instância
        /// </summary>
        private string idInternoUnico;
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de IdInternoUnico (Identificador único do objeto, para utilizar com o método "objeto.Equals()"))
        /// </summary>
        public virtual string IdInternoUnico
        {
            get
            {
                if (string.IsNullOrEmpty(this.idInternoUnico))
                {
                    this.idInternoUnico = Guid.NewGuid().ToString("N");
                }

                return this.idInternoUnico;
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Clonar o objeto
        /// </summary>
        /// <returns>Retorna o objeto clonado</returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Classes
        /// <summary>
        /// Classe para inicializar um lista em um objeto local, ou seja, sem o conhecimento de sessões do nHibernate
        /// </summary>
        /// <typeparam name="TType">Tipo do objeto</typeparam>
        public class PersistentGenericBagLocal<TType> : PersistentGenericBag<TType>
        {
            /// <summary>
            /// Inicia uma nova instância da classe <see cref="PersistentGenericBagLocal{TType}" />
            /// </summary>
            public PersistentGenericBagLocal()
            {
                // Instancia a lista dos objetos "TType"
                this.InternalBag = new List<TType>();

                // Define que o "objeto" já foi iniciado (Não válida a sessão do nHibernate)
                this.SetInitialized();
            }
        }
        #endregion
    }
}