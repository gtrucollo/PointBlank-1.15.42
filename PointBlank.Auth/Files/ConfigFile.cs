namespace PointBlank.Auth.Files
{
    using System;
    using System.IO;
    using OR.Base;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='ConfigFile'/>
    /// </summary>
    public class ConfigFile : BaseTextFile
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="ConfigFile"/>
        /// </summary>
        public ConfigFile()
            : base(@"Config/Configuracao.txt")
        {
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém ou define NetworkHost
        /// </summary>
        public string NetworkHost
        {
            get
            {
                return this.ObterValorArquivo(nameof(NetworkHost));
            }
        }

        /// <summary>
        /// Obtém ou define NetworkPort
        /// </summary>
        public int NetworkPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.ObterValorArquivo(nameof(NetworkPort)));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Obtém o valor de ShowSql
        /// </summary>
        public bool ShowSql
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.ObterValorArquivo(nameof(this.ShowSql)));
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Obtém o valor de ShowHex
        /// </summary>
        public bool ShowHex
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.ObterValorArquivo(nameof(this.ShowHex)));
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Informações padrões (Executado após a criação do arquivo)
        /// </summary>
        /// <param name="writer">Writer do arquivo gerado</param>
        protected override void NovoArquivoInformacaoPadraoPartial(StreamWriter writer)
        {
            writer.WriteLine("; Configurações do Game Server");
            writer.WriteLine("NetworkHost = 127.0.0.1");
            writer.WriteLine("NetworkPort = 39190");
        }
        #endregion
    }
}