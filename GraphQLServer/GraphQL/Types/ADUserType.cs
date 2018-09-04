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
            Field<StringGraphType>("GivenName", resolve: context => context.Source.Properties["GivenName"].Value);
            Field<StringGraphType>("SN", resolve: context => context.Source.Properties["SN"].Value);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.Properties["DisplayName"].Value);
            Field<StringGraphType>("Description", resolve: context => context.Source.Properties["Description"].Value);
            Field<StringGraphType>("Department", resolve: context => context.Source.Properties["Department"].Value);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Properties["Mail"].Value);
            Field<StringGraphType>("Title", resolve: context => context.Source.Properties["Title"].Value);
            Field<StringGraphType>("telephoneNumber", resolve: context => context.Source.Properties["telephoneNumber"].Value);
            Field<StringGraphType>("DistinguishedName", resolve: context => context.Source.Properties["DistinguishedName"].Value);
            Field<StringGraphType>("physicalDeliveryOfficeName", resolve: context => context.Source.Properties["physicalDeliveryOfficeName"].Value);
            Field<ListGraphType<ADGroupType>>("MemberOf", resolve: context => data.GetGroupNames((object[])context.Source.Properties["MemberOf"].Value));
            Field<ListGraphType<MessageType>>("Messages", resolve: context => data.GetMessages(context.Source.Properties["sAMAccountName"].Value.ToString(), "pm"));
            Field<PublicUserDataType>("Manager", resolve: context => data.GetManager(context.Source.Properties["Manager"].Value));
            Field<ListGraphType<PublicUserDataType>>("Employees", resolve: context => data.GetEmployees(context.Source.Properties["DistinguishedName"].Value));
        }
    }

    public class ADUserInputType : InputObjectGraphType<DirectoryEntry>
    {
        public ADUserInputType()
        {
            Name = "ADUserInput";
            Field<StringGraphType>("GivenName");
            Field<StringGraphType>("SN");
            Field<StringGraphType>("DisplayName");
            Field<StringGraphType>("Department");
            Field<StringGraphType>("Description");
            Field<StringGraphType>("telephoneNumber");
            Field<StringGraphType>("Title");
            Field<StringGraphType>("physicalDeliveryOfficeName");
        }
    }
}
