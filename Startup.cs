using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using userDashboard.Models;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace userDashboard
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string Server = "localhost";
            string Port = "8889";
            string Database = "UserDashboard";
            string UserId = "root";
            string Password = "root";
            // string Server = "csharpuserdashboarddb.c9hdj1bicurq.us-east-1.rds.amazonaws.com";
            // string Port = "3306";
            // string Database = "userdashboard";
            // string UserId = "evcallia";
            // string Password = "asdfasdf";
            string Connection = $"Server={Server};port={Port};database={Database};uid={UserId};pwd={Password}";
            services.AddDbContext<UserDashboardContext>(options => options.UseMySQL(Connection));

            // Add framework services.
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseStaticFiles();
            app.UseSession();
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
