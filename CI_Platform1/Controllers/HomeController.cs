using CI.Models;
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
        int i = 0, i1 = 0, j = 0, j1 = 0, k=0, k1=0;
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

        public IActionResult LandingPage(long id, string SearchingMission, int? pageIndex, int cntry, int Order, long[] ACountries, long[] ACities, long[] Atheme)
        {
            List<Mission> mission = _CiPlatformContext.Missions.ToList();
            List<Mission> finalmission = _CiPlatformContext.Missions.ToList();
            List<Mission> newmission = _CiPlatformContext.Missions.ToList();
            List<GoalMission> goalMissions = _CiPlatformContext.GoalMissions.ToList();
            ViewBag.Goal1 = goalMissions;

            ViewData["country"] = _CiPlatformContext.Countries.ToList();

            //if (cntry != 0)
            //{
            //    ViewData["city"] = _CiPlatformContext.Cities.Where(m => m.CountryId == cntry).ToList();
            //}

            //for printing the values inside the cards...............................
            List<City> Cities = _CiPlatformContext.Cities.ToList();
            ViewBag.listofcity = Cities;

            List<Country> countries = _CiPlatformContext.Countries.ToList();
            ViewBag.listofcountry = countries;

            List<MissionTheme> themes = _CiPlatformContext.MissionThemes.ToList();
            ViewBag.listoftheme = themes;

            //User Admin Name
            int? userid = HttpContext.Session.GetInt32("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Country filter
            if (ACountries != null && ACountries.Length > 0)
            {

                foreach (var country in ACountries)
                {
                    if (i == 0)
                    {
                        mission = mission.Where(m => m.CountryId == country + 700).ToList();
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
                            countries = _CiPlatformContext.Countries.Where(m => m.CountryId == country + 60000).ToList();
                            i1++;
                        }
                        countries.AddRange(countryElement);
                    }
                }
                ViewBag.country = countries;
            }

            //City Filter
            if (ACities != null && ACities.Length > 0)
            {
                foreach (var city in ACities)
                {
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
                  
                    }
                }
                ViewBag.city = Cities;
                Cities = _CiPlatformContext.Cities.ToList();


            }

            //Theme Filter
            if (Atheme != null && Atheme.Length > 0)
            {

                foreach (var theme in Atheme)
                {
                    if (k == 0)
                    {
                        mission = mission.Where(m => m.ThemeId == theme + 700).ToList();
                        k++;
                    }

                    finalmission = newmission.Where(m => m.ThemeId == theme).ToList();

                    mission.AddRange(finalmission);
                    if (mission.Count() == 0)
                    {
                        return RedirectToAction("NoMissionFound", "Home");
                    }

                    //------------------------------------for Button------------------------------ -

                    ViewBag.themeid = theme;
                    if (ViewBag.themeid != null)
                    {
                        var themeElement = _CiPlatformContext.MissionThemes.Where(m => m.MissionThemeId == theme).ToList();
                        if (k1 == 0)
                        {
                            themes = _CiPlatformContext.MissionThemes.Where(m => m.MissionThemeId == theme + 60000).ToList();
                            k1++;
                        }
                        themes.AddRange(themeElement);
                    }
                }
                ViewBag.theme = themes;
            }

            //Order By
            switch (Order)
            {
                case 1:
                    mission = _CiPlatformContext.Missions.OrderBy(e => e.Title).ToList();
                    break;
                case 2:
                    mission = _CiPlatformContext.Missions.OrderByDescending(e => e.StartDate).ToList();
                    break;
                case 3:
                    mission = _CiPlatformContext.Missions.OrderBy(e => e.EndDate).ToList();
                    break;
                //default:
                //    mission = _CiPlatformContext.Missions.OrderBy(e => e.Theme).ToList();
                //    break;


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
            int pageSize = 6;
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



        //Forget Password Model
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


                var fromAddress = new MailAddress("sangareen2019@gmail.com", "Sender Name");
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
                    Credentials = new NetworkCredential("sangareen2019@gmail.com", "ihjxxitmiyotxpym"),
                    EnableSsl = true
                };
                smtpClient.Send(message); 

                return RedirectToAction("Login", "Home");
            }

            return View();
        }


        //Reset Password
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




        //Volunteering Missions Model
        public IActionResult volunteering(int missionid)
        {

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();

            var volmission = _CiPlatformContext.Missions.FirstOrDefault(m => m.MissionId == missionid);
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == volmission.ThemeId);
            var City = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == volmission.CityId);
            var themeobjective = _CiPlatformContext.GoalMissions.FirstOrDefault(m => m.MissionId == missionid);
            string[] Startdate = volmission.StartDate.ToString().Split(" ");
            string[] Enddate = volmission.EndDate.ToString().Split(" ");
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            volunteeringVM.MissionId = missionid;
            volunteeringVM.Title = volmission.Title;
            volunteeringVM.ShortDescription = volmission.ShortDescription;
            volunteeringVM.OrganizationName = volmission.OrganizationName;
            volunteeringVM.Description = volmission.Description;
            volunteeringVM.OrganizationDetail = volmission.OrganizationDetail;
            volunteeringVM.Availability = volmission.Availability;
            volunteeringVM.MissionType = volmission.MissionType;
            volunteeringVM.Cityname = City.Name;
            volunteeringVM.Themename = theme.Title;
            volunteeringVM.EndDate = Enddate[0];
            volunteeringVM.StartDate = Startdate[0];
            volunteeringVM.GoalObjectiveText = themeobjective.GoalObjectiveText;
            ViewBag.Missiondetail = volunteeringVM;

            //Related Missions
            var relatedmission = _CiPlatformContext.Missions.Where(m => m.ThemeId == volmission.ThemeId && m.MissionId != missionid).ToList();
            foreach (var item in relatedmission.Take(3))
            {

                var relcity = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == item.CityId);
                var reltheme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == item.ThemeId);
                var relgoalobj = _CiPlatformContext.GoalMissions.FirstOrDefault(m => m.MissionId == item.MissionId);
                string[] Startdate1 = item.StartDate.ToString().Split(" ");
                string[] Enddate2 = item.EndDate.ToString().Split(" ");



                relatedlist.Add(new VolunteeringVM
                {
                    MissionId = item.MissionId,
                    Cityname = relcity.Name,
                    Themename = reltheme.Title,
                    Title = item.Title,
                    ShortDescription = item.ShortDescription,
                    StartDate = Startdate1[0],
                    EndDate = Enddate2[0],
                    Availability = item.Availability,
                    OrganizationName = item.OrganizationName,
                    GoalObjectiveText = relgoalobj.GoalObjectiveText,
                    MissionType = item.MissionType,
                }
                );
                ;
            }


            ViewBag.relatedmission = relatedlist.Take(3);

            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}