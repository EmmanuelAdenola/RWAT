using System.Configuration;
using MongoDB.Driver;

namespace RWAT.Utility
{
    public class MongoHelper
    {
        private const string ConnectionString = "MongoDB";
       
        public static MongoCollection<T> GetCollection<T>(string collection)
        {
            var conn =
                  new MongoConnectionStringBuilder(
                      ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString);
            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);

            return mongoDatabase.GetCollection<T>(collection);
        }
    }
}