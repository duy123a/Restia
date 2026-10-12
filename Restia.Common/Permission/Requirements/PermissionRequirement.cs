using Microsoft.AspNetCore.Authorization;
using Restia.Common.Permission.Constants;

namespace Restia.Common.Permission.Requirements
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public const string SEPARATOR = ":";
        public string Module { get; }
        public string Screen { get; }

        /// <summary> Null = controller level check </summary>
        public FormAuthoritySec? FormAuthoritySec { get; }

        public PermissionRequirement(string policyName)
        {
            var parts = policyName.Split(SEPARATOR);

            Module = parts[0];
            Screen = parts[1];

            // Controller case
            if (parts.Length == 2)
            {
                FormAuthoritySec = null;
            }
            else if (Enum.TryParse<Constants.FormAuthoritySec>(
                 parts[2], out var authority)
             && Enum.IsDefined(typeof(FormAuthoritySec), authority))
            {
                FormAuthoritySec = authority;
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid form authority: {parts[2]}",
                    nameof(policyName));
            }
        }
    }
}
