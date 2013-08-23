using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIAuth.Common.Configuration;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.ClientId = CIAuthSection.Instance().ClientId;
            ViewBag.AuthServer = CIAuthSection.Instance().AuthServer;
            ViewBag.RedirectUrl = Request.Url.GetLeftPart(UriPartial.Authority) + HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/callback";

            return View();
        }

 
    }
}
