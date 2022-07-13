using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>A third party login.</summary>
public class ThirdPartyLogin
{
    /// <summary>Default constructor.</summary>
    public ThirdPartyLogin()
    {
        this.GooglePlusId = "";
        this.GooglePlusSecretKey = "";
        this.FacebookId = "";
        this.FacebookSecretKey = "";
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="goolgleplusid">     The goolgleplusid.</param>
    /// <param name="googeplussecretkey">The googeplussecretkey.</param>
    /// <param name="facebookid">        The identifier of the facebook.</param>
    /// <param name="facebooksecretkey"> The facebook secret key.</param>
    
    public ThirdPartyLogin(string goolgleplusid, string googeplussecretkey, string facebookid, string facebooksecretkey)
    {
        this.GooglePlusId = goolgleplusid;
        this.GooglePlusSecretKey = googeplussecretkey;
        this.FacebookId = facebookid;
        this.FacebookSecretKey = facebooksecretkey;
    }
    
    /// <summary>Gets or sets the identifier of the google plus.</summary>
    ///
    /// <value>The identifier of the google plus.</value>
    
    [DisplayName("GooglePlus Id")]
    public string GooglePlusId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the google plus secret key.</summary>
    ///
    /// <value>The google plus secret key.</value>
    
    [DisplayName("GooglePlus SecretKey")]
    public string GooglePlusSecretKey
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the facebook.</summary>
    ///
    /// <value>The identifier of the facebook.</value>
    
    [DisplayName("Facebook Id")]
    public string FacebookId
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the facebook secret key.</summary>
    ///
    /// <value>The facebook secret key.</value>
    
    [DisplayName("Facebook SecretKey")]
    public string FacebookSecretKey
    {
        get;
        set;
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
/// <summary>Interface for third party login repository.</summary>
public interface IThirdPartyLoginRepository
{
    /// <summary>Edit third party login.</summary>
    ///
    /// <param name="cp">The cp.</param>
    
    void EditThirdPartyLogin(ThirdPartyLogin cp);
    
    /// <summary>Gets third party login.</summary>
    ///
    /// <returns>The third party login.</returns>
    
    ThirdPartyLogin GetThirdPartyLogin();
}
}