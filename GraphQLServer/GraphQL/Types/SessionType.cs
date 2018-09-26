using GraphQL.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Plasma.Data;
using System.Collections.Generic;

namespace Plasma.Types
{
    public class SessionType : ObjectGraphType<Session>
    {

        public SessionType(OrgData data)
        {
            Name = "Session";
            Field<IdGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<BooleanGraphType>("Open", resolve: context => context.Source.Open);
            Field<BooleanGraphType>("Closed", resolve: context => context.Source.Closed);
            Field<ListGraphType<QuestionType>>("Questions", resolve: context => context.Source.Questions);
        }
    }

    public class SessionInputType : InputObjectGraphType<Session>
    {
        public SessionInputType()
        {
            Name = "SessionInput";
            Field<IdGraphType>("Id");
            Field<BooleanGraphType>("Open");
            Field<BooleanGraphType>("Closed");
            Field<ListGraphType<QuestionInputType>>("Questions");
        }
    }

    public class Session
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("open")]
        public bool Open { get; set; }

        [BsonElement("closed")]
        public bool Closed { get; set; }

        [BsonElement("questions")]
        public List<Question> Questions { get; set; }
    }
}

