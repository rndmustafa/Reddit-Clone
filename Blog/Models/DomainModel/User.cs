using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Models.DomainModel
{
    public class User : IdentityUser
    {
        public DateTime StartDate { get; set; }

        public ICollection<Post> Posts { get; set; }
        public IList<Vote> Votes { get; set; }
    }
}
