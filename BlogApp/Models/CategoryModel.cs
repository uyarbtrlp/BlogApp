using BlogApp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class CategoryModel
    {
        public List<BlogCategoryModel> BlogCategories { get; set; }
    }
}
