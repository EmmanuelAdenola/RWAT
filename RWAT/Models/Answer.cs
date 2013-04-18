using System;

namespace RWAT.Models
{
    public class Answer
    {
        public User User { get; set; }
        public string Body { get; set; }
        public int Vote { get; set; }
        public string DateAnswered { get; set; }
    }
}