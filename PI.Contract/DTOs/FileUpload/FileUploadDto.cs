using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PI.Contract.DTOs.FileUpload
{
    public class FileUploadDto
    {
        public int Id { get; set; }
        public long TenantId { get; set; }
        public int CategoryId { get; set; }
        public long ReferenceId { get; set; }
        public string SubFolderName { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public string ClientFileName { get; set; }

        public string UploadedFileName { get; set; }

        public string FileAbsoluteURL { get; set; } 

        public string UserId { get; set; }

        public DocumentType DocumentType { get; set; } 
    }
}
