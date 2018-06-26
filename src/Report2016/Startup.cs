using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Report2016.AzureRepositories;
using AzureStorage.Tables;
using Common.Log;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using Lykke.AzureQueueIntegration;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Report2016.Authentication;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Logs;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Mvc;

namespace Report2016
{
    public class Startup
    {
        private IConfiguration _configuration;
        private SettingsModel _settings;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var settingsManager = _configuration.LoadSettings<SettingsModel>();
            _settings = settingsManager.CurrentValue;
            var connectionStringManager = settingsManager.ConnectionString(x => x.Report2016.VotesConnectionString);

            var log = CreateLogWithSlack(services, settingsManager);

            services.AddSingleton(log);
            services.BindAzureRepositories(connectionStringManager, log);

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
                x.ClientId = _settings.Report2016.Authentication.ClientId;
                x.ClientSecret = _settings.Report2016.Authentication.ClientSecret;
                x.CallbackPath = "/auth";
                x.Events = new AuthEvent(log);
                x.Authority = _settings.Report2016.Authentication.Authority;
                x.Scope.Add("email");
                x.Scope.Add("profile");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            var appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

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

            var dbLogConnectionStringManager = settings.Nested(x => x.Report2016.LogsConnectionString);
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
