using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Member.Models;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartup(typeof(Member.App_Start.Startup))]

namespace Member.App_Start
{
    public class Startup
    {
        public static Func<UserManager<AppUser>> UserManagerFactory { get; set; }

        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions() {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login")
            });

            UserManagerFactory = () =>
            {
                var usermanager = new UserManager<AppUser>(new UserStore<AppUser>(new AppContext()));
                usermanager.UserValidator = new UserValidator<AppUser>(usermanager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                return usermanager;
            };

       

        }
    }
}
