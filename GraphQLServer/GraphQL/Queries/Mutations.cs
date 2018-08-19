using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Plasma.Data;
using Plasma.Types;

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
        }
    }
}