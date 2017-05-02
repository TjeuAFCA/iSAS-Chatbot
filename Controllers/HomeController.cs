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

        private string AccessToken = "3f2523e2ea314c8cab00136d2820aedc";
        private string AccessOld = "28e8693eaed34021b9f83e6f1c17be7f"; // Oude van Luuk

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
            var config = new AIConfiguration(AccessToken, SupportedLanguage.Dutch);
            apiAi = new ApiAi(config);

            var response = apiAi.TextRequest("Ik ben opzoek naar het jaarrooster.");

            return response.Result.Fulfillment.Speech;
        }

        [HttpPost]
        public ActionResult Question(string question)
        {
            var config = new AIConfiguration(AccessToken, SupportedLanguage.Dutch);
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
