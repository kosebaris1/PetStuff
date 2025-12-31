using Microsoft.AspNetCore.Http;

namespace PetStuff.Web.Helpers
{
    public static class SessionHelper
    {
        private const string TokenKey = "AccessToken";

        public static void SetToken(ISession session, string token)
        {
            session.SetString(TokenKey, token);
        }

        public static string? GetToken(ISession session)
        {
            // Önce session'dan dene, yoksa null dön (JavaScript localStorage'dan okuyacak)
            return session.GetString(TokenKey);
        }

        public static void RemoveToken(ISession session)
        {
            session.Remove(TokenKey);
        }

        public static bool IsAuthenticated(ISession session)
        {
            return !string.IsNullOrEmpty(GetToken(session));
        }
    }
}

