using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using ApiAiSDK;
using ApiAiSDK.Model;
using Microsoft.AspNetCore.Http;


namespace chatbot_iSAS.Controllers
{
    public class HomeController : Controller
    {
        private ApiAi apiAi;
        private string sessionId;

        protected void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Session.SetString("sessionId", null);
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public string Response()
        {
            var config = new AIConfiguration("fd56e430546a4302b4085f0754b57843", SupportedLanguage.Dutch);
            apiAi = new ApiAi(config);

            var response = apiAi.TextRequest("Ik ben opzoek naar het jaarrooster.");

            return response.Result.Fulfillment.Speech;
        }

        [HttpPost]
        public ActionResult Question(string question)
        {
            var config = new AIConfiguration("fd56e430546a4302b4085f0754b57843", SupportedLanguage.Dutch);
            if (HttpContext.Session.GetString("sessionId") != null)
            {
                config.SessionId = HttpContext.Session.GetString("sessionId");
            }
            apiAi = new ApiAi(config);

            var response = apiAi.TextRequest(question);

            if (HttpContext.Session.GetString("sessionId") == null && response.SessionId != null)
            {
                HttpContext.Session.SetString("sessionId", response.SessionId);
            }

            return Content("<p>Jij: " + question + " <br>iSAS: " + response.Result.Fulfillment.Speech + "</p> <hr>", "text/html");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
