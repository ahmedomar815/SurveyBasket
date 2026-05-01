using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace SurveyBasket.Entities
{
    public sealed class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }
}
