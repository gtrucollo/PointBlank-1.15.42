namespace PointBlank.Auth
{
    using System.Net.Sockets;
    using OR.Base;
    using OR.Library.Exceptions;

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
        protected override void RunPacket(byte[] buffer)
        {
            throw new PointBlankException("Opção em desenvolvimento!");
        }
        #endregion
    }
}