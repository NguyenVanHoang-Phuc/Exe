using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
