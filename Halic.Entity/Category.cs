using System;
using System.Collections.Generic;
using System.Text;

namespace Halic.Entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public List<ArticleCategory> ArticleCategories { get; set; }
    }
}
