using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class User
    {
        public string HashedEmail { get; set; }
       
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId UserId { get; set; }

        public string UserName { get; set; }
    }
}