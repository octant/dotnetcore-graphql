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

        /* Question Start */

        /* Create */
        public async Task<Question> AddQuestion(Question question)
        {
            question.Id = ObjectId.GenerateNewId();
            foreach (Alternative alternative in question.Alternatives)
            {
                alternative.Id = ObjectId.GenerateNewId();
            }
            await _mongoContext.Questions.InsertOneAsync(question);
            return await _mongoContext.Questions.Find(n => n.Id == question.Id).FirstOrDefaultAsync();
        }

        /* Review */
        public async Task<IEnumerable<Question>> GetQuestions()
        {
            return await _mongoContext.Questions.Find(_ => true).ToListAsync();
        }

        public async Task<Question> GetQuestion(string id)
        {
            var filter = Builders<Question>.Filter;
            var query = filter.Eq(q => q.Id, new ObjectId(id));
            return await _mongoContext.Questions.Find(query).FirstOrDefaultAsync();
        }

        /* Update */
        public async Task<Question> UpdateQuestion(string id, Dictionary<string, dynamic> update)
        {
            //Dictionary<string, dynamic> update = context.GetArgument<Dictionary<string, dynamic>>("question");

            if (update.ContainsKey("alternatives"))
            {
                List<Alternative> alternatives = new List<Alternative>();
                foreach (Dictionary<string, dynamic> a in update["alternatives"])
                {
                    alternatives.Add(new Alternative
                    {
                        Id = a.ContainsKey("id") ? new ObjectId(a["id"]) : ObjectId.GenerateNewId(),
                        Text = a["text"],
                        Type = a["type"],
                        Value = a["value"]
                    });
                }
                update["alternatives"] = alternatives;
            }
            var filter = Builders<Question>.Filter;
            var query = filter.Eq(m => m.Id, new ObjectId(id));

            var updateSet = new List<UpdateDefinition<Question>>();

            foreach (KeyValuePair<string, dynamic> field in update)
            {
                updateSet.Add(Builders<Question>.Update.Set(field.Key, field.Value));
            }

            var mongoUpdate = Builders<Question>.Update.Combine(updateSet.ToArray());

            await _mongoContext.Questions.UpdateOneAsync(query, mongoUpdate);

            return await _mongoContext.Questions.Find(query).FirstOrDefaultAsync();
        }

        public async Task<Question> AnswerQuestion(string id, string username)
        {
            var filter = Builders<Question>.Filter;
            var query = filter.ElemMatch(q => q.Alternatives, a => a.Id == new ObjectId(id));
            Question question = await _mongoContext.Questions.Find(query).FirstOrDefaultAsync();
            if (question.Alternatives.Find(a => a.Id == new ObjectId(id)).Respondents == null)
            {
                question.Alternatives.Find(a => a.Id == new ObjectId(id)).Respondents = new List<string>();
            }
            if (question.Alternatives.Find(a => a.Id == new ObjectId(id)).Respondents.Contains(username))
            {
                return question;
            }
            question.Alternatives.Find(a => a.Id == new ObjectId(id)).Respondents.Add(username);
            var update = Builders<Question>.Update.Set("Alternatives.$.Respondents", question.Alternatives.Find(a => a.Id == new ObjectId(id)).Respondents);
            await _mongoContext.Questions.UpdateOneAsync(query, update);

            return question;
        }

        public async Task<List<QuestionAnalysis>> AnalyzeQuestion(string questionId)
        {
            var result = await _mongoContext.Questions.Aggregate()
                .Match(q => q.Id == new ObjectId(questionId))
                .Unwind(q => q.Alternatives)
                .Unwind(q => q["Alternatives.Respondents"])
                .Group(new BsonDocument { { "_id", "$Alternatives.Value" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument("_id", 1))
                .ToListAsync();
            List<QuestionAnalysis> analysis = new List<QuestionAnalysis>();
            foreach (BsonDocument a in result)
            {
                analysis.Add(new QuestionAnalysis(a));
            }
            return analysis;
        }
        /* Question End */

        /* Alternatives Start */
        public async Task<Alternative> GetAlternative(string sessionId, string questionId, string alternativeId)
        {
            var filter = Builders<Session>.Filter;
            var query = filter.Eq(s => s.Id, new ObjectId(sessionId));

            Session session = await _mongoContext.Sessions.Find(query).FirstOrDefaultAsync();
            Question question = session.Questions.Find(q => q.Id.ToString().Equals(questionId));
            Alternative alternative = question.Alternatives.Find(a => a.Id.ToString().Equals(alternativeId));
            return alternative;
        }
        /* Alternatives End */

        /* Session Start */

        /* Create */
        public async Task<Session> CreateSession(Session session)
        {
            session.Id = ObjectId.GenerateNewId();
            session.Status = "Pending";

            await _mongoContext.Sessions.InsertOneAsync(session);
            return await _mongoContext.Sessions.Find(n => n.Id == session.Id).FirstOrDefaultAsync();
        }

        /* Review */
        public async Task<Session> GetSession(string id)
        {
            var filter = Builders<Session>.Filter;
            var query = filter.Eq(q => q.Id, new ObjectId(id));

            return await _mongoContext.Sessions.Find(query).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Session>> GetSessions()
        {
            return await _mongoContext.Sessions.Find(_ => true).ToListAsync();
        }

        /* Update */
        public async Task<Session> AddQuestionsToSession(string id, List<string> questions)
        {
            List<ObjectId> questionIds = new List<ObjectId>();

            foreach (string question in questions)
            {
                questionIds.Add(new ObjectId(question));
            }

            var sessionFilter = Builders<Session>.Filter;
            var sessionQuery = sessionFilter.Eq(s => s.Id, new ObjectId(id));
            Session sessionUpdate;
            Session session = sessionUpdate = await _mongoContext.Sessions.Find(sessionQuery).FirstOrDefaultAsync();

            var questionsFilter = Builders<Question>.Filter;
            var questionsQuery = questionsFilter.In(q => q.Id, questionIds);
            List<Question> questions1 = _mongoContext.Questions.Find(questionsQuery).ToList();

            if (session.Questions == null)
            {
                sessionUpdate.Questions = new List<Question>();
            }

            foreach(Question q in questions1)
            {
                if (sessionUpdate.Questions.FindIndex(sq => sq.Id == q.Id) == -1)
                {
                    sessionUpdate.Questions.Add(q);
                }
                
            }

            UpdateResult result = await _mongoContext.Sessions.UpdateOneAsync(
                sessionQuery,
                Builders<Session>.Update.Set(s => s.Questions, sessionUpdate.Questions)
            );

            if (result.ModifiedCount > 0)
            {
                return sessionUpdate;
            }
            else
            {
                return session;
            }
        }

        public async Task<Session> SetSessionStatus(string id, string status)
        {
            var filter = Builders<Session>.Filter;
            var query = filter.Eq(s => s.Id, new ObjectId(id));
            var update = Builders<Session>.Update.Set(s => s.Status, status);
            await _mongoContext.Sessions.UpdateOneAsync(query, update);

            return await _mongoContext.Sessions.Find(query).FirstOrDefaultAsync();
        }

        public async Task<Session> AskQuestion(string sessionId, string questionId)
        {
            var filter = Builders<Session>.Filter;
            var query = filter.Eq(s => s.Id, new ObjectId(sessionId));
            var update = Builders<Session>.Update.Set(s => s.CurrentQuestion, questionId);
            await _mongoContext.Sessions.UpdateOneAsync(query, update);

            return await _mongoContext.Sessions.Find(query).FirstOrDefaultAsync();
        }
        /* Session End */

        /* User Start */
        public async Task<NewUser> CreateUser(NewUser newUser)
        {
            newUser.Id = ObjectId.GenerateNewId();
            await _mongoContext.NewUsers.InsertOneAsync(newUser);
            return await _mongoContext.NewUsers.Find(n => n.Id == newUser.Id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<NewUser>> GetNewUsers()
        {
            var filter = Builders<NewUser>.Filter;
            var query = filter.Eq(u => u.Created, false);
            return await _mongoContext.NewUsers.Find(query).ToListAsync();
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
                    user.Properties[field.Key].Value = field.Value == "" ? null : field.Value;
                }

                user.CommitChanges();
            });
            
            return user;
        }

        public List<ADUser> GetAPHUsers()
        {
            Dictionary<string, string> groupDNs = new Dictionary<string, string>();
            groupDNs.Add("IT_Department", "CN=IT_Department,OU=Secure Groups,OU=Willow Ave,DC=ahu,DC=on,DC=ca");

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

        /* User End */

        /* Answer Start */
        public async Task<Answer> AnswerQuestion(Answer answer)
        {
            answer.Id = ObjectId.GenerateNewId();
            await _mongoContext.Answers.InsertOneAsync(answer);
            return await _mongoContext.Answers.Find(n => n.Id == answer.Id).FirstOrDefaultAsync();
        }

        public async Task<List<QuestionAnalysis>> AnalyzeSessionQuestion(string sessionId, string questionId)
        {
            var result = await _mongoContext.Answers.Aggregate()
                .Match(a => a.SessionId == sessionId)
                .Match(a => a.QuestionId == questionId)
                .Group(new BsonDocument { { "_id", "$alternativeId" }, { "count", new BsonDocument("$sum", 1) } })
                .ToListAsync();
            List<QuestionAnalysis> analysis = new List<QuestionAnalysis>();
            foreach (BsonDocument a in result)
            {
                analysis.Add(new QuestionAnalysis(a));
            }
            return analysis;
        }

        public async Task<List<string>> GetRespondents(string sessionId, string questionId)
        {
            var result = await _mongoContext.Answers.Aggregate()
                .Match(a => a.SessionId == sessionId)
                .Match(a => a.QuestionId == questionId)
                .Project(new BsonDocument { { "user", "$userId" } })
                .ToListAsync();
            List<string> respondents = new List<string>();
            foreach (BsonDocument r in result)
            {
                respondents.Add(r["user"].ToString());
            }
            return respondents;
        }

        public async Task<List<Answer>> GetAnswers()
        {
            return await _mongoContext.Answers.Find(_ => true).ToListAsync();
        }

        public async Task<List<Answer>> GetAnswers(string sessionId, string questionId)
        {
            var filter = Builders<Answer>.Filter;
            var query = filter.Eq(a => a.SessionId, sessionId) & filter.Eq(a => a.QuestionId, questionId);
            return await _mongoContext.Answers.Find(query).ToListAsync();
        }
        /* Answer End */
    }
}