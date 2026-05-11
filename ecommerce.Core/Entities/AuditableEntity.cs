using System;

namespace ecommerce.Core.Entities;

public class AuditableEntity
{
    public int? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? UpdatedById { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }

}
