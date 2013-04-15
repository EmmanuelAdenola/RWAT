using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWAT.Models
{
    public class Question
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int Vote { get; set; }
        public IList<Comment> Comments { get; set; } 
    }
}