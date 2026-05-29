public record Error(string Code,string Descritpion, int? statuscode)
{
    public static readonly Error None= new Error(string.Empty,string.Empty, null);
}


