namespace PointBlank.OR.Library
{
    using Exceptions;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Classe  de controle (Log's dos servidores)
    /// </summary>
    public static class Logger
    {
        #region Campos
        /// <summary>
        /// Controle execução
        /// </summary>
        public static int ThreadControle = 0;
        #endregion

        #region Enumeradores
        /// <summary>
        /// Enumerador dos tipos de log
        /// </summary>
        public enum TypeEnum
        {
            [Description("Info")]
            Info,

            [Description("Warn")]
            Warning,

            [Description("Error")]
            Error
        }
        #endregion

        #region Métodos
        #region Públicos
        /// <summary>
        /// Show Logg Type Error
        /// </summary>
        /// <param name="Exception">Exception que gerou o erro</param>
        public static void Error(Exception exp)
        {
            if (exp is PointBlankException)
            {
                Warning(PointBlankException.GetMessageFull(exp, false));
                return;
            }

            Error(PointBlankException.GetMessageFull(exp, true));
        }

        /// <summary>
        /// Show Logg Type Error
        /// </summary>
        /// <param name="Exception">Exception que gerou o erro</param>
        /// <param name="complemento">Complemento da mensagem de erro</param>
        /// <param name="stackTrace">Se true indica que será mostrado stackTrace</param>
        public static void Error(Exception exp, string complemento, bool stackTrace)
        {
            Error(PointBlankException.GetMessageFull(new PointBlankException(complemento, exp), stackTrace));
        }

        /// <summary>
        /// Show Logg Type Error
        /// </summary>
        /// <param name="message">Mensagem</param>
        public static void Error(string message)
        {
            LoggerShow(TypeEnum.Error, message, true);
        }

        /// <summary>
        /// Show Logg Type Warning
        /// </summary>
        /// <param name="message">Mensagem</param>
        public static void Warning(string message)
        {
            LoggerShow(TypeEnum.Warning, message, true);
        }

        /// <summary>
        /// Show Logg Type Info
        /// </summary>
        /// <param name="message">Mensagem</param>
        public static void Info(string message)
        {
            LoggerShow(TypeEnum.Info, message, true);
        }
        #endregion

        #region Privados
        /// <summary>
        /// Grava uma mensagem em formato padrão
        /// </summary>
        /// <param name="type">TypeEnum</param>
        /// <param name="message">Mensagem a ser gravada</param>
        /// <param name="assync">Se true indica que é assync</param>
        private static void LoggerShow(TypeEnum type, string message, bool assync)
        {
            // Assync
            if (assync)
            {
                new Thread(() => LoggerShow(type, message)).Start();
                return;
            }

            LoggerShow(type, message);
        }

        /// <summary>
        /// Grava uma mensagem em formato padrão
        /// </summary>
        /// <param name="type">TypeEnum</param>
        /// <param name="message">Mensagem a ser gravada</param>
        private static void LoggerShow(TypeEnum type, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            // Controle para executar somente um por vez
            while (Interlocked.CompareExchange(ref ThreadControle, 1, 0) != 0)
            {
                Thread.Sleep(10);
            }

            try
            {
                // Gravar log em arquivo texto
                string path = string.Format(@"Logs/{0}/{1:yyyy-MM-dd}.txt", Process.GetCurrentProcess().ProcessName, DateTime.Today);
                if (!Directory.Exists(path.Substring(0, path.IndexOf(DateTime.Today.ToString("yyyy-MM-dd")))))
                {
                    Directory.CreateDirectory(path.Substring(0, path.IndexOf(DateTime.Today.ToString("yyyy-MM-dd"))));
                }

                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                // Opções de acordo com o tipo do log
                switch (type)
                {
                    case TypeEnum.Info:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;

                    case TypeEnum.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case TypeEnum.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }

                // Escrever mensagem no console
                using (FileStream fileStream = new FileStream(path, FileMode.Append))
                {
                    using (StreamWriter arquivo = new StreamWriter(fileStream))
                    {
                        string[] linhas = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        string mensagemAtual = string.Empty;
                        switch (linhas.Length <= 1)
                        {
                            case true:
                                mensagemAtual = string.Format("[{0}] - {1}", Util.Enumerador.ObterDescricao(type), message);
                                Console.WriteLine(mensagemAtual);
                                arquivo.WriteLine(mensagemAtual);
                                break;

                            default:
                                bool primeira = true;
                                foreach (string linhaAtual in linhas)
                                {
                                    if (primeira)
                                    {
                                        mensagemAtual = string.Format("[{0}] - {1}", Util.Enumerador.ObterDescricao(type), linhaAtual);
                                        Console.WriteLine(mensagemAtual);
                                        arquivo.WriteLine(mensagemAtual);
                                        primeira = false;
                                        continue;
                                    }

                                    mensagemAtual = string.Format("{0}{1}", string.Empty.PadLeft(9), linhaAtual);
                                    Console.WriteLine(mensagemAtual);
                                    arquivo.WriteLine(mensagemAtual);
                                }

                                break;
                        }

                        arquivo.Flush();
                    }
                }
            }
            finally
            {
                // Liberar execução
                Logger.ThreadControle = 0;
            }
        }
        #endregion
        #endregion
    }
}