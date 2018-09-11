using GraphQL.Types;
using System.DirectoryServices;
using Plasma.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Collections;

namespace Plasma.Types
{
    public class QuestionType : ObjectGraphType<Question>
    {
        public QuestionType(OrgData data)
        {
            Name = "Question";
            Field<IdGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<StringGraphType>("Type", resolve: context => context.Source.Type.ToString());
            Field<StringGraphType>("Stem", resolve: context => context.Source.Stem.ToString());
            Field<ListGraphType<AlternativeType>>("Alternatives", resolve: context => context.Source.Alternatives);
        }
    }

    public class QuestionInputType : InputObjectGraphType<Question>
    {
        public QuestionInputType()
        {
            Name = "QuestionInput";
            Field<StringGraphType>("Type");
            Field<StringGraphType>("Stem");
            Field<ListGraphType<AlternativeInputType>>("Alternatives");
        }
    }

    public class AlternativeType : ObjectGraphType<Alternative>
    {
        public AlternativeType(OrgData data)
        {
            Name = "Alternative";
            Field<IdGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<StringGraphType>("InputType", resolve: context => context.Source.InputType.ToString());
            Field<StringGraphType>("Value", resolve: context => context.Source.Value.ToString());
            Field<StringGraphType>("Text", resolve: context => context.Source.Text.ToString());
            Field<StringGraphType>("Type", resolve: context => context.Source.Type.ToString());
            Field<ListGraphType<StringGraphType>>("Respondents", resolve: context => context.Source.Respondents);

        }
    }

    public class AlternativeInputType : InputObjectGraphType<Alternative>
    {
        public AlternativeInputType()
        {
            Name = "AlternativeInput";
            Field<StringGraphType>("InputType");
            Field<StringGraphType>("Value");
            Field<StringGraphType>("Text");
            Field<StringGraphType>("Type");
            Field<ListGraphType<StringGraphType>>("Respondents");
        }
    }

    public class QuestionAnalysisType : ObjectGraphType<QuestionAnalysis>
    {
        public QuestionAnalysisType(OrgData data)
        {
            Name = "QuestionAnalysis";
            Field<StringGraphType>("Id", resolve: context => context.Source.Id.ToString());
            Field<IntGraphType>("Count", resolve: context => context.Source.Count);
        }
    }

    public class Question
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string Type { get; set; }

        [BsonElement]
        public string Stem { get; set; }

        [BsonElement]
        public List<Alternative> Alternatives { get; set; }
    }

    public class Alternative
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string InputType { get; set; }

        [BsonElement]
        public string Value { get; set; }

        [BsonElement]
        public string Text { get; set; }

        [BsonElement]
        public string Type { get; set; }

        [BsonElement]
        public List<string> Respondents { get; set; }
    }

    public class QuestionAnalysis : BsonDocument
    {
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("count")]
        public int Count { get; set; }
        public QuestionAnalysis()
        {

        }
        public QuestionAnalysis(BsonDocument aggregation)
        {
            IEnumerator aggregationEnum = aggregation.Values.GetEnumerator();
            if (aggregationEnum.MoveNext())
            {
                Id = aggregationEnum.Current.ToString(); 
            }
            if (aggregationEnum.MoveNext())
            {
                Count = int.Parse(aggregationEnum.Current.ToString());
            }

        }
    }
}
