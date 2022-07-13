using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
internal static class AsyncHelper
{
    private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
            
    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }
    
    public static void RunSync(Func<Task> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }
}
public static class UserManagerExtensions
{
    public static string GenerateShortEmailConfirmationToken<TUser, TKey>(this UserManager<TUser, TKey> manager, TKey userId)
    where TUser : class, IUser<TKey>
        where TKey : class, IEquatable<TKey>
        
    {
    
        if(manager == null)
        {
            throw new ArgumentNullException("manager");
        }
        
        return AsyncHelper.RunSync(() => manager.GenerateChangePhoneNumberTokenAsync(userId,""));
        
    }
    public static bool ConfirmShortEmail<TUser, TKey>(this UserManager<TUser, TKey> manager, TKey userId,
            string token)
    where TKey : IEquatable<TKey>
        where TUser : class, IUser<TKey>
    {
        if(manager == null)
        {
            throw new ArgumentNullException("manager");
        }
        return AsyncHelper.RunSync(() => manager.VerifyChangePhoneNumberTokenAsync(userId, token,""));
    }
    
    
}
}