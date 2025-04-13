using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.LoadMatrix.BusinessContinuity
{
    public class Crisis_mgmt
    {

        public Crisis_mgmt() { }

        [Key]
        public int EmplId { get; set; }

        public string? Name { get; set; }

        public string? Role { get; set; }

        public string? RoleDescription { get; set; }

        public string? LocationCode { get; set; }

        public string? Description { get; set; }

        public string? Company { get; set; }

        public string? Last { get; set; }

        public string? Action { get; set; }

        public int? AlternateEmplId { get; set; }

        public string? AlternateName { get; set; }

        public string? Purpose { get; set; }

        public string? Status { get; set; }
    }

    public class CrisisMgmtResponse
    {
        public List<Crisis_mgmt>? Result { get; set; }
        public int Status { get; set; }
    }
}
