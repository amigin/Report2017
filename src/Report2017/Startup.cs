using System;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Logs;
using Lykke.MonitoringServiceApiCaller;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Report2017.Authentication;
using Report2017.AzureRepositories;

namespace Report2017
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Environment { get; }
        private SettingsModel _settings;
        private ILog _log;

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .Build();

            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var settingsManager = Configuration.LoadSettings<SettingsModel>();
            _settings = settingsManager.CurrentValue;
            var connectionStringManager = settingsManager.ConnectionString(x => x.Report2017.VotesConnectionString);

            _log = CreateLogWithSlack(services, settingsManager);

            services.AddSingleton(_log);
            services.BindAzureRepositories(connectionStringManager, _log);

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(x => {
                x.ExpireTimeSpan = TimeSpan.FromHours(24);
                x.LoginPath = new PathString("/Home/SignIn");
                x.AccessDeniedPath = "/404";
            })
            .AddOpenIdConnect(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveTokens = true;
                x.ResponseType = "code";
                x.ClientId = _settings.Report2017.Authentication.ClientId;
                x.ClientSecret = _settings.Report2017.Authentication.ClientSecret;
                x.CallbackPath = _settings.Report2017.Authentication.PostLogoutRedirectUri;
                x.Events = new AuthEvent(_log);
                x.Authority = _settings.Report2017.Authentication.Authority;
                x.Scope.Add("email");
                x.Scope.Add("profile");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            var appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            
            appLifetime.ApplicationStarted.Register(() =>
            {
                try
                {
                    _log.WriteMonitor("StartApplication", null, "Application started");

                    if (!env.IsDevelopment())
                    {
                        if (_settings.MonitoringServiceClient?.MonitoringServiceUrl == null)
                            throw new ApplicationException("Monitoring settings is not provided.");

                        AutoRegistrationInMonitoring.RegisterAsync(Configuration, _settings.MonitoringServiceClient.MonitoringServiceUrl, _log).GetAwaiter().GetResult();
                    }

                }
                catch (Exception ex)
                {
                    _log.WriteFatalError("StartApplication", "", ex);
                    throw;
                }
            });
            
            app.UseLykkeMiddleware(appName, ex => ErrorResponse.Create("Technical problem"));
            app.UseLykkeForwardedHeaders();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        
        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<SettingsModel> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueSettings
            {
                ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.Report2017.LogsConnectionString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            // Creating azure storage logger, which logs own messages to console log
            if (!string.IsNullOrEmpty(dbLogConnectionString) && !(dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}")))
            {
                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "VotesLogs", consoleLogger),
                    consoleLogger);

                var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

                var azureStorageLogger = new LykkeLogToAzureStorage(
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);

                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }

            return aggregateLogger;
        }
    }
}
