using System.Collections.Generic;
using RWAT.Models;

namespace RWAT.ViewModel
{
    public class QuestionsViewModel
    {
        public bool? ShowNav { get; set; }
        public List<QuestionViewModel> QuestionViewModels { get; set; }
    }
}