using GraphQL.Types;
using System.DirectoryServices;
using Plasma.Data;

namespace Plasma.Types
{
    public class ADUserType : ObjectGraphType<DirectoryEntry>
    {
        public ADUserType(OrgData data)
        {
            Name = "ADUser";
            Field<IdGraphType>("sAMAccountName", resolve: context => context.Source.Properties["sAMAccountName"].Value);
            Field<StringGraphType>("DistinguishedName", resolve: context => context.Source.Properties["DistinguishedName"].Value);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.Properties["DisplayName"].Value);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Properties["Mail"].Value);
            Field<ListGraphType<StringGraphType>>("MemberOf", resolve: context => context.Source.Properties["MemberOf"].Value);
            Field<ListGraphType<MessageType>>("Messages", resolve: context => data.GetMessages(context.Source.Properties["sAMAccountName"].Value.ToString(), "pm"));
            Field<ListGraphType<PublicUserDataType>>("Employees", resolve: context => data.GetEmployees(context.Source.Properties["DistinguishedName"].Value.ToString()));
        }
    }
}
