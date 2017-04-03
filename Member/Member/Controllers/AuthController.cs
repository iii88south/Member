using Member.App_Start;
using Member.Models;
using Member.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Member.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        UserManager<AppUser> usermanager = Startup.UserManagerFactory.Invoke();


        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel() { returnUrl = returnUrl };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await usermanager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                await Signin(user);

                return Redirect(GetRedirectUrl(model.returnUrl));
            }
            else
            {
                ModelState.AddModelError("", "帳號或密碼輸入錯誤");
                return View();
            }

        }



        [HttpGet]
        public ActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Regist(RegistViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser()
            {
                UserName = model.Email,
                Country = model.Country,
                Sex = model.Sex
            };

            var result = await usermanager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await Signin(user);

                return RedirectToAction("Index", "Home");

            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item);
                }
            }

            return View(model);


        }


        public async Task<ActionResult> SignOut(AppUser user)
        {

            var identity = HttpContext.User.Identity;
            GetAuthenticationManager().SignOut();

            return RedirectToAction("Index", "Home");

        }



        private async Task Signin(AppUser user)
        {
            var identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim(ClaimTypes.Country, user.Country));
            GetAuthenticationManager().SignIn(identity);
        }

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();

            return ctx.Authentication;
        }


        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("index", "Home");
            }

            return returnUrl;
        }
    }
}