using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Plasma.Settings;
using Plasma.Types;

namespace Plasma.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database = null;

        public MongoContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Message> Messages
        {
            get
            {
                return _database.GetCollection<Message>("messages");
            }
            set
            {
                CreateCollectionOptions options = new CreateCollectionOptions<Message>()
                {
                    Validator = new BsonDocument(
                        "message",
                        new BsonDocument("$regex", "[^.*]")
                    ),
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                _database.CreateCollection("messages", options);
            }
        }
    }
}
