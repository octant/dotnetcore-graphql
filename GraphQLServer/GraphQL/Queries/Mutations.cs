using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Plasma.Data;
using Plasma.Types;
using System.Collections.Generic;
using System.Security.Principal;

namespace Plasma
{
    internal class Mutations : ObjectGraphType
    {
        public Mutations(OrgData data, IHttpContextAccessor accessor)
        {
            Name = "Mutation";

            /* Messages */

            /* Update */
            Field<MessageType>(
                "markAsRead",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "message id" }
                ),
                resolve: context =>
                {
                    return data.MarkMessageAsRead(context.GetArgument<string>("id"));
                }
            );

            /* Users */

            /* Update */
            Field<NewUserType>(
                "createUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewUserInputType>> { Name = "user", Description = "updated user fields" }
                ),
                resolve: context =>
                {
                    return data.CreateUser(context.GetArgument<NewUser>("user"));
                }
            );

            Field<ADUserType>(
                "updateADUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "user id" },
                    new QueryArgument<NonNullGraphType<ADUserInputType>> { Name = "user", Description = "updated user fields" }
                    ),
                resolve: context =>
                {

                    return data.UpdateADUser((WindowsIdentity)accessor.HttpContext.User.Identity, context.GetArgument<string>("id"), context.GetArgument<Dictionary<string, dynamic>>("user"));
                }
            );

            /* Questions */

            /* Create */
            Field<QuestionType>(
                "addQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<QuestionInputType>> { Name = "question", Description = "question fields" }
                ),
                resolve: context =>
                {
                    return data.AddQuestion(context.GetArgument<Question>("question"));
                }
            );

            /* Update */
            Field<QuestionType>(
                "updateQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "question id" },
                    new QueryArgument<NonNullGraphType<QuestionInputType>> { Name = "question", Description = "updated question fields" }
                    ),
                resolve: context =>
                {
                    return data.UpdateQuestion(context.GetArgument<string>("id"), context.GetArgument<Dictionary<string, dynamic>>("question"));
                }
            );

            //Field<QuestionType>(
            //    "answerQuestion",
            //    arguments: new QueryArguments(
            //        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "question id" },
            //        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username", Description = "username" }
            //    ),
            //    resolve: context =>
            //    {
            //        return data.AnswerQuestion(context.GetArgument<string>("id"), context.GetArgument<string>("username"));
            //    }
            //);

            /* Sessions */

            /* Create */
            Field<SessionType>(
                "createSession",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<SessionInputType>> { Name = "session", Description = "session fields" }
                ),
                resolve: context =>
                {
                    return data.CreateSession(context.GetArgument<Session>("session"));
                }
            );

            /* Update */
            Field<SessionType>(
                "addQuestionsToSession",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "session id" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "questions", Description = "question ids" }
                ),
                resolve: context =>
                {
                    return data.AddQuestionsToSession(context.GetArgument<string>("id"), context.GetArgument<List<string>>("questions"));
                }
            );

            Field<SessionType>(
                "setSessionStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "session id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "status", Description = "session status" }
                ),
                resolve: context =>
                {
                    return data.SetSessionStatus(context.GetArgument<string>("id"), context.GetArgument<string>("status"));
                }
            );

            Field<SessionType>(
                "askQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "sessionId", Description = "session id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "questionId", Description = "question id" }
                ),
                resolve: context =>
                {
                    return data.AskQuestion(context.GetArgument<string>("sessionId"), context.GetArgument<string>("questionId"));
                }
            );

            /* Answers */

            /* Create */
            Field<AnswerType>(
                "answerQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AnswerInputType>> { Name = "answer", Description = "answer" }
                ),
                resolve: context =>
                {
                    return data.AnswerQuestion(context.GetArgument<Answer>("answer"));
                }
            );
        }
    }
}