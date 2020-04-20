namespace PointBlank.BO.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using IBO.Base;
    using NHibernate;
    using OR.Base;
    using OR.Library;
    using OR.Library.Exceptions;

    /// <summary>
    /// Classe base para objetos BO's
    /// </summary>
    /// <typeparam name="TType">Tipo do objeto OR</typeparam>
    public abstract class BaseBo<TType> : IBaseBo<TType>, IDisposable where TType : BaseOr
    {
        #region Campos
        /// <summary>
        /// Sessão de controle
        /// </summary>
        private ISession sessaoControle;

        /// <summary>
        /// FactoryBo Local
        /// </summary>
        private FactoryBo factoryBoLocal;

        /// <summary>
        /// Transação de controle
        /// </summary>
        private ITransaction transacaoControle;

        /// <summary>
        /// Controle para sessaoPrivada
        /// </summary>
        private readonly bool sessaoPrivada;
        #endregion

        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseBo{TType}" />
        /// </summary>
        /// <param name="sessaoControle">Sessão de controle a ser utilizada</param>
        public BaseBo(ISession sessaoControle)
        {
            // Verificar se será criado uma nova sessão
            if (sessaoControle == null)
            {
                this.sessaoControle = SessionManager.ObterNovaSessao();
                this.sessaoPrivada = true;
                return;
            }

            // Utilizar sessão compartilhada
            this.sessaoControle = sessaoControle;
            this.sessaoPrivada = false;
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém o valor de SessaoControle
        /// </summary>
        protected ISession SessaoControle
        {
            get
            {
                if (!this.sessaoControle.IsConnected)
                {
                    this.sessaoControle.Reconnect();
                }

                return this.sessaoControle;
            }
        }

        /// <summary>
        /// Obtém o valor de FactoryBoLocal - Utiliza a sessão de dados do objeto atual
        /// </summary>
        public FactoryBo FactoryBoLocal
        {
            get
            {
                if (this.factoryBoLocal == null)
                {
                    this.factoryBoLocal = new FactoryBo(this.SessaoControle);
                }

                return this.factoryBoLocal;
            }
        }
        #endregion

        #region Métodos
        #region IDisposable Members
        /// <summary>
        /// Método Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        #region Transações
        /// <summary>
        /// Inicia uma transação com o banco de dados.
        /// </summary>
        protected void IniciarTransacao()
        {
            // Abrir nova transação
            this.transacaoControle = this.SessaoControle.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Grava as informações da transação no banco.
        /// </summary>
        /// <returns>True se executado com sucesso</returns>
        protected bool GravarTransacao()
        {
            // Gravar a transação
            try
            {
                this.transacaoControle.Commit();
                this.transacaoControle.Dispose();
                this.transacaoControle = null;

                // OK
                return true;
            }
            catch
            {
                try
                {
                    // Cancelar transação
                    this.CancelarTransacao();
                }
                catch (Exception exp)
                {
                    Logger.Error(exp, "[BaseBo] Erro ao cancelar a transação (GravarTransacao)", true);
                }

                throw;
            }
        }

        /// <summary>
        /// Cancela uma transação.
        /// </summary>
        protected void CancelarTransacao()
        {
            try
            {
                this.transacaoControle.Rollback();
            }
            finally
            {
                this.transacaoControle = null;
            }
        }
        #endregion

        #region Insert e Update
        /// <summary>
        /// Insere ou altera um objeto no banco de dados
        /// </summary>      
        /// <param name="objeto">Parâmetro Objeto</param>
        /// <returns>O objeto após a persistência</returns>
        public virtual TType InsertUpdate(TType objeto)
        {
            try
            {
                // Begin Transação
                this.IniciarTransacao();

                // Limpar dados de cache, se remover esse comando, em alguns casos da mensagem de erro que o objeto está desatualizado
                this.SessaoControle.Clear();

                // Atualizar
                this.SessaoControle.SaveOrUpdate(objeto);
                this.SessaoControle.Flush();

                // Commit Transação
                this.GravarTransacao();

                // OK
                return objeto;
            }
            catch
            {
                // Rollback Transação
                this.CancelarTransacao();
                throw;
            }
        }

        /// <summary>
        /// Insere ou altera vários objetos no banco de dados
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="listaObjetos">Lista com os Objetos</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        public void InsertUpdate(List<TType> listaObjetos)
        {
            // Validar
            if (listaObjetos.Count == 0)
            {
                return;
            }

            try
            {
                // Begin Transação
                this.IniciarTransacao();

                // Salvar todos os objetos
                listaObjetos.ForEach(x => this.InsertUpdate(x));

                // Commit Transação
                this.GravarTransacao();
            }
            catch
            {
                // Rollback Transação
                this.CancelarTransacao();
                throw;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Exclui um objeto do banco de dados.
        /// </summary>
        /// <param name="user">O usuário do controle de acesso</param>
        /// <param name="objeto">Parâmetro objeto</param>
        /// <param name="logInformacao">Informação adicional para o Log</param>
        public virtual void Delete(TType objeto)
        {
            try
            {
                // Begin Transação
                this.IniciarTransacao();

                // Limpar dados de cache, se remover esse comando, em alguns casos da mensagem de erro que o objeto está desatualizado
                this.SessaoControle.Clear();

                // Excluir
                this.SessaoControle.Delete(objeto);
                this.SessaoControle.Flush();

                // Commit Transação
                this.GravarTransacao();
            }
            catch
            {
                // Rollback Transação
                this.CancelarTransacao();

                // Limpar dados de cache
                this.SessaoControle.Clear();
                throw;
            }
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
                if ((this.sessaoControle != null) && this.sessaoPrivada)
                {
                    this.sessaoControle.Dispose();
                    this.sessaoControle = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[BaseBo] Ocorreu um erro ao finalizar sessão controle", exp));
            }

            try
            {
                if (this.factoryBoLocal != null)
                {
                    this.factoryBoLocal.Dispose();
                    this.factoryBoLocal = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[BaseBo] Ocorreu um erro ao finalizar factoryBoLocal", exp));
            }

            try
            {
                if (this.transacaoControle != null)
                {
                    this.transacaoControle.Dispose();
                    this.transacaoControle = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(new PointBlankException("[BaseBo] Ocorreu um erro ao finalizar transacaoControle", exp));
            }

            // Liberar a memoria (Testado)
            GC.Collect();
        }
        #endregion
        #endregion
    }
}