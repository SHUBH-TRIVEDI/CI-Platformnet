using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Platform1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace CI_Platform1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, CiPlatformContext CiPlatformContext)
        {
            _CiPlatformContext = CiPlatformContext;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Forget()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Resetpass()
        {
            return View();
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public IActionResult nomissionfound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Storylisting()
        {
            return View();
        }

        public IActionResult volunteering()
        {
            return View();
        }

        private readonly CiPlatformContext _CiPlatformContext;

        //login method
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
{
                var user=await _CiPlatformContext.Users.Where(u => u.Email == model.Email &&  u.Password== model.Password).FirstOrDefaultAsync();
                
                if(user==null)
                {
                    ViewBag.Error = "Email or Password has not Matched to the registered Credentials";
                }
                else
                {
                    return RedirectToAction("LandingPage", "Home");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forget(ForgetModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _CiPlatformContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgetPass", "Home");
                }


                var token = Guid.NewGuid().ToString();


                var passwordReset = new PasswordReset
                {
                    Email = model.Email,
                    Token = token
                };
                _CiPlatformContext.PasswordResets.Add(passwordReset);
                _CiPlatformContext.SaveChanges();


                var resetLink = Url.Action("ResetPassword", "Home", new { email = model.Email, token }, Request.Scheme);


                var fromAddress = new MailAddress("tatvahl@gmail.com", "Sender Name");
                var toAddress = new MailAddress(model.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("tatvahl@gmail.com", "dvbexvljnrhcflfw"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

                return RedirectToAction("ForgetPass", "Home");
            }

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}