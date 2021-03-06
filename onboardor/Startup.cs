using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GraphQL.Server.Transports.AspNetCore;
using Newtonsoft.Json;
using Autofac;
using System.Reflection;
using onboardor.GitHub;
using GraphQL.Types;
using GraphQL;
using GraphQL.Relay.Types;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Onboardor.Data;
using Onboardor.Components.graphQl;
using Onboardor.Components.GraphQl;
using onboardor.Components.shared.utilities;
using DotNetEnv;
using Onboardor.Repository;
using onboardor.Components.dashboard;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Primitives;
using AWS.Logger;
using System.Net;

namespace Onboardor
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Env.Load();
            }
            _env = env;
            loggerFactory.CreateLogger<Startup>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Env.GetString("DEFAULT_CONNECTION")));
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Env.GetString("DEFAULT_CONNECTION");
                options.SchemaName = "dbo";
                options.TableName = "Cache";
                options.DefaultSlidingExpiration = TimeSpan.FromDays(1);
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("::ffff:100.64.0.0"), 106));
            });
            services.AddSession();
            services.AddMemoryCache();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(30);
            });
            services.AddGraphQLHttp();
            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .SetGitHubWebHooks();

            var builder = RegisterServices();

            builder.Populate(services);

            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole((LogLevel)Env.GetInt("LOGGING_LOGLEVEL"), false);
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    ConfigFile = "./webpack.config.js",
                });
            }
            else
            {
                loggerFactory.AddAWSProvider(new AWSLoggerConfig
                {
                    Region = "eu-west-2",
                    LogGroup = "Group1",
                }, (LogLevel)Env.GetInt("LOGGING_LOGLEVEL"));
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            Context BuildUserContext(HttpContext c)
            {
                return new Context
                {
                    HttpContext = c
                };
            }
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None
            });
            app.UseStaticFiles();
            app.UseSession(new SessionOptions {
                IdleTimeout = TimeSpan.FromHours(5),
                Cookie = new CookieBuilder
                {
                    Name = ".AspNetCore.Session",
                    SameSite = SameSiteMode.None,
                }
            });

            app.UseGraphQLHttp<AppSchema>(new GraphQLHttpOptions
            {
                ExposeExceptions = !env.IsProduction(),
                BuildUserContext = BuildUserContext,
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "app",
                    "{controller}/{action}"
                );

                routes.MapSpaFallbackRoute(
                    "default",
                    new
                    {
                        controller = "App",
                        action = "Index"
                    }
                );
            });
        }

        private ContainerBuilder RegisterServices()
        {
            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();

                return new AppSchema(t =>
                {
                    var type = context.ResolveOptional(t);
                    var resolvedType = type ?? Activator.CreateInstance(t);

                    return resolvedType.As<IGraphType>();
                });
            });

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NodeGraphType<>));
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ObjectGraphType<>));
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(InterfaceGraphType<>));
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(MutationPayloadGraphType<,>));
            builder.RegisterAssemblyTypes(assembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces();

            builder.RegisterType<AppQuery>();
            builder.RegisterType<AppMutation>();
            builder.RegisterType<Repository<Organization, ApplicationDbContext>>().As<IRepository<Organization>>();
            builder.RegisterType<Repository<Member, ApplicationDbContext>>().As<IRepository<Member>>();
            builder.RegisterType<Repository<OnboardingPipeline, ApplicationDbContext>>().As<IRepository<OnboardingPipeline>>();
            builder.RegisterType<Repository<OnboardingStep, ApplicationDbContext>>().As<IRepository<OnboardingStep>>();
            builder.RegisterType<Repository<OnboardingProcess, ApplicationDbContext>>().As<IRepository<OnboardingProcess>>();

            return builder;
        }
    }
}
