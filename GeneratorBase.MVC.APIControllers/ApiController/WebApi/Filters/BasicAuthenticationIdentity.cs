using System.Security.Principal;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>Basic Authentication identity.</summary>
public class BasicAuthenticationIdentity : GenericIdentity
{
    /// <summary>Get/Set for password.</summary>
    ///
    /// <value>The password.</value>
    
    public string Password
    {
        get;
        set;
    }
    
    /// <summary>Get/Set for UserName.</summary>
    ///
    /// <value>The name of the user.</value>
    
    public string UserName
    {
        get;
        set;
    }
    
    /// <summary>Get/Set for UserId.</summary>
    ///
    /// <value>The identifier of the user.</value>
    
    public string UserId
    {
        get;
        set;
    }
    
    /// <summary>Basic Authentication Identity Constructor.</summary>
    ///
    /// <param name="userName">.</param>
    /// <param name="password">.</param>
    
    public BasicAuthenticationIdentity(string userName, string password)
        : base(userName, "Basic")
    {
        Password = password;
        UserName = userName;
    }
}
}