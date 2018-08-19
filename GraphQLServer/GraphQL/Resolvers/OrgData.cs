﻿using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Plasma.Settings;
using Plasma.Types;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

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
    }
}