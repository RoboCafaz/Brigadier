using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace Brigadier.Reader
{
    public static class RedditHandler
    {
        private static Reddit Reddit;

        public static Reddit GetReddit()
        {
            if (Reddit == null)
            {
                Console.WriteLine("Creating reddit instance.");
                var reddit = new Reddit(Options.UserName, Options.Password);
                reddit.InitOrUpdateUser();
                if (reddit.User == null)
                {
                    throw new RedditException("Could not authenticate user!");
                }
                Reddit = reddit;
                Console.WriteLine("Reddit instance retrieved.");
            }
            return Reddit;
        }
    }
}
