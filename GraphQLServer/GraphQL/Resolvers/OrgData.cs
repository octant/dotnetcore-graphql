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
            _mongoContext = new MongoContext(settings);
        }

        public DirectoryEntry GetUser(string userName)
        {
            ds.Filter = $"(sAMAccountName={userName})";
            return ds.FindOne().GetDirectoryEntry();
        }

        public List<DirectoryEntry> GetEmployees(object distinguishedName)
        {
            ds.Filter = $"(&(Manager={distinguishedName})(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
            List<DirectoryEntry> employees = new List<DirectoryEntry>();

            foreach (SearchResult result in ds.FindAll())
            {
                employees.Add(result.GetDirectoryEntry());
            }

            return employees;
        }

        public DirectoryEntry GetManager(object distinguishedName)
        {
            if (distinguishedName == null)
            {
                return new DirectoryEntry();
            }

            ds.Filter = $"(DistinguishedName={distinguishedName})";
            return ds.FindOne().GetDirectoryEntry();
        }
  
        public async Task<IEnumerable<Message>> GetMessages(string username)
        {
            var filter = Builders<Message>.Filter;
            var query = filter.Eq(m => m.To, username) | filter.Eq(m => m.From, username);
            return await _mongoContext.Messages.Find(query).ToListAsync();
        }

        public async Task<Message> MarkMessageAsRead(string id, string to)
        {
            var filter = Builders<Message>.Filter;
            var query = filter.Eq(n => n.Id, new ObjectId(id)) & filter.Eq(n => n.To, to);
            var update = Builders<Message>.Update.Set(n => n.Read, true);
            await _mongoContext.Messages.UpdateOneAsync(query, update);
            return await _mongoContext.Messages.Find(query).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetMessages(string username, string type)
        {
            var filter = Builders<Message>.Filter;
            var query = (filter.Eq(m => m.To, username) | filter.Eq(m => m.From, username) & filter.Eq(n => n.Type, type));
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
    }
}