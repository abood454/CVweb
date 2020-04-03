using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CVweb.Models;
using CVweb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace CVweb.Controllers
{
    public class AccountController : Controller
    {
        private readonly CVaction _cvaction;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(CVaction cVaction, UserManager<IdentityUser> userManager
                                                             ,SignInManager<IdentityUser> signInManager)
        {

            _cvaction = cVaction;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task<IActionResult> index(int page = 1)
        {
            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("login", "Account");
            }
            if (User.Identity.Name != "admin@admin.com")
            {
                return RedirectToAction("login", "Account");
            }
            var s = _cvaction.getadmin();
            var model = await PagingList.CreateAsync( s, 3, page);
            return View(model);
        }
        public IActionResult userhome()
        {
            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("login", "Account");
            }
            if (User.Identity.Name == "admin@admin.com")
            {
                return RedirectToAction("index", "Account");
            }
            var a = _cvaction.getyours(User.Identity.Name);

            return View(a);
        }



        public IActionResult er()
        {
            return View();
        }
        public async Task<IActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "Account");
        }
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Account", "login");
            }
            Reguser model = new Reguser
            {
                ReturnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(Reguser reguser )
        {
            reguser.ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid) {
                var user = new IdentityUser { Email = reguser.email,UserName=reguser.email};
               var result=await  userManager.CreateAsync(user, reguser.password);

                if (result.Succeeded)
                {
                
                    return RedirectToAction("er", "Account");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); 
                }
            }
            return View(reguser);
        }

        [HttpGet]
        public async Task<IActionResult> login(string returnUrl)
        {
            if (signInManager.IsSignedIn(User))
            {
               return RedirectToAction("Index", "Home");
            }
            LoginView model = new LoginView
            {
                ReturnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> login(LoginView model)
        {
            if (signInManager.IsSignedIn(User))
            {
                RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.email);
                if (user != null && !user.EmailConfirmed
                    && (await userManager.CheckPasswordAsync(user, model.password))) 
                {
                    ModelState.AddModelError("", "email not confirm yet");
                    return View(model);

                }
                var result = await signInManager.PasswordSignInAsync(model.email,model.password
                    ,model.rem,false);

                if (result.Succeeded)
                {
                 
                 return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError("", "invalid login");
               
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult EXlogin(string pro,string returnUrl2)
        {
            var rede = Url.Action("Exlogincallback", "Account", new { ReturnUrl = returnUrl2 });
            var porp = signInManager.ConfigureExternalAuthenticationProperties(pro, rede);
            return new ChallengeResult(pro, porp);
        }

        public async Task<IActionResult>
            Exlogincallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginView loginViewModel = new LoginView
            {
                ReturnUrl = returnUrl,
                ExternalLogin =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                            ,EmailConfirmed=true
                        };

                        await userManager.CreateAsync(user);
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on ";

                return View("Error");
            }
        }


    }
}