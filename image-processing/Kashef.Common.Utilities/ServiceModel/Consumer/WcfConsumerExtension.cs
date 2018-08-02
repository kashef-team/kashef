using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.ServiceModel.Consumer
{
    /// <summary>
    /// WCF Consumer accept only calls form hosted machine clients.
    /// </summary>
   public class WcfConsumer : Attribute, IDispatchMessageInspector, IServiceBehavior
    {
        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/>
        /// </summary>
        /// <param name="request"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="channel"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="instanceContext"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <returns><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></returns>
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
           OperationContext context = OperationContext.Current;
           if (context != null)
           { 
               IIdentity ClientIdentity = null;
               if (context.ServiceSecurityContext != null)
               {
                   ClientIdentity = context.ServiceSecurityContext.PrimaryIdentity;
               }

               if (ClientIdentity == null || !ClientIdentity.Name.Equals(WindowsIdentity.GetCurrent().Name))
               {
                   throw new UnauthorizedAccessException();
               }

               MessageProperties messageProperties = context.IncomingMessageProperties;
               RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
               List<string> addresses = GetLocalIPAddresses();
               if (!addresses.Contains(endpointProperty.Address) && !endpointProperty.Address.Equals("127.0.0.1") && !endpointProperty.Address.Equals("::1"))
               {
                   throw new Exception();
               }
           }
           return null;
        }

        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/>
        /// </summary>
        /// <param name="reply"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        /// <param name="correlationState"><see cref="System.ServiceModel.Dispatcher.IDispatchMessageInspector"/></param>
        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
             
        }
      
       /// <summary>
       /// 
       /// </summary>
       /// <param name="serviceDescription"></param>
       /// <param name="serviceHostBase"></param>
       /// <param name="endpoints"></param>
       /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            
        }
        
        /// <summary>
        /// <see cref="System.ServiceModel.Description.IServiceBehavior"/>
        /// </summary>
        /// <param name="serviceDescription"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        /// <param name="serviceHostBase"><see cref="System.ServiceModel.Description.IServiceBehavior"/></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {

            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="serviceDescription"></param>
       /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            
        }
        
       #region ------- private methods -----
   
       /// <summary>
       /// Get all local ip addresses
       /// </summary>
       /// <returns></returns>
       private List<string> GetLocalIPAddresses()
        {
            List<string> addresses = new List<string>();
            IPHostEntry host; 
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            { 
                addresses.Add(ip.ToString());
            }
            return addresses;
        } 
       
       #endregion
    }
 
   /// <summary>
   /// A WCF service behavior extension that audit any WCF service calls into database.
   /// </summary>
   public class WcfConsumerExtension : BehaviorExtensionElement
   {
       /// <summary>
       /// Type of service behavior
       /// </summary>
       public override Type BehaviorType
       {
           get
           {
               return typeof(WcfConsumer);
           }
       }

       /// <summary>
       /// Create wcf consumer behavior.
       /// </summary>
       /// <returns></returns>
       protected override object CreateBehavior()
       {
           return new WcfConsumer();
       }        
   }


}
