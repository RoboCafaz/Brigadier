using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
            Console.WriteLine(" - - Started new Analysis - - ");
            var reddit = RedditHandler.GetReddit();
            using (var context = new BrigadierEntities())
            {
                CheckSubs(reddit, context);
            }
            Console.WriteLine(" - - Analysis Complete - - ");
        }

        private static void CheckSubs(Reddit reddit, BrigadierEntities context)
        {
            Console.WriteLine("Getting analyzed subs.");
            var subs = context.WatchedSubs.Select(x => x.Url);
            if (subs.Any())
            {
                Console.WriteLine("Retrieved " + subs.Count() + " subs.");
                foreach (var sub in subs)
                {
                    Console.WriteLine(" - " + sub + " - ");
                    var newest = GetRecentPosts(sub, reddit, context);
                    AnalyzePosts(newest, context);
                }
                Console.WriteLine("Saving database...");
                context.SaveChanges();
            }
        }

        private static IEnumerable<RedditSharp.Things.Post> GetRecentPosts(string sub, Reddit reddit, BrigadierEntities context)
        {
            Console.WriteLine("Getting subreddit.");
            var subreddit = reddit.GetSubreddit(sub);
            Console.WriteLine("Getting 25 most recent non-self posts.");
            var newData = subreddit.New.Take(25).Where(x => !x.IsSelfPost);
            return newData;
        }

        private static void AnalyzePosts(IEnumerable<RedditSharp.Things.Post> newest, BrigadierEntities context)
        {
            Console.WriteLine("Analyzing threads...");
            foreach (var post in newest)
            {
                CreateThread(post, context);
            }
        }

        private static void CreateThread(RedditSharp.Things.Post reddit, BrigadierEntities context)
        {
            Console.WriteLine(" - Analyzing " + reddit.Shortlink + "- ");
            var type = GetLinkTypeOfUrl(reddit.Url.Host);
            if (type == 4)
            {
                Console.WriteLine("We don't handle this type yet.");
                return;
            }
            var url = "http://reddit.com" + reddit.Url.LocalPath;
            var local = context.Threads.SingleOrDefault(x => x.Url == reddit.Shortlink);
            if (local != null)
            {
                Console.WriteLine("It already exists.");
                return;
            }
            Console.WriteLine("New thread!");
            local = new Thread
            {
                Url = reddit.Shortlink,
                Author = reddit.AuthorName,
                Sub = GetSubOfUrl(reddit.Permalink.ToString())
            };
            context.Threads.Add(local);
            var target = context.Threads.SingleOrDefault(x => x.Url == url);
            if (target == null)
            {
                Console.WriteLine("New target!");
                target = new Thread
                {
                    Url = url,
                    Author = "Unknown",
                    Sub = GetSubOfUrl(url)
                };
                context.Threads.Add(target);
            }
            else
            {
                Console.WriteLine("Target already exists.");
            }
            var post = new Post
            {
                LinkTypeId = type,
                Created = reddit.Created,
                LocalThread = local,
                TargetThread = target
            };
            Console.WriteLine("New post record added!");
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
            if (url.Contains("np.red"))
            {
                type = 2;
            }
            else if (url.Contains("archive"))
            {
                type = 3;
            }
            else if (url.Contains("www.red") || url.Contains("red"))
            {
                type = 1;
            }
            return type;
        }
    }
}
