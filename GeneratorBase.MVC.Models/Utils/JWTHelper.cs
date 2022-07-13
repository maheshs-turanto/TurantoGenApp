using System;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using GeneratorBase.MVC.Models;
using System.Configuration;
using System.Security.Cryptography;
//using System.IdentityModel.Tokens;


namespace GeneratorBase.MVC
{
/// <summary>Helper class for JWT authentication.</summary>
public class JWTHelper
{
    /// <summary>
    /// Get JWT token
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Token string</returns>
    public string GetJWToken(ApplicationUser user)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        RsaKeyGenerationResult rsa = new RsaKeyGenerationResult();
        // Get Current Date Time for expiry Date
        var currentDT = System.DateTime.Now;
        // Creating Token Description part
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserName", user.UserName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(rsa.TimeOut),
            Issuer = rsa.IssuerName,
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa.PrivateKeyProvider), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature)
        };
        // Create Token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // convert Token to string
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }
    /// <summary>
    /// Verify JWT token
    /// </summary>
    /// <param name="token"></param>
    /// <returns>True if validated</returns>
    public bool VerifyJWT(string token)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        RsaKeyGenerationResult rsa = new RsaKeyGenerationResult();
        if(string.IsNullOrEmpty(rsa.PublicKeyOnly) || rsa.KeySize <= 0) return false;
        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa.PublicKeyProvider),// new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey)),//new System.IdentityModel.Tokens.RsaSecurityToken(publicOnly),//
            ValidIssuer = rsa.IssuerName, // same as Token Issuer Name
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
        };
        var algorithm = ((System.Security.Cryptography.AsymmetricAlgorithm)(((Microsoft.IdentityModel.Tokens.RsaSecurityKey)(validationParameters.IssuerSigningKey)).Rsa)).SignatureAlgorithm;
        if(algorithm != rsa.SecurityAlgorithm) return false;
        Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch(Exception ex)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Get user info from JWT token
    /// </summary>
    /// <param name="token"></param>
    /// <returns>The ClaimsPrincipal</returns>
    public ClaimsPrincipal GetJWTClaims(string token)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        RsaKeyGenerationResult rsa = new RsaKeyGenerationResult();
        // validation parameters, JWtValidator requires this object to validate the token.
        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa.PublicKeyProvider),// new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey)),//new System.IdentityModel.Tokens.RsaSecurityToken(publicOnly),//
            ValidIssuer = rsa.IssuerName, // same as Token Issuer Name
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
        };
        Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
        try
        {
            // if token is valid, it will output the validated token that contains the JWT information
            // otherwise it will throw an exception
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return principal;
        }
        catch(Exception ex)
        {
            return null;
        }
        return null;
    }
}
public class RsaKeyGenerationResult
{
    public string PublicKeyOnly
    {
        get;
        set;
    }
    public string PublicAndPrivateKey
    {
        get;
        set;
    }
    public RSACryptoServiceProvider PrivateKeyProvider
    {
        get;
        set;
    }
    public RSACryptoServiceProvider PublicKeyProvider
    {
        get;
        set;
    }
    public string SecurityAlgorithm
    {
        get;
        set;
    }
    public int KeySize
    {
        get;
        set;
    }
    public string IssuerName
    {
        get;
        set;
    }
    public int TimeOut
    {
        get;    //in minutes
        set;
    }
    //public RsaKeyGenerationResult()
    //{
    //    this.KeySize = 2048;
    //    RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider(this.KeySize);
    //    RSAParameters publicKey = myRSA.ExportParameters(true);
    //    this.PublicAndPrivateKey = myRSA.ToXmlString(true);
    //    this.PublicKeyOnly = myRSA.ToXmlString(false);
    //}
    public RsaKeyGenerationResult(string publicandprivatekey, string publickeyonly, int keysize)
    {
        this.KeySize = keysize;
        this.PrivateKeyProvider = new RSACryptoServiceProvider(this.KeySize);
        this.PublicKeyProvider = new RSACryptoServiceProvider(this.KeySize);
        this.PublicAndPrivateKey = publicandprivatekey;
        this.PublicKeyOnly = publickeyonly;
        PrivateKeyProvider.FromXmlString(publicandprivatekey);
        PublicKeyProvider.FromXmlString(publickeyonly);
        this.IssuerName = "[Turanto]";
        this.TimeOut = 1440;
    }
    public RsaKeyGenerationResult()
    {
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        this.SecurityAlgorithm = commonObj.ExternalSecurityAlgorithm(); // "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        this.KeySize = 2048;
        this.PrivateKeyProvider = new RSACryptoServiceProvider(this.KeySize);
        this.PublicKeyProvider = new RSACryptoServiceProvider(Convert.ToInt32(commonObj.ExternalValidationKeySize()));
        this.PublicKeyOnly = commonObj.ExternalValidationKey();// "<RSAKeyValue><Modulus>3i8mxDkPBgCPxaR0skZHiIaYHl/e2G+Fuc2vpRHL77q7qSghUrOM5JYHjCAKiJyAQtzLtnMQ4Nk25MqoDQEPrIcB2vkfAFyWvZjKPyj2ELt7D7O3SsZfVI9h2z6NDgufPM20/n0TgrSkC+kiQNNmRNYuAUigz0KjOjR7UCX4WQE6Qb/INepZZ2dzTtE10sg+S/bcwTmspnzSQ6+/o+G4hzTgiGm8cZ8Cr7fIMr+7TxqSmJ5bF132bQ/23XkUzqyAVjbP/CktCsuVZ31hra4DSvyuX42Hf4TgEETG5Ne7aEycuJ5wt0Er49vWtvgd4MzrQ877KwqK+qHcn20zbSQ/BQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        this.PublicAndPrivateKey = "<RSAKeyValue><Modulus>3i8mxDkPBgCPxaR0skZHiIaYHl/e2G+Fuc2vpRHL77q7qSghUrOM5JYHjCAKiJyAQtzLtnMQ4Nk25MqoDQEPrIcB2vkfAFyWvZjKPyj2ELt7D7O3SsZfVI9h2z6NDgufPM20/n0TgrSkC+kiQNNmRNYuAUigz0KjOjR7UCX4WQE6Qb/INepZZ2dzTtE10sg+S/bcwTmspnzSQ6+/o+G4hzTgiGm8cZ8Cr7fIMr+7TxqSmJ5bF132bQ/23XkUzqyAVjbP/CktCsuVZ31hra4DSvyuX42Hf4TgEETG5Ne7aEycuJ5wt0Er49vWtvgd4MzrQ877KwqK+qHcn20zbSQ/BQ==</Modulus><Exponent>AQAB</Exponent><P>3we2/OjvzJrjO/WXQ4lQURSsIVw289EGRmSUhRoP+m2V/Xp4iQPBCtHdEEOkFxU8hK5ZAG0naiSPMA2gGkOBC47Box4eIzDwf/qgk70rD+xct36pnQc2RfqhyCQiGBPqP5da96qltk/zQRuz5mVEIl0Bt787TrqXLGDlASzdFdE=</P><Q>/wdsOErfo3wh5Bca/4O4NVSBF3ewjHB85n6vlCG6CuE0buhlwDgoccOQM7aLVFOlsscxhUpaUSVJCOIzmbFCiGkhk92GoQrZUNVZkoyI2xJz+EQ1o45IZrgfER4sF3068iMnpeyHeuFq4uCvNv4jkK75oJeXFMc4hfTYWpiZ/vU=</Q><DP>Qq//2yebbEZz453PmPYZ+eSAg4kbNVQu2CWC8zmTxYG285AHMpDYy+9sdkyDaBOFgPbQzfvVaTt9RRUN5kyA7X/GDowW3tbnUp0SHprVoXE/V+6bsMRTSiFi9oE3YJOz20faf2ubGSRWzAUIib+F2/CLbvHTPfs08KQYVX2grcE=</DP><DQ>kcBdQiLQg2+7Ms9dt7BBBAGisZXPCcR0LYszr3ZDDFq/C6+4D05JFxOMg3GNWYlgxrb+/02KJpwnjHMyyGC9RJbJAOf3PGyl3IPCaHX29TUroDTjbvEvQb1yy+axjk8c9CvRSgvPNhREcRu2J7jA/LxzoCtixhZBoJVZXRWf1U0=</DQ><InverseQ>xIN48IVT7cKmpx7QrPCRjpxrfR9JBcHzscf8IIriSsUQJ7exT8bx9gsPdhWx2tm61xwLZW9cyDN4XPjLyv3ImN0UiCIKWT+L6bMVwekf38cEvFVIiqOEBbZjesEUrAwmBOoVZ1L/GFD03GBSP6mR9No6gvZ8aD2ojBlS3CSPw1M=</InverseQ><D>CJKdK6O1TAVS3CIKS/lLjUhUL5KCEKvM5xcOmI86j9dj95TleZa/8EScVJsajPGCPrzc3Kn36Wpizypv7YCFpaHX6nuuPIoArvQUPvefoZRPnT177FYBxkt5+nGhaPyoejodqla9HU/xpxv/BwKhHDV/X5ajzcYZHN0lh47Pfu+WE8XYI5P86JhtSfcpEC7jiMvq7UnnDfEBS9pwzey2142+ntbl7RlB7AXFWuwGiTAneWUB2J8GER0iJjyOo28fSkxuHNz/BkMKiNC2xpbBv+lP+6T7INB4buVGPVKSo2Hmo5it6WZGnVhy6va7fBsfCNhmR0tGCp1hUFKJPvOpgQ==</D></RSAKeyValue>";
        PrivateKeyProvider.FromXmlString(this.PublicAndPrivateKey);
        PublicKeyProvider.FromXmlString(this.PublicKeyOnly);
        this.IssuerName = commonObj.ExternalIssuerName();
        this.TimeOut = 1440;
    }
}
}