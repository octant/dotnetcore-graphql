using GraphQL.Types;
using System.DirectoryServices;
using Plasma.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Plasma.Types
{
    public class MessageType : ObjectGraphType<Message>
    {
        public MessageType(OrgData data)
        {
            Name = "Message";
            Field<IdGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<PublicUserDataType>("From", resolve: context => data.GetUser(context.Source.From.ToString()));
            Field<PublicUserDataType>("To", resolve: context => data.GetUser(context.Source.To.ToString()));
            Field<StringGraphType>("Body", resolve: context => context.Source.Body.ToString());
            Field<StringGraphType>("Type", resolve: context => context.Source.Type.ToString());
            Field<BooleanGraphType>("Read", resolve: context => context.Source.Read.ToString());
        }
    }

    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string From { get; set; }

        [BsonElement]
        public string To { get; set; }

        [BsonElement]
        public string Body { get; set; }

        [BsonElement]
        public string Type { get; set; }

        [BsonElement]
        public bool Read { get; set; }
    }
}
