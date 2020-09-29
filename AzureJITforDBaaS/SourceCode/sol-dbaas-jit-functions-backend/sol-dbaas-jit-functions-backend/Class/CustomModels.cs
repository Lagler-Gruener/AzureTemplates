using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace sol_dbaas_jit_functions_backend.Models
{
    class DatabaseModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("ServerName")]
        public string ServerName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("ResourceGroup")]
        public string ResourceGroup { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Location")]
        public string Location { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("DatabaseName")]
        public string DatabaseName { get; set; }
    }
}
