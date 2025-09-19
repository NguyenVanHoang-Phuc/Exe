using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AiAdvice> AiAdvices { get; set; } = new List<AiAdvice>();

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
