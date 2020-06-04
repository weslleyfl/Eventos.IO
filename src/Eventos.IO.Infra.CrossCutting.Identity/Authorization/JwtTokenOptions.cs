using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Eventos.IO.Infra.CrossCutting.Identity.Authorization
{
    /// <summary>
    ///  Options Pattern
    /// </summary>
    public class JwtTokenOptions
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public DateTime NotBefore { get; set; } = DateTime.UtcNow;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromHours(2);

        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());

        public SigningCredentials SigningCredentials { get; set; }

        public bool Authenticated { get; set; }
        public string AccessToken { get; set; }
        public int Seconds { get; set; }
    }
}