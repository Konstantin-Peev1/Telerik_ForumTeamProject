using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
           
             
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                // The connection string can be found in the appsettings.json file. 
                // It's a good practice to keep the connection string in a separate file,
                //  because it's easier to change the connection string without recompiling the entire application.
                // Also, the connection string is a sensitive information and should not be exposed in the code.
                string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);

                // The following helps with debugging the trobled relationship between EF and SQL ¯\_(-_-)_/¯ 
                options.EnableSensitiveDataLogging();
            });

            //Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IReplyRepository, ReplyRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();    

            //Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ITagService,  TagService>();

            // Helpers
            builder.Services.AddScoped<ModelMapper>();
            builder.Services.AddScoped<AuthManager>();
            builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
