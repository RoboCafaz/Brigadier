using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Brigadier.Web.Controllers
{
    public class SubredditsController : Controller
    {
        // GET: Subreddits
        public ActionResult Index()
        {
            return View();
        }
    }
}