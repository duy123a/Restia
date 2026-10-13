using Microsoft.AspNetCore.Identity;

namespace Restia.Common.Shared.Models
{
    public class ServiceResult
    {
        public bool Succeeded { get; private init; }

        /// <summary>Business rule error code (e.g. "NotFound", "CannotDeleteSelf", "DuplicateRoleName").</summary>
        public string? ErrorCode { get; private init; }

        /// <summary>Identity / validation error messages for display.</summary>
        public IReadOnlyList<string> Errors { get; private init; } = [];

        private ServiceResult() { }

        public static ServiceResult Success() => new() { Succeeded = true };

        public static ServiceResult Fail(string errorCode) =>
            new() { Succeeded = false, ErrorCode = errorCode };

        public static ServiceResult FromIdentityResult(IdentityResult result)
        {
            if (result.Succeeded) return Success();

            return new()
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
    }
}
