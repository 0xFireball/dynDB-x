using LiteDB;

namespace dynDBx.Models
{
    public class DatabaseObject
    {
        [BsonId]
        public ObjectId DatabaseId { get; set; }
    }
}