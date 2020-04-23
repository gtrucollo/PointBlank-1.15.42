namespace PointBlank.OR.Database
{
    using Base;

    /// <summary>
    /// Classe de controle (Canais)
    /// </summary>
    public class GameChannel : BaseOr
    {
        #region Propriedades
        /// <summary>
        /// Obtém ou define IdGameChannel
        /// </summary>
        public virtual int IdGameChannel { get; set; }

        /// <summary>
        /// Obtém ou define IdGameServer
        /// </summary>
        public virtual GameServer IdGameServer { get; set; }

        /// <summary>
        /// Obtém ou define Tipo
        /// </summary>
        public virtual int Tipo { get; set; }

        /// <summary>
        /// Obtém ou define Porta
        /// </summary>
        public virtual int Porta { get; set; }

        /// <summary>
        /// Obtém ou define MensagemTopo
        /// </summary>
        public virtual string MensagemTopo { get; set; }

        /// <summary>
        /// Obtém ou Define Nome
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// Obtém ou define IP
        /// </summary>
        public virtual string IP { get; set; }

        /// <summary>
        /// Obtém ou define MaximoJogadores
        /// </summary>
        public virtual int MaximoJogadores { get; set; }
        #endregion
    }
}