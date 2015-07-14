using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brigadier.EntityFramework;
using RedditSharp;

namespace Brigadier.Reader.Analyzer
{
    public static class SubReader
    {
        public static void Run()
        {
            var reddit = GetReddit();
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

        private static void CheckSubs()
        {
            using (var context = new BrigadierEntities())
            {
                
            }
        }
    }
}
