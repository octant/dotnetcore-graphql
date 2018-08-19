using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using Plasma.Types;
using Plasma.Data;

namespace Plasma
{
    public class Queries : ObjectGraphType
    {
        public Queries(OrgData data, IHttpContextAccessor _context)
        {
            Name = "Query";

            Field<ADUserType>("my", resolve: context => data.GetUser(_context.HttpContext.User.Identity.Name.Split("\\")[1]));

            Field<PublicUserDataType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username", Description = "sAMAccountName of the user" }
                ),
                resolve: context => data.GetUser(context.GetArgument<string>("username")));

            Field<ListGraphType<MessageType>>(
                "messages",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {
                        Name = "type", Description = "message type"
                    }
                ),
                resolve: context => data.GetMessages(_context.HttpContext.User.Identity.Name.Split("\\")[1], context.GetArgument<string>("type")));
        }
    }
}