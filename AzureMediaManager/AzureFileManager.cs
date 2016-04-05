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

namespace AzureMediaManager
{
    public class AzureFileManager
    {       
            private CloudQueue _imagesQueue;
            private static CloudBlobContainer _imagesBlobContainer;
                   
          
            public void InitializeStorage(string tenantName, string subfolderName)
            {
                // Open storage account using credentials from .cscfg file.
                // TODO: Shanel Should be moved to the configuration file
                var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=pidocuments;AccountKey=6NaiQmUiUKwiWafuzYMeVv9i3TEREe81DTKTUCRmkh5dUp7QVtW/kQ9cAlVJeQhFiLD8zcTPdgpQbBhCNKJ8ag==;BlobEndpoint=https://pidocuments.blob.core.windows.net/;TableEndpoint=https://pidocuments.table.core.windows.net/;QueueEndpoint=https://pidocuments.queue.core.windows.net/;FileEndpoint=https://pidocuments.file.core.windows.net/");

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
            public async Task<bool> Upload(System.Web.HttpPostedFileBase imageFile, string imageFileNameInFull)
            {
              
                CloudBlockBlob imageBlob = null;
                string imageUrl = string.Empty;

                // Insert record to Document table.
                //_documentService.InsertDocumentRecord(new GetDocumentInput
                //{
                //    CategoryId = input.CategoryId,
                //    ReferenceId = input.ReferenceId,
                //    FileNameWithExtention = imageFile.FileName,
                //    UploadedFileName = imageFileName,
                //    DocumentType = input.DocumentType,
                //    ContentDescription = input.ContentDescription,
                //    Classification = input.Classification,
                //    SearchAttributes = input.SearchAttributes,
                //    Tag = input.Tag
                //});

                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await UploadAndSaveBlobAsync(imageFile, imageFileNameInFull);
                    imageUrl = imageBlob.Uri.ToString();
                }

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(imageUrl);
                    await _imagesQueue.AddMessageAsync(queueMessage);
                }

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
            private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFileBase imageFile, string imageFileName)
            {
                var blobName = imageFileName; 
                // Retrieve reference to a blob. 
                var imageBlob = _imagesBlobContainer.GetBlockBlobReference(blobName);
                // Create the blob by uploading a local file.
                using (var fileStream = imageFile.InputStream)
                {
                    await imageBlob.UploadFromStreamAsync(fileStream);
                }

                return imageBlob;
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
