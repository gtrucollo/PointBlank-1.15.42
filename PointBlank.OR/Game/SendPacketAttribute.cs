namespace PointBlank.OR.Game
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SendPacketAttribute : Attribute
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova Instância de <seealso cref="SendPacketAttribute"/>
        /// </summary>
        public SendPacketAttribute()
        {
        }

        /// <summary>
        /// Inicia uma nova Instância de <seealso cref="SendPacketAttribute"/>
        /// </summary>
        /// <param name="id">Valor da opcode</param>
        public SendPacketAttribute(ushort id)
        {
            this.Id = id;
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém ou define Id
        /// </summary>
        public ushort Id { get; set; }
        #endregion
    }
}
