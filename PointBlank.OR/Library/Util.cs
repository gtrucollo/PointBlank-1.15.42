namespace PointBlank.OR.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='Util'/>
    /// </summary>
    public static class Util
    {
        #region Classes
        /// <summary>
        /// Regras de negócio para gerenciamento da classe <see cref='Classes'/>
        /// </summary>
        public static class Classes
        {
            /// <summary>
            /// Obtém a lista de classes com determinado attributo
            /// </summary>
            /// <param name="assembly">Caminho para o assembly a ser pesquisado</param>
            /// <param name="attributo">Attributo a ser procurado</param>
            /// <returns>A lista de classes encontradas</returns>
            public static IEnumerable<Type> ObterClassesPorAtributo<Attribute>(Assembly assembly)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(Attribute), true).Length > 0)
                    {
                        yield return type;
                    }
                }
            }
        }

        #endregion

        #region Enumerador
        /// <summary>
        /// Regras de negócio para gerenciamento da classe <see cref='Enumerador'/>
        /// </summary>
        public static class Enumerador
        {
            /// <summary>
            /// Obtém a descrição de um determinado Enumerador.
            /// </summary>
            /// <param name="valor">Enumerador que terá a descrição obtida</param>
            /// <returns>String com a descrição do Enumerador.</returns>
            public static string ObterDescricao(Enum valor)
            {
                if (valor == null)
                {
                    return string.Empty;
                }

                try
                {
                    FieldInfo fieldInfo = valor.GetType().GetField(valor.ToString());
                    DescriptionAttribute[] atributos = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return atributos.Length > 0 ? atributos[0].Description ?? "Não encontrado" : valor.ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Obtém o atributo customizado de um determinado Enumerador.
            /// </summary>
            /// <param name="valor">Enumerador que terá o abributo obtido</param>
            /// <returns>String com a descrição do atributo do Enumerador.</returns>
            public static string ObterValorAtributo(Enum valor)
            {
                try
                {
                    // Obtém o nome do enumerador
                    string nome = Enum.GetName(valor.GetType(), valor);

                    // Obtém os atributos
                    List<EnumMemberAttribute> atributos = valor.GetType().GetField(nome).GetCustomAttributes(false).OfType<EnumMemberAttribute>().ToList();
                    if ((atributos == null) || (atributos.Count == 0))
                    {
                        return string.Empty;
                    }

                    // Retorna o valor do primeiro atributo
                    return atributos[0].Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region Buffer
        /// <summary>
        /// Regras de negócio para gerenciamento da classe <see cref='BufferExtensions'/>
        /// </summary>
        public static class BufferExtensions
        {
            /// <summary>
            /// Return string hex from byte array
            /// </summary>
            /// <param name="buffer"></param>
            /// <returns></returns>
            public static string ToHex(byte[] buffer)
            {
                var sb = new StringBuilder();
                int length = buffer.Length;
                int counter = 0;

                sb.AppendLine("|--------------------------------------------------------------------------|");
                sb.AppendLine("|       00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F                    |");
                sb.AppendLine("|--------------------------------------------------------------------------|");

                for (int i = 0; i < length; i++)
                {
                    if (counter % 16 == 0)
                    {
                        sb.Append("| " + i.ToString("X4") + ": ");
                    }

                    sb.Append(buffer[i].ToString("X2") + " ");
                    counter++;

                    if (counter == 16)
                    {
                        sb.Append("   ");

                        int charpoint = i - 15;

                        for (int j = 0; j < 16; j++)
                        {
                            byte n = buffer[charpoint++];

                            if (n > 0x1f && n < 0x80)
                            {
                                sb.Append((char)n);
                            }
                            else
                            {
                                sb.Append('.');
                            }
                        }

                        sb.Append(Environment.NewLine);
                        counter = 0;
                    }
                }

                int rest = length % 16;

                if (rest > 0)
                {
                    for (int i = 0; i < 17 - rest; i++)
                    {
                        sb.Append("   ");
                    }

                    int charpoint = length - rest;

                    for (int j = 0; j < rest; j++)
                    {
                        byte n = buffer[charpoint++];

                        if (n > 0x1f && n < 0x80)
                        {
                            sb.Append((char)n);
                        }
                        else
                        {
                            sb.Append('.');
                        }
                    }

                    sb.Append(Environment.NewLine);
                }

                sb.AppendLine("|--------------------------------------------------------------------------|");
                return sb.ToString();
            }
        }
        #endregion
    }
}