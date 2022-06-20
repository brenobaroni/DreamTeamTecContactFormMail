using Domain.Entities;
using DreamTeamTecContactFormMail.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Repository.Context;
using Repository.Repository;
using Repository.Repository.Contracts;
using Service;
using Service.Interfaces;


namespace DreamTeamTecContactFormMail
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
            services.AddScoped<ILeadsRepository, LeadsRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddDbContext<DtContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("WebApiDatabase");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DreamTeamTecContactFormMail", Version = "v1" });
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "XApiKey",
                    Description = "ApiKey Authentication"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins(this.Configuration.GetSection("FrontUrl").Value).AllowAnyHeader()
                    .AllowAnyMethod()); ;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DreamTeamTecContactFormMail v1"));
            }

            app.UseCors("AllowSpecificOrigin");

            //app.UseMiddleware<MiddlewareApiKey>();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("SiteCorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
