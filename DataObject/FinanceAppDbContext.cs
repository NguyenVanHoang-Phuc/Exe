using System;
using System.Collections.Generic;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataObject;

public partial class FinanceAppDbContext : DbContext
{
    public FinanceAppDbContext()
    {
    }

    public FinanceAppDbContext(DbContextOptions<FinanceAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiAdvice> AiAdvices { get; set; }

    public virtual DbSet<AiRequest> AiRequests { get; set; }

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PremiumPlan> PremiumPlans { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiAdvice>(entity =>
        {
            entity.HasKey(e => e.AdviceId).HasName("PK__AI_Advic__A2B9EF6AC18802E1");

            entity.ToTable("AI_Advice");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_AIAdv_UserDate").IsDescending(false, true);

            entity.Property(e => e.AdviceId).HasColumnName("advice_id");
            entity.Property(e => e.AdviceText).HasColumnName("advice_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Request).WithMany(p => p.AiAdvices)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AI_Advice__reque__571DF1D5");

            entity.HasOne(d => d.Transaction).WithMany(p => p.AiAdvices)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__AI_Advice__trans__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.AiAdvices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AI_Advice__user___5812160E");
        });

        modelBuilder.Entity<AiRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__AI_Reque__18D3B90FA50AF8D6");

            entity.ToTable("AI_Requests");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_AIReq_UserDate").IsDescending(false, true);

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.PromptText).HasColumnName("prompt_text");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AiRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AI_Reques__user___534D60F1");
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.BudgetId).HasName("PK__Budgets__3A655C14F8E23AD0");

            entity.HasIndex(e => new { e.UserId, e.Year, e.Month }, "UX_Budgets_Goal_Month_Year").IsUnique();

            entity.Property(e => e.BudgetId).HasColumnName("budget_id");
            entity.Property(e => e.AmountLimit)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount_limit");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.User).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Budgets__user_id__60A75C0F");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B4C59DCAA4");

            entity.HasIndex(e => new { e.UserId, e.Name, e.Type }, "UX_Categories_User_Name_Type").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Categories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Categorie__user___4BAC3F29");
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Goals__76679A2431669DE5");

            entity.HasIndex(e => e.UserId, "IX_Goals_User");

            entity.Property(e => e.GoalId).HasColumnName("goal_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("current_amount");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TargetAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("target_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Goals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Goals__user_id__5AEE82B9");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EAE6CCA9C8");

            entity.ToTable("Payment");

            entity.HasIndex(e => new { e.UserId, e.PaymentDate }, "IX_Payment_UserDate").IsDescending(false, true);

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasColumnName("currency");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("payment_date");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__plan_id__46E78A0C");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__Payment__subscri__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__user_id__45F365D3");
        });

        modelBuilder.Entity<PremiumPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Premium___BE9F8F1DF7C12FD2");

            entity.ToTable("Premium_Plans");

            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.DurationMonths).HasColumnName("duration_months");
            entity.Property(e => e.Features).HasColumnName("features");
            entity.Property(e => e.PlanName)
                .HasMaxLength(100)
                .HasColumnName("plan_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCA5DCEF1A");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__72E12F1BA07EDDC9").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__85C600AF2E4EF8AE");

            entity.HasIndex(e => new { e.UserId, e.CategoryId, e.TransactionDate }, "IX_Trans_UserCatDate").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.UserId, e.TransactionDate }, "IX_Trans_UserDate").IsDescending(false, true);

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CategoryId)
                .HasDefaultValue(1)
                .HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.TransactionDate).HasColumnName("transaction_date");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .HasColumnName("transaction_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__categ__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__user___5165187F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F20B790B8");

            entity.HasIndex(e => e.RoleId, "IX_Users_Role");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616471F9C307").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(200)
                .HasColumnName("avatar");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__3C69FB99");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__User_Sub__863A7EC10C59868C");

            entity.ToTable("User_Subscriptions");

            entity.HasIndex(e => e.UserId, "IX_Sub_User");

            entity.HasIndex(e => new { e.UserId, e.Status }, "UX_Sub_User_Active")
                .IsUnique()
                .HasFilter("([status]='active')");

            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Subs__plan___4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Subs__user___412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
