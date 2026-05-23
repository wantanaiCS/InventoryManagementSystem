namespace InventoryManagementSystem.Helpers
{
    public static class HttpContextSessionExtensions
    {
        public static int? GetCurrentUserId(this ISession session)
        {
            var value = session.GetString(SessionKeys.UserId);
            return int.TryParse(value, out var id) ? id : null;
        }

        public static string? GetCurrentUserRole(this ISession session)
            => session.GetString(SessionKeys.UserRole);

        public static bool IsAdmin(this ISession session)
            => string.Equals(session.GetCurrentUserRole(), "Admin", StringComparison.OrdinalIgnoreCase);
    }
}
