using System.Security.Principal;

namespace LIMS
{
    public static class PrincipalExtensions
    {
        public static bool IsAdmin(this IPrincipal principal)
        {
            return principal.IsInRole(Roles.Administrator);
        }

        public static bool IsFaculty(this IPrincipal principal)
        {
            return principal.IsInRole(Roles.Faculty);
        }

        public static bool IsPrivileged(this IPrincipal principal)
        {
            return principal.IsAdmin() || principal.IsFaculty();
        }
    }
}
