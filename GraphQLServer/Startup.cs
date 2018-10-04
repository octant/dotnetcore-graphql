using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Plasma.Data;
using Plasma.Types;
using Plasma.Settings;

namespace Plasma
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<MongoSettings>(settings =>
            {
                settings.ConnectionString
                    = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                settings.Database
                = Configuration.GetSection("MongoConnection:Database").Value;
            });

            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<ISchema, OrgSchema>();
            services.AddSingleton<OrgData>();
            services.AddSingleton<Queries>();
            services.AddSingleton<Mutations>();
            services.AddSingleton<ADUserType>();
            services.AddSingleton<ADGroupType>();
            services.AddSingleton<ADUserInputType>();
            services.AddSingleton<PublicUserDataType>();
            services.AddSingleton<MessageType>();
            services.AddSingleton<NewUserType>();
            services.AddSingleton<NewUserInputType>();
            services.AddSingleton<QuestionType>();
            services.AddSingleton<QuestionInputType>();
            services.AddSingleton<QuestionAnalysisType>();
            services.AddSingleton<AlternativeType>();
            services.AddSingleton<AlternativeInputType>();
            services.AddSingleton<SessionType>();
            services.AddSingleton<SessionInputType>();
            services.AddSingleton<AnswerType>();
            services.AddSingleton<AnswerInputType>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
