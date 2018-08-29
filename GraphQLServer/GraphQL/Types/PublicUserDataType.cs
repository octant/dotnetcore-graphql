using Plasma.Data;
using GraphQL.Types;
using System.DirectoryServices;
using System.Collections;

namespace Plasma.Types
{
    public class ADUser
    {
        public string SAMAccountName { get; set; }
        public string Mail { get; set; }
        public string GivenName { get; set; }
        public string SN { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }

        public ADUser()
        {

        }

        public ADUser(DirectoryEntry user)
        {
            SAMAccountName = user.Properties["sAMAccountName"].Value.ToString();
            Mail = user.Properties["Mail"].Value.ToString();
            GivenName = user.Properties["GivenName"].Value.ToString();
            SN = user.Properties["SN"].Value.ToString();
            DisplayName = user.Properties["DisplayName"].Value.ToString();
            Department = user.Properties["Department"].Value.ToString();
        }

        public ADUser(SearchResult user)
        {
            IEnumerator SAMAccountNameEnum = user.Properties["sAMAccountName"].GetEnumerator();
            IEnumerator MailEnum = user.Properties["Mail"].GetEnumerator();
            IEnumerator GivenNameEnum = user.Properties["GivenName"].GetEnumerator();
            IEnumerator SNEnum = user.Properties["SN"].GetEnumerator();
            IEnumerator DisplayNameEnum = user.Properties["DisplayName"].GetEnumerator();
            IEnumerator DepartmentEnum = user.Properties["Department"].GetEnumerator();

            if (SAMAccountNameEnum.MoveNext())
                SAMAccountName = SAMAccountNameEnum.Current.ToString();
            if (MailEnum.MoveNext())
                Mail = MailEnum.Current.ToString();
            if (GivenNameEnum.MoveNext())
                GivenName = GivenNameEnum.Current.ToString();
            if (SNEnum.MoveNext())
                SN = SNEnum.Current.ToString();
            if (DisplayNameEnum.MoveNext())
                DisplayName = DisplayNameEnum.Current.ToString();
            if (DepartmentEnum.MoveNext())
                Department = DepartmentEnum.Current.ToString();
        }
    }

    public class PublicUserDataType : ObjectGraphType<ADUser>
    {
        public PublicUserDataType()
        {
            Name = "PublicUserData";
            Field<StringGraphType>("sAMAccountName", resolve: context => context.Source.SAMAccountName);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Mail);
            Field<StringGraphType>("GivenName", resolve: context => context.Source.GivenName);
            Field<StringGraphType>("SN", resolve: context => context.Source.SN);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.DisplayName);
            Field<StringGraphType>("Department", resolve: context => context.Source.Department);
        }
    }
}
