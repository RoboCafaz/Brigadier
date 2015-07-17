using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Brigadier.EntityFramework;
using RedditSharp;
using RedditSharp.Things;
using Post = Brigadier.EntityFramework.Post;

namespace Brigadier.Reader.Analyzer
{
    public static class SubReader
    {
        private static readonly Regex SubredditRegex = new Regex("/r/.+?/");

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
            var subs = context.WatchedSubs.Select(x => x.Url);
            if (subs.Any())
            {
                var updated = false;
                foreach (var sub in subs)
                {
                    var newest = GetRecentPosts(sub, reddit, context);
                    AnalyzePosts(newest, context);
                }
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    throw e;
                }
            }
        }

        private static IEnumerable<RedditSharp.Things.Post> GetRecentPosts(string sub, Reddit reddit, BrigadierEntities context)
        {
            var subreddit = reddit.GetSubreddit(sub);
            var existing = context.Threads.Where(x => x.Sub == sub).Select(x => x.Url);
            var newData = subreddit.New.Take(25).Where(x => !x.IsSelfPost);
            return newData.Where(x => !existing.Contains(x.Shortlink));
        }

        private static void AnalyzePosts(IEnumerable<RedditSharp.Things.Post> newest, BrigadierEntities context)
        {
            foreach (var post in newest)
            {
                CreateThread(post, context);
            }
        }

        private static void CreateThread(RedditSharp.Things.Post reddit, BrigadierEntities context)
        {
            if (context.Posts.Any(x => x.LocalThread.Url == reddit.Shortlink && x.TargetThread.Url == reddit.Url.ToString()))
            {
                return;
            }
            var local = context.Threads.SingleOrDefault(x => x.Url == reddit.Shortlink);
            if (local == null)
            {
                local = new Thread
                {
                    Url = reddit.Shortlink,
                    Author = reddit.AuthorName,
                    Sub = GetSubOfUrl(reddit.Permalink.ToString())
                };
                context.Threads.Add(local);
            }
            var target = context.Threads.SingleOrDefault(x => x.Url == reddit.Url.ToString());
            if (target == null)
            {
                target = new Thread
                {
                    Url = reddit.Url.ToString(),
                    Author = "Unknown",
                    Sub = GetSubOfUrl(reddit.Url.ToString())
                };
                context.Threads.Add(target);
            }
            var post = new Post
            {
                LinkTypeId = GetLinkTypeOfUrl(reddit.Url.ToString()),
                Created = reddit.Created,
                LocalThread = local,
                TargetThread = target
            };
            context.Posts.Add(post);
        }

        private static string GetSubOfUrl(string url)
        {
            var sub = "Unknown";
            var match = SubredditRegex.Match(url);
            if (match.Success)
            {
                var str = match.Value;
                sub = match.Value.Substring(3, str.Length - 4);
            }
            return sub;
        }

        private static int GetLinkTypeOfUrl(string url)
        {
            var type = 4;
            if (url.Contains("//np.red"))
            {
                type = 2;
            }
            else if (url.Contains("//archive"))
            {
                type = 3;
            }
            else if (url.Contains("//www.red") || url.Contains("//red"))
            {
                type = 1;
            }
            return type;
        }
    }
}
