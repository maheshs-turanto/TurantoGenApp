using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using System.Reflection;
using System.Linq;
using System;
[assembly: OwinStartupAttribute(typeof(GeneratorBase.MVC.Startup))]
namespace GeneratorBase.MVC
{
public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
		//mahesh test ggggg
        var allDbContextsTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == (typeof(DbContext))).ToList();
        foreach(Type dbContextType in allDbContextsTypes)
        {
            MethodInfo initializerMethod = typeof(Database).GetMethod("SetInitializer");
            MethodInfo dbContextInitializerMethod = initializerMethod.MakeGenericMethod(dbContextType);
            dbContextInitializerMethod.Invoke(null, new object[] { null });
        }
        ConfigureAuth(app);
    }
}
}