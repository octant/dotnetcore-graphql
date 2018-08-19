using Plasma.Data;
using GraphQL.Types;
using System.DirectoryServices;

namespace Plasma.Types
{
    public class PublicUserDataType : ObjectGraphType<DirectoryEntry>
    {
        public PublicUserDataType(OrgData data)
        {
            Name = "PublicUserData";
            Field<StringGraphType>("username", resolve: context => context.Source.Properties["sAMAccountName"].Value);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.Properties["DisplayName"].Value);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Properties["Mail"].Value);
        }
    }
}
