using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WithoutIdentity.Data;
using WithoutIdentity.Models;

namespace WithoutIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("IdentityDb");
            services.AddDbContext<ApplicationDataContext>(options => options.UseSqlServer(connectionString));

            // ApplicationUser - classe que representa o usuario
            // IdentityRole - Regras do usuario
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDataContext>() // Registra o EntityFramework como responsavel pelo armazenamento dos dados do Identity
                .AddDefaultTokenProviders(); // Adiciona o provider padrao de token

            services.Configure<IdentityOptions>(options =>
            {
                // Configurações de senha
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8; // Valor default 6 digitos
                options.Password.RequiredUniqueChars = 6; // Valor default 1
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            });

            // Configurações dos cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Account/Login"; // Caminho para login
                options.LogoutPath = "/Account/Logout"; // Caminho para logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // Caminho para erro 403 - acesso negado
                options.SlidingExpiration = true; // Se passou da metade do tempo de expiração do cookie é renovado automaticamente
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Adicionando o Identity ao pipeline da aplicação
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
