﻿using Microsoft.Extensions.Options;
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

        public IMongoCollection<NewUser> NewUsers
        {
            get
            {
                return _database.GetCollection<NewUser>("newUsers");
            }
            set
            {
                CreateCollectionOptions options = new CreateCollectionOptions<NewUser>()
                {
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                _database.CreateCollection("newUsers", options);
            }
        }

        public IMongoCollection<Question> Questions
        {
            get
            {
                return _database.GetCollection<Question>("questions");
            }
            set
            {
                CreateCollectionOptions options = new CreateCollectionOptions<Question>()
                {
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                _database.CreateCollection("questions", options);
            }
        }

        public IMongoCollection<Session> Sessions
        {
            get
            {
                return _database.GetCollection<Session>("sessions");
            }
            set
            {
                CreateCollectionOptions options = new CreateCollectionOptions<Session>()
                {
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                _database.CreateCollection("sessions", options);
            }
        }

        public IMongoCollection<Answer> Answers
        {
            get
            {
                return _database.GetCollection<Answer>("answers");
            }
            set
            {
                CreateCollectionOptions options = new CreateCollectionOptions<Answer>()
                {
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                _database.CreateCollection("answers", options);
            }
        }
    }
}
