using GraphQL.Types;
using Microsoft.AspNetCore.Http;
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

            Field<ADUserType>(
                "updateADUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "message id" },
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