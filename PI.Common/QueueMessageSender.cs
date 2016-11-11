using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Microsoft.ServiceBus;

namespace PI.Common
{
    public class QueueMessageSender
    {
        public string ServiceBusConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceBusConnectionString"].ToString();
            }
        }
        
        
        public void SendQueueMessage<T>(T message, string QueueName, string messageType, string reference)
        {
            // Create the queue if it does not exist already
        
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }
            // Initialize the connection to Service Bus Queue
            var client = QueueClient.CreateFromConnectionString(ServiceBusConnectionString, QueueName);
            // Create message, with the message body being automatically serialized
            BrokeredMessage brokeredMessage = new BrokeredMessage(message);
            brokeredMessage.Properties["messageType"] = message.GetType().AssemblyQualifiedName;
            brokeredMessage.Properties["messageContet"] = messageType;
            brokeredMessage.Properties["shipmentReference"] = reference;
            client.Send(brokeredMessage);
        }
    }
}
