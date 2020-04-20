namespace PointBlank.BO
{
    using System.Collections.Generic;
    using Database;
    using IBO;
    using IBO.Database;

    public static class ServiceList
    {
        #region Propriedades
        public static IList<ServiceType> ListaServicos
        {
            get
            {
                return new List<ServiceType>()
                {
                    new ServiceType(typeof(ContaBo), typeof(IContaBo)),
                };
            }
        }
        #endregion
    }
}
