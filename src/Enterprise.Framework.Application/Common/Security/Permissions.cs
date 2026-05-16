namespace Enterprise.Framework.Application.Common.Security;

public static class Permissions
{
    public static class System
    {
        public const string Module = "Sistem (Süper Yetkiler)";

        public const string SuperAdmin = "*";
    }

    public static class Identity
    {
        public const string Module = "Kimlik Yönetimi";

        public const string UsersView = "Identity.Users.View";
        public const string UsersWrite = "Identity.Users.Write";
        public const string RolesView = "Identity.Roles.View";
        public const string RolesWrite = "Identity.Roles.Write";
    }

    public static class Settings
    {
        public const string Module = "Ayarlar";

        public const string SystemView = "Settings.System.View";
        public const string SystemWrite = "Settings.System.Write";
    }
}

