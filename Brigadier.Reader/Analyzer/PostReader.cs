using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brigadier.EntityFramework;
using Newtonsoft.Json;
using RedditSharp;

namespace Brigadier.Reader.Analyzer
{
    public class PostReader
    {
        public static void Run()
        {
            Debug.WriteLine(" - - Started New Post Update Sweep - - ");
            var reddit = RedditHandler.GetReddit();
            using (var context = new BrigadierEntities())
            {
                var toCheck = GetPostsFromLastDay(context);
                CheckThreads(reddit, context, toCheck);
                context.SaveChanges();
            }
            Debug.WriteLine(" - - Sweep Complete - - ");
        }

        private static IEnumerable<Thread> GetPostsFromLastDay(BrigadierEntities context)
        {
            Debug.WriteLine("Getting incomplete posts from the past 24 hours.");
            var yesterday = DateTime.Now.AddHours(-24);
            return context.Posts.Where(x => !x.Done && x.Created > yesterday).Select(x => x.TargetThread).Distinct();
        }

        private static void CheckThreads(Reddit reddit, BrigadierEntities context, IEnumerable<Thread> threads)
        {
            Debug.WriteLine("Checking " + threads.Count() + " threads.");
            foreach (var thread in threads)
            {
                CheckThread(reddit, context, thread);
            }
        }

        private static void CheckThread(Reddit reddit, BrigadierEntities context, Thread thread)
        {
            Debug.WriteLine(" - Checking " + thread.Url + " - ");
            Uri uri;
            if (Uri.TryCreate(thread.Url, UriKind.Absolute, out uri))
            {
                try
                {
                    Debug.WriteLine("Fetching post.");
                    var thing = reddit.GetPost(uri);
                    if (thing == null)
                    {
                        Debug.WriteLine("Could not find post.");
                    }
                    else
                    {
                        var history = new History
                        {
                            Time = DateTime.Now,
                            ThreadId = thread.Id,
                            Score = thing.Score
                        };
                        Debug.WriteLine("Score was " + history.Score + " at " + history.Time);
                        context.Histories.Add(history);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Post fetching failed.");
                }
            }
            else
            {
                Debug.WriteLine("Could not create thread for url.");
            }
            KillThread(thread);
        }

        private static void KillThread(Thread thread)
        {
            foreach (var post in thread.TargetingPosts.Concat(thread.LocalPosts))
            {
                KillPost(post);
            }
        }

        private static void KillPost(Post post)
        {
            Debug.WriteLine("Killing post " + post.Id + "...");
            post.Done = true;
        }
    }
}
