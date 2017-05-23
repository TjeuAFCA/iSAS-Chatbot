using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using ApiAiSDK;
using ApiAiSDK.Model;
using chatbot_iSAS.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

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

        private string GetUserId(string sessionId)
        {
            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest) WebRequest.Create(
                        new Uri("https://api.api.ai/v1/userEntities/User?v=20150910&sessionId=" + sessionId));
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + "fd56e430546a4302b4085f0754b57843");
                using (StreamReader streamReader =
                    new StreamReader((httpWebRequest.GetResponse() as HttpWebResponse).GetResponseStream()))
                {
                    string end = streamReader.ReadToEnd();
                    Console.WriteLine(end);
                    return end;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }

        private Userentity createUserentity(string userId)
        {
            Userentity json = new Userentity();
            json.sessionId = sessionId;
            json.name = "User";
            json.extend = false;
            Entry entry = new Entry();
            entry.value = userId;
            entry.synonyms = new string[] { userId };
            json.entries = new Entry[] { entry };
            return json;
        }

        private void PostUserId(string sessionId, string userId)
        {
            var json = createUserentity(userId);

            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest) WebRequest.Create(
                        new Uri("https://api.api.ai/v1/userEntities?v=20150910&sessionId=" + sessionId));
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + "fd56e430546a4302b4085f0754b57843");
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                string str = JsonConvert.SerializeObject((object) json, Formatting.None, settings);
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(str);
                    streamWriter.Close();
                }
                using (StreamReader streamReader =
                    new StreamReader((httpWebRequest.GetResponse() as HttpWebResponse).GetResponseStream()))
                {
                    string end = streamReader.ReadToEnd();
                    Console.WriteLine(end);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

            PostUserId(response.SessionId, "user01");
            GetUserId(response.SessionId);

            if (HttpContext.Session.GetString("sessionId") == null && response.SessionId != null)
            {
                HttpContext.Session.SetString("sessionId", response.SessionId);
            }

            return Content("<p>Jij: " + question + " <br>iSAS: " + response.Result.Fulfillment.Speech + "</p> <hr>", "text/html");
        }

        [Authorize]
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
