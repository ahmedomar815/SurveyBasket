namespace SurveyBasket.Errors
{
    public static  class UserErrors
    {
        public static Error InvalidCredentials => new Error("User.InvalidCredentials", "Invalid email or password",StatusCodes.Status401Unauthorized);
    }
}
