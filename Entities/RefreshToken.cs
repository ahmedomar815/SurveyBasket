

namespace SurveyBasket.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string Token {get;set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }= DateTime.UtcNow;

        public DateTime? RovkedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => !IsExpired && RovkedOn == null;
    }
}
