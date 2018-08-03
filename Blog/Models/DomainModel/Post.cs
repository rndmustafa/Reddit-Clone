using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Blog.Models.DomainModel
{
    public class Post
    {
        public int PostId { get; set; }
        public string UserId { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(2000)]
        public string Content { get; set; }

        public string Type { get; set; }

        public DateTime WhenPosted { get; set; }

        public User User { get; set; }
        public IList<Vote> Votes { get; set; }
    }
}