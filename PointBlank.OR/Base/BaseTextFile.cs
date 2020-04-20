namespace PointBlank.OR.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Library;
    using Library.Exceptions;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='BaseTextFile'/>
    /// </summary>
    public abstract class BaseTextFile : IDisposable
    {
        #region Campos
        /// <summary>
        /// Controle para o arquivo
        /// </summary>
        public readonly FileInfo file;

        /// <summary>
        /// Lista de controle dos dados do arquivo
        /// </summary>
        private IList<FileControl> listaValores;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="BaseTextFile"/>
        /// </summary>
        public BaseTextFile(string path)
        {
            // Atualizar controles
            this.file = new FileInfo(path);

            // Carregar informações
            this.Carregar();
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém ou define CoreHost 
        /// </summary>
        public string CoreHost
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.CoreHost));
            }
        }

        /// <summary>
        /// Obtém ou define CorePort
        /// </summary>
        public int CorePort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.ObterValorArquivo(nameof(this.CorePort)));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Obtém ou define CoreKey 
        /// </summary>
        public string CoreKey
        {
            get
            {
                return this.ObterValorArquivo(nameof(this.CoreKey));
            }
        }

        /// <summary>
        /// Lista de Controle de valores do Arquivo
        /// </summary>
        private IList<FileControl> ListaValores
        {
            get
            {
                if (this.listaValores == null)
                {
                    this.listaValores = new List<FileControl>();
                }

                return this.listaValores;
            }

            set
            {
                this.listaValores = value;
            }
        }
        #endregion

        #region Métodos
        #region Públicos
        /// <summary>
        /// Executa o carregamento das informações para a memória
        /// </summary>
        public void Carregar()
        {
            // Criar pasta senão existir
            if (!this.file.Directory.Exists)
            {
                this.file.Directory.Create();
            }

            // Criar arquivo senão existir
            if (!this.file.Exists || this.file.Length <= 0)
            {
                this.NovoArquivoInformacaoPadrao();
            }

            // Ler arquivo
            using (StreamReader reader = new StreamReader(this.file.FullName))
            {
                // Limpar a lista
                this.listaValores = null;

                // Carregar as informações
                while (!reader.EndOfStream)
                {
                    // Carregar texto da linha
                    string linha = reader.ReadLine();

                    // Validar
                    if (linha.Length == 0 || linha.StartsWith(";") || !linha.Contains('='))
                    {
                        continue;
                    }

                    // Obter valores
                    string[] valores = linha.Split('=');
                    this.ListaValores.Add(new FileControl() { Propriedade = valores[0].Trim(), Valor = valores[1].Trim() });
                }
            }
        }
        #endregion

        #region Protegidos
        /// <summary>
        /// Obtém o dado de uma das propriedades do arquivo
        /// </summary>
        /// <param name="nomePropriedade">Propriedade que deseja obter o valor</param>
        /// <returns>O valor encontrado</returns>
        protected internal string ObterValorArquivo(string nomePropriedade)
        {
            // Obter configuração
            FileControl valor = this.ListaValores.Where(x => x.Propriedade.Contains(nomePropriedade)).FirstOrDefault();

            // Retorno
            return valor?.Valor ?? string.Empty;
        }

        /// <summary>
        /// Informações padrões (Executado após a criação do arquivo)
        /// </summary>
        protected virtual void NovoArquivoInformacaoPadrao()
        {
            // Informações padrão
            using (StreamWriter writer = this.file.CreateText())
            {
                writer.WriteLine("; Configurações de Conexão com o Servidor Core");
                writer.WriteLine("CoreHost = 127.0.0.1");
                writer.WriteLine("CorePort = 5900");
                writer.WriteLine("CoreKey = point_blank_best_emulator_ever");

                // Gerar informações personalizadas
                this.NovoArquivoInformacaoPadraoPartial(writer);

                // Fechar o arquivo
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Informações padrões (Executado após a criação do arquivo)
        /// </summary>
        /// <param name="writer">Writer do arquivo gerado</param>
        protected virtual void NovoArquivoInformacaoPadraoPartial(StreamWriter writer)
        {
            return;
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
                this.ListaValores.Clear();
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[Dispose] Erro ao limpar os dados", exp));
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }
        #endregion
        #endregion

        #region Classes
        /// <summary>
        /// Regras de negócio para gerenciamento da classe <see cref='FileControl'/>
        /// </summary>
        internal class FileControl
        {
            #region Propriedades
            /// <summary>
            /// Obtém ou define Propriedade
            /// </summary>
            public string Propriedade { get; set; }

            /// <summary>
            /// Obtém ou define Valor
            /// </summary>
            public string Valor { get; set; }
            #endregion
        }
        #endregion
    }
}