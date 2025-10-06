using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class UserSubscription
{
    public int SubscriptionId { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
