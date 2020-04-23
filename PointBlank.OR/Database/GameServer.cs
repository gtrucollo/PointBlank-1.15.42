namespace PointBlank.OR.Database
{
    using System.Collections.Generic;
    using Base;

    /// <summary>
    /// Classe de controle (Servidores)
    /// </summary>
    public class GameServer : BaseOr
    {
        #region Campos
        /// <summary>
        /// Controle para a lista de canais
        /// </summary>
        private IList<GameChannel> listaGameChannel;
        #endregion

        #region Enumeradores
        /// <summary>
        /// Enumerador DisponivelEnum
        /// </summary>
        public enum DisponivelEnum
        {
            /// <summary>
            /// Opção - Não
            /// </summary>
            Nao = 0,

            /// <summary>
            /// Opção - Sim
            /// </summary>
            Sim = 1
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém ou define IdGameServer 
        /// </summary>
        public virtual int IdGameServer { get; set; }

        /// <summary>
        /// Obtém ou define Disponivel 
        /// </summary>
        public virtual int Disponivel { get; set; }

        /// <summary>
        /// Obtém o valor de DisponivelFormatado 
        /// </summary>
        public virtual DisponivelEnum DisponivelFormatado
        {
            get
            {
                return (DisponivelEnum)this.Disponivel;
            }
        }

        /// <summary>
        /// Obtém ou define Nome 
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// Obtém ou define Tipo 
        /// </summary>
        public virtual int Tipo { get; set; }

        /// <summary>
        /// Obtém ou define MaximoJogadores 
        /// </summary>
        public virtual int MaximoJogadores { get; set; }

        /// <summary>
        /// Obtém ou define EnderecoIp 
        /// </summary>
        public virtual string EnderecoIp { get; set; }

        /// <summary>
        /// Obtém ou define Porta 
        /// </summary>
        public virtual int Porta { get; set; }

        /// <summary>
        /// Obtém ou define Senha
        /// </summary>
        public virtual string Senha { get; set; }

        /// <summary>
        /// Obtém ou define a lista de ListaGameChannel
        /// </summary>
        public virtual IList<GameChannel> ListaGameChannel
        {
            get
            {
                if (this.listaGameChannel == null)
                {
                    this.listaGameChannel = new PersistentGenericBagLocal<GameChannel>();
                }

                return this.listaGameChannel;
            }
        }
        #endregion
    }
}