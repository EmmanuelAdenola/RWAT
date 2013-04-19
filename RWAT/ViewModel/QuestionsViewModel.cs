using System.Collections.Generic;

namespace RWAT.ViewModel
{
    public class QuestionsViewModel
    {
        public bool? ShowNav { get; set; }
        public List<QuestionViewModel> QuestionViewModels { get; set; }
    }
}