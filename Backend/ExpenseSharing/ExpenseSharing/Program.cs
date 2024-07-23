using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

builder.Services.AddTransient<IUserBL, UserBL>();
builder.Services.AddTransient<IGroupBL, GroupBL>();
builder.Services.AddTransient<IExpenseBL, ExpenseBL>();
builder.Services.AddScoped<IGroupMemberBL, GroupMemberBL>();
builder.Services.AddScoped<IExpenseSplitBL, ExpenseSplitBL>();

builder.Services.AddTransient<IUserDAL, UserDAL>();
builder.Services.AddTransient<IGroupDAL, GroupDAL>();
builder.Services.AddTransient<IExpenseDAL, ExpenseDAL>();
builder.Services.AddScoped<IGroupMemberDAL, GroupMemberDAL>();
builder.Services.AddScoped<IExpenseSplitDAL, ExpenseSplitDAL>();

var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
    mig => mig.MigrationsAssembly("DataAccessLayer")));

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

DataSeeder.SeedData(app);

app.Run();

