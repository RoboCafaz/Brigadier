using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using Brigadier.EntityFramework;
using Brigadier.Web.Models;

namespace Brigadier.Web.Controllers
{
    public class StatsController : Controller
    {
        // GET: Subreddits
        public ActionResult Index()
        {
            using (var context = new BrigadierEntities())
            {
                var posts = context.Posts.GroupBy(x => x.LocalThread.Author).Select(x => new UserPostCountModel
                {
                    Name = x.Key,
                    Count = x.Count(),
                    SubPosts = x.GroupBy(y => y.LocalThread.Sub).Select(y => new PostCountModel
                    {
                        Name = y.Key,
                        Count = y.Count()
                    }).OrderByDescending(y => y.Count)
                }).OrderByDescending(x => x.Count);
                return View(posts.ToList());
            }
        }
    }
}