using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Web;
using System.IO;
using System.Net;
using System.Configuration;

namespace AzureMediaManager
{
    public class AzureFileManager
    {
        private CloudQueue _imagesQueue;
        private static CloudBlobContainer _imagesBlobContainer;

        public string BlobStorageKey
        {
            get
            {
                return ConfigurationManager.AppSettings["BlobStorageKey"].ToString();
            }
        }

        public void InitializeStorage(string tenantName, string subfolderName)
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(BlobStorageKey);

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            // TODO: Shanel  please move this "fa-documents" to the configuration file
            _imagesBlobContainer = blobClient.GetContainerReference(string.Format("piblobstorage/TENANT_{0}/{1}", tenantName, subfolderName));

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            var queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            _imagesQueue = queueClient.GetQueueReference(string.Format("piblobstorage/TENANT_{0}/{1}", tenantName, subfolderName));

        }

        /// <summary>
        /// Creates the specified image file.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        public async Task<bool> Upload(Stream imageFileStream, string imageFileNameInFull)
        {

            CloudBlockBlob imageBlob = null;
            string imageUrl = string.Empty;

            if (imageFileStream != null && imageFileStream.Length != 0)
            {
                imageBlob = await UploadAndSaveBlobAsync(imageFileStream, imageFileNameInFull);
                imageUrl = imageBlob.Uri.ToString();
            }

            //if (imageBlob != null)
            //{
            //    var queueMessage = new CloudQueueMessage(imageUrl);
            //    await _imagesQueue.AddMessageAsync(queueMessage);
            //}

            return true;
        }


        /// <summary>
        /// Creates file from URL and upload to Azure
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        public async Task<bool> UploadFromFileURL(string sourceFileURL, string newFileName)
        {

            CloudBlockBlob imageBlob = null;
            string imageUrl = string.Empty;

            if (sourceFileURL != null && newFileName != null)
            {
                imageBlob = await GetFileURLAndSaveBlobAsync(sourceFileURL, newFileName);
                imageUrl = imageBlob.Uri.ToString();
            }

            //if (imageBlob != null)
            //{
            //    var queueMessage = new CloudQueueMessage(imageUrl);
            //    await _imagesQueue.AddMessageAsync(queueMessage);
            //}

            return true;
        }


        /// <summary>
        /// Deletes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public async Task Delete(string url)
        {
            await DeleteAdBlobsAsync(url);
        }

        /// <summary>
        /// Upload the and save BLOB asynchronous.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <param name="imageFileName">Name of the image file.</param>
        /// <returns></returns>
        private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(Stream imageFileStream, string imageFileName)
        {
            var blobName = imageFileName;
            // Retrieve reference to a blob. 
            var imageBlob = _imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = imageFileStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            return imageBlob;

        }



        /// <summary>
        /// Upload the and save BLOB asynchronous.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <param name="imageFileName">Name of the image file.</param>
        /// <returns></returns>
        private async Task<CloudBlockBlob> GetFileURLAndSaveBlobAsync(string sourceFileURL, string newFileName)
        {
            //string accountKey = "key";
            //string newFileName = "newfile2.png";
            //string destinationContainer = "destinationcontainer";
            //string sourceUrl = "http://www.site.com/docs/doc1.xls";

            //CloudStorageAccount csa = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            //CloudBlobClient blobClient = csa.CreateCloudBlobClient();
            //var blobContainer = blobClient.GetContainerReference(destinationContainer);
            //blobContainer.CreateIfNotExists();
            //var newBlockBlob = blobContainer.GetBlockBlobReference(newFileName);
            //newBlockBlob.StartCopyFromBlob(new Uri(sourceUrl), null, null, null);

            // Retrieve reference to a blob. 
            var newFileBlob = _imagesBlobContainer.GetBlockBlobReference(newFileName);

            WebRequest req = HttpWebRequest.Create(sourceFileURL);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                await newFileBlob.UploadFromStreamAsync(stream);
            }

            //  await newFileBlob.StartCopyAsync(new Uri(sourceFileURL), null, null, null,null);

            return newFileBlob;
        }


        /// <summary>
        /// Deletes the ad blobs asynchronous.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns></returns>
        private async Task DeleteAdBlobsAsync(string imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                var blobUri = new Uri(imageUrl);

                var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
                var blobToDelete = _imagesBlobContainer.GetBlockBlobReference(blobName);
                await blobToDelete.DeleteAsync();
            }
        }
    }
}
