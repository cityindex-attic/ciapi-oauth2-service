using System;
using System.Web.Mvc;
using CIAUTH.Models;

namespace CIAUTH.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index(int statusCode, Exception exception)
        {
            ErrorModel model = new ErrorModel {HttpStatusCode = statusCode, Exception = exception};

            Response.StatusCode = statusCode;

            return View(model);
        }
    }
}
