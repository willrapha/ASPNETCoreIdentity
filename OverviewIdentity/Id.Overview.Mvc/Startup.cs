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
                    // Lockout Options
                    options.Lockout.AllowedForNewUsers = true; // Determina se um novo usuario podera ser bloqueado ou nao
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tempo que o usuario ficara bloqueado
                    options.Lockout.MaxFailedAccessAttempts = 5; // Numero de tentativa de acesso com falha antes do o usuario ser bloqueado com o Lockout ativo

                    // Password Options
                    options.Password.RequireDigit = true; // Determina a obrigatoriedade de um numero entre 0 e 9 na senha
                    options.Password.RequiredLength = 6; // Tamanho minimo aceitavel para o usuario criar uma senha
                    options.Password.RequiredUniqueChars = 1; // Aplicavel somente ao ASP.NET Core 2.0, Determina a quantidade de caracteres distintos na senha 'unicos'
                    options.Password.RequireLowercase = true; // Determina a obrigatoriedade de uma letra minuscula na senha
                    options.Password.RequireUppercase = true; // Determina a obrigatoriedade de uma letra maiuscula na senha
                    options.Password.RequireNonAlphanumeric = true; // Determina a obrigatoriedade de um caracterer especial na senha

                    // SignIn Options
                    options.SignIn.RequireConfirmedEmail = false; // Determina para o usuario realizar o signIn no sistema ele deve ter um email confirmado
                    options.SignIn.RequireConfirmedPhoneNumber = false; // Determina para o usuario realizar o signIn no sistema o usuario deve ter confirmado um numero de telefone

                    // Tokens Options
                    // options.Tokens.AuthenticatorTokenProvider // Utilizado para gerar valores para o two factory authentication
                    // options.Tokens.ChangeEmailTokenProvider // Obtem ou definine o ChangeEmailTokenProvider usado para gerar tokens do email de confirmação do processo de alteração de email do usuario
                    // options.Tokens.ChangePhoneNumberTokenProvider // Obtem ou definine o ChangePhoneNumberTokenProvider usado para gerar tokens para o telefone de confirmação do processo de alteração do telefone do usuario
                    // options.Tokens.EmailConfirmationTokenProvider // Define o token provider utilizado para gerar tokens utilizados na configuração de um email utilizado pelo usuario
                    // options.Tokens.PasswordResetTokenProvider // Usuado para gerar tokens no processo de alteração da senha do usuario         
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
