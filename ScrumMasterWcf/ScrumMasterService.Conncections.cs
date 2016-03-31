using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ScrumMasterWcf
{
    partial class ScrumMasterService
    {
        #region IParameterInspector+IEndpointBehavior Members
        /// <summary>
        /// Disconnect the client if the security check fails to release resources.
        /// But to prevent the execution continue, each method will check the permissions again 
        /// </summary>
        /// <param name="operationName">The client required operation</param>
        /// <param name="inputs">The parameters who passed by the client</param>
        /// <returns> true - anyway</returns>
        public object BeforeCall(string operationName, object[] inputs)
        {
            OperationContext operationContext = OperationContext.Current;
            var opNameLower = operationName.ToLower();
            if (opNameLower.Contains("is") || opNameLower == "getuser") return true;
            else
            {
                if (inputs.Length == 0) operationContext.Channel.Close();
                var lastInput = inputs[inputs.Length - 1];
                if (!SecurityCheck(lastInput, opNameLower))
                    operationContext.Channel.Close();
            }
            return true;
        }
        /// <summary>
        /// Updating the update-times respectively to operationName        /// 
        /// </summary>
        /// <param name="operationName">The client required operation</param>
        /// <param name="outputs"></param>
        /// <param name="returnValue"></param>
        /// <param name="correlationState"></param>
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            // Normalizing the operation name
            string operNameLower = operationName.ToLower();
            // If the operation makes any changes, we need to update the flags
            switch (operNameLower)
            {
                case "bindtasktouser":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "createnewuser":
                    usersUpdateTime = DateTime.Now;
                    break;
                case "createnewsprint":
                    sprintUpdateTime = DateTime.Now;
                    break;
                case "createnewuserstory":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "createnewscrumtask":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "changejobstatus":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "updateexistinguser":
                    usersUpdateTime = DateTime.Now;
                    break;
                case "removeuser":
                    usersUpdateTime = DateTime.Now;
                    break;
                case "updateuserstory":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "removeuserstory":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "updatescrumtask":
                    ussUpdateTime = DateTime.Now;
                    break;
                case "removescrumtask":
                    ussUpdateTime = DateTime.Now;
                    break;
            }
        }
        /// <summary>
        /// Not in use
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint)
        {

        }
        /// <summary>
        /// Not in use
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }
        /// <summary>
        /// Registering the object to invoke it's BeforeCall and AfterCall
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (DispatchOperation op in endpointDispatcher.DispatchRuntime.Operations)
                op.ParameterInspectors.Add(this);
        }
        /// <summary>
        /// Not in use
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }
        #endregion

    }
}
