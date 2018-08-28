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
            Field<StringGraphType>("sAMAccountName", resolve: context => context.Source.Properties["sAMAccountName"].Value);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Properties["Mail"].Value);
            Field<StringGraphType>("GivenName", resolve: context => context.Source.Properties["GivenName"].Value);
            Field<StringGraphType>("SN", resolve: context => context.Source.Properties["SN"].Value);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.Properties["DisplayName"].Value);
            Field<StringGraphType>("Department", resolve: context => context.Source.Properties["Department"].Value);
        }
    }
}
