using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Halic.Entity
{
    public class Activities
    {
        [Key]
        public int ActivityId { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string img { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }
    }
}
