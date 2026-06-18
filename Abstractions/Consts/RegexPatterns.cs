namespace SurveyBasket.Abstractions.Consts
{
    public static class RegexPatterns
    {
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";
    }
}
