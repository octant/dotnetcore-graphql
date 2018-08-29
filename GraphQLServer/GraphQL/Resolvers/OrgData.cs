using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Plasma.Settings;
using Plasma.Types;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using System.Security.Principal;

namespace Plasma.Data
{
    public class OrgData
    {
        private readonly DirectorySearcher ds;
        private readonly MongoContext _mongoContext;

        public OrgData(IOptions<MongoSettings> settings, IConfiguration configuration)
        {
            ds = new DirectorySearcher(configuration.GetSection("Org:ldap").Value);
            string[] userProperties = new string[] {
                "sAMAccountName",
                "GivenName",
                "SN",
                "DisplayName",
                "Department",
                "Mail",
                "Title",
                "telephoneNumber",
                "DistinguishedName",
                "physicalDeliveryOfficeName",
                "MemberOf",
                "Manager",
                "Employees"
            };
            ds.PropertiesToLoad.AddRange(userProperties);

            _mongoContext = new MongoContext(settings);
        }

        public DirectoryEntry GetDirectoryEntry(string userName)
        {
            ds.Filter = $"(sAMAccountName={userName})";
            return ds.FindOne().GetDirectoryEntry();
        }

        public ADUser GetUser(string userName)
        {
            ds.Filter = $"(sAMAccountName={userName})";
            return new ADUser(ds.FindOne().GetDirectoryEntry());
        }

        public List<ADUser> GetEmployees(object distinguishedName)
        {
            ds.Filter = $"(&(Manager={distinguishedName})(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
            List<ADUser> employees = new List<ADUser>();

            foreach (SearchResult result in ds.FindAll())
            {
                employees.Add(new ADUser(result));
            }

            return employees;
        }

        public ADUser GetManager(object distinguishedName)
        {
            if (distinguishedName == null)
            {
                return new ADUser();
            }

            ds.Filter = $"(DistinguishedName={distinguishedName})";
            return new ADUser(ds.FindOne().GetDirectoryEntry());
        }

        public async Task<Message> MarkMessageAsRead(string id)
        {
            var filter = Builders<Message>.Filter;
            var query = filter.Eq(m => m.Id, new ObjectId(id));
            var update = Builders<Message>.Update.Set(m => m.Read, true);
            await _mongoContext.Messages.UpdateOneAsync(query, update);
            return await _mongoContext.Messages.Find(query).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetMessages(string username, string type)
        {
            var filter = Builders<Message>.Filter;
            var query = filter.Eq(m => m.Type, type);
            return await _mongoContext.Messages.Find(query).ToListAsync();
        }

        public List<ADGroup> GetGroupNames(object[] groupDNs)
        {
            List<ADGroup> groups = new List<ADGroup>();

            foreach (string dn in groupDNs)
            {
                ADGroup group = new ADGroup();
                Match match = Regex.Match(dn, @"^CN=(.*?),");

                if(match.Success)
                {
                    group.DistinguishedName = dn;
                    group.Name = match.Groups[1].Value;
                }
                

                groups.Add(group);
            }

            return groups;
        }

        public DirectoryEntry UpdateADUser(WindowsIdentity identity, string userName, Dictionary<string, dynamic> update)
        {

            ds.Filter = $"(sAMAccountName={userName})";
            DirectoryEntry user = ds.FindOne().GetDirectoryEntry();

            WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
            {
                foreach (KeyValuePair<string, dynamic> field in update)
                {
                    user.Properties[field.Key].Value = field.Value;
                }

                user.CommitChanges();
            });
            
            return user;
        }

        public List<ADUser> GetAPHUsers()
        {
            Dictionary<string, string> groupDNs = new Dictionary<string, string>();
            groupDNs.Add("IT_Department", "CN=IT,OU=Secure Groups,OU=Willow Ave,DC=ahu,DC=on,DC=ca");

            ds.Filter = $"(&(!(userAccountControl:1.2.840.113556.1.4.803:=2))(!(objectCategory=computer))(|(EmployeeID=*)(MemberOf={groupDNs["IT_Department"]})))";
            ds.Sort.Direction = System.DirectoryServices.SortDirection.Ascending;

            ds.Sort.PropertyName = "DisplayName";
            List<ADUser> Users = new List<ADUser>();

            foreach (SearchResult s in ds.FindAll())
            {
                Users.Add(new ADUser(s));
            };

            return Users;
        }
    }
}