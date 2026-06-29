namespace SurveyBasket.Errors
{
    public static  class RoleErros
    {
        public static Error RoleNotFound => new Error("Role.NotFound", "The Role Not Found",StatusCodes.Status404NotFound);

        public static Error DuplicatedNameRole => new Error("Role.DuplicatedNameRole", "Another Role with same name is already exists", StatusCodes.Status409Conflict);
        public static Error InvalidPermissions => new Error("Role.InvalidPermissions", "InvalidPermissions", StatusCodes.Status400BadRequest);

       /* public static Error EmailNotConfirmed => new Error("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
     *//*   
        public static Error DulicatedConifrmedEmail => new Error("User.DulicatedConfirmdEmail", "The Email is alread confirmed", StatusCodes.Status409Conflict);*/

    }
}
