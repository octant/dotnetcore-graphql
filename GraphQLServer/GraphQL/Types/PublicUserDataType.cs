using Plasma.Data;
using GraphQL.Types;
using System.DirectoryServices;
using System.Collections;

namespace Plasma.Types
{
    public class ADUser
    {
        public string Id { get; set; }
        public string SAMAccountName { get; set; }
        public string Mail { get; set; }
        public string GivenName { get; set; }
        public string SN { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string TelephoneNumber { get; set; }
        public string PhysicalDeliveryOfficeName { get; set; }

        public ADUser()
        {

        }

        public ADUser(DirectoryEntry user)
        {
            Id = user.Properties["sAMAccountName"].Value.ToString();
            SAMAccountName = user.Properties["sAMAccountName"].Value.ToString();
            Mail = user.Properties["Mail"].Value.ToString();
            GivenName = user.Properties["GivenName"].Value.ToString();
            SN = user.Properties["SN"].Value.ToString();
            DisplayName = user.Properties["DisplayName"].Value.ToString();
            Department = user.Properties["Department"].Value.ToString();
            TelephoneNumber = user.Properties["telephoneNumber"].Value.ToString();
            PhysicalDeliveryOfficeName = user.Properties["physicalDeliveryOfficeName"].Value.ToString();
        }

        public ADUser(SearchResult user)
        {
            IEnumerator SAMAccountNameEnum = user.Properties["sAMAccountName"].GetEnumerator();
            IEnumerator MailEnum = user.Properties["Mail"].GetEnumerator();
            IEnumerator GivenNameEnum = user.Properties["GivenName"].GetEnumerator();
            IEnumerator SNEnum = user.Properties["SN"].GetEnumerator();
            IEnumerator DisplayNameEnum = user.Properties["DisplayName"].GetEnumerator();
            IEnumerator DepartmentEnum = user.Properties["Department"].GetEnumerator();
            IEnumerator TelephoneNumberEnum = user.Properties["telephoneNumber"].GetEnumerator();
            IEnumerator PhysicalDeliveryOfficeNameEnum = user.Properties["physicalDeliveryOfficeName"].GetEnumerator();

            if (SAMAccountNameEnum.MoveNext())
                Id = SAMAccountNameEnum.Current.ToString();
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
            if (TelephoneNumberEnum.MoveNext())
                TelephoneNumber = TelephoneNumberEnum.Current.ToString();
            if (PhysicalDeliveryOfficeNameEnum.MoveNext())
                PhysicalDeliveryOfficeName = PhysicalDeliveryOfficeNameEnum.Current.ToString();
        }
    }

    public class PublicUserDataType : ObjectGraphType<ADUser>
    {
        public PublicUserDataType()
        {
            Name = "PublicUserData";
            Field<IdGraphType>("Id", resolve: context => context.Source.SAMAccountName);
            Field<StringGraphType>("sAMAccountName", resolve: context => context.Source.SAMAccountName);
            Field<StringGraphType>("Mail", resolve: context => context.Source.Mail);
            Field<StringGraphType>("GivenName", resolve: context => context.Source.GivenName);
            Field<StringGraphType>("SN", resolve: context => context.Source.SN);
            Field<StringGraphType>("DisplayName", resolve: context => context.Source.DisplayName);
            Field<StringGraphType>("Department", resolve: context => context.Source.Department);
            Field<StringGraphType>("telephoneNumber", resolve: context => context.Source.TelephoneNumber);
            Field<StringGraphType>("physicalDeliveryOfficeName", resolve: context => context.Source.PhysicalDeliveryOfficeName);
        }
    }
}
