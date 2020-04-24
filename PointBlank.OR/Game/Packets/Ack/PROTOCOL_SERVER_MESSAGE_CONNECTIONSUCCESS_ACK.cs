namespace PointBlank.OR.Game.Packets.Ack
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Game;
    using OR.Database;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='PROTOCOL_SERVER_MESSAGE_CONNECTIONSUCCESS_ACK'/>
    /// </summary>
    [SendPacket(Id = 2049)]
    public sealed class PROTOCOL_SERVER_MESSAGE_CONNECTIONSUCCESS_ACK : BaseSendPacket
    {
        #region Campos
        /// <summary>
        /// Controle para client
        /// </summary>
        private readonly BaseClient client;

        /// <summary>
        /// Controle para listaServers
        /// </summary>
        private readonly IList<GameServer> listaServers;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="PROTOCOL_SERVER_MESSAGE_CONNECTIONSUCCESS_ACK"/>
        /// </summary>
        /// <param name="listaServers">Lista de Servidores</param>
        public PROTOCOL_SERVER_MESSAGE_CONNECTIONSUCCESS_ACK(BaseClient client, IList<GameServer> listaServers)
        {
            // Atualizar informações
            this.client = client;
            this.listaServers = listaServers;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Executa a escrita das informações
        /// </summary>
        public override void WriteStream()
        {
            Write<uint>(this.client.sessionId);
            Write<byte[]>(this.client.remoteIPAddress);
            Write<short>((short)29890);
            Write<short>((short)this.client.sessionId);
            foreach (GameChannel gameChannel in this.listaServers.Select(y => y.ListaGameChannel).ToList())
            {
                Write<byte>((byte)gameChannel.Tipo);
            }

            Write<byte>((byte)1); // ?

            // Informações por servidor
            Write<int>(this.listaServers.Count);
            foreach (GameServer gameServer in this.listaServers)
            {
                Write<int>(gameServer.Disponivel);
                Write<byte[]>(IPAddress.Parse(gameServer.EnderecoIp).GetAddressBytes());
                Write<short>((short)gameServer.Porta);
                Write<byte>((byte)gameServer.Tipo);
                Write<short>((short)gameServer.MaximoJogadores);
                Write<int>(0);
            }

            Write<short>((short)1);
            Write<short>((short)300);
            Write<int>(200);
            Write<int>(100);
            Write<byte>((byte)1);
            Write<int>(3);
            Write<int>(100);
            Write<int>(150);
        }
        #endregion
    }
}