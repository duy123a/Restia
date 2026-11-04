namespace Restia.Common.Permission.Constants
{
    /// <summary>
    /// Policy names: "Form:{Code}" (Default = anything but Unavailable)
    /// or "Form:{Code}:{FormAuthoritySec}" (exact authority).
    /// </summary>
    public static class PermissionConstants
    {
        public const string Prefix = "Form";

        public static class FormCodes
        {
            public const string Administrator = "Administrator";
            public const string User = "User";
            public const string Role = "Role";
            public const string Permission = "Permission";
        }

        public static class User
        {
            public const string Default = $"{Prefix}:{FormCodes.User}";
            public const string Available = $"{Default}:{nameof(FormAuthoritySec.Available)}";
            public const string ReferablePrintable = $"{Default}:{nameof(FormAuthoritySec.ReferablePrintable)}";
            public const string Unavailable = $"{Default}:{nameof(FormAuthoritySec.Unavailable)}";
        }

        public static class Role
        {
            public const string Default = $"{Prefix}:{FormCodes.Role}";
            public const string Available = $"{Default}:{nameof(FormAuthoritySec.Available)}";
            public const string ReferablePrintable = $"{Default}:{nameof(FormAuthoritySec.ReferablePrintable)}";
            public const string Unavailable = $"{Default}:{nameof(FormAuthoritySec.Unavailable)}";
        }

        public static class Permission
        {
            public const string Default = $"{Prefix}:{FormCodes.Permission}";
            public const string Available = $"{Default}:{nameof(FormAuthoritySec.Available)}";
            public const string ReferablePrintable = $"{Default}:{nameof(FormAuthoritySec.ReferablePrintable)}";
            public const string Unavailable = $"{Default}:{nameof(FormAuthoritySec.Unavailable)}";
        }

        public static string ForForm(string formCode) => $"{Prefix}:{formCode}";
    }
}
