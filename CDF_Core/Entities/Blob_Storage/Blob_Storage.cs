using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.Blob_Storage
{
    public class Blob_Storage
    {
        public Blob_Storage() { }
        public string Name { get; set; }
        public string Version { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
       // public string BlobUrlWithSas { get; set;}
    }
}
