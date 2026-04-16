using System;

using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Base
{
    public class MetaFields
    {
        public string? created_by { get; set; }
        public string? updated_by { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public EntityStatus status { get; set; } = EntityStatus.Active;
    }
}
