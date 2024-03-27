using Api.Infrastructure.Swagger;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme()
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.CustomSchemaIds(d => d.GetSchemaId());

                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

                options.DocumentFilter<DocumentsFilter>();
                options.OperationFilter<ResultOfOperationFilter>();
                options.SchemaFilter<SchemaFilters>();

                options.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });

                options.MapType<TimeSpan?>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });

                options.TagActionsBy(d => new List<string>
                {
                    d.ActionDescriptor.EndpointMetadata.OfType<EndpointGroupNameAttribute>().FirstOrDefault()?.EndpointGroupName
                    ??  d.GroupName
                    ?? (d.ActionDescriptor as ControllerActionDescriptor).ControllerName,
                });

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly().GetName().Name}.xml"));
            });
        }

        public static async Task RunDatabaseMigrations<T>(this WebApplication app, IServiceProvider serviceProvider, ILogger logger)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            logger.LogInformation("Program start, applying migrations");

            try
            {
                var db = serviceProvider.GetRequiredService<ApiDbContext>();

                await db.Database.MigrateAsync();

                logger.LogInformation("Migrations applied");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migrations error");
            }
        }
    }
}
