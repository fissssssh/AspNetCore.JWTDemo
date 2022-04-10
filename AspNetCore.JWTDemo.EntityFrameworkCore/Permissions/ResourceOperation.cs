namespace AspNetCore.JWTDemo.EntityFrameworkCore.Permissions
{
    [Flags]
    public enum ResourceOperation
    {
        None = 0,
        List = 1,
        Create = 2,
        Update = 4,
        Delete = 8,
        ReadOnly = List,
        WriteOnly = Create | Update | Delete,
        ReadWrite = ReadOnly | WriteOnly
    }
}
