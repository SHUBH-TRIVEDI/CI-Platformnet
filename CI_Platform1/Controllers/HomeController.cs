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
using System.Web;

namespace CI_Platform1.Controllers
{
    public class HomeController : Controller
    {
        int i = 0, i1 = 0, j=0, j1=0;
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

        public IActionResult LandingPage(long id, string SearchingMission, int ? pageIndex, int Order, long[] ACountries, long[] ACities)
        {
            var mission = _CiPlatformContext.Missions.ToList();
            var newmission = _CiPlatformContext.Missions.ToList();
            var finalmission = _CiPlatformContext.Missions.ToList();
           // ViewData["country"] = _CiPlatformContext.Cities.ToList();


            ////for printing the values inside the cards...............................
            List<City> Cities = _CiPlatformContext.Cities.ToList();
            ViewBag.listofcity = Cities;

            List<Country> countries  = _CiPlatformContext.Countries.ToList();
            ViewBag.listofcountry = countries;

            List<MissionTheme> themes = _CiPlatformContext.MissionThemes.ToList();
            ViewBag.listoftheme = themes;

            //User Admin Name
            int? userid = HttpContext.Session.GetInt32("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //country filter
            if (ACountries != null && ACountries.Length > 0)
            {

                foreach (var country in ACountries)
                {
                    //mission = mission.Where(m => m.CountryId == country).ToList();
                    if (i == 0)
                    {
                        mission = mission.Where(m => m.CountryId == country + 500).ToList();
                        i++;
                    }

                    finalmission = newmission.Where(m => m.CountryId == country).ToList();

                    mission.AddRange(finalmission);
                    if (mission.Count() == 0)
                    {
                        return RedirectToAction("NoMissionFound", "Home");
                    }

                    //------------------------------------for Button------------------------------ -

                    ViewBag.countryId = country;
                    if (ViewBag.countryId != null)
                    {
                        var countryElement = _CiPlatformContext.Countries.Where(m => m.CountryId == country).ToList();
                        if (i1 == 0)
                        {
                            countries = _CiPlatformContext.Countries.Where(m => m.CountryId == country + 50000).ToList();
                            i1++;
                        }
                        countries.AddRange(countryElement);
                        //var c1 = _CIDbContext.Countries.FirstOrDefault(m => m.CountryId == country);
                        //ViewBag.country = c1.Name;
                    }
                }
                ViewBag.country = countries;
                //Countries = _CIDbContext.Countries.ToList();


            }

            //city filter
            if (ACities != null && ACities.Length > 0)
            {
                foreach (var city in ACities)
                {
                    //mission = mission.Where(m => m.CityId == city).ToList();
                    if (j == 0)
                    {
                        mission = mission.Where(m => m.CityId == city + 500).ToList();
                        j++;
                    }

                    finalmission = newmission.Where(m => m.CityId == city).ToList();

                    mission.AddRange(finalmission);
                    if (mission.Count() == 0)
                    {
                        return RedirectToAction("NoMissionFound", "Home");
                    }
                    ViewBag.city = city;
                    if (ViewBag.city != null)
                    {
                        var city1 = _CiPlatformContext.Cities.Where(m => m.CityId == city).ToList();
                        if (j1 == 0)
                        {
                            Cities = _CiPlatformContext.Cities.Where(m => m.CityId == city + 50000).ToList();
                            j1++;
                        }
                        Cities.AddRange(city1);
                        //var c1 = _CIDbContext.Cities.FirstOrDefault(m => m.CityId == city);
                        //ViewBag.city = c1.Name;
                    }
                }
                ViewBag.city = Cities;
                Cities = _CiPlatformContext.Cities.ToList();


            }

            //switch (Order)
            //{
            //    case 1:
            //        mission = _CiPlatformContext.Missions.OrderBy(e=>e.Title).ToList();
            //        break;
            //    case 2:
            //        mission = _CiPlatformContext.Missions.OrderByDescending(e => e.StartDate).ToList();
            //        break;
            //    case 3:
            //        mission = _CiPlatformContext.Missions.OrderBy(e => e.EndDate).ToList();
            //        break;
            //    default:
            //        mission = _CiPlatformContext.Missions.OrderBy(e => e.Theme).ToList();
            //        break;

            //       // return View(mission.ToList());
            //}        


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

            //Pagination  const
            //int pageSize = 6; // Number of items to display per page
            //int pageNumber = (page ?? 1); // Default to the first page
            //var totalItems = _CiPlatformContext.Missions.Count(); // Total number of items
            ////var items = _CiPlatformContext.Missions
            //var items = mission
            //.Skip((pageNumber - 1) * pageSize)
            //.Take(pageSize)
            //.ToList();

            //ViewBag.TotalItems = totalItems;
            //ViewBag.PageNumber = pageNumber;
            //ViewBag.PageSize = pageSize;
            //ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            //ViewData["mission"] = items.ToList();
            int pageSize = 3;
            int skip = (pageIndex ?? 0) * pageSize;
            var Missions = mission.Skip(skip).Take(pageSize).ToList();
            int totalMissions = mission.Count();
            ViewBag.TotalMission = totalMissions;
            ViewBag.TotalPages = (int)Math.Ceiling(totalMissions / (double)pageSize);
            ViewBag.CurrentPage = pageIndex ?? 0;


            // Get the current URL
            UriBuilder uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host);
            if (Request.Host.Port.HasValue)
            {
                uriBuilder.Port = Request.Host.Port.Value;
            }
            uriBuilder.Path = Request.Path;

            // Remove the query parameter you want to exclude
            var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            query.Remove("pageIndex");
            uriBuilder.Query = query.ToString();



            ViewBag.currentUrl = uriBuilder.ToString();



            return View(Missions);
            
        }

        private void ToList()
        {
            throw new NotImplementedException();
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

        public IActionResult volunteering(int id)
        {
            var mission1 = _CiPlatformContext.Missions.FirstOrDefault(m => m.MissionId == id);
            ViewBag.mission1= mission1;

            var city = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == mission1.CityId);
            ViewBag.city = city;

            //List<MissionTheme> themes = _CiPlatformContext.MissionThemes.ToList();
            //ViewBag.listoftheme = themes;

            var Theme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == mission1.ThemeId);
            ViewBag.Theme = Theme;

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