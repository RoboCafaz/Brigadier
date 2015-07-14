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
                    var analyzed = AnalyzePosts(newest).Where(x => x.LinkTypeId != 4);
                    if (analyzed.Any())
                    {
                        foreach (var thread in analyzed)
                        {
                            context.Threads.Add(thread);
                        }
                        updated = true;
                    }
                }
                if (updated)
                {
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
        }

        private static IEnumerable<Post> GetRecentPosts(string sub, Reddit reddit, BrigadierEntities context)
        {
            var subreddit = reddit.GetSubreddit(sub);
            var existing = context.Threads.Where(x => x.Sub == sub).Select(x => x.Url);
            var newData = subreddit.New.Take(25).Where(x => !x.IsSelfPost);
            return newData.Where(x => !existing.Contains(x.Shortlink));
        }

        private static IEnumerable<Thread> AnalyzePosts(IEnumerable<Post> newest)
        {
            return newest.Select(CreateThread);
        }

        private static Thread CreateThread(Post post)
        {
            var thread = new Thread
            {
                Url = post.Shortlink,
                Author = post.AuthorName,
                Sub = GetSubOfUrl(post.Permalink.ToString()),
                LinkTypeId = GetLinkTypeOfUrl(post.Url.ToString()),
                TargetUrl = post.Url.ToString(),
                TargetAuthor = "Unknown",
                TargetSub = GetSubOfUrl(post.Url.ToString()),
                Comment = false,
                Created = post.Created
            };
            return thread;
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
