using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Member.ViewModel
{
    public class RegistViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage ="請輸入正確的信箱")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    
        public string Country { get; set; }

     
        public bool Sex { get; set; }
    }
}