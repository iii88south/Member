using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Member.Models
{
    public class AppUser : IdentityUser
    {
        public string Country { get; set; }

        public bool Sex { get; set; }



    }
}