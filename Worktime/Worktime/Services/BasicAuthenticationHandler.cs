using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Worktime.Models;

namespace Worktime.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly WorktimeDbContext _db;
        public BasicAuthenticationHandler(
          IOptionsMonitor<AuthenticationSchemeOptions> options,
          ILoggerFactory logger,
          UrlEncoder encoder,
          ISystemClock clock, WorktimeDbContext dbContext)
          : base(options, logger, encoder, clock)
        {
            _db = dbContext;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                // Verify username and password against your data source.
                // If authentication is successful, set the ClaimsPrincipal to indicate success.
                // Otherwise, return AuthenticateResult.Fail with appropriate error message.

                foreach (var item in _db.Users)
                {
                    if(item.Login == username && item.Mdp == password)
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, username) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            return AuthenticateResult.Fail("");
        }
    }
}
