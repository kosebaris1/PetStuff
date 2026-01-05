using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? "https://localhost:7120";
        options.RequireHttpsMetadata = false; // Development için false, production'da true olmalı
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false, // Gateway tüm API'leri kapsadığı için audience kontrolü yapmıyoruz
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7166", "http://localhost:5180")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticated", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "PetStuff API Gateway", 
        Version = "v1",
        Description = "Central API Gateway for PetStuff microservices"
    });

    // JWT Authentication için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetStuff API Gateway v1");
    });
}

app.UseHttpsRedirection();

// CORS middleware'i UseAuthentication'den ÖNCE olmalı
app.UseCors("AllowFrontend");

// OPTIONS request'lerini authentication'dan muaf tut
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(string.Empty);
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Map Reverse Proxy
app.MapReverseProxy();

app.Run();
