namespace PointBlank.OR.Base
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Library;
    using Library.Exceptions;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='BaseClient'/>
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        #region Constantes
        /// <summary>
        /// Constante para time out
        /// </summary>
        public const int TIMEOUT_VALUE = 120;
        #endregion

        #region Campos
        /// <summary>
        /// Controle para a Sessão (Cliente)
        /// </summary>
        public readonly TcpClient tcpClient;

        /// <summary>
        /// Controle para a Sessão (Cliente)
        /// </summary>
        public readonly NetworkStream stream;

        /// <summary>
        /// Controle para o identificador da sessão
        /// </summary>
        public readonly uint sessionId;

        /// <summary>
        /// Controle para o endereço de ip do cliente
        /// </summary>
        public readonly byte[] remoteIPAddress;

        /// <summary>
        /// Controle para a porta de conexão udp
        /// </summary>
        public readonly int remoteUdpPort;

        /// <summary>
        /// controle para mostrar os hex recebidos
        /// </summary>
        public readonly bool showHex;

        /// <summary>
        /// controle para o buffer
        /// </summary>
        private readonly byte[] buffer;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="Client"/>
        /// </summary>
        /// <param name="tcpClient">Sessão criada para a conexão</param>
        /// <param name="sessionId">Session seed da conexao</param>
        /// <param name="showHex">Se true indica que é para mostrar os hex recebidos</param>
        public BaseClient(TcpClient tcpClient, uint sessionId, bool showHex)
        {
            // Atualizar informações
            this.tcpClient = tcpClient;
            this.tcpClient.NoDelay = true;
            this.tcpClient.ReceiveTimeout = BaseClient.TIMEOUT_VALUE;
            this.tcpClient.SendTimeout = BaseClient.TIMEOUT_VALUE;
            this.stream = this.tcpClient.GetStream();
            this.sessionId = sessionId;
            this.showHex = showHex;
            this.remoteIPAddress = ((IPEndPoint)this.tcpClient.Client.RemoteEndPoint).Address.GetAddressBytes();
            this.remoteUdpPort = 29890;
            this.buffer = new byte[2048];

            // Pacote inicial
            new Thread(() => this.RunStartPacket()).Start();

            // Iniciar a leitura dos dados
            new Thread(() => this.ReadPacket()).Start();
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de Shift
        /// </summary>
        public virtual int Shift
        {
            get
            {
                return (int)((this.sessionId + 29890) % 7) + 1;
            }
        }

        /// <summary>
        /// Obtém o valor de IdConta
        /// </summary>
        public long IdConta
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region Métodos
        #region Packets
        /// <summary>
        /// Envia um pacote para o servidor
        /// </summary>
        /// <param name="packet">Pacote a ser enviado</param>
        public virtual void SendPacket(BaseSendPacket packet)
        {
            // Validar
            if (packet == null)
            {
                return;
            }

            try
            {
                packet.WriteStream();
                byte[] packetBytes = packet.mstream.ToArray();
                if (packetBytes.Length < 2)
                {
                    return;
                }

                ushort size = Convert.ToUInt16(packetBytes.Length - 2);
                List<byte> list = new List<byte>(packetBytes.Length + 2);
                list.AddRange(BitConverter.GetBytes(size));
                list.AddRange(packetBytes);

                if (list.Count > 0)
                {
                    this.stream.Write(list.ToArray(), 0, list.Count);
                }

                list.Clear();
            }
            catch (Exception exp)
            {
                Logger.Error(
                    new PointBlankException(string.Format("[SendPacket] Erro no ao efetuar o envio da packet: {0}", packet.GetType().Name), exp));
            }
            finally
            {
                try
                {
                    packet.Dispose();
                    packet = null;
                }
                catch (Exception exp)
                {
                    Logger.Error(new PointBlankException("[SendPacket] Erro no ao efetuar 'Dispose' da packet", exp));
                }
            }
        }

        /// <summary>
        /// Leitura dos dados Recebidos
        /// </summary>
        protected virtual void ReadPacket()
        {
            try
            {
                this.stream.BeginRead(this.buffer, 0, this.buffer.Length, this.OnReceiveCallback, this.stream);
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[ReadPacket] Erro no ao efetuar 'O Recebimento' da packet", exp));
            }
        }

        /// <summary>
        /// Leitura dos dados Recebidos
        /// </summary>
        protected virtual void OnReceiveCallback(IAsyncResult result)
        {
            try
            {
                int bytesCount = this.stream.EndRead(result);
                if (bytesCount <= 0)
                {
                    return;
                }

                byte[] babyBuffer = new byte[bytesCount];
                Array.Copy(this.buffer, 0, babyBuffer, 0, bytesCount);
                int length = BitConverter.ToUInt16(babyBuffer, 0) & 0x7FFF;
                byte[] buffer = new byte[length + 2];
                Array.Copy(babyBuffer, 2, buffer, 0, buffer.Length);
                this.RunPacket(this.Decrypt(buffer, this.Shift));
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[OnReceiveCallback] Erro no ao efetuar 'O Recebimento' da packet", exp));
            }
            finally
            {
                // Sempre executar por ultimo (Inicia a nova leitura dos dados)
                new Thread(() => ReadPacket()).Start();
            }
        }

        /// <summary>
        /// Envia o pacote inicial
        /// </summary>
        protected virtual void RunStartPacket()
        {
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Método Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        #region Outros
        /// <summary>
        /// Método Dispose
        /// </summary>
        /// <param name="disposing">Parâmetro disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            // Finalizar instancias
            try
            {
                if (this.tcpClient != null)
                {
                    this.tcpClient.Close();
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[Dispose] Erro ao finalizar conexão do cliente", exp));
            }

            // Finalizar instancias
            try
            {
                if (this.stream != null)
                {
                    this.stream.Close();
                    this.stream.Dispose();
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[Dispose] Erro ao finalizar conexão do cliente", exp));
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }

        /// <summary>
        /// Descriptrografar os dados
        /// </summary>
        /// <param name="buffer">Buffer recebido</param>
        /// <param name="shift">Shift da conexão</param>
        /// <returns>Os dados no formato Bytearray</returns>
        protected virtual byte[] Decrypt(byte[] buffer, int shift)
        {
            byte last = buffer[buffer.Length - 1];
            for (int i = buffer.Length - 1; i > 0; --i)
            {
                buffer[i] = (byte)(((buffer[i - 1] & 255) << (8 - shift)) | ((buffer[i] & 255) >> shift));
            }

            buffer[0] = (byte)((last << (8 - shift)) | ((buffer[0] & 255) >> shift));

            // Retorno
            return buffer;
        }

        /// <summary>
        /// Executa os Pacotes
        /// </summary>
        /// <param name="buffer">Buffer Recebido</param>
        protected abstract void RunPacket(byte[] buffer);

        /// <summary>
        /// Obtém informações da Client(Conexão)
        /// </summary>
        /// <returns>Retorna as informações</returns>
        public override string ToString()
        {
            return string.Format(
                "Session Id...: {1}{0}" +
                "Shift........: {2}",
                Environment.NewLine,
                this.sessionId,
                this.Shift);
        }
        #endregion
        #endregion
    }
}