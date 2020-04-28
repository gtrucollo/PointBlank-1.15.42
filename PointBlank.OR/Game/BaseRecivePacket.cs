namespace PointBlank.OR.Game
{
    using System;
    using System.IO;
    using System.Text;
    using Library;
    using Library.Exceptions;

    public abstract class BaseRecivePacket
    {
        #region Campos
        /// <summary>
        /// Controle para o buffer recebido
        /// </summary>
        public readonly byte[] buffer;

        /// <summary>
        /// Controle para a cliente
        /// </summary>
        protected readonly BaseClient cliente;

        /// <summary>
        /// Controle para a stream do buffer recebido
        /// </summary>
        private readonly MemoryStream stream;

        /// <summary>
        /// Controle para a stream do buffer recebido
        /// </summary>
        private readonly BinaryReader reader;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia de <see cref="BaseRecivePacket"/>
        /// </summary>
        /// <param name="cliente">Cliente atual</param>
        /// <param name="buffer">Buffer recebido</param>
        /// <param name="ignorarBytes">Se False irá não será ignorardo bytes</param>
        public BaseRecivePacket(BaseClient cliente, byte[] buffer)
        {
            if (this.cliente == null)
            {
                throw new PointBlankException("Não foi informado a conexão do cliente!");
            }

            // Atualizar informações
            this.cliente = cliente;
            this.buffer = buffer;
            switch (this.IgnorarBytes && (this.buffer.Length > 4))
            {
                case true:
                    this.stream = new MemoryStream(this.buffer, 4, this.buffer.Length - 4);
                    break;

                default:
                    this.stream = new MemoryStream(this.buffer, 0, this.buffer.Length);
                    break;
            }

            this.reader = new BinaryReader(this.stream);
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de IgnorarBytes
        /// </summary>
        protected virtual bool IgnorarBytes
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Métodos
        #region Protegidos
        /// <summary>
        /// Método que Faz a Leitura dos Dados
        /// </summary>
        protected virtual void Read()
        {
        }

        /// <summary>
        /// Método que Executa os dados
        /// </summary>
        protected virtual void Run()
        {
        }

        /// <summary>
        /// Executa a leitura de um inteiro
        /// </summary>
        /// <returns>o valor convertido</returns>
        protected internal int ReadInt()
        {
            try
            {
                return this.reader.ReadInt32();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Executa a leitura de um byte
        /// </summary>
        /// <returns>o valor convertido</returns>
        protected internal byte ReadByte()
        {
            try
            {
                return this.reader.ReadByte();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Executa a leitura de um byte array
        /// </summary>
        /// <param name="size">quantidade de bytes</param>
        /// <returns>o valor convertido</returns>
        protected internal byte[] ReadByteArray(int size)
        {
            try
            {
                return reader.ReadBytes(size);
            }
            catch
            {
                return new byte[] { };
            }
        }

        /// <summary>
        /// Executa a leitura de um short
        /// </summary>
        /// <returns>o valor convertido</returns>
        protected internal short ReadShort()
        {
            try
            {
                return reader.ReadInt16();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Executa a leitura de um double
        /// </summary>
        /// <returns>o valor convertido</returns>
        protected internal double ReadDouble()
        {
            try
            {
                return reader.ReadDouble();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Executa a leitura de um long
        /// </summary>
        /// <returns>o valor convertido</returns>
        protected internal long ReadLong()
        {
            try
            {
                return reader.ReadInt64();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Executa a leitura de uma string
        /// </summary>
        /// <param name="length">tamanho da string</param>
        /// <returns>o valor convertido</returns>
        protected internal string ReadString(int length)
        {
            try
            {
                return Encoding.GetEncoding(1251).GetString(this.ReadByteArray(length)).Replace("\0", string.Empty).Trim();
            }
            catch
            {
                return string.Empty;
            }
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

            try
            {
                if (this.stream != null)
                {
                    this.stream.Dispose();
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[BaseRecivePacket] Erro ao limpar os dados", exp));
            }

            try
            {
                if (this.reader != null)
                {
                    this.reader.Dispose();
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[BaseRecivePacket] Erro ao limpar os dados", exp));
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }
        #endregion
        #endregion
    }
}