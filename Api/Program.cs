using Api.Infrastructure;
using Api.Infrastructure.Behaviors;
using Api.Infrastructure.Converters;
using Api.Infrastructure.Extensions;
using Api.Infrastructure.Options;
using Api.Infrastructure.Security;
using Api.Infrastructure.Services;
using Api.Infrastructure.Services.Interfaces;
using API.Infrastructure.Options;
using Azure.Storage.Blobs;
using Data;
using FluentValidation;
using FluentValidation.Validators;
using Mapster;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Nudes.Paginator.FluentValidation;
using Nudes.Retornator.AspnetCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.SeedMaster.Seeder;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static Nudes.SeedMaster.SeedScanner;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

var assembly = Assembly.GetExecutingAssembly();

#region DependencyInjection

#region MediatR

if (builder.Environment.IsDevelopment())
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

AssemblyScanner.FindValidatorsInAssembly(assembly)
               .ForEach(r => builder.Services.AddScoped(r.InterfaceType, r.ValidatorType));

builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(assembly); });
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

#endregion

#region Retornator and MVC
builder.Services.AddErrorTranslator(new ErrorHttpTranslatorBuilder()
                                            .TranslationFor<NotFoundError>(err => HttpStatusCode.NotFound)
                                            .TranslationFor<BadRequestError>(err => HttpStatusCode.BadRequest)
                                            .TranslationFor<UnauthorizedError>(err => HttpStatusCode.Unauthorized)
                                            .TranslationFor<ForbiddenError>(err => HttpStatusCode.Forbidden)
                                            .TranslationFor<InternalServerError>(err => HttpStatusCode.InternalServerError));

builder.Services.AddRazorPages();

builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(config =>
                {
                    config.InvalidModelStateResponseFactory = ctx => new UnprocessableEntityObjectResult(ctx.ModelState);
                })
                .AddJsonOptions(d =>
                {
                    d.JsonSerializerOptions.WriteIndented = true;
                    d.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    d.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    d.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    d.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
                    d.JsonSerializerOptions.Converters.Add(new NullableTimeSpanJsonConverter());
                    d.JsonSerializerOptions.Converters.Add(new StringSensitiveContentConverter());
                })
                .AddRetornator();

builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression(config =>
{
    config.EnableForHttps = true;
});

#endregion

#region Mapster

TypeAdapterConfig.GlobalSettings.Apply(TypeAdapterConfig.GlobalSettings.Scan(assembly));

#if DEBUG
TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();
#endif

#endregion

#region DbContext

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    //options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    //       .EnableDetailedErrors(false)
    //       .EnableSensitiveDataLogging(false);

    //options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Data"))
    //       .EnableDetailedErrors(false)
    //       .EnableSensitiveDataLogging(false);

    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
                 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")),
                 b => b.MigrationsAssembly("Data"))
    .EnableDetailedErrors(false)
    .EnableSensitiveDataLogging(false);
});

builder.Services.AddScoped<DbContext, ApiDbContext>(sp => sp.GetService<ApiDbContext>());

builder.Services.TryAddTransient<IValidatorFactory, ServiceProviderValidatorFactory>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddScoped(x => GetSeeds(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<EfCoreSeeder>();

//builder.Services.AddTransient<ISeeder, MigrationSeeder>();
#endregion

#region Authentication and Authorization
builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Bearer", op =>
{
    var authConfig = builder.Configuration.GetSection(nameof(AuthTokenOptions))
                                          .Get<AuthTokenOptions>();

    op.RequireHttpsMetadata = false;
    op.SaveToken = true;

    op.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Key)),

        ValidateAudience = true,
        ValidAudience = authConfig.Audience,

        ValidateIssuer = true,
        ValidIssuer = authConfig.Issuer,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    op.Events = new JwtBearerEvents
    {
        OnChallenge = a =>
        {
            a.HttpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        },
    };
});

builder.Services.AddAuthorization(op =>
{
    op.DefaultPolicy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .Build();

    PolicyFactory.AddPolicies(op);
});

builder.Services.AddCors(setup => setup
            .AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()));

#endregion

#region Swagger

if (!builder.Environment.IsProduction() && !builder.Environment.IsEnvironment("Test"))
    builder.Services.ConfigureSwaggerGen();

#endregion

#region Options
builder.Services.AddOptions<AuthTokenOptions>().Bind(builder.Configuration.GetSection(nameof(AuthTokenOptions)));
builder.Services.AddOptions<SeedOptions>().Bind(builder.Configuration.GetSection(nameof(SeedOptions)));
builder.Services.AddOptions<SpaceOptions>().Bind(builder.Configuration.GetSection(nameof(SpaceOptions)));
builder.Services.AddOptions<EmailOptions>().Bind(builder.Configuration.GetSection(nameof(EmailOptions)));
builder.Services.AddOptions<ResetPasswordOptions>().Bind(builder.Configuration.GetSection(nameof(ResetPasswordOptions)));
#endregion

#region Services

#region BLOB Service
builder.Services.AddScoped<BlobServiceClient>(sp => new BlobServiceClient(builder.Configuration.GetConnectionString("Storage")));
builder.Services.AddScoped<BlobContainerClient>(sp => sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(builder.Configuration["Storage:Container"]));

#endregion

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<HashService>();
builder.Services.AddScoped<IActor, Actor>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISpaceService, SpaceService>();
#endregion

#region Validation
foreach (Type validatorType in typeof(Program).Assembly.GetTypes())
    if (validatorType.IsAssignableTo(typeof(IPropertyValidator)))
        builder.Services.AddTransient(validatorType);

builder.Services.AddTransient(typeof(PageRequestValidator<>));
#endregion
#endregion DependencyInjection

var app = builder.Build();

#region Pipeline

using (var scope = app.Services.CreateScope())
{
    await app.RunDatabaseMigrations<Program>(scope.ServiceProvider, app.Logger);
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    app.Use(async (context, result) =>
    {
        if (context.Request.Path == "/seed")
        {
            await context.RequestServices.GetRequiredService<EfCoreSeeder>().Clean();
            await context.RequestServices.GetRequiredService<EfCoreSeeder>().Seed();
            await context.RequestServices.GetRequiredService<EfCoreSeeder>().Commit();

            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Seeded"), 0, 6);
        }
        else await result();
    });
}

if (!builder.Environment.IsProduction() && !builder.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
        c.InjectStylesheet("/css/Swagger.css");
        c.DisplayRequestDuration();
    });
}

app.UseResponseCompression();

app.UseStaticFiles();

app.UseCors();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");

    endpoints.MapHealthChecks("/health");
});

#endregion

//if (args.Any(d => d.Equals("seed", StringComparison.InvariantCultureIgnoreCase)))
//{
//    using var scope = app.Services.CreateScope();
//    using var seeder = scope.ServiceProvider.GetService<ISeeder>();

//    await seeder.Run();
//}


await app.RunAsync();

