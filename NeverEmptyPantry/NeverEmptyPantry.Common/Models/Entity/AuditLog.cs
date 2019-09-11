using System;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;
using Newtonsoft.Json;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class AuditLog
    {
        public int Id { get; set; }
        public AuditAction AuditAction { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string Json { get; set; }
        public ApplicationUser User { get; set; }

        public static AuditLog From<T>(T entity, AuditAction action, ApplicationUser user)
        {
            return new AuditLog()
            {
                Id = 0,
                AuditAction = action,
                DateTimeUtc = DateTime.UtcNow,
                Json = JsonConvert.SerializeObject(entity),
                User = user
            };
        }
    }

    public enum AuditAction
    {
        CREATE = 0,
        UPDATE = 1,
        HARD_DELETE = 2,
        SOFT_DELETE = 3
    }
}