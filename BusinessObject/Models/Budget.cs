using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Budget
{
    public int BudgetId { get; set; }

    public int UserId { get; set; }

    public int GoalId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal AmountLimit { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Goal Goal { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
