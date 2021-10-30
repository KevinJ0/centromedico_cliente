using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services
{
    public class IdentityErrorCodes
    {
        public const string DefaultError = "DefaultError";
        public const string ConcurrencyFailure = "ConcurrencyFailure";
        public const string PasswordMismatch = "PasswordMismatch";
        public const string InvalidToken = "InvalidToken";
        public const string LoginAlreadyAssociated = "LoginAlreadyAssociated";
        public const string InvalidUserName = "InvalidUserName";
        public const string InvalidEmail = "InvalidEmail";
        public const string DuplicateUserName = "DuplicateUserName";
        public const string DuplicateEmail = "DuplicateEmail";
        public const string InvalidRoleName = "InvalidRoleName";
        public const string DuplicateRoleName = "DuplicateRoleName";
        public const string UserAlreadyHasPassword = "UserAlreadyHasPassword";
        public const string UserLockoutNotEnabled = "UserLockoutNotEnabled";
        public const string UserAlreadyInRole = "UserAlreadyInRole";
        public const string UserNotInRole = "UserNotInRole";
        public const string PasswordTooShort = "PasswordTooShort";
        public const string PasswordRequiresNonAlphanumeric = "PasswordRequiresNonAlphanumeric";
        public const string PasswordRequiresDigit = "PasswordRequiresDigit";
        public const string PasswordRequiresLower = "PasswordRequiresLower";
        public const string PasswordRequiresUpper = "PasswordRequiresUpper";
        public const string DuplicateName = "DuplicateName";
        public const string ExternalLoginExists = "ExternalLoginExists";
        public const string NoTokenProvider = "NoTokenProvider";
        public const string NoTwoFactorProvider = "NoTwoFactorProvider";
        public const string PasswordRequireDigit = "PasswordRequireDigit";
        public const string PasswordRequireLower = "PasswordRequireLower";
        public const string PasswordRequireNonLetterOrDigit = "PasswordRequireNonLetterOrDigit";
        public const string PasswordRequireUpper = "PasswordRequireUpper";
        public const string PropertyTooShort = "PropertyTooShort";
        public const string RoleNotFound = "RoleNotFound";
        public const string StoreNotIQueryableRoleStore = "StoreNotIQueryableRoleStore";
        public const string StoreNotIQueryableUserStore = "StoreNotIQueryableUserStore";
        public const string StoreNotIUserClaimStore = "StoreNotIUserClaimStore";
        public const string StoreNotIUserConfirmationStore = "StoreNotIUserConfirmationStore";
        public const string StoreNotIUserEmailStore = "StoreNotIUserEmailStore";
        public const string StoreNotIUserLockoutStore = "StoreNotIUserLockoutStore";
        public const string StoreNotIUserLoginStore = "StoreNotIUserLoginStore";
        public const string StoreNotIUserPasswordStore = "StoreNotIUserPasswordStore";
        public const string StoreNotIUserPhoneNumberStore = "StoreNotIUserPhoneNumberStore";
        public const string StoreNotIUserRoleStore = "StoreNotIUserRoleStore";
        public const string StoreNotIUserSecurityStampStore = "StoreNotIUserSecurityStampStore";
        public const string StoreNotIUserTwoFactorStore = "StoreNotIUserTwoFactorStore";

        public static string[] All = {
        DuplicateName,
        ExternalLoginExists,
        NoTokenProvider,
        NoTwoFactorProvider,
        PasswordRequireDigit,
        PasswordRequireLower,
        PasswordRequireNonLetterOrDigit,
        PasswordRequireUpper,
        PropertyTooShort,
        RoleNotFound,
        StoreNotIQueryableRoleStore,
        StoreNotIQueryableUserStore,
        StoreNotIUserClaimStore,
        StoreNotIUserConfirmationStore,
        StoreNotIUserEmailStore,
        StoreNotIUserLockoutStore,
        StoreNotIUserLoginStore,
        StoreNotIUserPasswordStore,
        StoreNotIUserPhoneNumberStore,
        StoreNotIUserRoleStore,
        StoreNotIUserSecurityStampStore,
        StoreNotIUserTwoFactorStore,
        DefaultError,
        ConcurrencyFailure,
        PasswordMismatch,
        InvalidToken,
        LoginAlreadyAssociated,
        InvalidUserName,
        InvalidEmail,
        DuplicateUserName,
        DuplicateEmail,
        InvalidRoleName,
        DuplicateRoleName,
        UserAlreadyHasPassword,
        UserLockoutNotEnabled,
        UserAlreadyInRole,
        UserNotInRole,
        PasswordTooShort,
        PasswordRequiresNonAlphanumeric,
        PasswordRequiresDigit,
        PasswordRequiresLower,
        PasswordRequiresUpper
    };
    }
}
