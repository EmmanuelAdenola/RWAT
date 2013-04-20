using RWAT.Models;

namespace RWAT.ViewModel
{
    public class AnswerViewModel
    {
        public Answer Answer { get; set; }
        public User Answerer { get; set; }
        public VoteViewModel VoteViewModel { get; set; }
    }
}