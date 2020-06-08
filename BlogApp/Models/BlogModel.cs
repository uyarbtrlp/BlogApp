using BlogApp.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class BlogModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Başlık girilmesi gereklidir.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "İçerik girilmesi gereklidir.")]
        public string Content { get; set; }
        public string Date { get; set; }
        
        public string Image { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public List<BlogCategoryModel> BlogCategories { get; set; }
        public List<CategoryClass> Categories { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}
