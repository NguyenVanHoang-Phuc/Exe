using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class AiAdvice
{
    public int AdviceId { get; set; }

    public int RequestId { get; set; }

    public int UserId { get; set; }

    public int? TransactionId { get; set; }

    public string AdviceText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AiRequest Request { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual User User { get; set; } = null!;
}
