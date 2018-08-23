using Blog.Models.DomainModel;
using Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModel
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public Vote Vote { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IQueryable<Comment> CommentsIQ { get; set; }
        public int PageIndex { get; set; }

        [MinLength(1)]
        public string CommentContent { get; set; }

        public PostDetailsViewModel(ApplicationDbContext _context,
                                    SignInManager<IdentityUser> _signInManager,
                                    Post post, 
                                    string userId,
                                    int pageIndex)
        {
            Post = post;
            CommentsIQ = _context.Comment.Include(c => c.User).Where(c => c.PostId == Post.PostId);
            Vote = _context.Vote.Where(v => v.PostId == Post.PostId && v.UserId == userId).SingleOrDefault();
            SignInManager = _signInManager;
            PageIndex = pageIndex;
        }

        public PaginatedList<Comment> GetTopLevelComments()
        {
            IQueryable<Comment> topCommentsIQ = CommentsIQ.Where(c => c.ParentCommentId == null);
            return new PaginatedList<Comment>(topCommentsIQ, PageIndex, 10);
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

    }
}
