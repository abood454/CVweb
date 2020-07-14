using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CVweb.Models;
using CVweb.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using MailKit.Net.Smtp;
using MimeKit;

namespace CVweb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly CVaction _cvaction;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(CVaction cVaction, UserManager<IdentityUser> userManager
                                                             ,SignInManager<IdentityUser> signInManager
                                                                                  , IHostingEnvironment hostingEnvironment)
        {

            _hostingEnvironment = hostingEnvironment;
            _cvaction = cVaction;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public IActionResult upload(int id,IFormFile cv)
        {
            string unicv = null;
            var model = _cvaction.get(id);

            if (cv != null)
            {
                string upload = Path.Combine(_hostingEnvironment.WebRootPath, "pdf");
                unicv = Guid.NewGuid().ToString() + "_" + cv.FileName;
                string pathphoto = Path.Combine(upload, unicv);
                cv.CopyTo(new FileStream(pathphoto, FileMode.Create)); 
                model.allwork = unicv;
                model.inwork = true;
                _cvaction.update(model);
                /////////////////////////////////////////////////////////////////////////
                var path = Path.Combine(
                           _hostingEnvironment.WebRootPath,
                            "pdf", model.cvpath);
                FileInfo fi = new FileInfo(path);
                if (fi != null)
                {
                    System.IO.File.Delete(path);
                    fi.Delete();
                    
                }
                ///////////////////////////////////////////////////////////////////////
                 path = Path.Combine(
                          _hostingEnvironment.WebRootPath,
                           "img", model.photopath);
                fi = new FileInfo(path);
                if (fi != null)
                {
                    System.IO.File.Delete(path);
                    fi.Delete();
                }

            }
           
            return RedirectToAction("index", "Account");

        }

        public async Task<IActionResult> Download(string filename)
        {

            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/img", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        public async Task<IActionResult> Downloadpdf(string filename)
        {

            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/pdf", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
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
                var user = new IdentityUser { Email = reguser.email,UserName=reguser.email,EmailConfirmed=true};
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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        { 
            
            
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    // Generate the reset password token
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    MimeMessage message = new MimeMessage();

                    MailboxAddress from = new MailboxAddress("first step",
                    "abood.almansour@gmil.com");
                    message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User",
                    model.Email);
                    message.To.Add(to);

                    message.Subject = "ForgotPassword";

                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = passwordResetLink;

                    message.Body = bodyBuilder.ToMessageBody();

                    SmtpClient client = new SmtpClient();
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("abood.almansour@gmail.com", "aaa76543210");

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            
        }
       [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
           
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
        }

    
}