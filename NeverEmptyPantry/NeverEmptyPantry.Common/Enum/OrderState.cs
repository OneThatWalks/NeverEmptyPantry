namespace NeverEmptyPantry.Common.Enum
{
    public enum OrderState
    {
        /// <summary>
        /// List created is when a list is accepting items
        /// </summary>
        LIST_CREATED = 0,
        /// <summary>
        /// List removed is when a list has been removed/deleted
        /// </summary>
        LIST_REMOVED = 1,
        /// <summary>
        /// List pending is when a list is no longer accepting items
        /// </summary>
        LIST_PENDING = 2,
        /// <summary>
        /// List processed is when a list is processed and will be assigned a delivery date
        /// </summary>
        LIST_PROCESSED = 3,
        /// <summary>
        /// List received is when a list is marked as received and items added to inventory
        /// </summary>
        LIST_RECEIVED = 4
    }
}