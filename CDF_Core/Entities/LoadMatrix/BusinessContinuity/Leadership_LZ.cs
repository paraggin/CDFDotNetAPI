using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.LoadMatrix.BusinessContinuity
{
    public class Leadership_LZ
    {
        public Leadership_LZ() { }
        [Key]
        public int EmplId { get; set; }

        public string? Name { get; set; }

        public string? Role { get; set; }

        public int? AlternateEmplId { get; set; }

        public string? AlternateName { get; set; }

        public string? Purpose { get; set; }

        public string? Status { get; set; }

    }

    public class Leadership_LZResponse
    {
        public List<Leadership_LZ>? Result { get; set; }
        public int Status { get; set; }
    }
}
