using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Platform1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            HttpContext.Session.Clear();
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

        public IActionResult LandingPage(long id, string SearchingMission, int ? page)
        {
            int? userid = HttpContext.Session.GetInt32("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home");
            }


            List<Mission> mission = _CiPlatformContext.Missions.ToList();
            foreach (var item in mission)
            {
                var City = _CiPlatformContext.Cities.FirstOrDefault(u => u.CityId == item.CityId);
                var Theme = _CiPlatformContext.MissionThemes.FirstOrDefault(u => u.MissionThemeId == item.ThemeId);
            }
            

            //Search Mission
            if (SearchingMission != null)
            {
                mission = _CiPlatformContext.Missions.Where(m => m.Title.Contains(SearchingMission)).ToList();
                ViewBag.InputSearch = SearchingMission;

                if (mission.Count() == 0)
                {
                    return RedirectToAction("NoMissionFound", "Home");
                }
            }

            //Pagination

            const int pageSize = 5; // Number of items to display per page
            int pageNumber = (page ?? 1); // Default to the first page
            var totalItems = _CiPlatformContext.Missions.Count(); // Total number of items
            var items = _CiPlatformContext.Missions
            //.OrderByDescending(i => i.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(items);
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

        public dynamic SearchingMission { get; private set; }

        //login method

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _CiPlatformContext.Users.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefaultAsync();
                var username = model.Email.Split('@')[0];
                if (user == null)
                {
                    ViewBag.Error = "Email or Password has not Matched to the registered Credentials";
                }
                else
                {
                    HttpContext.Session.SetString("userID", username);
                    HttpContext.Session.SetString("Firstname", user.FirstName);

                    return RedirectToAction("LandingPage", "Home");
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Forget(ForgetModel model)
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


                var resetLink = Url.Action("Resetpass", "Home", new { email = model.Email, token }, Request.Scheme);


                var fromAddress = new MailAddress("officeciplatform@gmail.com", "Sender Name");
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
                    Credentials = new NetworkCredential("officeciplatform@gmail.com", "mhhzcvqiqcaqeggc"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

                return RedirectToAction("Forget", "Home");
            }

            return View();
        }



        [HttpGet]
        public IActionResult Resetpass(string email, string token)
        {
            var passwordReset = _CiPlatformContext.PasswordResets.FirstOrDefault(u => u.Email == email && u.Token == token);
            if (passwordReset == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // Pass the email and token to the view for resetting the password
            var model = new PasswordReset
            {
                Email = email,
                Token = token
            };
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Resetpass(Resetpassword model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = _CiPlatformContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return RedirectToAction("Forget", "User");
                }

                // Find the password reset record by email and token
                var passwordReset = _CiPlatformContext.PasswordResets.FirstOrDefault(u => u.Email == model.Email && u.Token == model.Token);
                if (passwordReset == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Update the user's password
                user.Password = model.Password;
                _CiPlatformContext.SaveChanges();

            }

            return RedirectToAction("Login", "Home");
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}