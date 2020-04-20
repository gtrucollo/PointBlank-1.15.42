namespace PointBlank.BO.Database
{
    using System;
    using Base;
    using IBO.Database;
    using NHibernate;
    using OR.Database;

    /// <summary>
    /// Regras de negócio para gerenciamento da classe <see cref='ContaBo'/>
    /// </summary>
    public class ContaBo : BaseBo<Conta>, IContaBo
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        public ContaBo()
            : this(null)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ContaBo"/>
        /// </summary>
        /// <param name="sessaoControle">Sessão de controle a ser utilizada</param>
        public ContaBo(ISession sessaoControle)
            : base(sessaoControle)
        {
        }
        #endregion

        #region Métodos
        #region Públicos
        /// <summary>
        /// Obtém uma conta do banco de dados
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="senha">Senha</param>
        /// <param name="ip">endereço de ip</param>
        /// <param name="mac">mac da maquina</param>
        /// <param name="validar">Se True irá Validar se não encontrar uma conta</param>
        /// <param name="validarSenha">Se true irá validar a senha</param>
        /// <returns>A conta se houver</returns>
        public Conta ObterContaPorLogin(string login, string senha, string ip, string mac, bool validar, bool validarSenha)
        {
            Conta contaTmp = this.SessaoControle.QueryOver<Conta>()
                .Where(x => x.Usuario == login.ToUpper())
                .Take(1)
                .SingleOrDefault<Conta>();
            if ((contaTmp == null) && validar)
            {
                return null;
            }

            // Criar nova conta
            if (!validar && (contaTmp == null))
            {
                contaTmp =
                    new Conta()
                    {
                        Alteracao = DateTime.Now,
                        Lancamento = DateTime.Now,
                        Usuario = login.ToUpper(),
                        Senha = senha,
                        Status = (int)Conta.StatusEnum.Ativa
                    };
            }

            // Validar
            if (validarSenha && (!senha.ToUpper().Equals(contaTmp?.Senha.ToUpper() ?? string.Empty)))
            {
                return null;
            }

            // Atualizar
            contaTmp.Alteracao = DateTime.Now;

            // Endereço de IP
            if (!string.IsNullOrEmpty(ip))
            {
                contaTmp.Ip = ip;
            }

            // MAC
            if (!string.IsNullOrEmpty(mac))
            {
                contaTmp.Mac = mac;
            }

            // Retorno
            return this.InsertUpdate(contaTmp);
        }
        #endregion
        #endregion
    }
}