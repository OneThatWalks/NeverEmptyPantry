using System;

namespace NeverEmptyPantry.Common.Interfaces.Entity
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        bool Active { get; set; }
        DateTime CreatedDateTimeUtc { get; set; }
        DateTime ModifiedDateTimeUtc { get; set; }

        /// <summary>
        /// Merges the properties of the specified entity with this instance
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <param name="updatedEntity">The updated entity</param>
        void MergeProperties<T>(T updatedEntity) where T : IBaseEntity;
    }
}