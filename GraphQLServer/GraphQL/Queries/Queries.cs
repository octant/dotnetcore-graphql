using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using Plasma.Types;
using Plasma.Data;

namespace Plasma
{
    public class Queries : ObjectGraphType
    {
        public Queries(OrgData data, IHttpContextAccessor accessor)
        {
            Name = "Query";

            Field<ADUserType>("my", resolve: context => data.GetDirectoryEntry(accessor.HttpContext.User.Identity.Name.Split("\\")[1]));

            Field<ADUserType>(
                "directoryEntry",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username", Description = "sAMAccountName of the directory entry" }
                ),
                resolve: context => data.GetDirectoryEntry(context.GetArgument<string>("username"))
            );

            Field<ListGraphType<PublicUserDataType>>("users", resolve: context => data.GetAPHUsers());

            Field<PublicUserDataType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username", Description = "sAMAccountName of the user" }
                ),
                resolve: context => data.GetUser(context.GetArgument<string>("username"))
            );

            Field<ListGraphType<MessageType>>(
                "messages",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {
                        Name = "type", Description = "message type"
                    }
                ),
                resolve: context => data.GetMessages(accessor.HttpContext.User.Identity.Name.Split("\\")[1], context.GetArgument<string>("type"))
            );

            Field<ListGraphType<QuestionType>>(
                "questions",
                resolve: context => data.GetQuestions()
            );

            Field<ListGraphType<QuestionAnalysisType>>(
                "analyzeQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "sessionId",
                        Description = "session id"
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "questionId",
                        Description = "question id"
                    }
                ),
                resolve: context => data.AnalyzeSessionQuestion(context.GetArgument<string>("sessionId"), context.GetArgument<string>("questionId"))
            );

            Field<ListGraphType<StringGraphType>>(
                "getRespondents",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "sessionId",
                        Description = "session id"
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "questionId",
                        Description = "question id"
                    }
                ),
                resolve: context => data.GetRespondents(context.GetArgument<string>("sessionId"), context.GetArgument<string>("questionId"))
            );

            Field<QuestionType>(
                "question",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "id",
                        Description = "question id"
                    }
                ),
                resolve: context => data.GetQuestion(context.GetArgument<string>("id"))
            );

            Field<SessionType>(
                "session",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "id",
                        Description = "session id"
                    }
                ),
                resolve: context => data.GetSession(context.GetArgument<string>("id"))
            );

            Field<ListGraphType<SessionType>>(
                "sessions",
                resolve: context => data.GetSessions()
            );

            Field<ListGraphType<AnswerType>>(
                "answers",
                resolve: context => data.GetAnswers()
            );

            Field<ListGraphType<AnswerType>>(
                "answersForSessionQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "sessionId",
                        Description = "session id"
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "questionId",
                        Description = "question id"
                    }
                ),
                resolve: context => data.GetAnswers(context.GetArgument<string>("sessionId"), context.GetArgument<string>("questionId"))
            );

            Field<ListGraphType<NewUserType>>(
                "newUsers",
                resolve: context => data.GetNewUsers()
            );
        }
    }
}