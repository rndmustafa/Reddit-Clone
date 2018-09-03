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
        public ApplicationDbContext Context { get; set; }
        private const int MaxDepth = 3;

        [MinLength(1)]
        public string CommentContent { get; set; }

        public PostDetailsViewModel(ApplicationDbContext _context,
                                    SignInManager<IdentityUser> _signInManager,
                                    Post post, 
                                    string userId)
        {
            Context = _context;
            Post = post;

            IQueryable<Comment> commentsIQ = _context.Comment
                .Include(c => c.User)
                .Where(c => c.PostId == Post.PostId);
            Comments = GetTopLevelComments(commentsIQ);
            foreach(Comment c in Comments)
            {
                LoadChildComments(Context,c,0);
            }

            Vote = _context.Vote.Where(v => v.PostId == Post.PostId && v.UserId == userId).SingleOrDefault();
            SignInManager = _signInManager;
        }

        public PaginatedList<Comment> GetTopLevelComments(IQueryable<Comment> commentsIQ)
        {
            IQueryable<Comment> topCommentsIQ = commentsIQ.Where(c => c.ParentCommentId == null);
            return new PaginatedList<Comment>(topCommentsIQ, 1, 10);
        }

        public static void LoadChildComments(ApplicationDbContext context, Comment comment, int depth)
        {
            if(depth + 1 < MaxDepth)
            {
                comment.ChildComments = context.Comment
                    .Include(c => c.User)
                    .Where(c => c.ParentCommentId == comment.CommentId)
                    .AsNoTracking()
                    .ToList();
                if(comment.ChildComments != null)
                {
                    foreach(Comment child in comment.ChildComments)
                    {
                        LoadChildComments(context, child, depth + 1);
                    }
                }
            }
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
                AppendComment(result, comments[i]);
                if (i != comments.Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("]");
            return result.ToString();
        }

        private static void AppendComment(StringBuilder result, Comment comment)
        {
            result.Append("{");
            result.Append($"\"commentId\": \"{comment.CommentId}\",");
            result.Append($"\"userName\": \"{comment.User.UserName}\",");
            result.Append($"\"whenPosted\": \"{comment.WhenPosted}\",");
            result.Append($"\"content\": \"{comment.Content}\",");

            if(comment.ChildComments != null)
            {
                result.Append("\"childComments\": [");
                for (int i = 0; i < comment.ChildComments.Count; i++)
                {
                    AppendComment(result, comment.ChildComments[i]);
                    if (i != comment.ChildComments.Count - 1)
                    {
                        result.Append(",");
                    }
                }
                result.Append("]");
            }
            else
            {
                result.Append("\"childComments\": \"[]\"");
            }

            result.Append("}");
        }
    }
}
