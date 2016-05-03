using Microsoft.Owin.Security.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PI.Service
{
    public class MyJwtOptions : JwtBearerAuthenticationOptions
    {
        public MyJwtOptions()
        {
            var issuer = "localhost";
            var audience = "all";
            var key = Convert.FromBase64String("UHxNtYMRYwvfpO1dS5pWLKL0M2DgOj40EbN4SoBWgfc"); ;

            AllowedAudiences = new[] { audience };
            IssuerSecurityTokenProviders = new[]
            {
            new SymmetricKeyIssuerSecurityTokenProvider(issuer, key)
        };
        }
    }
}

