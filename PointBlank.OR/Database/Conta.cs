namespace PointBlank.OR.Database
{
    using System;
    using System.ComponentModel;
    using Base;

    /// <summary>
    /// Classe de controle (Contas)
    /// </summary>
    public class Conta : BaseOr
    {
        #region Enumeradores
        /// <summary>
        /// Enumerador do Status das Contas (Usado para Formatado)
        /// </summary>
        public enum StatusEnum
        {
            /// <summary>
            /// Status - Inativa
            /// </summary>
            [Description("Conta Inativa")]
            Inativa = 0,

            /// <summary>
            /// Status - Ativa
            /// </summary>
            [Description("Conta Ativa")]
            Ativa = 1,

            /// <summary>
            /// Status - Banida
            /// </summary>
            [Description("Conta Banida")]
            Banida = 2
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de IdInternoUnico (Identificador único do objeto, para utilizar com o método "objeto.Equals()"))
        /// </summary>
        public override string IdInternoUnico
        {
            get
            {
                if (this.IdConta > 0)
                {
                    return this.IdConta.ToString();
                }

                return base.IdInternoUnico;
            }
        }

        /// <summary>
        /// Obtém ou define o valor de IdConta
        /// </summary>
        public virtual long IdConta { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Status
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// Obtém o valor de StatusFormatado
        /// </summary>
        public virtual StatusEnum StatusFormatado
        {
            get
            {
                return (StatusEnum)this.Status;
            }
        }

        /// <summary>
        /// Obtém ou define o valor de Lancamento
        /// </summary>
        public virtual DateTime Lancamento { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Alteracao
        /// </summary>
        public virtual DateTime Alteracao { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Usuario
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Senha
        /// </summary>
        public virtual string Senha { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Ip
        /// </summary>
        public virtual string Ip { get; set; }

        /// <summary>
        /// Obtém ou define o valor de Mac
        /// </summary>
        public virtual string Mac { get; set; }
        #endregion
    }
}