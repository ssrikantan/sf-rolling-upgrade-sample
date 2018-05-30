using Microsoft.AspNetCore.Mvc;
using sfwebapp.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace sfwebapp.Controllers
{
    public class HomeController : Controller
    {
        ServiceNames sName;
        public HomeController(ServiceNames _sName)
        {
            sName = _sName;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Message"] = "Your application description page.";
            string l = HttpContext.Request.Host.Value;
            string sUri = string.Format("http://{0}:{1}/api/values", sName._sfWebApiServiceName,sName._portno);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(sUri);
                string apiresponse = await response.Content.ReadAsStringAsync();
                ViewData["webapphost"] = "Host name of web app: " + HttpContext.Request.Host.Value;
                ViewData["webapihost"] = "Host name of API: "+ apiresponse;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                ViewData["webapphost"] = error;

            }
            return View();
        }

        public IActionResult About()
        {
           
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
