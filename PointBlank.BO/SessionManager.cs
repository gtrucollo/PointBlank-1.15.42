namespace PointBlank.BO
{
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using OR.Database;

    /// <summary>
    /// NHibernate Session Management
    /// </summary>
    public static class SessionManager
    {
        #region Campos
        /// <summary>
        /// Configuration do NHibernate
        /// </summary>
        private static Configuration configuration;

        /// <summary>
        /// Session Factory
        /// </summary>
        private static ISessionFactory sessionFactory;
        #endregion

        #region Propriedades
        #region Privadas
        /// <summary>
        /// Configuration do Servidor
        /// </summary>
        private static string Servidor { get; set; }

        /// <summary>
        /// Configuration do Porta
        /// </summary>
        private static int Porta { get; set; }

        /// <summary>
        /// Configuration do NomeUsuario
        /// </summary>
        private static string NomeUsuario { get; set; }

        /// <summary>
        /// Configuration do Senha
        /// </summary>
        private static string Senha { get; set; }

        /// <summary>
        /// Configuration do NomeBanco
        /// </summary>
        private static string NomeBanco { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica que é ShowSql
        /// </summary>
        private static bool ShowSql { get; set; }

        /// <summary>
        /// Obtém o valor de Configuration
        /// Retorna a configuração do NHibernate. Útil para acessos direto aos arquivos de mapeamentos (.hbm.xml).
        /// </summary>
        private static Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = ObterNovaConfiguracao(ShowSql);
                }

                return configuration;
            }
        }

        /// <summary>
        /// Obtém o valor de StringConexao
        /// </summary>
        private static string StringConexao
        {
            get
            {
                return string.Format(
                    "Server={0};" +
                    "Port={1};" +
                    "User ID={2};" +
                    "Password={3}{4};" +
                    "CommandTimeout={5};" +
                    "Pooling=true;" +
                    "MinPoolSize=10;" +
                    "MaxPoolSize=100;" +
                    "ApplicationName={6};",
                    Servidor,
                    Porta,
                    NomeUsuario,
                    Senha,
                    string.Format(";Database={0}", NomeBanco),
                    15,
                    "Point Blank Emulatador (Core)");
            }
        }

        /// <summary>
        /// Obtém o valor de SessionFactory
        /// </summary>
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    IPersistenceConfigurer configurarDb = PostgreSQLConfiguration.PostgreSQL82.ConnectionString(StringConexao);
                    sessionFactory = Fluently.Configure(Configuration)
                        .Database(configurarDb)
                        .Mappings(x => x.FluentMappings.AddFromAssemblyOf<ContaMapping>())
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                        .BuildSessionFactory();
                }

                return sessionFactory;
            }
        }
        #endregion
        #endregion

        #region Métodos
        #region Publicos
        /// <summary>
        /// Inicializar as configurações de conexão com o banco de dados
        /// </summary>
        /// <param name="servidor">Endereço de conexão com banco de dados</param>
        /// <param name="porta">Porta de conexão com banco de dados</param>
        /// <param name="usuario">Usuário de conexão com banco de dados</param>
        /// <param name="senha">Senha de conexão com o banco de dados</param>
        /// <param name="nomeBanco">Nome do banco de dados</param>
        public static void Inicializar(string servidor, int porta, string usuario, string senha, string nomeBanco)
        {
            SessionManager.Servidor = servidor;
            SessionManager.Porta = porta;
            SessionManager.NomeUsuario = usuario;
            SessionManager.Senha = senha;
            SessionManager.NomeBanco = nomeBanco;
        }

        /// <summary>
        /// Retorna uma nova sessão
        /// </summary>
        /// <returns>Objeto Sessão</returns>
        public static ISession ObterNovaSessao()
        {
            return SessionFactory.OpenSession();
        }
        #endregion

        #region Privadas
        /// <summary>
        /// Instância as configurações de NHibernate
        /// </summary>
        /// <param name="showSQL">Se true irá mostrar o SQL</param>
        /// <returns>Retorna uma nova instância</returns>
        private static Configuration ObterNovaConfiguracao(bool showSQL)
        {
            // Criar nova configuração
            Configuration configurationTemp = new Configuration();

            // Show SQL
            configurationTemp.SetProperty(Environment.ShowSql, showSQL ? "True" : "False");

            // Many-to-many
            configurationTemp.SetProperty(Environment.MaxFetchDepth, "1");

            // Format SQL
            configurationTemp.SetProperty(Environment.FormatSql, "true");

            // Estatísticas
            configurationTemp.SetProperty(Environment.GenerateStatistics, "false");

            // Driver
            configurationTemp.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.NpgsqlDriver");

            // Isolation da Conexão
            configurationTemp.SetProperty(Environment.Isolation, "ReadCommitted");

            // String de conexão
            configurationTemp.SetProperty(Environment.ConnectionString, SessionManager.StringConexao);

            // Proxy
            configurationTemp.SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate");

            // Time-out de comando
            configurationTemp.SetProperty(Environment.CommandTimeout, "10");

            // Retorno
            return configurationTemp;
        }
        #endregion
        #endregion
    }
}