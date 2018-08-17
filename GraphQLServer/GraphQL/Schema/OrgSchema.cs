using GraphQL;
using GraphQL.Types;

namespace Plasma
{
    public class OrgSchema : Schema
    {
        public OrgSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<Queries>();
            Mutation = resolver.Resolve<Mutations>();
        }
    }
}
