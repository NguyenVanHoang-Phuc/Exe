using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Goal
{
    public int GoalId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
