using GraphQL.Types;
using System.DirectoryServices;
using Plasma.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Plasma.Types
{
    public class NewUserType : ObjectGraphType<NewUser>
    {
        public NewUserType(OrgData data)
        {
            Name = "NewUser";
            Field<StringGraphType>("employeeID", resolve: context => context.Source.EmployeeID.ToString());
            Field<StringGraphType>("givenName", resolve: context => context.Source.GivenName.ToString());
            Field<StringGraphType>("sN", resolve: context => context.Source.SN.ToString());
            Field<StringGraphType>("title", resolve: context => context.Source.Title.ToString());
            Field<StringGraphType>("description", resolve: context => context.Source.Description.ToString());
            Field<StringGraphType>("department", resolve: context => context.Source.Department.ToString());
            Field<StringGraphType>("manager", resolve: context => context.Source.Manager.ToString());
            Field<StringGraphType>("physicalDeliveryOfficeName", resolve: context => context.Source.PhysicalDeliveryOfficeName.ToString());
            Field<StringGraphType>("homeDirectoryServer", resolve: context => context.Source.HomeDirectoryServer.ToString());
            Field<StringGraphType>("telephoneNumber", resolve: context => context.Source.TelephoneNumber.ToString());
            Field<BooleanGraphType>("created", resolve: context => context.Source.Created.ToString());
        }
    }

    public class NewUserInputType : InputObjectGraphType<NewUser>
    {
        public NewUserInputType()
        {
            Name = "NewUserInput";
            Field<StringGraphType>("employeeID");
            Field<StringGraphType>("givenName");
            Field<StringGraphType>("sN");
            Field<StringGraphType>("title");
            Field<StringGraphType>("description");
            Field<StringGraphType>("department");
            Field<StringGraphType>("manager");
            Field<StringGraphType>("physicalDeliveryOfficeName");
            Field<StringGraphType>("homeDirectoryServer");
            Field<StringGraphType>("telephoneNumber");
            Field<StringGraphType>("created");
        }
    }

    public class NewUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string EmployeeID { get; set; }

        [BsonElement]
        public string GivenName { get; set; }

        [BsonElement]
        public string SN { get; set; }

        [BsonElement]
        public string Title { get; set; }

        [BsonElement]
        public string Description { get; set; }

        [BsonElement]
        public string Department { get; set; }

        [BsonElement]
        public string Manager { get; set; }

        [BsonElement]
        public string PhysicalDeliveryOfficeName { get; set; }

        [BsonElement]
        public string HomeDirectoryServer { get; set; }

        [BsonElement]
        public string TelephoneNumber { get; set; }

        [BsonElement]
        public bool Created { get; set; }
    }
}
