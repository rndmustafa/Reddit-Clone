using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.DomainModel
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public int Dir { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}
