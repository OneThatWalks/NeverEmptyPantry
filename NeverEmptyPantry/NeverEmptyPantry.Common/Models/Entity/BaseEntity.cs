using System;
using NeverEmptyPantry.Common.Interfaces.Entity;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime ModifiedDateTimeUtc { get; set; }

        public virtual void MergeProperties<T>(T updatedEntity) where T : IBaseEntity
        {
            if (updatedEntity == null)
            {
                throw new ArgumentNullException(nameof(updatedEntity));
            }

            if (updatedEntity.Id != Id)
            {
                throw new Exception("Error merging properties, entity ids do not match");
            }

            Active = updatedEntity.Active;
            ModifiedDateTimeUtc = DateTime.UtcNow;
        }
    }
}