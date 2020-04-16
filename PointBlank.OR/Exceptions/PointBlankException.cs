namespace PointBlank.OR.Library.Exceptions
{
    using System;

    /// <summary>
    /// Classe de controle das Exceptions (Tratadas)
    /// </summary>
    public class PointBlankException : Exception
    {
        #region Construtores
        /// <summary>
        /// Inicia uma nova instancia de <see cref="PointBlankException"/>
        /// </summary>
        public PointBlankException()
            : base()
        {
        }

        /// <summary>
        /// Inicia uma nova instancia de <see cref="PointBlankException"/>
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        public PointBlankException(string mensagem)
            : base(mensagem)
        {
        }

        /// <summary>
        /// Inicia uma nova instancia de <see cref="PointBlankException"/>
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="exp">Exception que gerou o erro</param>
        public PointBlankException(string mensagem, Exception exp)
            : base(mensagem, exp)
        {
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Obtém a mensagem de erro completa
        /// </summary>
        /// <param name="exp">Exception que deseja obter o erro</param>
        /// <param name="stackTrace">Se true irá conter o stackTrace</param>
        /// <returns>A mensagem completa</returns>
        public static string GetMessageFull(Exception exp, bool stackTrace)
        {
            // Controle
            string retorno = string.Empty;

            // Obter a mensagem
            retorno += exp.Message;

            // Obter mensagem complementar
            while (exp.InnerException != null)
            {
                // Formatar conteudo
                retorno = string.Format("{1}{0}{2}", Environment.NewLine, retorno, exp.InnerException.Message);

                // Atualizar
                exp = exp.InnerException;
            }

            // Indica se irá adicionar stackTrace
            if (stackTrace)
            {
                retorno = string.Format("{1}{0} StackTrace:{0}{2}", Environment.NewLine, retorno, exp.StackTrace);
            }

            // Retorno
            return retorno;
        }
        #endregion
    }
}