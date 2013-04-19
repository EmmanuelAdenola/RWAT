using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MongoDB.Bson;
using RWAT.Models;

namespace RWAT.Hubs
{
    [HubName("answerhub")]
    public class AnswerHub :Hub
    {
        public void ShowAnswer(Answer answer)
        {
            Clients.Caller.showAnswer(answer.ToJson());
        }

    }
}