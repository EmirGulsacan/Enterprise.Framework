namespace Enterprise.Framework.Application.Common.Security;

public static class Permissions
{
    public static class Identity
    {
        public const string Module = "Kimlik Yönetimi";

        public const string UsersView = "Identity.Users.View";
        public const string UsersWrite = "Identity.Users.Write";
        public const string RolesView = "Identity.Roles.View";
        public const string RolesWrite = "Identity.Roles.Write";
    }

    public static class Organization
    {
        public const string Module = "Organizasyon";

        public const string BranchesView = "Organization.Branches.View";
        public const string BranchesWrite = "Organization.Branches.Write";
        public const string UnitsView = "Organization.Units.View";
        public const string UnitsWrite = "Organization.Units.Write";
    }

    public static class Settings
    {
        public const string Module = "Ayarlar";

        public const string SystemView = "Settings.System.View";
        public const string SystemWrite = "Settings.System.Write";
    }
}

