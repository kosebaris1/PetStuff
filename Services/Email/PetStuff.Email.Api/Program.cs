using PetStuff.Email.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Identity Service Client
builder.Services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>();

// RabbitMQ Consumer (Background Service)
builder.Services.AddHostedService<OrderStatusChangedConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
