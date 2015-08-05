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
                    Incoming = context.Posts.Where(y => y.TargetThread.Sub == x.Url).GroupBy(y => y.LocalThread.Sub).Select(y => new SubredditPair
                    {
                        Key = y.Key,
                        Value = y.Count()
                    }).OrderByDescending(y => y.Value).ThenBy(y => y.Key),
                    Outgoing = context.Posts.Where(y => y.LocalThread.Sub == x.Url).GroupBy(y => y.TargetThread.Sub).Select(y => new SubredditPair
                    {
                        Key = y.Key,
                        Value = y.Count()
                    }).OrderByDescending(y => y.Value).ThenBy(y => y.Key)
                }).ToList();
                ViewBag.Title = "Watched Subreddits";
                return View(subs);
            }
        }

        public ActionResult View(string id)
        {
            if (id != null)
            {
                using (var context = new BrigadierEntities())
                {
                    var sub = context.WatchedSubs.SingleOrDefault(x => x.Url == id);
                    if (sub == null)
                    {
                        ViewBag.Title = "Not Tracked";
                        ViewBag.Message = "The sub '" + id + "' is not currently having its metrics tracked.";
                        return View("Error");
                    }
                    ViewBag.Title = sub.Url;
                    var model = context.Posts.Where(x => x.LocalThread.Sub == sub.Url).OrderByDescending(x=>x.Created).Select(x => new PostModel
                    {
                        LocalThread = x.LocalThread.Url.Substring(0,20),
                        LocalAuthor = x.LocalThread.Author,
                        LocalSub = x.LocalThread.Sub,
                        LocalType = x.LinkType.Type,
                        LocalHistory = x.LocalThread.History.OrderBy(y=>y.Time).Select(y => new ScoreInstance { Date = y.Time, Score = y.Score }),
                        TargetThread = x.TargetThread.Url.Substring(0, 20),
                        TargetAuthor = x.TargetThread.Author,
                        TargetSub = x.TargetThread.Sub,
                        TargetHistory = x.TargetThread.History.OrderBy(y => y.Time).Select(y => new ScoreInstance { Date = y.Time, Score = y.Score })
                    });
                    return View(model.ToList());
                }
            }
            return RedirectToAction("Index");
        }
    }
}