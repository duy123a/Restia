using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Restia.Common.Localization.Resources;

namespace Restia.Common.Identity.Configurations
{
    public class AppErrorDescriber(IStringLocalizer<SharedResources> localizer) : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = localizer["AppErrorDescriber_DuplicateUserName"]
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = localizer["AppErrorDescriber_DuplicateEmail"]
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = localizer["AppErrorDescriber_DuplicateRoleName"]
            };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = localizer["AppErrorDescriber_InvalidEmail"]
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = localizer["AppErrorDescriber_PasswordTooShort", length]
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = localizer["AppErrorDescriber_PasswordRequiresDigit"]
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = localizer["AppErrorDescriber_PasswordRequiresLower"]
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = localizer["AppErrorDescriber_PasswordRequiresUpper"]
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = localizer["AppErrorDescriber_PasswordRequiresNonAlphanumeric"]
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = localizer["AppErrorDescriber_PasswordRequiresUniqueChars", uniqueChars]
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = localizer["AppErrorDescriber_InvalidToken"]
            };
        }
    }
}
