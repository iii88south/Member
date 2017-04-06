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
        [ValidateAntiForgeryToken]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            returnUrl = GetRedirectUrl(returnUrl);
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallBack", "Auth", new { returnUrl = returnUrl }));
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallBack(string returnUrl)
        {
            var logininfo = await GetAuthenticationManager().GetExternalLoginInfoAsync();

            if (logininfo == null)
            {
                return RedirectToAction("Login");
            }

            if (logininfo != null)
            {


                var id = new ClaimsIdentity(logininfo.ExternalIdentity.Claims, DefaultAuthenticationTypes.ApplicationCookie);

                var user = await usermanager.FindByEmailAsync(logininfo.Email);

                if (user != null)
                {
                     await Signin(user);


                }
                else
                {
                    user = new AppUser()
                    {
                        UserName = logininfo.Email,
                        Email = logininfo.Email
                    };

                    var result = await usermanager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        await Signin(user);

                        return RedirectToAction("Index", "Home");
                    }
                }

                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login");
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
            if (!string.IsNullOrEmpty(user.Country))
            {
                identity.AddClaim(new Claim(ClaimTypes.Country, user.Country));
            }
           
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

    internal class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUrl, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUrl;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary["XsrfKey"] = UserId;

            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);

        }
    }


}