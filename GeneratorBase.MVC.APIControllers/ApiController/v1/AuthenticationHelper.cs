using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GeneratorBase.MVC.ApiControllers.v1
{
/// <summary>An authentication helper.</summary>
public class AuthenticationHelper
{
    /// <summary>The user database.</summary>
    ApplicationDbContext UserDb;
    /// <summary>The database.</summary>
    ApplicationContext db;
    /// <summary>True to apply security policy.</summary>
    private bool applySecurityPolicy;
    /// <summary>The um.</summary>
    private UserManager<ApplicationUser> um;
    /// <summary>The user.</summary>
    private ApplicationUser user;
    /// <summary>A token that allows processing to be cancelled.</summary>
    private CancellationToken cancellationToken;
    
    /// <summary>Gets the manager for user.</summary>
    ///
    /// <value>The user manager.</value>
    
    public UserManager<ApplicationUser> UserManager
    {
        get
        {
            return this.um;
        }
    }
    
    /// <summary>Gets the user.</summary>
    ///
    /// <value>The user.</value>
    
    public ApplicationUser User
    {
        get
        {
            return this.user;
        }
    }
    
    #region Initialize
    
    /// <summary>Creates a new Task&lt;AuthenticationHelper&gt;</summary>
    ///
    /// <param name="userName">         Name of the user.</param>
    /// <param name="cancellationToken">    (Optional) A token that allows processing to be cancelled.</param>
    ///
    /// <returns>An asynchronous result that yields an AuthenticationHelper.</returns>
    
    public static async Task<AuthenticationHelper> Create(string userName, CancellationToken cancellationToken = default(CancellationToken))
    {
        ApplicationDbContext UserDb = new ApplicationDbContext();
        ApplicationContext db = new ApplicationContext(new SystemUser());
        var helper = new AuthenticationHelper(UserDb, db);
        await helper.Initialize(userName, cancellationToken);
        return helper;
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="userDb">The user database.</param>
    /// <param name="db">    The database.</param>
    
    private AuthenticationHelper(ApplicationDbContext userDb, ApplicationContext db)
    {
        this.UserDb = userDb;
        this.db = db;
    }
    
    /// <summary>Initializes this object.</summary>
    ///
    /// <param name="userName">         Name of the user.</param>
    /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
    ///
    /// <returns>An asynchronous result.</returns>
    
    private async Task Initialize(string userName, CancellationToken cancellationToken)
    {
        // order is important
        this.cancellationToken = cancellationToken;
        await SetApplySecurityPolicy();
        await CreateUserManager();
        await SetCurrentUserByName(userName);
    }
    
    /// <summary>Sets apply security policy.</summary>
    ///
    /// <returns>An asynchronous result.</returns>
    
    private async Task SetApplySecurityPolicy()
    {
        var applySecurityPolicySetting =
            await this.db.AppSettings
            .Where(p => p.Key == "ApplySecurityPolicy")
            .FirstOrDefaultAsync(this.cancellationToken);
        // defaults to false
        this.applySecurityPolicy = applySecurityPolicySetting != null &&
                                   applySecurityPolicySetting.Value.ToLower() == "yes";
    }
    
    /// <summary>Creates user manager.</summary>
    ///
    /// <returns>An asynchronous result.</returns>
    
    private async Task CreateUserManager()
    {
        //var dataProtectionProvider = Startup.DataProtectionProvider;
        //var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext(user)));
        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.UserDb));
        //userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
        var dataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("AppName");
        userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
        userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
        {
            AllowOnlyAlphanumericUserNames = false
        };
        userManager.UserLockoutEnabledByDefault = true;
        if(this.applySecurityPolicy)
        {
            var settingKeys = new string[]
            {
                "DefaultAccountLockoutTimeSpan",
                "MaxFailedAccessAttemptsBeforeLockout",
                "PasswordMinimumLength",
                "PasswordRequireSpecialCharacter",
                "PasswordRequireDigit",
                "PasswordRequireLowerCase",
                "PasswordRequireUpperCase"
            };
            var settings = await this.db.AppSettings.Where(p => settingKeys.Contains(p.Key)).ToDictionaryAsync(p => p.Key, this.cancellationToken);
            Func<string, string, string> get = (s, d) => settings.ContainsKey(s) ? settings[s].Value : d;
            Func<string, bool, bool> getBooleanFromYes = (s, d) => settings.ContainsKey(s) ? "yes".Equals(settings[s].Value, StringComparison.OrdinalIgnoreCase) : d;
            userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(Double.Parse(get("DefaultAccountLockoutTimeSpan", "1")));
            userManager.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(get("MaxFailedAccessAttemptsBeforeLockout", "3"));
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = Convert.ToInt32(get("PasswordMinimumLength", "12")),
                RequireNonLetterOrDigit = getBooleanFromYes("PasswordRequireSpecialCharacter", true),
                RequireDigit = getBooleanFromYes("PasswordRequireDigit", true),
                RequireLowercase = getBooleanFromYes("PasswordRequireLowerCase", true),
                RequireUppercase = getBooleanFromYes("PasswordRequireUpperCase", true),
            };
        }
        this.um = userManager;
    }
    
    /// <summary>Sets current user by name.</summary>
    ///
    /// <param name="userName">Name of the user.</param>
    ///
    /// <returns>An asynchronous result.</returns>
    
    private async Task SetCurrentUserByName(string userName)
    {
        this.user = await this.um.FindByNameAsync(userName);
    }
    
    #endregion
    
    #region Checks
    
    /// <summary>Has user.</summary>
    ///
    /// <returns>An AuthenticationResult.</returns>
    
    public AuthenticationResult HasUser()
    {
        return this.user != null ?
               AuthenticationResult.Good
               : AuthenticationResult.Bad("invalid");
    }
    
    /// <summary>Not locked out.</summary>
    ///
    /// <returns>An AuthenticationResult.</returns>
    
    public AuthenticationResult NotLockedOut()
    {
        if(this.user == null) return AuthenticationResult.Bad();
        var userLockedOut = this.user.LockoutEnabled && this.user.LockoutEndDateUtc > DateTime.UtcNow;
        if(userLockedOut)
        {
            var value = this.applySecurityPolicy ? (DateTime?)DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc) : null;
            return AuthenticationResult.Bad("locked", value);
        }
        return AuthenticationResult.Good;
    }
    
    /// <summary>Password is correct or increment asynchronous.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>An asynchronous result that yields the password is correct or increment.</returns>
    
    public async Task<AuthenticationResult> PasswordIsCorrectOrIncrementAsync(string password)
    {
        if(this.user == null) return AuthenticationResult.Bad();
        if(await this.um.CheckPasswordAsync(this.user, password)) return AuthenticationResult.Good;
        if(!this.applySecurityPolicy) return AuthenticationResult.Bad("invalid");
        var result = await this.um.AccessFailedAsync(this.user.Id);
        if(!result.Succeeded) throw new Exception("Couldn't upate user.");
        return AuthenticationResult.Bad("invalid");
    }
    
    /// <summary>Password has not expired asynchronous.</summary>
    ///
    /// <returns>An asynchronous result that yields the password has not expired.</returns>
    
    public async Task<AuthenticationResult> PasswordHasNotExpiredAsync()
    {
        if(this.user == null) return AuthenticationResult.Bad();
        if(!this.applySecurityPolicy) return AuthenticationResult.Good;
        var passwordExpirationDaysSetting =
            await db.AppSettings
            .Where(p => p.Key == "PasswordExpirationInDays")
            .FirstOrDefaultAsync(this.cancellationToken);
        var passwordExpirationDays = Convert.ToInt32(passwordExpirationDaysSetting != null ? passwordExpirationDaysSetting.Value : "30");
        var lastPasswordChange =
            await UserDb.PasswordHistorys
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.Date)
            .FirstOrDefaultAsync(this.cancellationToken);
        // this will allow a user through if they have no password history... (from BaseController.cs)
        if(lastPasswordChange != null && lastPasswordChange.Date.AddDays(passwordExpirationDays) < DateTime.UtcNow)
        {
            return AuthenticationResult.Bad("expired");
        }
        return AuthenticationResult.Good;
    }
    
    /// <summary>Creates a new password is valid.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>An asynchronous result that yields an AuthenticationResult.</returns>
    
    public async Task<AuthenticationResult> NewPasswordIsValid(string password)
    {
        var validationResults = await this.um.PasswordValidator.ValidateAsync(password);
        if(!validationResults.Succeeded)
        {
            return AuthenticationResult.Bad("badPassword", validationResults.Errors);
        }
        return AuthenticationResult.Good;
    }
    
    /// <summary>Password not recently used.</summary>
    ///
    /// <param name="password">The password.</param>
    ///
    /// <returns>An asynchronous result that yields an AuthenticationResult.</returns>
    
    public async Task<AuthenticationResult> PasswordNotRecentlyUsed(string password)
    {
        if(this.user == null) return AuthenticationResult.Bad();
        var passwordRecentlyUsedCountSetting = await db.AppSettings.Where(p => p.Key == "OldPasswordGenerationCount").FirstOrDefaultAsync();
        var passwordRecentlyUsedCount = Convert.ToInt32(passwordRecentlyUsedCountSetting != null ? passwordRecentlyUsedCountSetting.Value : Int32.MaxValue.ToString());
        if(await PasswordUsedRecently(this.um, this.user, password, passwordRecentlyUsedCount))
        {
            return AuthenticationResult.Bad("recentlyUsed", passwordRecentlyUsedCount);
        }
        return AuthenticationResult.Good;
    }
    
    /// <summary>Password used recently.</summary>
    ///
    /// <param name="um">         The um.</param>
    /// <param name="user">       The user.</param>
    /// <param name="NewPassword">The new password.</param>
    /// <param name="count">      Number of.</param>
    ///
    /// <returns>An asynchronous result that yields true if it succeeds, false if it fails.</returns>
    
    private static async Task<bool> PasswordUsedRecently(UserManager<ApplicationUser> um, ApplicationUser user, string NewPassword, int count)
    {
        if(count <= 0) return false;
        using(ApplicationDbContext UserDb = new ApplicationDbContext())
        {
            var history = await UserDb.PasswordHistorys
                          .Where(p => p.UserId == user.Id)
                          .OrderByDescending(p => p.Date)
                          .Take(count)
                          .ToListAsync();
            foreach(var pwd in history)
            {
                var result = um.PasswordHasher.VerifyHashedPassword(pwd.HashedPassword, NewPassword);
                if(result == PasswordVerificationResult.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    #endregion
    
    #region Login and Renew
    
    /// <summary>Confirm successful login.</summary>
    ///
    /// <returns>An asynchronous result.</returns>
    
    public async Task ConfirmSuccessfulLogin()
    {
        await this.um.ResetAccessFailedCountAsync(this.user.Id);
    }
    
    /// <summary>Saves the new password in history.</summary>
    public void SaveNewPasswordInHistory()
    {
        var user = um.FindById(this.user.Id);
        PasswordHistory history = new PasswordHistory();
        history.Date = DateTime.UtcNow;
        history.UserId = user.Id;
        history.HashedPassword = user.PasswordHash;
        UserDb.PasswordHistorys.Add(history);
        UserDb.SaveChanges();
    }
    
    /// <summary>Login API asynchronous.</summary>
    ///
    /// <returns>An asynchronous result that yields the login API.</returns>
    
    public async Task<ApiToken> LoginApiAsync()
    {
        var token =
            await db.ApiTokens
            .Where(p => p.T_UsersID == user.Id && p.T_ExpiresOn >= DateTime.UtcNow)
            .FirstOrDefaultAsync(this.cancellationToken);
        if(token == null)
        {
            token = CreateToken(user.Id);
        }
        return token;
    }
    
    /// <summary>Creates a token.</summary>
    ///
    /// <param name="userId">Identifier for the user.</param>
    ///
    /// <returns>The new token.</returns>
    
    private ApiToken CreateToken(string userId)
    {
        string token = Guid.NewGuid().ToString();
        DateTime issuedOn = DateTime.UtcNow;
        DateTime expiredOn = DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
        bool neverExpireToken = ConfigurationManager.AppSettings["NeverExpireToken"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["NeverExpireToken"]);
        if(neverExpireToken)
        {
            var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userId);
            if(obj != null)
            {
                token = obj.T_AuthToken;
                var tokenModelexisting = new ApiToken
                {
                    T_UsersID = userId,
                    T_AuthToken = token,
                    T_IssuedOn = issuedOn,
                    T_ExpiresOn = expiredOn
                };
                return tokenModelexisting;
            }
        }
        var tokendomain = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        this.db.ApiTokens.Add(tokendomain);
        this.db.SaveChanges();
        var tokenModel = new ApiToken
        {
            T_UsersID = userId,
            T_AuthToken = token,
            T_IssuedOn = issuedOn,
            T_ExpiresOn = expiredOn
        };
        return tokenModel;
    }
    #endregion
    
    #region Result
    
    /// <summary>Based on IdentityResult.</summary>
    public class AuthenticationResult : AuthenticationResult<object>
    {
        /// <summary>Gets the good.</summary>
        ///
        /// <value>The good.</value>
        
        new public static AuthenticationResult Good
        {
            get
            {
                return new AuthenticationResult(true);
            }
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <returns>An AuthenticationResult.</returns>
        
        new public static AuthenticationResult Bad()
        {
            return new AuthenticationResult();
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>An AuthenticationResult.</returns>
        
        new public static AuthenticationResult Bad(string message)
        {
            return new AuthenticationResult(message);
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <param name="message">The message.</param>
        /// <param name="value">  The value.</param>
        ///
        /// <returns>An AuthenticationResult.</returns>
        
        new public static AuthenticationResult Bad(string message, object value)
        {
            return new AuthenticationResult(message, value);
        }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="good">True to good.</param>
        
        public AuthenticationResult(bool good)
            : base(good)
        { }
        
        /// <summary>Default constructor.</summary>
        public AuthenticationResult()
            : this(default(string))
        { }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="message">The message.</param>
        
        public AuthenticationResult(string message)
            : this(message, default(object))
        { }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="message">The message.</param>
        /// <param name="value">  The value.</param>
        
        public AuthenticationResult(string message, object value)
            : base(message, value)
        { }
    }
    
    /// <summary>Encapsulates the result of an authentication.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    
    public class AuthenticationResult<T>
    {
        /// <summary>Gets the good.</summary>
        ///
        /// <value>The good.</value>
        
        public static AuthenticationResult<T> Good
        {
            get
            {
                return new AuthenticationResult<T>(true);
            }
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <returns>An AuthenticationResult&lt;T&gt;</returns>
        
        public static AuthenticationResult<T> Bad()
        {
            return new AuthenticationResult<T>();
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>An AuthenticationResult&lt;T&gt;</returns>
        
        public static AuthenticationResult<T> Bad(string message)
        {
            return new AuthenticationResult<T>(message);
        }
        
        /// <summary>Bads.</summary>
        ///
        /// <param name="message">The message.</param>
        /// <param name="value">  The value.</param>
        ///
        /// <returns>An AuthenticationResult&lt;T&gt;</returns>
        
        public static AuthenticationResult<T> Bad(string message, T value)
        {
            return new AuthenticationResult<T>(message, value);
        }
        
        /// <summary>Gets or sets a value indicating whether this object is good.</summary>
        ///
        /// <value>True if this object is good, false if not.</value>
        
        public bool IsGood
        {
            get;
            private set;
        }
        
        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
        
        public string Message
        {
            get;
            private set;
        }
        
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        
        public T Value
        {
            get;
            private set;
        }
        
        /// <summary>Implicit cast that converts the given AuthenticationResult&lt;T&gt; to a bool.</summary>
        ///
        /// <param name="result">The result.</param>
        ///
        /// <returns>The result of the operation.</returns>
        
        public static implicit operator bool(AuthenticationResult<T> result)
        {
            return !object.ReferenceEquals(result, null) && result.IsGood;
        }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="good">True to good.</param>
        
        public AuthenticationResult(bool good)
        {
            this.IsGood = good;
        }
        
        /// <summary>Default constructor.</summary>
        public AuthenticationResult()
            : this(default(string), default(T))
        { }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="message">The message.</param>
        
        public AuthenticationResult(string message)
            : this(message, default(T))
        { }
        
        /// <summary>Constructor.</summary>
        ///
        /// <param name="message">The message.</param>
        /// <param name="value">  The value.</param>
        
        public AuthenticationResult(string message, T value)
        {
            this.IsGood = false;
            this.Message = message;
            this.Value = value;
        }
    }
    
    #endregion
}
}