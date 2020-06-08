using BlogApp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class BlogCategoryModel
    {
        public string BlogId { get; set; }
        public Blog Blog { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
