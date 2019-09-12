using System;

namespace NeverEmptyPantry.Common.Interfaces.Entity
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        bool Active { get; set; }
        DateTime CreatedDateTimeUtc { get; set; }
        DateTime ModifiedDateTimeUtc { get; set; }
    }
}