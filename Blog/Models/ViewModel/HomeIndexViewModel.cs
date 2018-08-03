using Blog.Models.DomainModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModel
{
    public class HomeIndexViewModel
    {
        public PaginatedList<Post> Posts;
        public string SearchFilter;
        public Dictionary<int, int> UserVote = new Dictionary<int, int>();
        public string ImageFilePath = "~\\images\\Posts\\";

        public void CalculateCurrentUserVotes(string userId)
        {
            if(userId == null)
            {
                return;
            }

            foreach (Post post in Posts)
            {
                Vote vote = post.Votes.Where(v => v.UserId == userId).FirstOrDefault();
                if (vote != null)
                {
                    UserVote.Add(post.PostId, vote.Dir);
                }
            }
        }

        public string UserUpvoted(int postId)
        {
            if (UserVote.ContainsKey(postId) && UserVote[postId] == 1)
            {
                return "active";
            }
            else
            {
                return "";
            }
        }

        public string UserDownvoted(int postId)
        {
            if (UserVote.ContainsKey(postId) && UserVote[postId] == -1)
            {
                return "active";
            }
            else
            {
                return "";
            }
        }
    }
}
