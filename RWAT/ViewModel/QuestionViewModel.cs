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

    public class AnswerViewModel
    {
        public Answer Answer { get; set; }
        public User Answerer { get; set; }
    }
}