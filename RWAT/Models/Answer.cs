using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class Answer
    {
        private Vote _vote;

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId AnswerId { get; set; }
       
        [BsonRepresentation(BsonType.String)]
        public ObjectId QuestionId { get; set;  }

        [BsonRepresentation(BsonType.String)]
        public ObjectId UserId { get; set; }

        public Vote Vote
        {
            get
            {
                if (_vote == null)
                {
                    _vote = new Vote();
                }
                return _vote;
            }
            set { _vote = value; }
        }

        public string Body { get; set; }
        public string DateAnswered { get; set; }
    }
}