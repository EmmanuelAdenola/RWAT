using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RWAT.Models;

namespace RWAT.Hubs
{
    [HubName("questionhub")]
    public class QuestionHub : Hub
    {
       public void ShowQuestion(QuestionViewModel questionModel)
        {
            Clients.Caller.showQuestion(questionModel);
        }

        public void ShowAnswer(Answer  answer)
        {
            Clients.Caller.showAnswer(answer);
        }
    }
}