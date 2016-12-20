using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using PI.Contract.DTOs.User;
using PI.Contract.DTOs.Shipment;
using Newtonsoft.Json;

namespace PI.StatusUpdater
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        static HttpClient client = new HttpClient();


        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            //var host = new JobHost();
            //// The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            RunAsync().Wait();
            Console.WriteLine("Success !!!");
        }


        static async Task RunAsync()
        {
            // New code:
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServiceURL"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Get the product
                var httpResponse = await UpdateShipmentStatusAsync();
                Console.WriteLine($"Shipment Records updated. (HTTP Status = {httpResponse.StatusCode})");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        static async Task<HttpResponseMessage> UpdateShipmentStatusAsync()
        {
    
            HttpResponseMessage response = null;

            var userDto = new UserDto { UserName = ConfigurationManager.AppSettings["UserName"].ToString(),
                                        Password = ConfigurationManager.AppSettings["Password"].ToString()
            };

            var shipmentResponse = await client.PostAsJsonAsync($"api/shipments/GetAllShipmentsForWebJob", userDto);

            var returnValue = await shipmentResponse.Content.ReadAsAsync<List<ShipmentDto>>();
            
            foreach (var item in returnValue.ToList())
            {
                if (item.GeneralInformation.TrackingNumber != "")
                {
                    item.InvokingUserDetails = new UserDto { UserName = userDto.UserName, Password = userDto.Password };
                    response = await client.PostAsJsonAsync($"api/shipments/UpdateAllShipmentsFromWebJob", item);
                    response.EnsureSuccessStatusCode();
                }
            }

            response.EnsureSuccessStatusCode();
            // Deserialize the updated product from the response body.
            return response;
        }
    }
}
