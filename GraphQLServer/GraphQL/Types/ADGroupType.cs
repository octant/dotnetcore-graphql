using GraphQL.Types;
using Plasma.Data;

namespace Plasma.Types
{
    public class ADGroupType : ObjectGraphType<ADGroup>
    {
        public ADGroupType(OrgData data)
        {
            Field<StringGraphType>("Name", resolve: context => context.Source.Name);
            Field<StringGraphType>("DistinguishedName", resolve: context => context.Source.DistinguishedName);
        }
    }

    public class ADGroup
    {
        public string DistinguishedName;
        public string Name;
    }
}
