using System;
using NeverEmptyPantry.Common.Interfaces.Entity;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class BaseEntity : IBaseEntity, IMergeableEntity<BaseEntity>
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime ModifiedDateTimeUtc { get; set; }

        public void MergeProperties(BaseEntity updatedEntity)
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