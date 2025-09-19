using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class AiRequest
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    public string PromptText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AiAdvice> AiAdvices { get; set; } = new List<AiAdvice>();

    public virtual User User { get; set; } = null!;
}
