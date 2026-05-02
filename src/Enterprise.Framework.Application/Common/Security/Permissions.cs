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

    public static class Employees
    {
        public const string Module = "Çalışan Yönetimi";

        public const string EmployeesView = "Employees.View";
        public const string EmployeesWrite = "Employees.Write";
    }

    public static class Assets
    {
        public const string Module = "Varlık Yönetimi";

        public const string AssetsView = "Assets.View";
        public const string AssetsWrite = "Assets.Write";
    }

    public static class Maintenances
    {
        public const string Module = "Bakım Yönetimi";

        public const string MaintenancesView = "Maintenances.View";
        public const string MaintenancesWrite = "Maintenances.Write";
    }

    public static class Labors
    {
        public const string Module = "İşçilik Yönetimi";

        public const string LaborsView = "Labors.View";
        public const string LaborsWrite = "Labors.Write";
    }

    public static class Documents
    {
        public const string Module = "Doküman Yönetimi";

        public const string DocumentsView = "Documents.View";
        public const string DocumentsWrite = "Documents.Write";
    }

    public static class Settings
    {
        public const string Module = "Ayarlar";

        public const string SystemView = "Settings.System.View";
        public const string SystemWrite = "Settings.System.Write";
    }
}

