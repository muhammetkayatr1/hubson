﻿using Halic.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalicHub.Models
{
    public class ArticleListViewModel
    {
        public PageInfo pageInfo { get; set; }

        public List<Article> Articles { get; set; }
    
    }
}
