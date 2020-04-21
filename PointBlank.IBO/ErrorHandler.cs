namespace PointBlank.IBO
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// Classe ErrorHandler
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        #region Métodos Públicos
        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether
        /// subsequent HandleError implementations are called.
        /// </summary>
        /// <param name="error">The exception thrown during processing</param>
        /// <returns>true if subsequent System.ServiceModel.Dispatcher.IErrorHandler implementations
        /// must not be called; otherwise, false. The default is false.</returns>
        public bool HandleError(Exception error)
        {
            if (error is FaultException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enables the creation of a custom System.ServiceModel.FaultException
        /// that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The System.Exception object thrown in the course of the service operation</param>
        /// <param name="version">The SOAP version of the message</param>
        /// <param name="fault">The System.ServiceModel.Channels.Message object that is returned to the client,
        /// or service, in the duplex case</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException)
            {
                // Let WCF do normal processing
                return;
            }

            // Generate fault message manually
            MessageFault messageFault = MessageFault.CreateFault(
                new FaultCode("Sender"),
                new FaultReason(error.Message),
                error,
                new NetDataContractSerializer());
            fault = Message.CreateMessage(version, messageFault, null);
        }
        #endregion
    }
}