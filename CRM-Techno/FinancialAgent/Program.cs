using FinancialAgent.Data;
using FinancialAgent.Services;
using Quartz;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<AnomalyDetectionService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<ReportSchedulerService>();
builder.Services.AddSingleton<DataGenerator>();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Financial Agent API", 
        Version = "v1",
        Description = "API for generating financial reports and detecting anomalies"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial Agent API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the root URL (e.g., http://localhost:5168/)
    });
    // Disable HTTPS redirection in development to avoid issues with Swagger
    // app.UseHttpsRedirection(); // Comment out or remove for development
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Geração de dados de teste
using (var scope = app.Services.CreateScope())
{
    var dataGenerator = scope.ServiceProvider.GetRequiredService<DataGenerator>();
    await dataGenerator.ClearTestDataAsync(); // Limpa dados antigos
    await dataGenerator.GenerateTestDataAsync(1000); // Gera 1000 registros
}

// Iniciar o agendador de relatórios
var schedulerService = app.Services.GetRequiredService<ReportSchedulerService>();
await schedulerService.StartAsync();

app.Run();