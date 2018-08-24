using Blog.Models.DomainModel;
using Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blog.Models.ViewModel
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public Vote Vote { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public PaginatedList<Comment> Comments { get; set; }

        [MinLength(1)]
        public string CommentContent { get; set; }

        public PostDetailsViewModel(ApplicationDbContext _context,
                                    SignInManager<IdentityUser> _signInManager,
                                    Post post, 
                                    string userId)
        {
            Post = post;
            IQueryable<Comment> commentsIQ = _context.Comment.Include(c => c.User).Where(c => c.PostId == Post.PostId);
            Comments = GetTopLevelComments(commentsIQ);
            Vote = _context.Vote.Where(v => v.PostId == Post.PostId && v.UserId == userId).SingleOrDefault();
            SignInManager = _signInManager;
        }

        public PaginatedList<Comment> GetTopLevelComments(IQueryable<Comment> commentsIQ)
        {
            IQueryable<Comment> topCommentsIQ = commentsIQ.Where(c => c.ParentCommentId == null);
            return new PaginatedList<Comment>(topCommentsIQ, 1, 10);
        }

        public string UserUpvoted()
        {
            if(Vote != null && Vote.Dir == 1)
            {
                return "active";
            }
            return "";
        }

        public string UserDownvoted()
        {
            if (Vote != null && Vote.Dir == -1)
            {
                return "active";
            }
            return "";
        }

        public static string Serialize(PaginatedList<Comment> comments)
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < comments.Count; i++)
            {
                result.Append("{");
                result.Append($"'commentId': '{comments[i].CommentId}',");
                result.Append($"'userName': '{comments[i].User.UserName}',");
                result.Append($"'whenPosted': '{comments[i].WhenPosted}',");
                result.Append($"'content': '{comments[i].Content}'");
                result.Append("}");
                if (i != comments.Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("]");
            return result.ToString();
        }

    }
}
