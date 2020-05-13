using System;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json;
using CoviIDApiCore.Middleware;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.V1.Brokers;
using System.Net.Http.Headers;
using AspNetCoreRateLimit;
using CoviIDApiCore.Data;
using CoviIDApiCore.V1.Configuration;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using CoviIDApiCore.V1.Repositories;
using CoviIDApiCore.V1.Services;
using Hangfire;
using Hangfire.SqlServer;
using Sentry;

namespace CoviIDApiCore
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly string _environment, _connectionString, _applicationName;
        private const string _applicationJson = "application/json";

        public Startup(IConfiguration configuration)
        {
            _applicationName = Assembly.GetExecutingAssembly().GetName().Name;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _environment = _configuration.GetValue<string>("Environment");
            Console.WriteLine($"Environment: {_environment}");
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            ConfigureSentry();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCORS(services);
            ConfigureSwagger(services);
            ConfigureHangfire(services);
            BindAppSettingConfiguration(services);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(o => o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddJsonOptions(o => o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problems = new CustomBadRequestMiddleware(context);
                        return new BadRequestObjectResult(problems);
                    };
                });

            ConfigureDatabaseContext(services);
            ConfigureDependecyInjection(services);
            ConfigureHttpClients(services);
            ConfigureRateLimiting(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (_environment != "Production")
            {
                app.UseHangfireDashboard();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(swagger =>
                {
                    swagger.SwaggerEndpoint("/swagger/v1/swagger.json", $"Cov-ID Core {_applicationName}");
                    swagger.RoutePrefix = string.Empty;
                });
            }
            else
                app.UseHsts();

            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            ConfigureDefaultResponses(app);
            app.UseMiddleware<ExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        #region Private Configuration Methods

        private void ConfigureDatabaseContext(IServiceCollection services)
        {
            var connection = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectRetryInterval = 3,
                MinPoolSize = 3
            };
            services.AddDbContext<ApplicationDbContext>(
                b => b.UseSqlServer(
                    connection.ToString(),
                    options => options.EnableRetryOnFailure())
            );
        }
        private void ConfigureHangfire(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(_configuration.GetConnectionString("HangfireConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }));

            // This adds the processing server as IHostedService
            services.AddHangfireServer();
        }

        private void ConfigureCORS(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
        }

        private void ConfigureDependecyInjection(IServiceCollection services)
        {
            #region Service layer
            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<IVerifyService, VerifyService>();
            services.AddTransient<ICredentialService, CredentialService>();
            services.AddScoped<IOrganisationService, OrganisationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IQRCodeService, QRCodeService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddTransient<ITestResultService, TestResultService>();
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddScoped<IWalletDetailService, WalletDetailService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddTransient<ISmsService, SmsService>();
            #endregion

            #region Repository Layer
            services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            services.AddScoped<IOrganisationAccessLogRepository, OrganisationAccessLogRepository>();
            services.AddScoped<IOtpTokenRepository, OtpTokenRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ICovidTestRepository, CovidTestRepository>();
            services.AddScoped<IWalletDetailRepository, WalletDetailRepository>();
            services.AddScoped<IWalletTestResultRepository, WalletTestResultRepository>();
            #endregion

            #region Broker Layer
            services.AddTransient<IAgencyBroker, AgencyBroker>();
            services.AddTransient<ICustodianBroker, CustodianBroker>();
            services.AddTransient<ISendGridBroker, SendGridBroker>();
            services.AddTransient<IClickatellBroker, ClickatellBroker>();
            services.AddSingleton<IAmazonS3Broker, AmazonS3Broker>();
            #endregion
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            var agencyApiBaseUrl = _configuration.GetConnectionString("AgencyApiBaseUrl");
            var custodianApiBaseUrl = _configuration.GetConnectionString("CustodianApiBaseUrl");
            var tenantId = _configuration.GetValue<string>("TenantId");
            var sendGridCredentials = new SendGridCredentials();
            _configuration.Bind(nameof(SendGridCredentials), sendGridCredentials);

            var streetCredCredentials = new StreetCredCredentials();
            _configuration.Bind(nameof(StreetCredCredentials), streetCredCredentials);

            var clickatellCredentials = new ClickatellCredentials();
            _configuration.Bind(nameof(ClickatellCredentials), clickatellCredentials);

            services.AddHttpClient<IAgencyBroker, AgencyBroker>(client =>
            {
                client.BaseAddress = new Uri(agencyApiBaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", streetCredCredentials.AuthorizationToken);
                client.DefaultRequestHeaders.Add("X-Streetcred-Subscription-Key", streetCredCredentials.SubscriptionKey);
                client.DefaultRequestHeaders.Add("X-Streetcred-Tenant-Id", tenantId);
            });

            services.AddHttpClient<ICustodianBroker, CustodianBroker>(client =>
            {
                client.BaseAddress = new Uri(custodianApiBaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", streetCredCredentials.AuthorizationToken);
                client.DefaultRequestHeaders.Add("X-Streetcred-Subscription-Key", streetCredCredentials.SubscriptionKey);
                client.DefaultRequestHeaders.Add("X-Streetcred-Tenant-Id", tenantId);
            });

            services.AddHttpClient<ISendGridBroker, SendGridBroker>(client =>
                {
                    client.BaseAddress = new Uri(sendGridCredentials.BaseUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sendGridCredentials.Key);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_applicationJson));
                }
            );

            services.AddHttpClient<IClickatellBroker, ClickatellBroker>(client =>
                {
                    client.BaseAddress = new Uri(clickatellCredentials.BaseUrl);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", clickatellCredentials.Key);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_applicationJson));
                }
            );
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = $"The Delta Studio .NET Core 2.2 Web API {_applicationName}", Version = "v1" });
                options.AddSecurityDefinition("AccessToken", new ApiKeyScheme
                {
                    Description = "Your authorization token allows this organization to authenticate to the API. Example: \"Bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.AddSecurityDefinition("Subscription-Key", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Your subscription key allows the API to understand the permissions associated with your account.",
                    Name = "X-Subscription-Key",
                    Type = "apiKey",
                });
            });
        }

        private void ConfigureDefaultResponses(IApplicationBuilder app)
        {
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await context.HttpContext.Response.WriteAsync(
                        JsonConvert.SerializeObject(new V1.DTOs.System.Response(false, HttpStatusCode.Unauthorized,
                        "Unauthorised")));
                }
            });
        }

        private void ConfigureRateLimiting(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(_configuration.GetSection("PlatformSettings:IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(_configuration.GetSection("PlatformSettings:IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
        
        private void ConfigureSentry()
        {
            var url = _configuration?.GetSection("Sentry")?.GetSection("Url").Value ?? throw new Exception("Failed to setup sentry.");
            SentrySdk.Init(
                x =>
                {
                    x.Environment = _environment;
                    x.Dsn = new Dsn(url);
                });
        }

        private void BindAppSettingConfiguration(IServiceCollection services)
        {
            var awsS3BucketCredentials = new AwsS3BucketCredentials();
            _configuration.Bind(nameof(AwsS3BucketCredentials), awsS3BucketCredentials);
            services.AddSingleton(awsS3BucketCredentials);
        }

        #endregion Private Configuration Methods
    }
}
