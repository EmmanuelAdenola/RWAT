using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class User
    {
        private List<Question> _questions;

        
        public string HashedEmail { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId UserId { get; set; }

        public string UserName { get; set; }

        public List<Question> Questions
        {
            get
            {
                if (_questions == null)
                {
                    _questions = new List<Question>();
                }
                return _questions;
            }
            set { _questions = value; }
        }
    }
}