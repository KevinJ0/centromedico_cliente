using Amazon.S3;
using AutoMapper;
using Centromedico.Database.DbModels;
using Centromedico.Database.Context;
using CentromedicoCliente.Exceptions;
using Cliente.Repository.Repositories;
using Cliente.Repository.Repositories.Interfaces;
using CentromedicoCliente.Services;
using CentromedicoCliente.Services.Helpers;
using CentromedicoCliente.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CentromedicoCliente.Profiles;
using CentromedicoCliente.Interfaces.Services;

namespace CentromedicoCliente

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
            // configure strongly typed settings object
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<TwilioSettings>();

            services.AddHttpContextAccessor();

            services.AddScoped<IWhatsappService, WhatsappService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<IHorarioMedicoService, HorarioMedicoService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMedicoService, MedicoService>();
            services.AddScoped<ISeguroService, SeguroService>();
            services.AddScoped<IPreguntaService, PreguntaService>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ICitaRepository, CitaRepository>();
            services.AddScoped<IHorarioMedicoRepository, HorarioMedicoRepository>();
            services.AddScoped<IPreguntaRepository, PreguntaRepository>();
            services.AddScoped<IMedicoRepository, MedicoRepository>();
            services.AddScoped<ICoberturaRepository, CoberturaRepository>();
            services.AddScoped<IServicioRepository, ServicioRepository>();
            services.AddScoped<IHorarioMedicoReservaRepository, HorarioMedicoReservaRepository>();
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<ISeguroRepository, SeguroRepository>();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();


            services.AddSingleton<IS3Service, S3Service>();
            services.AddAWSService<IAmazonS3>();

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));
            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    string errors = "";
                    var errorsList = new List<string>();

                    foreach (var state in actionContext.ModelState)
                    {

                        foreach (var error in state.Value.Errors)
                        {

                            errors = error.ErrorMessage;
                            var n = errors.IndexOf("Path");
                            if (n! > 0)
                                errorsList.Add(errors.Substring(0, n).Trim());
                            else
                                errorsList.Add(errors);

                        }

                    }

                    var modelState = actionContext.ModelState;
                    return new BadRequestObjectResult(new
                    {
                        error = new[] { errorsList[0] },
                    })
                    ;
                };
            });

            services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

                });
            });

            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());

            });
            IMapper mapper = mapperConfig.CreateMapper();
            // services.AddSingleton(mapper);
            services.AddAutoMapper(typeof(Startup));
            // Token Model
            services.AddScoped<token>();

            //services.AddTransient<ITokenService, TokenService>();
            services.AddIdentity<MyIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            }).AddEntityFrameworkStores<MyDbContext>().
                 AddDefaultTokenProviders(); ;

            services.AddDbContext<MyDbContext>(option => option.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
                AddJwtBearer(o =>
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Authorization:Issuer"],
                    ValidAudience = Configuration["Authorization:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authorization:LlaveSecreta"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireLoggedIn",
                    policy => policy.RequireRole("Client", "Patient", "Secretary", "Doctor").RequireAuthenticatedUser());

            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Centro Medico Cliente Api",
                    Version = "v1",
                    Description = "Esta api describe las funciones de los diferentes endpoint que trabajan en la applicaci�n que da vista al cliente",
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*if (env.IsDevelopment())
            {*/
            app.UseDeveloperExceptionPage();
            /*      }
               else
            {
                   app.UseExceptionHandler("/Error");
                   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                   app.UseHsts();
               }*/
            app.UseCors("EnableCORS");

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Centro Medico Cliente API");
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4211");
                    // spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
    }
}
