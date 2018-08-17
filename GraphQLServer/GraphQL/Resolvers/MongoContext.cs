using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Plasma.Settings;

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
    }
}
