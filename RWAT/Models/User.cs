using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class User
    {
        [BsonId]
        public ObjectId UserId { get; set; }
        public string Email { get; set; }
        public List<Question> Questions { get; set; }
    }
}