using System.ComponentModel.DataAnnotations;

namespace RWAT.Models
{
    public class AnswerModel
    {
        public string QuestionId { get; set; }
        [Required]
        public string Answer { get; set; }
        public User  Answerer { get; set; }
    }
}