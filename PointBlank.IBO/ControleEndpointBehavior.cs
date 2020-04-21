namespace PointBlank.IBO
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Xml;
    using PointBlank.OR.Library.Exceptions;

    public class ControleEndpointBehavior : IEndpointBehavior, IDispatchMessageInspector, IClientMessageInspector
    {
        #region Métodos Públicos
        #region IEndpointBehavior
        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized</param>
        /// <param name="clientRuntime">The client runtime to be customized</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (clientRuntime.MessageInspectors.Any(x => x is ControleEndpointBehavior))
            {
                return;
            }

            clientRuntime.MessageInspectors.Add(this);
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (!endpointDispatcher.ChannelDispatcher.ErrorHandlers.Any(x => x is ErrorHandler))
            {
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new ErrorHandler());
            }

            if (!endpointDispatcher.DispatchRuntime.MessageInspectors.Any(x => x is ControleEndpointBehavior))
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }
        }

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }
        #endregion

        #region IDispatchMessageInspector (Server)
        /// <summary>
        /// Implementação AfterReceiveRequest
        /// </summary>
        /// <param name="request">Parâmetro Message</param>
        /// <param name="channel">Parâmetro IClientChannel</param>
        /// <param name="instanceContext">Parâmetro InstanceContext</param>
        /// <returns>Objeto de controle</returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // Retorno
            return Stopwatch.StartNew();
        }

        /// <summary>
        /// Implementação BeforeSendReply
        /// </summary>
        /// <param name="reply">Parâmetro Message</param>
        /// <param name="correlationState">Parâmetro correlationState</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            return;
        }
        #endregion

        #region IClientMessageInspector (Cliente)
        /// <summary>
        /// Enables inspection or modification of a message after a reply message is
        /// received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application</param>
        /// <param name="correlationState">Correlation state data</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Verificar exception
            if (!reply.IsFault)
            {
                return;
            }

            var faultDetail = IdentificarException(reply);
            if (faultDetail is PointBlankException)
            {
                throw faultDetail as PointBlankException;
            }

            if (faultDetail is TimeoutException)
            {
                throw faultDetail as TimeoutException;
            }

            if (faultDetail is CommunicationException)
            {
                throw faultDetail as CommunicationException;
            }
        }

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service</param>
        /// <param name="channel">The WCF client object channel</param>
        /// <returns>The object that is returned as the correlationState argument of the
        /// IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)
        /// method. This is null if no correlation state is used.The best practice is
        /// to make this a System.Guid to ensure that no two correlationState objects are the same.</returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return Stopwatch.StartNew();
        }
        #endregion
        #endregion

        #region Métodos Privados
        /// <summary>
        /// Identifica se existe uma exception
        /// </summary>
        /// <param name="mensagem">Mensagem a ser verificada</param>
        /// <returns>Objeto que representa a exception</returns>
        private static object IdentificarException(Message mensagem)
        {
            const string detailElementName = "Detail";
            using (var reader = mensagem.GetReaderAtBodyContents())
            {
                // Find <soap:Detail>
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == detailElementName)
                    {
                        break;
                    }
                }

                // Did we find it?
                if (reader.NodeType != XmlNodeType.Element || reader.LocalName != detailElementName)
                {
                    return null;
                }

                // Move to the contents of <soap:Detail>
                if (!reader.Read())
                {
                    return null;
                }

                // Deserialize the fault
                var serializer = new NetDataContractSerializer();
                try
                {
                    return serializer.ReadObject(reader);
                }
                catch (FileNotFoundException)
                {
                    // Serializer was unable to find assembly where exception is defined
                    return null;
                }
            }
        }
        #endregion
    }
}