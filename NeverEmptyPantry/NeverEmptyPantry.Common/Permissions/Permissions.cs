namespace NeverEmptyPantry.Common.Permissions
{
    public static class Permissions
    {
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }
    }

    public static class DefaultRoles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
    }
}