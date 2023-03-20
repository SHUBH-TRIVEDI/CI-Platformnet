//using CI.Models;
using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Platform1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CI_Platform1.Controllers
{
    public class HomeController : Controller
    {
        int i = 0, i1 = 0, j = 0, j1 = 0, k = 0, k1 = 0;
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
            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;


            List<Mission> mission = _CiPlatformContext.Missions.Include(x => x.MissionRatings).ToList();
            ViewBag.listofmission = mission;
            List<Mission> finalmission = _CiPlatformContext.Missions.ToList();
            List<Mission> newmission = _CiPlatformContext.Missions.ToList();
            List<GoalMission> goalMissions = _CiPlatformContext.GoalMissions.ToList();
            ViewBag.Goal1 = goalMissions;
            ViewBag.user = _CiPlatformContext.Users.FirstOrDefault(e => e.UserId == id);
            //for printing the values inside the cards...............................
            List<City> Cities = _CiPlatformContext.Cities.ToList();
            ViewBag.listofcity = Cities;

            List<Country> countries = _CiPlatformContext.Countries.ToList();
            ViewBag.listofcountry = countries;

            List<MissionTheme> themes = _CiPlatformContext.MissionThemes.ToList();
            ViewBag.listoftheme = themes;

            //User Admin Name
            var  userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            int finalrating = 0;
            foreach(var missions in mission)
            {
                var ratinglist = _CiPlatformContext.MissionRatings.Where(m => m.MissionId == missions.MissionId).ToList();
                if (ratinglist.Count() > 0)
                {
                    int rat = 0;
                    foreach (var m in ratinglist)
                    {
                        rat = rat + int.Parse(m.Rating);


                    }
                    finalrating = rat / ratinglist.Count();
                }
            }
            
            ViewBag.finalrating=finalrating;


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
            //switch (Order)
            //{
            //    case 1:
            //        mission = _CiPlatformContext.Missions.OrderBy(e => e.Title).ToList();
            //        break;
            //    case 2:
            //        mission = _CiPlatformContext.Missions.OrderByDescending(e => e.StartDate).ToList();
            //        break;
            //    case 3:
            //        mission = _CiPlatformContext.Missions.OrderBy(e => e.EndDate).ToList();
            //        break;
            //    //default:
            //    //    mission = _CiPlatformContext.Missions.OrderBy(e => e.Theme).ToList();
            //    //    break;


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
                    HttpContext.Session.SetString("userID", user.UserId.ToString());
                    HttpContext.Session.SetString("Firstname", user.FirstName);

                    return RedirectToAction("LandingPage", "Home", new {@id = user.UserId});
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
        public IActionResult volunteering(long missionid, long id, long missionId)
            
        {
            List<Mission> mission = _CiPlatformContext.Missions.ToList();
            ViewBag.listofmission = mission;

            var userid = HttpContext.Session.GetString("userID");
            ViewBag.UserId = int.Parse(userid);

            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            ViewBag.user = _CiPlatformContext.Users.FirstOrDefault(e => e.UserId == id);
            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();

            IEnumerable<Mission> objMis = _CiPlatformContext.Missions.ToList();
            IEnumerable<Comment> objComm = _CiPlatformContext.Comments.ToList();
            IEnumerable<Mission> selected = _CiPlatformContext.Missions.Where(m => m.MissionId == missionid).ToList();

            var volmission = _CiPlatformContext.Missions.FirstOrDefault(m => m.MissionId == missionid);
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == volmission.ThemeId);
            var City = _CiPlatformContext.Cities.FirstOrDefault(m => m.CityId == volmission.CityId);
            var prevRating = _CiPlatformContext.MissionRatings.Where(e => e.MissionId == missionid && e.UserId == id).FirstOrDefault();
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
            if (prevRating != null) { volunteeringVM.UserPrevRating = prevRating.Rating; }
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


                int finalrating = 0;
                foreach (var missions in mission)
                {
                    var ratinglist = _CiPlatformContext.MissionRatings.Where(m => m.MissionId == missions.MissionId).ToList();
                    if (ratinglist.Count() > 0)
                    {
                        int rat = 0;
                        foreach (var m in ratinglist)
                        {
                            rat = rat + int.Parse(m.Rating);


                        }
                        finalrating = rat / ratinglist.Count();
                    }
                }

                ViewBag.finalrating = finalrating;


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
            return View(selected);
        }

        //Add to favrouite
        [HttpPost]
        public async Task<IActionResult> Addfav(long Id, long missionId)
        {
            FavoriteMission fav = await _CiPlatformContext.FavoriteMissions.FirstOrDefaultAsync(fm => fm.UserId == Id && fm.MissionId == missionId);
            if (fav != null)
            {

                _CiPlatformContext.Remove(fav);
                _CiPlatformContext.SaveChanges();
                return Json(new { success = true, favmission = "1" });
            }
            else
            {
                var ratingele = new FavoriteMission();

                ratingele.UserId = Id;
                ratingele.MissionId = missionId;
                _CiPlatformContext.AddAsync(ratingele);
                _CiPlatformContext.SaveChanges();
                return Json(new { success = true, favmission = "0" });
            }
        }
        //comments
        public JsonResult PostComment(int missionId, string Content)
        {
            Comment objComment = new Comment();
            objComment.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
            objComment.MissionId = missionId;
            objComment.Comment1 = Content;
            objComment.CreatedAt = DateTime.Now;
            _CiPlatformContext.Comments.Add(objComment);
            _CiPlatformContext.SaveChanges();
            return Json(objComment);
        }

        //===============================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //--------REccomend to coworker--------

        [HttpPost]
        public async Task<IActionResult> Sendmail(long[] userid, int id)
        {


            foreach (var item in userid)
            {
                var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == item);
                var resetLink = Url.Action("Volunteering", "Home", new { user = user.UserId, missionId = id }, Request.Scheme);

                var fromAddress = new MailAddress("officehl1882@gmail.com", "Sender Name");
                var toAddress = new MailAddress(user.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />This is to <br /><br /><a href='{resetLink}'>{resetLink}</a>";
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("officehl1882@gmail.com", "yedkuuhuklkqfzwx"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

            }
            return Json(new { success = true });
        }

        //-------------------------------Rating---------------------------
        [HttpPost]
        public async Task<IActionResult> AddRating(string rating, long id, long missionId)
        {
            MissionRating ratingExists = await _CiPlatformContext.MissionRatings.FirstOrDefaultAsync(fm => fm.UserId == id && fm.MissionId == missionId);

            if (ratingExists != null)
            {
                ratingExists.Rating = rating;
                //ratingExists.UserId = id;
                //ratingExists.MissionId = missionId;
                _CiPlatformContext.MissionRatings.Update(ratingExists);
                await _CiPlatformContext.SaveChangesAsync();
                return Json(new { success = true, ratingExists, isRated = true });

            }
            else
            {
                var newRating = new MissionRating();
                newRating.Rating = rating;
                newRating.UserId = id;
                newRating.MissionId = missionId;
                await _CiPlatformContext.MissionRatings.AddAsync(newRating); await _CiPlatformContext.SaveChangesAsync();
                return Json(new { success = true, newRating, isRated = true });
            }

        }
    }
}