using System;
using System.Collections.Generic;
using System.Text;

namespace Halic.Entity
{
    public class NCategory
    {
        public int NCategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public List<NewsHCategory> newsHCategories { get; set; }
    }
}
