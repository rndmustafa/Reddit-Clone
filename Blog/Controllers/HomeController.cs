using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Blog.Models.DomainModel;
using Blog.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly Blog.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(Blog.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(string searchFilter, int pageIndex, string mode)
        {
            HomeIndexViewModel viewModel = new HomeIndexViewModel();

            IQueryable<Post> postIQ = _context.Post
                .Include(p => p.User)
                .Include(p => p.Votes)
                .AsNoTracking()
                .OrderByDescending(p => p.WhenPosted);

            if(!String.IsNullOrEmpty(searchFilter))
            {
                viewModel.SearchFilter = searchFilter;
                postIQ = postIQ.Where(p => p.Title.Contains(searchFilter) 
                || p.Content.Contains(searchFilter)
                || p.User.UserName.Contains(searchFilter));
            }

            pageIndex = pageIndex == 0 ? 1 : pageIndex;

            viewModel.Posts = new PaginatedList<Post>(postIQ, pageIndex, 10);
            viewModel.CalculateCurrentUserVotes(_userManager.GetUserId(User));
            return View(viewModel);
        }

        public IActionResult Vote(int postId, int dir)
        {
            string id = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(id))
            {
                return StatusCode(401);
            }

            Vote vote = _context.Vote.Where(v => v.PostId == postId && v.UserId == id).FirstOrDefault();
            if(vote == null)
            {
                vote = new Vote
                {
                    PostId = postId,
                    UserId = id,
                    Dir = dir
                };
                _context.Vote.Add(vote);
            }
            else
            {
                if(dir == 0)
                {
                    _context.Vote.Remove(vote);
                }
                else
                {
                    vote.Dir = dir;
                    _context.Vote.Update(vote);
                }
            }
            _context.SaveChanges();

            int count = _context.Vote.Where(v => v.PostId == postId).Sum(v => v.Dir);
            return new JsonResult(count);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
