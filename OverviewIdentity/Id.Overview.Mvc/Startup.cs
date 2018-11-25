using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Id.Overview.Mvc.Data;
using Id.Overview.Mvc.Models;
using Id.Overview.Mvc.Services;

namespace Id.Overview.Mvc
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // AddEntityFrameworkStores - Adiciona uma implementação do entity framework para armazenação do identity
            // AddDefaultTokenProviders - Adiciona o provider padrão de token que é utilizado para gerar token de reset de senha, mudança de email
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Lockout.AllowedForNewUsers = true; // Determina se um novo usuario podera ser bloqueado ou nao
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tempo que o usuario ficara bloqueado
                    options.Lockout.MaxFailedAccessAttempts = 5; // Numero de tentativa de acesso com falha antes do o usuario ser bloqueado com o Lockout ativo

                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication(); // Adicionamos o indetity ao pipeline da aplicação para que possa ser utilizado

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
