using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.DomainModel
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        [ForeignKey("ParentComment")]
        public int? ParentCommentId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime WhenPosted { get; set; }

        public Post Post { get; set; }
        public Comment ParentComment { get; set; }
        public IList<Comment> ChildComments { get; set; }
        public User User { get; set; }
    }
}
