using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Identity;
using Newtonsoft.Json;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class AuditLog : BaseEntity
    {
        public AuditAction AuditAction { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string BeforeAuditJson { get; set; }
        public string AfterAuditJson { get; set; }
        public ApplicationUser User { get; set; }

        public static AuditLog From<T>(EntityEntry<T> entity, AuditAction action, ApplicationUser user) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new AuditLog()
            {
                Id = 0,
                AuditAction = action,
                DateTimeUtc = DateTime.UtcNow,
                BeforeAuditJson = JsonConvert.SerializeObject(entity.OriginalValues.ToObject()),
                AfterAuditJson = JsonConvert.SerializeObject(entity.CurrentValues.ToObject()),
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