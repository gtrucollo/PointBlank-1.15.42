namespace PointBlank.OR.Game
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RecivePacketAttribute : Attribute
    {
        #region Construtor
        /// <summary>
        /// Inicia uma nova Instância de <seealso cref="RecivePacketAttribute"/>
        /// </summary>
        public RecivePacketAttribute()
        {
        }

        /// <summary>
        /// Inicia uma nova Instância de <seealso cref="RecivePacketAttribute"/>
        /// </summary>
        /// <param name="id">Valor da opcode</param>
        public RecivePacketAttribute(ushort id)
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
