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
using Microsoft.AspNetCore.Http;

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

                    // Tokens Options // Comentado para não sobrescrever valor default
                    // options.Tokens.AuthenticatorTokenProvider // Utilizado para gerar valores para o two factory authentication
                    // options.Tokens.ChangeEmailTokenProvider // Obtem ou definine o ChangeEmailTokenProvider usado para gerar tokens do email de confirmação do processo de alteração de email do usuario
                    // options.Tokens.ChangePhoneNumberTokenProvider // Obtem ou definine o ChangePhoneNumberTokenProvider usado para gerar tokens para o telefone de confirmação do processo de alteração do telefone do usuario
                    // options.Tokens.EmailConfirmationTokenProvider // Define o token provider utilizado para gerar tokens utilizados na configuração de um email utilizado pelo usuario
                    // options.Tokens.PasswordResetTokenProvider // Usuado para gerar tokens no processo de alteração da senha do usuario         

                    // User Options
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";//  Define os caracteres permitido para o nome do usuario
                    options.User.RequireUniqueEmail = false; // Determina se em nossa aplicação cada usuario tenha um email unico
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Cookies Identity
            services.ConfigureApplicationCookie(options =>
            {
                // Alguns parametros foram comentados para manter o padrao default dos dados do Identity
                options.AccessDeniedPath = "/Account/AcessDenied"; // Caminho para o handle responsavel por tratar o erro de status 403
                // options.ClaimsIssuer = ""; // Obtem ou define um Issuer que sera utilizado para criação de qualquer claim
                // options.Cookie.Domain = ""; // Define o dominio ao qual o cookie que sera criado pertence
                // options.Cookie.Expiration = ""; // Obtem ou define o tempo de vida do cookie http
                options.Cookie.HttpOnly = true; // Define se a propriedade pode ou não ser acessada pelo lado do Cliente
                options.Cookie.Name = ".AspNetCore.Cookies"; // Nome do cookie
                options.Cookie.Path = ""; // Caminho do cookie
                options.Cookie.SameSite = SameSiteMode.Lax; // Define uma propriedade do cookie SameSite, SameSite são cookies que não devem ser anexo as solicitações pro side - Strict cookies nao são enviados em navegações de nivel superior - Lax cookies são enviados para navegações de nivel superior 
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Configuração das policy
                // options.CookieManager = ; // Define o componente que sera utilizado para obter os cookies nos request ou dos disponse
                // options.DataProtectionProvider = ; // Provider usado pelo CookieAuthenticationHandler para a proteção dos dados
                // options.Events = ; // Handler ou manipulador responsavel por chamar os metodos do provider que dara controle da aplicação em certos pontos que ocorrem processamento
                // options.EventsType = ; // Servico que sera responsavel por obter a instacia dos eventos
                options.ExpireTimeSpan = TimeSpan.FromDays(14); // Controla o tempo do ticket de autenticacao permanecera valido a partir do momento que foi criado
                options.LoginPath = "/Account/Login"; // Caminho onde o usuario ira logar caso determinado controler precise de autorização
                options.LogoutPath = "/Accout/Logout"; // Caminho onde o usuario ira se deslogar 
                options.ReturnUrlParameter = "ReturnUrl"; // Nome do parametro que recebera a url que usuario devera ser redireciado apos realizar o login
                // options.SessionStore = ; // Define um container opcional ao qual armazenara a identidade do usuario que fara as requisicoes
                options.SlidingExpiration = true; // Quando habilitado um novo cookie sera criado com uma nova hora de expiração quando o atual tiver passada da metade do tempo de expiração
                // options.TicketDataFormat = ; // Utilizado para proteger e desproteger a identidade e outras propriedades armazenadas no valor do cookie
            });

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
