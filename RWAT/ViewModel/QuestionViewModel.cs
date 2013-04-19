using RWAT.Models;

namespace RWAT.ViewModel
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public User User { get; set; }
        public VoteViewModel VoteViewModel { get; set; }
    }
}