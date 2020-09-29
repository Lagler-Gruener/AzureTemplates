using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sol_dbaas_jit_webapp.Models
{
    public class AdminModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Server")]
        public string Server { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Database")]
        public string Database { get; set; }
    }
}
