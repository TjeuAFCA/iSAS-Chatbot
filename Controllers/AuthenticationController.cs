using System.Web.Security;
using chatbot_iSAS.Models;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_iSAS.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Form is not valid; please review and try again.";
                return View("Login");
            }

            if (user.Username == "user" && user.Password == "password")
                FormsAuthentication.RedirectFromLoginPage(user.Username, true);

            ViewBag.Error = "Credentials invalid. Please try again.";
            return View("Login");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}