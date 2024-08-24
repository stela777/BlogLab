using System;
using System.Collections.Generic;
using System.Text;

namespace BlogLab.Models.Blog
{
    public class BlogCreate
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? PhotoId { get; set; }
    }
}
