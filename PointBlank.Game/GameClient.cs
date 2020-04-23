namespace PointBlank.Game
{
    using System.Net.Sockets;
    using OR.Game;

    public class GameClient : BaseClient
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="GameClient"/>
        /// </summary>
        /// <param name="tcpClient">Sessão criada para a conexão</param>
        /// <param name="sessionSeed">Session seed da conexao</param>
        /// <param name="showHex">Se true indica que é para mostrar os hex recebidos</param>
        public GameClient(TcpClient tcpClient, uint sessionId, bool showHex)
            : base(tcpClient, sessionId, showHex)
        {
        }
        #endregion

        #region Métodos
        /// <inheritdoc/>
        protected override void RunPacketPartial(ushort opcode, byte[] buffer, ref BaseRecivePacket packet)
        {
        }
        #endregion
    }
}