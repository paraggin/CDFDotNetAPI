using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CDF_Core.Entities
{
    public class TransactionEntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [JsonProperty(Order = -1)]
        public int Id { get; set; }

        [JsonProperty(Order = 20)]
        public DateTime CreationDate { get; set; }
        [JsonProperty(Order = 21)]
        public int CreationUserId { get; set; }
        [JsonProperty(Order = 22)]
        public DateTime? LastUpdateDate { get; set; }
        [JsonProperty(Order = 23)]
        public int? LastUpdateUserId { get; set; }
    }

    public class TransactionEntityBaseINone
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [JsonProperty(Order = -1)]
        public int Id { get; set; }

        [JsonProperty(Order = 20)]
        public DateTime CreationDate { get; set; }
        [JsonProperty(Order = 21)]
        public int CreationUserId { get; set; }
        [JsonProperty(Order = 22)]
        public DateTime? LastUpdateDate { get; set; }
        [JsonProperty(Order = 23)]
        public int? LastUpdateUserId { get; set; }
    }
}
