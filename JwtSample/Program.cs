using RedisSaple.Data;
using RedisSaple.Entity;
using RedisSaple.Helper;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
});
builder.Services.AddDbContextSettings();

builder.Services.AddStackExchangeRedisCache(x =>
{
    x.Configuration = builder.Configuration.GetSection("RedisCacheOptions:Configuration").Value;
    x.InstanceName = builder.Configuration.GetSection("RedisCacheOptions:InstanceName").Value;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthMiddleware();

app.MapControllers();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<JwtContext>();
dbContext
    .Set<User>()
    .AddRange(
        new List<User>()
        {
            new User()
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Email = "admin@demo.com",
                Password = "admin@demo.com",
                UserType = UserType.Admin
            },
            new User()
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Email = "user@demo.com",
                Password = "user@demo.com",
                UserType = UserType.User
            },
            new User()
            {
                Id = Guid.NewGuid(),
                Name = "Auditor",
                Email = "auditor@demo.com",
                Password = "auditor@demo.com",
                UserType = UserType.Auditor
            }
        });

dbContext.SaveChanges();

app.Run();
