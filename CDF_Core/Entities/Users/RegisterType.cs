using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.Users
{

    [Table("RegisterType", Schema = "Lookup")]
    public class RegisterType : EntityBase
    {
        public RegisterType() { }

        public string TypeAr { get; set; }
        public string TypeEn { get; set; }
        [ForeignKey("FkUserRegisterTypeId")]
        public ICollection<Users.Profile> Users { get; set; }

    }
}
