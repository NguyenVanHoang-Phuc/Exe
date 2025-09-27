using BusinessObject.Models;
using DataObject;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Service;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContext<FinanceAppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    )
    );

builder.Services.AddScoped<AiAdviceDAO>();
builder.Services.AddScoped<AiRequestDAO>();
builder.Services.AddScoped<BudgetDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<GoalDAO>();
builder.Services.AddScoped<PaymentDAO>();
builder.Services.AddScoped<PremiumPlanDAO>();
builder.Services.AddScoped<RoleDAO>();
builder.Services.AddScoped<TransactionDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<UserSubscriptionDAO>();

builder.Services.AddScoped<IAiAdviceRepository, AiAdviceRepository>();
builder.Services.AddScoped<IAiRequestRepository, AiRequestRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPremiumPlanRepository, PremiumPlanRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();

builder.Services.AddScoped<IAiAdviceService, AiAdviceService>();
builder.Services.AddScoped<IAiRequestService, AiRequestService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPremiumPlanService, PremiumPlanService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();

builder.Services.AddScoped<IAIRepository, AIRepository>();
builder.Services.AddScoped<IAIService, AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=HomePage}/{id?}")
    .WithStaticAssets();


app.Run();
