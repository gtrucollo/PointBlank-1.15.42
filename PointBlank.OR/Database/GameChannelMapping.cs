namespace PointBlank.OR.Database
{
    using FluentNHibernate.Mapping;

    public class GameChannelMap : ClassMap<GameChannel>
    {
        public GameChannelMap()
        {
            Id(x => x.IdGameChannel, "id_game_channel").GeneratedBy.Increment();
            References(x => x.IdGameServer, "id_game_server").Not.Nullable();
            Map(x => x.Tipo, "tipo");
            Map(x => x.MaximoJogadores, "maximo_jogadores");
            Map(x => x.MensagemTopo, "mensagem_topo");
            Map(x => x.IP, "endereco_ip");
            Map(x => x.Porta, "porta");
            Table("channel_server");
        }
    }
}
