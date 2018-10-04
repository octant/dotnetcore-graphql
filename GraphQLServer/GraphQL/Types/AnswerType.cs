using GraphQL.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Plasma.Data;
using System.Collections.Generic;

namespace Plasma.Types
{
    public class AnswerType : ObjectGraphType<Answer>
    {
        public AnswerType(OrgData data)
        {
            Name = "Answer";
            Field<IdGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<SessionType>("Session", resolve: context => data.GetSession(context.Source.SessionId));
            Field<QuestionType>("Question", resolve: context => data.GetQuestion(context.Source.QuestionId.ToString()));
            Field<AlternativeType>("Alternative", resolve: context => data.GetAlternative(
                context.Source.SessionId,
                context.Source.QuestionId,
                context.Source.AlternativeId
            ));
            Field<PublicUserDataType>("User", resolve: context => data.GetUser(context.Source.UserId.ToString()));
            Field<StringGraphType>("Value", resolve: context => context.Source.Value.ToString());
        }
    }

    public class AnswerInputType : InputObjectGraphType<Answer>
    {
        public AnswerInputType()
        {
            Name = "AnswerInput";
            Field<IdGraphType>("Id");
            Field<StringGraphType>("SessionId");
            Field<StringGraphType>("QuestionId");
            Field<StringGraphType>("AlternativeId");
            Field<StringGraphType>("UserId");
            Field<StringGraphType>("Value");
        }
    }

    public class Answer
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("sessionId")]
        public string SessionId { get; set; }

        [BsonElement("questionId")]
        public string QuestionId { get; set; }

        [BsonElement("alternativeId")]
        public string AlternativeId { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }
}
