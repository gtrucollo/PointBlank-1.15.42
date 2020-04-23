namespace PointBlank.OR.Database
{
    using FluentNHibernate.Mapping;

    public class GameServerMap : ClassMap<GameServer>
    {
        public GameServerMap()
        {
            Id(x => x.IdGameServer, "id_game_server").GeneratedBy.Increment();
            Map(x => x.Disponivel, "disponivel");
            Map(x => x.Nome, "nome");
            Map(x => x.Tipo, "tipo");
            Map(x => x.MaximoJogadores, "maximo_jogadores");
            Map(x => x.EnderecoIp, "endereco_ip");
            Map(x => x.Porta, "porta");
            Map(x => x.Senha, "senha");
            HasMany(x => x.ListaGameChannel)
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .Inverse()
                .KeyColumn("id_game_server");
            Table("game_server");
        }
    }
}
