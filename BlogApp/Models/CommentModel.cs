using BlogApp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class CommentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CommentText { get; set; }
        public string Photo { get; set; }
        public string BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
