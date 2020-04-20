namespace PointBlank.OR.Database
{
    using FluentNHibernate.Mapping;

    public class ContaMapping : ClassMap<Conta>
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova instancia da classe <see cref="ContaMapping"/>
        /// </summary>
        public ContaMapping()
        {
            Id(x => x.IdConta, "id_conta").GeneratedBy.Increment();
            Map(x => x.Alteracao, "alteracao");
            Map(x => x.Lancamento, "lancamento");
            Map(x => x.Usuario, "usuario").Length(20);
            Map(x => x.Senha, "senha");
            Map(x => x.Status, "status");
            Map(x => x.Ip, "ip").Length(15);
            Map(x => x.Mac, "mac");
            Table(nameof(Conta));
        }
        #endregion
    }
}