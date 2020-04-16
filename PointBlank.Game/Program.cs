namespace PointBlank.Game
{
    using OR.Library;
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Classe Program
    /// </summary>
    public class Program
    {
        #region Métodos
        /// <summary>
        /// Método Main
        /// </summary>
        /// <param name="args">Parâmetro args</param>
        public static void Main(string[] args)
        {
            try
            {
                // Log de inicio
                Logger.Info("Point Blank - Game");


                // Não finalizar o servidor
                Process.GetCurrentProcess().WaitForExit();
            }
            catch (ThreadAbortException)
            {
                Logger.Error("Ocorreu um erro não tratado, todas as conexões serão finalizadas");
                throw;
            }
            catch (Exception exp)
            {
                Logger.Error(exp, "Ocorreu um erro e com isso as conexões serão finalizadas", true);
            }
        }
        #endregion
    }
}