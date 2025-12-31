using System.Security.Claims;
using System.Text.Json;

namespace PetStuff.Web.Helpers
{
    public static class JwtHelper
    {
        public static IEnumerable<Claim> ParseToken(string token)
        {
            var claims = new List<Claim>();
            
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return claims;

                var payload = parts[1];
                // Base64 padding ekle
                payload = payload.Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);
                var payloadJson = JsonSerializer.Deserialize<JsonElement>(json);

                // Claims çıkar
                if (payloadJson.TryGetProperty("sub", out var sub))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.GetString() ?? ""));
                
                if (payloadJson.TryGetProperty("name", out var name))
                    claims.Add(new Claim(ClaimTypes.Name, name.GetString() ?? ""));
                
                if (payloadJson.TryGetProperty("email", out var email))
                    claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? ""));

                // Role claims
                if (payloadJson.TryGetProperty("role", out var role))
                {
                    if (role.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in role.EnumerateArray())
                        {
                            var roleValue = r.GetString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                claims.Add(new Claim(ClaimTypes.Role, roleValue));
                            }
                        }
                    }
                    else if (role.ValueKind == JsonValueKind.String)
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }
            catch
            {
                // Token parse edilemezse boş claims döndür
            }

            return claims;
        }
    }
}

