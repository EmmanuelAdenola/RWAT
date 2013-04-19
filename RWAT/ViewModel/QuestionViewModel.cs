using System.Collections.Generic;
using RWAT.Models;

namespace RWAT.ViewModel
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public User User { get; set; }
        public VoteViewModel VoteViewModel { get; set; }
    }
}