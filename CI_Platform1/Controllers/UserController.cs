using CI_Entities1.Data;
using CI_Entities1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CI_Platform1.Controllers
{
    public class UserController : Controller
    {
        private readonly CiPlatformContext _CiPlatformContext;

        public UserController(CiPlatformContext CiPlatformContext)
        {
            _CiPlatformContext = CiPlatformContext;
        }


        public IActionResult Registration()
        {
           // User user = new User();
            return View();
        }

        [HttpPost]
        public IActionResult Registration(User user)
        {
            var obj = _CiPlatformContext.Users.FirstOrDefault(x => x.Email == user.Email);
            if (obj == null)
            {
                _CiPlatformContext.Users.Add(user);
                _CiPlatformContext.SaveChanges();
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Email = "Email already exists";
            }
            return View();
        }
    }
}