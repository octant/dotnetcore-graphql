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
        public Mutations(OrgData data, IHttpContextAccessor _context)
        {
            Name = "Mutation";

            Field<MessageType>(
                "markAsRead",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "message id" }
                ),
                resolve: context =>
                {
                    return data.MarkMessageAsRead( context.GetArgument<string>("id"), _context.HttpContext.User.Identity.Name.Split("\\")[1]);
                }
            );

            Field<ADUserType>(
                "updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "message id" },
                    new QueryArgument<NonNullGraphType<ADUserInputType>> { Name = "user", Description = "updated user fields" }
                    ),
                resolve: context =>
                {
                    
                    return data.UpdateADUser((WindowsIdentity)_context.HttpContext.User.Identity, context.GetArgument<string>("id"), context.GetArgument<Dictionary<string, dynamic>>("user"));
                }
            );
        }
    }
}