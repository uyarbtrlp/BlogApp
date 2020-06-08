using BlogApp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class CategoryClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
    }
}
