using System.Collections.Generic;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MongoDB.Driver;
using RWAT.Models;
using System.Linq;
namespace RWAT.Hubs
{
    [HubName("questionhub")]
    public class QuestionHub : Hub
    {
        public void GetQuestions()
        {
            var conn = new MongoConnectionStringBuilder(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);

            IEnumerable<User> users = mongoDatabase.GetCollection<User>("users").FindAll();
            Clients.Caller.getQuestions(users.SelectMany(u=>u.Questions).ToList());
        }

        public void AddQuestion(Question question)
        {
            Clients.Caller.addQuestion(question);
        }
    }
}