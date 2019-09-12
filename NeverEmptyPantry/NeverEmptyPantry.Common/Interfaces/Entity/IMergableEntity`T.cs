namespace NeverEmptyPantry.Common.Interfaces.Entity
{
    public interface IMergeableEntity<in T>
    {
        /// <summary>
        /// Merges the updated properties into the entity
        /// </summary>
        /// <param name="updatedEntity">The in memory entity to update from</param>
        /// <returns>The updated entity</returns>
        void MergeProperties(T updatedEntity);
    }
}