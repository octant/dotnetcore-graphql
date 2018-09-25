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

            Field<QuestionType>(
                "updateQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "question id" },
                    new QueryArgument<NonNullGraphType<QuestionInputType>> { Name = "question", Description = "updated question fields" }
                    ),
                resolve: context =>
                {
                    Dictionary<string, dynamic> update = context.GetArgument<Dictionary<string, dynamic>>("question");

                    if (update.ContainsKey("alternatives"))
                    {
                        List<Alternative> alternatives = new List<Alternative>();
                        foreach (Dictionary<string, dynamic> a in update["alternatives"])
                        {
                            alternatives.Add(new Alternative
                            {
                                Id = new ObjectId(a["id"]),
                                Text = a["text"],
                                Type = a["type"],
                                Value = a["value"]
                            });
                        }
                        update["alternatives"] = alternatives;
                    }

                    return data.UpdateQuestion(context.GetArgument<string>("id"), update).Result;
                }
            );

            Field<QuestionType>(
                "answerQuestion",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "question id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username", Description = "username" }
                ),
                resolve: context =>
                {
                    return data.AnswerQuestion(context.GetArgument<string>("id"), context.GetArgument<string>("username"));
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
        }
    }
}