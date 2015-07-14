using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Brigadier.EntityFramework;

namespace Brigadier.Web.Controllers
{
    public class SubredditsController : Controller
    {
        // GET: Subreddits
        public ActionResult Index()
        {
            using (var context = new BrigadierEntities())
            {
                var subs = context.WatchedSubs.OrderBy(x => x.Url).ToList();
                ViewBag.Title = "Watched Subreddits";
                return View(subs);
            }
        }
    }
}