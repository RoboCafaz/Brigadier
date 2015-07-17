using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brigadier.EntityFramework;
using RedditSharp;

namespace Brigadier.Reader.Analyzer
{
    public class PostReader
    {
        public static void Run()
        {
            var reddit = RedditHandler.GetReddit();
            using (var context = new BrigadierEntities())
            {
                var toCheck = GetPostsFromLastDay(context);
                CheckThreads(reddit, context, toCheck);
                context.SaveChanges();
            }
        }

        private static IEnumerable<Thread> GetPostsFromLastDay(BrigadierEntities context)
        {
            var yesterday = DateTime.Now.AddHours(-24);
            return context.Posts.Where(x => !x.Done && x.Created > yesterday).Select(x => x.TargetThread).Distinct();
        }

        private static void CheckThreads(Reddit reddit, BrigadierEntities context, IEnumerable<Thread> threads)
        {
            foreach (var thread in threads)
            {
                CheckThread(reddit, context, thread);
            }
        }

        private static void CheckThread(Reddit reddit, BrigadierEntities context, Thread thread)
        {
            Uri uri;
            if (Uri.TryCreate(thread.Url, UriKind.Absolute, out uri))
            {
                var thing = reddit.GetPost(uri);
                if (thing == null)
                {
                    KillThread(thread);
                }
                else
                {
                    var history = new History
                    {
                        Time = DateTime.Now,
                        ThreadId = thread.Id,
                        Score = thing.Score
                    };
                    context.Histories.Add(history);
                }
            }
            else
            {
                KillThread(thread);
            }
        }

        private static void KillThread(Thread thread)
        {
            foreach (var post in thread.TargetingPosts)
            {
                KillPost(post);
            }
        }

        private static void KillPost(Post post)
        {
            post.Done = true;
        }
    }
}
