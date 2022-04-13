namespace AspNetCore.JWTDemo.EntityFrameworkCore.Permissions
{
    public class PolicyDefinitions
    {
        public const string PermissionPrefix = "Permission";

        public const string RBAC = $"{PermissionPrefix}.RBAC";
        public const string OWNER_ONLY = $"{PermissionPrefix}.OwnerOnly";
    }

    public class PolicyHelper
    {
        public static bool TryParseRBAC(string policyName, out Resource resource, out Operation operation)
        {
            var args = policyName.Split('.');
            if (args.Length >= 3)
            {
                if (Enum.TryParse(args[2], out resource) && int.TryParse(args[3], out var operationValue))
                {
                    operation = (Operation)operationValue;
                    return true;
                }
            }
            resource = default;
            operation = default;
            return false;
        }
    }
}
