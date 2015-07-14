using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brigadier.EntityFramework;
using RedditSharp;
using RedditSharp.Things;

namespace Brigadier.Reader.Analyzer
{
    public static class SubReader
    {
        public static void Run()
        {
            var reddit = GetReddit();
            using (var context = new BrigadierEntities())
            {
                CheckSubs(reddit, context);
            }
        }

        private static Reddit GetReddit()
        {
            var reddit = new Reddit(Options.UserName, Options.Password);
            reddit.InitOrUpdateUser();
            if (reddit.User == null)
            {
                throw new RedditException("Could not authenticate user!");
            }
            return reddit;
        }

        private static void CheckSubs(Reddit reddit, BrigadierEntities context)
        {
            var subs = context.WatchedSubs.Select(x => x.Url).ToList();
            if (subs.Any())
            {
                foreach (var sub in subs)
                {
                    GetRecentPosts(sub, reddit, context);
                }
            }
        }

        private static void GetRecentPosts(string sub, Reddit reddit, BrigadierEntities context)
        {
            var subreddit = reddit.GetSubreddit(sub);
            if (subreddit != null)
            {
                var newest = subreddit.New.Take(100);
                var threads = AnalyzeThreads(sub, newest);
                var existing = context.Threads.Where(x => x.Location == sub).Select(x => x.Url);
                var newThreads = threads.Where(x => !existing.Contains(x.Url));
            }
        }

        private static IEnumerable<Thread> AnalyzeThreads(string sub, IEnumerable<Post> newest)
        {
            return newest.Select(x => new Thread
            {
                Url = x.Permalink.ToString(),
                Location = sub
            });
        }
    }
}
