using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Brigadier.EntityFramework;
using Brigadier.Web.Models;

namespace Brigadier.Web.Controllers
{
    public class SubredditsController : Controller
    {
        // GET: Subreddits
        public ActionResult Index()
        {
            using (var context = new BrigadierEntities())
            {
                var subs = context.WatchedSubs.OrderBy(x => x.Url).Select(x => new SubredditModel
                {
                    Id = x.Id,
                    Url = x.Url,
                    Incoming = context.Threads.Where(y => y.Sub == x.Url).GroupBy(y => y.Sub).Select(y => new SubredditPair
                    {
                        Key = y.Key,
                        Value = y.Count()
                    }).OrderByDescending(y=>y.Value).ThenBy(y => y.Key),
                    Outgoing = context.Threads.Where(y => y.Sub == x.Url).GroupBy(y => y.Sub).Select(y => new SubredditPair
                    {
                        Key = y.Key,
                        Value = y.Count()
                    }).OrderByDescending(y => y.Value).ThenBy(y=>y.Key)
                }).ToList();
                ViewBag.Title = "Watched Subreddits";
                return View(subs);
            }
        }
    }
}