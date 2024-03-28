using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Common
{
    public class DeleteModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
