using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Plasma.Settings;

namespace Plasma.Data
{
    public class OrgData
    {
        private readonly DirectorySearcher ds;
        private readonly MongoContext _mongoContext;

        public OrgData(IOptions<MongoSettings> settings, IConfiguration configuration)
        {
            ds = new DirectorySearcher(configuration.GetSection("Org:ldap").Value);
            _mongoContext = new MongoContext(settings);
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