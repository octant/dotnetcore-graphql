using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using GraphQLServer.Models;
using GraphQLServer.Data;

namespace GraphQLServer
{
    public class Queries : ObjectGraphType
    {
        public Queries(OrgData data, IHttpContextAccessor _context)
        {
            Name = "Query";

            Field<ADUserType>("my", resolve: context => data.GetUser(_context.HttpContext.User.Identity.Name.Split("\\")[1]));
        }
    }
}