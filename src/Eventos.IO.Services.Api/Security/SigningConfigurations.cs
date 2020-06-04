using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Security
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }
              

        public SigningConfigurations(string secretKey)
        {
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
            Key = new SymmetricSecurityKey(keyByteArray);
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        }

        //public SigningConfigurations()
        //{
        //    using (var provider = new RSACryptoServiceProvider(2048))
        //    {
        //        Key = new RsaSecurityKey(provider.ExportParameters(true));
        //    }

        //    SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        //}
    }
}
