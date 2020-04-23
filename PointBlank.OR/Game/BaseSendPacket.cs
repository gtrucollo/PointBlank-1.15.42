namespace PointBlank.OR.Game
{
    using System;
    using System.IO;
    using System.Text;
    using Library;
    using Library.Exceptions;

    public abstract class BaseSendPacket : IDisposable
    {
        #region Campos
        /// <summary>
        /// Obtém o valor de protocol
        /// </summary>
        public short protocol;

        /// <summary>
        /// Stream da memoria
        /// </summary>
        public MemoryStream mstream;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="BaseSendPacket"/>
        /// </summary>
        public BaseSendPacket(short protocol)
        {
            this.protocol = protocol;
            byte[] bytes = BitConverter.GetBytes(protocol);
            this.mstream = new MemoryStream();
            this.mstream.Write(bytes, 0, bytes.Length);
        }
        #endregion

        #region Métodos
        #region Públicos
        /// <summary>
        /// Método que faz a escrita dos dados
        /// </summary>
        public abstract void WriteStream();
        #endregion

        #region Protegidos
        /// <summary>
        /// Faz a escrita de um valor do tipo na memoria 
        /// </summary>
        /// <typeparam name="TType">Tipo a ser escrito na memoria</typeparam>
        /// <param name="value">Valor que será escrito na memoria</param>
        protected internal void Write<TType>(object value)
        {
            // Opções de acordo com o tipo do valor repassado
            byte[] valorTmp;
            switch (Type.GetTypeCode(typeof(TType)))
            {
                case TypeCode.Int32:
                    valorTmp = BitConverter.GetBytes((int)value);
                    break;

                case TypeCode.Int64:
                    valorTmp = BitConverter.GetBytes((long)value);
                    break;

                case TypeCode.Int16:
                    valorTmp = BitConverter.GetBytes((short)value);
                    break;

                case TypeCode.UInt32:
                    valorTmp = BitConverter.GetBytes((uint)value);
                    break;

                case TypeCode.UInt64:
                    valorTmp = BitConverter.GetBytes((ulong)value);
                    break;

                case TypeCode.UInt16:
                    valorTmp = BitConverter.GetBytes((ushort)value);
                    break;

                case TypeCode.Double:
                    valorTmp = BitConverter.GetBytes((double)value);
                    break;

                case TypeCode.Boolean:
                    valorTmp = BitConverter.GetBytes((bool)value);
                    break;

                case TypeCode.Byte:
                    this.mstream.WriteByte((byte)value);
                    return;

                case TypeCode.Object:
                    if (value is byte[])
                    {
                        valorTmp = (value as byte[]);
                        break;
                    }

                    throw new PointBlankException(
                        string.Format("TypeCode \"{0}\" não definido. Erro no desenvolvimento", Type.GetTypeCode(typeof(TType))));

                case TypeCode.String:
                    throw new PointBlankException("Para o tipo \"String deve ser utilizado o método WriteS\". Erro no desenvolvimento");

                default:
                    throw new PointBlankException(
                        string.Format("TypeCode \"{0}\" não definido. Erro no desenvolvimento", Type.GetTypeCode(typeof(TType))));
            }

            // Validar
            if (valorTmp == null)
            {
                throw new PointBlankException("Nenhum valor foi atribuido. Erro no desenvolvimento");
            }

            // Escrever na memoria
            this.mstream.Write(valorTmp, 0, valorTmp.Length);
        }

        /// <summary>
        /// Escreve uma String
        /// </summary>
        /// <param name="value">Valor a ser enviado</param>
        protected internal void WriteS(string conteudo, int tamanho)
        {
            // Validar
            if (string.IsNullOrWhiteSpace(conteudo))
            {
                conteudo = string.Empty;
            }

            // Obter bytes da string
            byte[] arrayTmp = Encoding.GetEncoding(1251).GetBytes(conteudo);
            this.mstream.Write(arrayTmp, 0, arrayTmp.Length);
            arrayTmp = new byte[tamanho - conteudo.Length];
            this.mstream.Write(arrayTmp, 0, arrayTmp.Length);
        }

        /// <summary>
        /// Escreve uma String
        /// </summary>
        /// <param name="Conteudo">Valor a ser enviado</param>
        protected internal void WriteUnicodeString(string conteudo)
        {
            if (!string.IsNullOrWhiteSpace(conteudo))
            {
                byte[] arrayTmp = Encoding.Unicode.GetBytes(conteudo);
                this.mstream.Write(arrayTmp, 0, arrayTmp.Length);
            }

            Write<short>((short)0);
        }
        #endregion
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
                if (this.mstream != null)
                {
                    this.mstream.Close();
                    this.mstream.Dispose();
                    this.mstream = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[Dispose] Erro ao finalizar o MemoryStream", exp));
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }
        #endregion
    }
}