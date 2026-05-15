public record Error(string Code,string Descritpion)
{
    public static readonly Error None= new Error(string.Empty,string.Empty);
}


