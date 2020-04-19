namespace PointBlank.Core.Files
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
        /// Obtém ou define DatabaseHost
        /// </summary>
        public string DatabaseHost
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.DatabaseHost));
            }
        }

        /// <summary>
        /// Obtém ou define DatabaseName
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.DatabaseName));
            }
        }

        /// <summary>
        /// Obtém ou define DatabaseUser 
        /// </summary>
        public string DatabaseUser
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.DatabaseUser));
            }
        }

        /// <summary>
        /// Obtém ou define DatabasePassword 
        /// </summary>
        public string DatabasePassword
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.DatabasePassword));
            }
        }

        /// <summary>
        /// Obtém ou define DatabasePort 
        /// </summary>
        public int DatabasePort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.ObterValorArquivo(nameof(this.DatabasePort)));
                }
                catch
                {
                    return 0;
                }
            }
        }

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
        #endregion

        #region Métodos
        /// <summary>
        /// Informações padrões (Executado após a criação do arquivo)
        /// </summary>
        protected override void NovoArquivoInformacaoPadrao()
        {
            using (StreamWriter writer = this.file.CreateText())
            {
                // Informações banco de dados
                writer.WriteLine("; Configurações do Banco de Dados");
                writer.WriteLine("DatabaseHost = localhost");
                writer.WriteLine("DatabaseName = pb");
                writer.WriteLine("DatabaseUser = postgres");
                writer.WriteLine("DatabasePassword = postgres");
                writer.WriteLine("DatabasePort = 5432");

                // Informações do core
                writer.WriteLine("; Configurações do Core Server");
                writer.WriteLine("NetworkHost = 127.0.0.1");
                writer.WriteLine("NetworkPort = 5900");

                // Fechar o arquivo
                writer.Flush();
                writer.Close();
            }
        }
        #endregion
    }
}