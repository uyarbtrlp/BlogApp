using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogApp.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using BlogApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly BlogContext _context;
        public BlogController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IWebHostEnvironment env, SignInManager<AppUser> signInManager, BlogContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _signInManager = signInManager;
            _context = context;


        }

        // GET: Blog
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.Username = user.UserName;
            ViewBag.Photo = user.ProfilePhoto;
            ViewBag.Name = user.Name;
            ViewBag.Surname = user.Surname;
            ViewBag.Email = user.Email;
            ViewBag.Phone = user.PhoneNumber;
            var blogContext = _context.Blogs.Where(i => i.UserId == user.Id).Select(i=>new BlogModel() {
                Id=i.Id,
                BlogCategories=i.BlogCategories.Select(i=>new BlogCategoryModel() {
                    Category=i.Category
                }).ToList(),
                Content = i.Content.Length > 100 ? i.Content.Substring(0, 100) + "..." : i.Content,
                Date =i.Date,
                Image=i.Image,
                Title=i.Title,
                User=i.User,
                UserId=i.UserId
            }).OrderByDescending(i => i.Date); 
            return View(await blogContext.ToListAsync());
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comments = _context.Comments.Where(i => i.BlogId == id).ToList();
            var blog = await _context.Blogs
                .Include(b => b.User)
                .Select(i=>new BlogModel()
                {
                    Id = i.Id,
                    BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                    {
                        Category = i.Category
                    }).ToList(),
                    Content = i.Content,
                    Date = i.Date,
                    Image = i.Image,
                    Title = i.Title,
                    User = i.User,
                    UserId = i.UserId,
                    Comments = comments.Select(i => new CommentModel()
                    {
                        Blog = i.Blog,
                        BlogId = i.BlogId,
                        CommentText = i.CommentText,
                        Id = i.Id,
                        Name = i.Name,
                        Photo = i.Photo,
                        Surname = i.Surname
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, BlogModel blogModel, string values)
        {
            string fileName = "";
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.Length > 0)
                    {
                        var extensition = Path.GetExtension(file.FileName);
                        if (extensition == ".jpg" || extensition == ".png")
                        {
                            var dir = _env.ContentRootPath + "\\upload";
                            var randomFilename = Path.GetRandomFileName();
                            fileName = Path.ChangeExtension(randomFilename, ".jpg");
                            var path = Path.Combine(dir, fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                            {
                                file.CopyTo(fileStream);
                                blogModel.Date = DateTime.Now.ToString();
                                blogModel.UserId = user.Id;
                                blogModel.User = user;
                                blogModel.Image = fileName;
                                var blog = new Blog()
                                {

                                    UserId = blogModel.UserId,
                                    User = blogModel.User,
                                    Title = blogModel.Title,
                                    Content = blogModel.Content,
                                    Date = blogModel.Date,
                                    Image = blogModel.Image
                                };









                                if (values != null)
                                {
                                    var categories = values.Split(" ");
                                    var list = new List<BlogCategory>();
                                    //var categoryList = new List<Category>();
                                    if (categories.Length <= 7)
                                    {
                                        foreach (var item in categories)
                                        {

                                            if (item == null || item == "")
                                            {

                                            }
                                            else
                                            {
                                                var category = _context.Categories.Where(i => i.Name == item).FirstOrDefault();
                                                list.Add(new BlogCategory()
                                                {
                                                    BlogId = blog.Id,
                                                    Blog = blog,
                                                    Category = category,
                                                    CategoryId = category.Id
                                                });
                                                // burada kendimiz kategorileri ekledik.
                                                /*categoryList.Add(new Category()
                                                {
                                                    Name = item
                                                });*/
                                            }

                                        }
                                        _context.BlogCategories.AddRange(list);
                                        blog.BlogCategories = list;
                                    }
                                
                               
                                    // _context.Categories.AddRange(categoryList);

                                }
                                
                                await _context.AddAsync(blog);
                                await _context.SaveChangesAsync();
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Lütfen resim dosyası seçiniz.");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Lütfen bir resim dosyası seçiniz.");
                    }

                }

                return View(blogModel);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog =  _context.Blogs.Select(i=>new BlogModel() {
                Id = i.Id,
                BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                {
                    Category = i.Category
                }).ToList(),
                Content = i.Content,
                Date = i.Date,
                Image = i.Image,
                Title = i.Title,
                User = i.User,
                UserId = i.UserId
            }).Where(i=>i.Id==id).FirstOrDefault();
            
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Content,UserId,Date,Image,User")] BlogModel blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    blog.UserId = user.Id;
                    blog.User = user;
                    var newBlog = new Blog()
                    {
                        Content = blog.Content,
                        User = blog.User,
                        Date = blog.Date,
                        Id = blog.Id,
                        Image = blog.Image,
                        Title = blog.Title,
                        UserId = blog.UserId
                    };

                    _context.Update(newBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(string id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
        public async Task <IActionResult> CreateCategory()
        {
            _context.Categories.Add(new Category() { Name = "Spor" });
            _context.SaveChanges();
            return View();
        }
    }
}
