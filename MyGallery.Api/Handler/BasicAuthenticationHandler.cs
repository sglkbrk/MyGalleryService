using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Authorization Başlığı Eksik"));

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var authHeaderVal = authHeader.Split(' ')[1];
            var authBytes = Convert.FromBase64String(authHeaderVal);
            var credentials = Encoding.UTF8.GetString(authBytes).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            // Gerçek doğrulama işleminizi buraya ekleyin
            var validUsername = _configuration["Authentication:Username"];
            var validPassword = _configuration["Authentication:Password"];

            if (username == validUsername && password == validPassword) // Gerçek doğrulama yerine geçer
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Geçersiz Kullanıcı Adı veya Şifre"));
            }
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Geçersiz Authorization Başlığı"));
        }
    }
}
