namespace SurveyBasket.Errors
{
    public record UserErrors
    {

        public static Error UserNotFound => new Error("User.UserNotFound", "UserNotFound", StatusCodes.Status400BadRequest);
        public static Error InvalidCredentials => new Error("User.InvalidCredentials", "Invalid email or password",StatusCodes.Status401Unauthorized);
        public static Error LockedUser => new Error("User.LockedUser", "the user is locked please contact with admin", StatusCodes.Status401Unauthorized);
        public static Error DisabledUser => new Error("User.DisabledUser", "DisabledUser please contact with admin", StatusCodes.Status401Unauthorized);
        public static Error DuplicatedEmail => new Error("User.DuplicatedEmail", "Another user with same email is already exists", StatusCodes.Status409Conflict);
        public static Error EmailNotConfirmed=> new Error("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
        public static Error InvalidCode => new Error("User.InvalidCode", "invalid code", StatusCodes.Status401Unauthorized);
        public static Error DulicatedConifrmedEmail => new Error("User.DulicatedConfirmdEmail", "The Email is alread confirmed", StatusCodes.Status409Conflict);
        public static Error InvalidRoles => new Error("User.InvalidRoles", "InvalidRoles ", StatusCodes.Status400BadRequest);

    }
}
