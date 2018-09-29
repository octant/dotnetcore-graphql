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
            Field<StringGraphType>("Name", resolve: context => context.Source.Name.ToString());
            Field<PublicUserDataType>("Leader", resolve: context => data.GetUser(context.Source.Leader.ToString()));
            Field<StringGraphType>("Status", resolve: context => context.Source.Status.ToString());
            Field<ListGraphType<QuestionType>>("Questions", resolve: context => context.Source.Questions);
            Field<QuestionType>("CurrentQuestion", resolve: context => {
                return context.Source.CurrentQuestion == null
                ? null
                : data.GetQuestion(context.Source.CurrentQuestion.ToString());
             });
        }
    }

    public class SessionInputType : InputObjectGraphType<Session>
    {
        public SessionInputType()
        {
            Name = "SessionInput";
            Field<IdGraphType>("Id");
            Field<StringGraphType>("Name");
            Field<StringGraphType>("Leader");
            Field<StringGraphType>("Status");
            Field<ListGraphType<QuestionInputType>>("Questions");
            Field<StringGraphType>("CurrentQuestion");
        }
    }

    public class Session
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("leader")]
        public string Leader { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("questions")]
        public List<Question> Questions { get; set; }

        [BsonElement("currentQuestion")]
        public string CurrentQuestion { get; set; }
    }
}

