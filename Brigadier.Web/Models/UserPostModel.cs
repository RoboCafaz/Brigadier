using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brigadier.Web.Models
{
    public class PostCountModel
    {
        public String Name { get; set; }
        public int Count { get; set; }
    }

    public class UserPostCountModel : PostCountModel
    {
        public UserPostCountModel()
        {
            SubPosts = new List<PostCountModel>();
        }

        public IEnumerable<PostCountModel> SubPosts { get; set; }
    }
}