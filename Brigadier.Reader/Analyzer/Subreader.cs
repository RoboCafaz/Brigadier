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
            Debug.WriteLine(" - - Started new Analysis - - ");
            var reddit = GetReddit();
            using (var context = new BrigadierEntities())
            {
                CheckSubs(reddit, context);
            }
            Debug.WriteLine(" - - Analysis Complete - - ");
        }

        private static Reddit GetReddit()
        {
            Debug.WriteLine("Grabbing reddit instance.");
            var reddit = new Reddit(Options.UserName, Options.Password);
            reddit.InitOrUpdateUser();
            if (reddit.User == null)
            {
                throw new RedditException("Could not authenticate user!");
            }
            Debug.WriteLine("Reddit instance retrieved.");
            return reddit;
        }

        private static void CheckSubs(Reddit reddit, BrigadierEntities context)
        {
            Debug.WriteLine("Getting analyzed subs.");
            var subs = context.WatchedSubs.Select(x => x.Url);
            if (subs.Any())
            {
                Debug.WriteLine("Retrieved " + subs.Count() + " subs.");
                foreach (var sub in subs)
                {
                    Debug.WriteLine(" - " + sub + " - ");
                    var newest = GetRecentPosts(sub, reddit, context);
                    AnalyzePosts(newest, context);
                }
                try
                {
                    Debug.WriteLine("Saving database...");
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
            Debug.WriteLine("Getting subreddit.");
            var subreddit = reddit.GetSubreddit(sub);
            Debug.WriteLine("Getting 25 most recent non-self posts.");
            var newData = subreddit.New.Take(25).Where(x => !x.IsSelfPost);
            return newData;
        }

        private static void AnalyzePosts(IEnumerable<RedditSharp.Things.Post> newest, BrigadierEntities context)
        {
            Debug.WriteLine("Analyzing threads...");
            foreach (var post in newest)
            {
                CreateThread(post, context);
            }
        }

        private static void CreateThread(RedditSharp.Things.Post reddit, BrigadierEntities context)
        {
            Debug.WriteLine(" - Analyzing " + reddit.Shortlink + "- ");
            var url = reddit.Url.ToString();
            var type = GetLinkTypeOfUrl(url);
            if (type == 4)
            {
                Debug.WriteLine("We don't handle this type yet.");
                return;
            }
            var local = context.Threads.SingleOrDefault(x => x.Url == reddit.Shortlink);
            if (local != null)
            {
                Debug.WriteLine("It already exists.");
                return;
            }
            Debug.WriteLine("New thread!");
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
                Debug.WriteLine("New target!");
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
                Debug.WriteLine("Target already exists.");
            }
            var post = new Post
            {
                LinkTypeId = type,
                Created = reddit.Created,
                LocalThread = local,
                TargetThread = target
            };
            Debug.WriteLine("New post record added!");
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
