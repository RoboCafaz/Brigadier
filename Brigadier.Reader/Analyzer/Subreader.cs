using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace Brigadier.Reader.Analyzer
{
    public static class SubReader
    {
        public static void Run()
        {
            var reddit = GetReddit();
        }

        public static Reddit GetReddit()
        {
            var reddit = new Reddit(Options.Secret);
            reddit.InitOrUpdateUser();
            if (reddit.User == null)
            {
                throw new RedditException("Could not authenticate user!");
            }
            return reddit;
        }
    }
}
