namespace PointBlank.Auth
{
    using System.Net.Sockets;
    using OR.Base;

    public class Network : BaseNetwork
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia de <see cref="Network"/>
        /// </summary>
        /// <param name="endereco">Endereço de conexão (Servidor)</param>
        /// <param name="porta">Porta de conexão (Servidor)</param>
        /// <param name="showHex">Se true indica que é para mostrar os hex recebidos</param>
        public Network(string endereco, int porta, bool showHex)
            : base(endereco, porta, showHex)
        {
        }
        #endregion

        #region Métodos
        /// <inheritdoc/>
        protected override BaseClient CriarCliente(TcpClient tcpClient, uint sessionId, bool showHex)
        {
            return new Client(tcpClient, sessionId, showHex);
        }
        #endregion
    }
}