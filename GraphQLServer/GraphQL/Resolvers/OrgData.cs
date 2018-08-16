using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.DirectoryServices;

namespace GraphQLServer.Data
{
    public class OrgData
    {
        private readonly DirectorySearcher ds;

        public OrgData()
        {
            ds = new DirectorySearcher("LDAP://DC=AHU,DC=ON,DC=CA");
        }

        public DirectoryEntry GetUser(string userName)
        {
            ds.Filter = $"(sAMAccountName={userName})";
            return ds.FindOne().GetDirectoryEntry();
        }

        public List<DirectoryEntry> GetEmployees(string distinguishedName)
        {
            ds.Filter = $"(Manager={distinguishedName})";
            List<DirectoryEntry> employees = new List<DirectoryEntry>();

            foreach (SearchResult result in ds.FindAll())
            {
                employees.Add(result.GetDirectoryEntry());
            }

            return employees;
        }
    }
}