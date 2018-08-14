using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        public class PostCreateInputModel
        {
            public string Type { get; set; }

            [Required]
            [StringLength(30, MinimumLength = 3)]
            public string Title { get; set; }

            [MinLength(3)]
            [MaxLength(2000)]
            public string Content { get; set; }

            [MinLength(3)]
            [MaxLength(2000)]
            [DataType(DataType.Url)]
            public string Url { get; set; }

            [Display(Name = "File Upload")]
            public IFormFile PictureFile { get; set; }
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly Blog.Data.ApplicationDbContext _context;

        public PostController(UserManager<IdentityUser> userManager, Blog.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Post
        public ActionResult Index()
        {
            return View();
        }

        // GET: Post/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Post/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(PostCreateInputModel postCreateInputModel)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if(postCreateInputModel.Type == "Text")
            {
                if(String.IsNullOrEmpty(postCreateInputModel.Content))
                {
                    ModelState.AddModelError("Content", "Content is a required field.");
                }

                if(!ModelState.IsValid)
                {
                    return View();
                }

                Post newPost = new Post
                {
                    UserId = _userManager.GetUserId(User),
                    Title = postCreateInputModel.Title,
                    Content = postCreateInputModel.Content,
                    WhenPosted = DateTime.Now,
                    Type = "Text"
                };
                _context.Post.Add(newPost);
                _context.SaveChanges();
            }
            else if (postCreateInputModel.Type == "Upload")
            {
                ValidateFormFile(postCreateInputModel.PictureFile, ModelState);

                if(!ModelState.IsValid)
                {
                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(postCreateInputModel.PictureFile.FileName);
                string filePath = "wwwroot\\images\\Posts\\" + fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    postCreateInputModel.PictureFile.CopyTo(stream);
                }

                Post newPost = new Post
                {
                    UserId = _userManager.GetUserId(User),
                    Title = postCreateInputModel.Title,
                    Content = "/images/Posts/" + fileName,
                    WhenPosted = DateTime.Now,
                    Type = "Image"
                };
                _context.Post.Add(newPost);
                _context.SaveChanges();
            }
            else if(postCreateInputModel.Type == "Link")
            {
                string contentType = GetContentType(postCreateInputModel.Url);

                if(contentType == "Video")
                {
                    postCreateInputModel.Url = postCreateInputModel.Url.Replace("watch?v=", "embed/");
                }

                Post newPost = new Post
                {
                    UserId = _userManager.GetUserId(User),
                    Title = postCreateInputModel.Title,
                    Content = postCreateInputModel.Url,
                    WhenPosted = DateTime.Now,
                    Type = contentType
                };
                _context.Post.Add(newPost);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
            
        }

        private string GetContentType(string url)
        {
            string[] imageTypes = { ".gif", ".png", ".jpg", ".jpeg" };
            foreach (string type in imageTypes)
            {
                if (url.EndsWith(type))
                {
                    return "Image";
                }
            }

            if(url.Contains("youtube.com"))
            {
                return "Video";
            }

            return "Other";
        }

        private void ValidateFormFile(IFormFile pictureFile, ModelStateDictionary modelState)
        {
            if(!pictureFile.ContentType.ToLower().Contains("image"))
            {
                modelState.AddModelError("PictureFile", "File must be an image.");
            }
            if(pictureFile.Length == 0)
            {
                modelState.AddModelError("PictureFile", "File size is zero.");
            }
            else if(pictureFile.Length > 20000000)
            {
                modelState.AddModelError("PictureFile", "Images can't be larger than 20 megabytes.");
            }
        }

        // GET: Post/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}