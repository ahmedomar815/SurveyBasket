using MimeKit.Cryptography;

namespace SurveyBasket.Abstractions.Consts;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Nanme = nameof(Admin);
        public const string Id = "019f016b-5d2c-7838-8817-b9bda94e8ded";
        public const string ConcurrencyStamp = "019f01db-ea48-73e7-8e51-4738f9a74412";
    }

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "019f016b-5d2c-7838-8817-b9bf6308b890";
        public const string ConcurrencyStamp = "019f01db-ea48-73e7-8e51-473dfbc6bcd6";
    }
    

}
