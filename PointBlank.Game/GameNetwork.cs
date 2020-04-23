namespace PointBlank.Game
{
    using System.Net.Sockets;
    using OR.Game;

    public class GameNetwork : BaseNetwork
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia de <see cref="GameNetwork"/>
        /// </summary>
        /// <param name="endereco">Endereço de conexão (Servidor)</param>
        /// <param name="porta">Porta de conexão (Servidor)</param>
        /// <param name="showHex">Se true indica que é para mostrar os hex recebidos</param>
        public GameNetwork(string endereco, int porta, bool showHex)
            : base(endereco, porta, showHex)
        {
        }
        #endregion

        #region Métodos
        /// <inheritdoc/>
        protected override BaseClient CriarCliente(TcpClient tcpClient, uint sessionId, bool showHex)
        {
            return new GameClient(tcpClient, sessionId, showHex);
        }
        #endregion
    }
}