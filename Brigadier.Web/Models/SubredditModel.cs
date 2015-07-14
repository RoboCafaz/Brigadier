using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brigadier.Web.Models
{
    public class SubredditModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public IEnumerable<SubredditPair> Incoming { get; set; }
        public IEnumerable<SubredditPair> Outgoing { get; set; }
    }

    public class SubredditPair
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
}