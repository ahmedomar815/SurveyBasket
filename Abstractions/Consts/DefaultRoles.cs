using MimeKit.Cryptography;

namespace SurveyBasket.Abstractions.Consts;

public static class DefaultRoles
{
    public const string Admin =nameof(Admin);
    public const string AdminRoleId = "019f016b-5d2c-7838-8817-b9bda94e8ded";
    public const string AdminRoleConcurrencyStamp = "019f01db-ea48-73e7-8e51-4738f9a74412";
    public const string Member=nameof(Member);
    public const string MemberRoleId = "019f016b-5d2c-7838-8817-b9bf6308b890";
    public const string MemberRoleConcurrencyStamp = "019f01db-ea48-73e7-8e51-473dfbc6bcd6";

}
