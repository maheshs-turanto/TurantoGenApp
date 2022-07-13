using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Configuration;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling token services.</summary>
public class TokenServicesController : Controller
{
    #region Public member methods.
    
    /// <summary>Gets or sets the database.</summary>
    ///
    /// <value>The database.</value>
    
    public ApplicationContext db
    {
        get;    //removed static for race condition
        private set;
    }
    /// <summary>Context for the user.</summary>
    private ApplicationDbContext UserContext = new ApplicationDbContext();
    
    /// <summary>Gets or sets the user.</summary>
    ///
    /// <value>The user.</value>
    
    public new IUser User
    {
        get;    //removed static for race condition
        private set;
    }
    
    /// <summary>Function to generate unique token with expiry against the provided userId. Also add a
    /// record in database for generated token.</summary>
    ///
    /// <param name="userId">.</param>
    ///
    /// <returns>The user token.</returns>
    
    public ApiToken GenerateToken(string userId)
    {
        string token = Guid.NewGuid().ToString();
        DateTime issuedOn = DateTime.UtcNow;
        DateTime expiredOn = DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
        var tokendomain = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        db.ApiTokens.Add(tokendomain);
        db.SaveChanges();
        var tokenModel = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        return tokenModel;
    }
    
    /// <summary>Method to validate token against expiry and existence in database.</summary>
    ///
    /// <param name="tokenId">.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ValidateToken(string tokenId)
    {
        ApplicationContext db1 = new ApplicationContext(new SystemUser());
        bool neverExpireToken = ConfigurationManager.AppSettings["NeverExpireToken"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["NeverExpireToken"]);
        if(!neverExpireToken)
        {
            var token = db1.ApiTokens.Where(t => t.T_AuthToken == tokenId && t.T_ExpiresOn > DateTime.UtcNow).FirstOrDefault();
            if(token != null && !(DateTime.UtcNow > token.T_ExpiresOn))
            {
                try
                {
                    token.T_ExpiresOn = token.T_ExpiresOn.AddSeconds(
                                            Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                    db1.Entry(token).State = EntityState.Modified;
                    db1.SaveChanges();
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                return true;
            }
        }
        else
        {
            var token = db1.ApiTokens.Where(t => t.T_AuthToken == tokenId).FirstOrDefault();
            if(token != null)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>Method to kill the provided token id.</summary>
    ///
    /// <param name="tokenId">true for successful delete.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool Kill(string tokenId)
    {
        var token = db.ApiTokens.Where(t => t.T_AuthToken == tokenId && t.T_ExpiresOn > DateTime.UtcNow).FirstOrDefault();
        db.Entry(token).State = EntityState.Deleted;
        db.ApiTokens.Remove(token);
        db.SaveChanges();
        return true;
    }
    
    /// <summary>Delete tokens for the specific deleted user.</summary>
    ///
    /// <param name="userId">.</param>
    ///
    /// <returns>true for successful delete.</returns>
    
    public bool DeleteByUserId(string userId)
    {
        var token = db.ApiTokens.Where(t => t.T_UsersID == userId).FirstOrDefault();
        db.Entry(token).State = EntityState.Deleted;
        db.ApiTokens.Remove(token);
        db.SaveChanges();
        return true;
    }
    
    #endregion
}
}