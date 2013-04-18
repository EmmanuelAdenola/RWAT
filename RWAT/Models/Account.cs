using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class Account
    {
        [Required]
        [BsonRequired]
        public string UserName { get; set; }

        [Required]
        [BsonRequired]
        public string Email { get; set; }


        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }
        
        [Required]
        [BsonRequired]
        public string Password { get; set; }

    }
}