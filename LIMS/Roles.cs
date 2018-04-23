namespace LIMS
{
    /// <summary>
    /// The global roles for the system. See <see cref="PrincipalExtensions"/> for helper methods.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Manage users, assign members as faculty.
        /// </summary>
        public const string Administrator = "Administrator";

        /// <summary>
        /// Create and manage members of labs, assign lab managers to their labs.
        /// </summary>
        public const string Faculty = "Faculty";

        /// <summary>
        /// Either administrator or faculty.
        /// </summary>
        public const string Privileged = Administrator + "," + Faculty;
    }
}
