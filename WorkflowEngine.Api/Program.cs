using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Services;
using WorkflowEngine.Infrastructure.Data;
using WorkflowEngine.RuleEngine.Engines;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DB Context
builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Workflow Services
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddSingleton<RuleEvaluator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
