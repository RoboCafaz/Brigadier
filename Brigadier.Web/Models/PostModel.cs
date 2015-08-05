using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brigadier.Web.Models
{
    public class PostModel
    {
        public PostModel()
        {
            LocalHistory = new List<ScoreInstance>();
            TargetHistory = new List<ScoreInstance>();
        }

        public String LocalThread { get; set; }
        public String LocalType { get; set; }
        public String LocalAuthor { get; set; }
        public String LocalSub { get; set; }
        public String TargetThread { get; set; }
        public String TargetAuthor { get; set; }
        public String TargetSub { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<ScoreInstance> LocalHistory { get; set; }
        public IEnumerable<ScoreInstance> TargetHistory { get; set; }
    }

    public class ScoreInstance
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
    }
}