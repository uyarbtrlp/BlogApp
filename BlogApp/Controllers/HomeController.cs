using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using BlogApp.Context;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly BlogContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IWebHostEnvironment env, SignInManager<AppUser> signInManager, BlogContext context, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;


        }



        public IActionResult Index()
        {
            var categories = _context.Categories.Select(i => new CategoryClass() { Id = i.Id, Name = i.Name }).ToList();
            var blogContext = _context.Blogs.Select(i => new BlogModel()
            {
                Id = i.Id,
                BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                {
                    Category = i.Category,
                    Blog = i.Blog,
                    BlogId = i.BlogId,
                    CategoryId = i.CategoryId
                }).ToList(),
                Content = i.Content.Length>100?i.Content.Substring(0,100)+"...":i.Content,
                Date = i.Date,
                Image = i.Image,
                Title = i.Title,
                User = i.User,
                UserId = i.UserId,
                Categories = categories

            }).OrderByDescending(i=>i.Date);
            return View(blogContext.ToList());

        }

        public IActionResult List(string? id, string? ara)
        {
            var list = new List<BlogModel>();

            var categories = _context.Categories.Select(i => new CategoryClass() { Id = i.Id, Name = i.Name }).ToList();
            if (id == null)
            {

                var blog = _context.Blogs.Where(i => i.Content.ToLower().Contains(ara.ToLower()) || i.Title.ToLower().Contains(ara.ToLower())).Select(i => new BlogModel()
                {
                    Id = i.Id,
                    BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                    {
                        Category = i.Category

                    }).ToList(),
                    Content = i.Content.Length > 100 ? i.Content.Substring(0, 100) + "..." : i.Content,
                    Date = i.Date,
                    Image = i.Image,
                    Title = i.Title,
                    User = i.User,
                    UserId = i.UserId,
                    Categories = categories
                }).OrderByDescending(i => i.Date);
                list.AddRange(blog.ToList());

            }
            else
            {
                var blogCategories = _context.BlogCategories.Where(i => i.CategoryId == id).ToList();

                foreach (var item in blogCategories)
                {
                    var blog = _context.Blogs.Where(i => i.Id == item.BlogId).Select(i => new BlogModel()
                    {
                        Id = i.Id,
                        BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                        {
                            Category = i.Category

                        }).ToList(),
                        Content = i.Content.Length > 100 ? i.Content.Substring(0, 100) + "..." : i.Content,
                        Date = i.Date,
                        Image = i.Image,
                        Title = i.Title,
                        User = i.User,
                        UserId = i.UserId,
                        Categories = categories
                    }).OrderByDescending(i => i.Date); ;
                    list.AddRange(blog.ToList());
                }
            }
            if (id != null && ara != null)
            {
                list= new List<BlogModel>();
                var blogCategories = _context.BlogCategories.Where(i => i.CategoryId == id).ToList();

                foreach (var item in blogCategories)
                {
                    var blog = _context.Blogs.Where(i => i.Content.ToLower().Contains(ara.ToLower()) || i.Title.ToLower().Contains(ara.ToLower()) &&  i.Id == item.BlogId).Select(i => new BlogModel()
                    {
                        Id = i.Id,
                        BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                        {
                            Category = i.Category

                        }).ToList(),
                        Content = i.Content.Length > 100 ? i.Content.Substring(0, 100) + "..." : i.Content,
                        Date = i.Date,
                        Image = i.Image,
                        Title = i.Title,
                        User = i.User,
                        UserId = i.UserId,
                        Categories = categories
                    }).OrderByDescending(i => i.Date); ;
                    list.AddRange(blog.ToList());

                }
                id = null;
                ara = null;




                
            }
            return View(list);

        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> UserBlogs(string? id)
        {
           
            var blogContext = _context.Blogs.Where(i => i.UserId==id).Select(i => new BlogModel()
            {
                Id = i.Id,
                BlogCategories = i.BlogCategories.Select(i => new BlogCategoryModel()
                {
                    Category = i.Category
                }).ToList(),
                Content = i.Content.Length > 100 ? i.Content.Substring(0, 100) + "..." : i.Content,
                Date = i.Date,
                Image = i.Image,
                Title = i.Title,
                User = i.User,
                UserId = i.UserId
            }).OrderByDescending(i => i.Date);
            return View(await blogContext.ToListAsync());
            
        }
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comments = _context.Comments.Where(i => i.BlogId == id).ToList();
            var blog = await _context.Blogs
                .Include(b => b.User)
                .Select(i => new BlogModel()
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
                        Blog=i.Blog,
                        BlogId=i.BlogId,
                        CommentText=i.CommentText,
                        Id=i.Id,
                        Name=i.Name,
                        Photo=i.Photo,
                        Surname=i.Surname
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(UserModel userModel, IFormFile file)
        {
            string fileName = "";

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
                            AppUser user = new AppUser()
                            {
                                Name = userModel.Name,
                                Surname = userModel.Surname,
                                UserName = userModel.Username,
                                Email = userModel.Email,
                                PhoneNumber = userModel.PhoneNumber,
                                Date = DateTime.Now.ToString(),
                                ProfilePhoto = fileName





                            };
                            var result = await _userManager.CreateAsync(user, userModel.Password);
                            if (result.Succeeded)
                            {





                                return RedirectToAction("Login");
                            }
                            else
                            {
                                foreach (var item in result.Errors)
                                {
                                    if (item.Code == "PasswordRequiresNonAlphanumeric")
                                        ModelState.AddModelError("", "Şifre en az bir adet simge içermelidir.");
                                    else if (item.Code == "PasswordRequiresLower")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet küçük harf içermelidir.");
                                    }
                                    else if (item.Code == "PasswordRequiresUpper")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet büyük harf içermelidir.");
                                    }
                                    else if (item.Code == "DuplicateUserName")
                                    {
                                        ModelState.AddModelError("", "Kullanıcı adı " + userModel.Username + " şu anda kullanılıyor. Lütfen başka kullanıcı adı seçiniz.");
                                    }
                                    else if (item.Code == "DuplicateEmail")
                                    {
                                        ModelState.AddModelError("", userModel.Email + " adında zaten üyeliğimiz bulunmaktadır. Şifrenizi unuttuysanız şifrenizi sıfırlayınız.");
                                    }
                                    else if (item.Code == "InvalidUserName")
                                    {
                                        ModelState.AddModelError("", "Lütfen Türkçe karakter içeren bir kullanıcı adı almayınız");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Resim dosyası seçiniz");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Bir dosya seçiniz");
                }




            }
            return View(userModel);

        }
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel userModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(userModel.Username);



                if (user != null)
                {

                    var signInResult = await _signInManager.PasswordSignInAsync(userModel.Username, userModel.Password, true, false);
                    if (signInResult.Succeeded)
                    {




                        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/Home" : returnUrl);








                    }
                    else
                    {
                        ModelState.AddModelError("", "Yanlış kullanıcı adı veya şifre girdiniz.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı bulunamadı. Lütfen üye olunuz.");
                }

            }
            ViewBag.returnUrl = returnUrl;


            return View(userModel);


        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [Authorize]
        public async Task<IActionResult> AddComment(string commentText, string? id)
        {
            var comment = new Comment();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Blogs.Where(i => i.Id == id).FirstOrDefault();
            comment.BlogId = store.Id;
            comment.Name = user.Name;
            comment.Surname = user.Surname;
            comment.Photo=user.ProfilePhoto;
            comment.CommentText = commentText;





            await _context.Comments.AddAsync(comment);

            _context.SaveChanges();
            return RedirectToAction("Details","Home",new { id = id });
        }
    }
}
