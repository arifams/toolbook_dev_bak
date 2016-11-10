using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using PI.Contract.DTOs.Shipment;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using PI.Data;
using PI.Contract.Business;
using PI.Business;
using PI.Common;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using System.Security.Cryptography.X509Certificates;

namespace WorkerRoleWithSBQueue2
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "addshipmentstosisqueue";
     
        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        
        static HttpClient client = new HttpClient();
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);
      


        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage(async (receivedMessage) =>
               {
                   try
                   {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                       var type = receivedMessage.GetType();
                       if (receivedMessage.Properties["messageContet"] != null && receivedMessage.Properties["messageContet"].ToString() == "addShipmentXML")
                       {
                           await HandleSISRequest(receivedMessage);
                       }
                   }
                   catch (Exception e)
                   {
                        // Handle any message processing specific exceptions here
                    }
               });

            CompletedEvent.WaitOne();
        }

        public async Task HandleSISRequest(BrokeredMessage brokeredMessage)
        {           
                      
            client.BaseAddress = new Uri(CloudConfigurationManager.GetSetting("ServiceURL").ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SISShipmentCreateDto createDto = new SISShipmentCreateDto()
            {
                ShipmentReference = brokeredMessage.Properties["shipmentReference"].ToString(),
                AddShipmentXml = brokeredMessage.GetBody<string>().ToString()
            };


            try
            {

                var httpResponse = await CreateShipmentAsync(createDto);
                Console.WriteLine($"Shipment Records updated. (HTTP Status = {httpResponse.StatusCode})");
                //   Console.WriteLine($"Shipment Records updated. (HTTP Status = {System.Web.HttpResponse.StatusCode})");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

           
            }

        static async Task<HttpResponseMessage> CreateShipmentAsync(SISShipmentCreateDto createDto)
        {
                                                                                        
            HttpResponseMessage response = await client.PostAsJsonAsync($"api/shipments/HandleSISRequest", createDto);
            response.EnsureSuccessStatusCode();           
            // Deserialize the updated product from the response body.
            return response;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
