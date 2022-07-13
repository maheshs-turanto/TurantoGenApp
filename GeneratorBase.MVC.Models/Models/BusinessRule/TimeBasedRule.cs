using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GeneratorBase.MVC.Models
{
public class getEmail
{
    private ApplicationDbContext usercontext = new ApplicationDbContext(true);
    public List<ApplicationUser> getUsers(IUser user, string[] values, object Entity, string[] roles, string userName)
    {
        List<ApplicationUser> result = new List<ApplicationUser>();
        var entityType = ObjectContext.GetObjectType(Entity.GetType());
        var EntityName = entityType.Name;
        var Ent = ModelReflector.Entities.First(p => p.Name == EntityName);
        var associations = Ent.Associations;
        //ApplicationContext db = new ApplicationContext(user);
        PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        foreach(string obj in values)
        {
            var assocPropArray = obj.Split('.');
            if(obj == "Owner")
            {
                var objuser = usercontext.Users.FirstOrDefault(p => p.UserName == userName);
                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    //result += objuser != null ? objuser.Email + "," : "";
                    result.Add(objuser);
            }
            if(EntityName == assocPropArray[0])
            {
                //var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                object PropValue = Property.GetValue(Entity, null);
                var propertyFormat = Property.GetCustomAttribute(typeof(EmailAddressAttribute));
                if(PropValue != null)
                {
                    var objuser = usercontext.Users.FirstOrDefault(p => p.Id == PropValue);
                    if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    {
                        if(new EmailAddressAttribute().IsValid(objuser.Email))
                            //result += objuser != null ? objuser.Email + "," : "";
                            result.Add(objuser);
                    }
                }
            }
            else
            {
                var assoc = associations.Find(p => p.Name == assocPropArray[0]);
                if(assoc != null)
                {
                    var targetEnity = ModelReflector.Entities.Find(p => p.Name == assoc.Target);
                    var emailProperty = targetEnity.Properties.Find(p => p.Name == assocPropArray[1]);
                    // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[0] + "ID");
                    object PropValue = Property.GetValue(Entity, null);
                    long targetvalue = 0;
                    if(PropValue != null)
                    {
                        targetvalue = Convert.ToInt64(PropValue);
                    }
                    try
                    {
                        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + targetEnity.Name + "Controller, GeneratorBase.MVC.Controllers");
                        Type controller = new CreateControllerType(targetEnity.Name).controllerType;
                        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                        {
                            MethodInfo mc = controller.GetMethod("GetPropertyValueByEntityId");
                            object[] MethodParams = new object[] { targetvalue, emailProperty.Name,"" };
                            var obj1 = mc.Invoke(objController, MethodParams);
                            if(obj1 != null)
                            {
                                var data = Convert.ToString(((System.Web.Mvc.JsonResult)(obj1)).Data);
                                var objuser = usercontext.Users.FirstOrDefault(p => p.Id == data);
                                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                                {
                                    if(new EmailAddressAttribute().IsValid(objuser.Email))
                                        //result += objuser != null ? objuser.Email + "," : "";
                                        result.Add(objuser);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
        }
        //return result.Trim().TrimEnd(",".ToCharArray());
        return result;
    }
    public string getEmailids(IUser user, string[] values, object Entity, string[] roles, string userName)
    {
        string result = "";
        var entityType = ObjectContext.GetObjectType(Entity.GetType());
        var EntityName = entityType.Name;
        var Ent = ModelReflector.Entities.First(p => p.Name == EntityName);
        var associations = Ent.Associations;
        //ApplicationContext db = new ApplicationContext(user);
        PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        foreach(string obj in values)
        {
            var assocPropArray = obj.Split('.');
            if(obj == "Owner")
            {
                var objuser = usercontext.Users.FirstOrDefault(p => p.UserName == userName);
                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    result += objuser != null ? objuser.Email + "," : "";
            }
            if(EntityName == assocPropArray[0])
            {
                //var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                object PropValue = Property.GetValue(Entity, null);
                var propertyFormat = Property.GetCustomAttribute(typeof(EmailAddressAttribute));
                if(PropValue != null)
                {
                    var objuser = usercontext.Users.FirstOrDefault(p => p.Id == PropValue);
                    if(Property.Name == "Id" && objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    {
                        if(new EmailAddressAttribute().IsValid(objuser.Email))
                            result += objuser != null ? objuser.Email + "," : "";
                    }
                    else
                    {
                        if(Property.Name != "Id")
                        {
                            if(new EmailAddressAttribute().IsValid(PropValue))
                                result += PropValue + ",";
                        }
                    }
                }
            }
            else
            {
                var assoc = associations.Find(p => p.Name == assocPropArray[0]);
                if(assoc != null)
                {
                    var targetEnity = ModelReflector.Entities.Find(p => p.Name == assoc.Target);
                    var emailProperty = targetEnity.Properties.Find(p => p.Name == assocPropArray[1]);
                    // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[0] + "ID");
                    object PropValue = Property.GetValue(Entity, null);
                    long targetvalue = 0;
                    if(PropValue != null)
                    {
                        targetvalue = Convert.ToInt64(PropValue);
                    }
                    try
                    {
                        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + targetEnity.Name + "Controller, GeneratorBase.MVC.Controllers");
                        Type controller = new CreateControllerType(targetEnity.Name).controllerType;
                        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                        {
                            MethodInfo mc = controller.GetMethod("GetPropertyValueByEntityId");
                            object[] MethodParams = new object[] { targetvalue, emailProperty.Name,"" };
                            var obj1 = mc.Invoke(objController, MethodParams);
                            if(obj1 != null)
                            {
                                var data = Convert.ToString(((System.Web.Mvc.JsonResult)(obj1)).Data);
                                var objuser = usercontext.Users.FirstOrDefault(p => p.Id == data);
                                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                                {
                                    if(new EmailAddressAttribute().IsValid(objuser.Email))
                                        result += objuser != null ? objuser.Email + "," : "";
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
        }
        return result.Trim().TrimEnd(",".ToCharArray());
    }
    public string getUserEmailids(object obj1, string EntityName, string[] roles,IUser user)
    {
        string result = "";
        if(obj1 == null) return result;
        var Ent = ModelReflector.Entities.First(p => p.Name == EntityName);
        var associations = Ent.Associations;
        foreach(var asso in associations.Where(p => p.Target == "IdentityUser"))
        {
            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == asso.AssociationProperty);
            object PropValue = Property.GetValue(obj1, null);
            if(PropValue != null)
            {
                var objuser = usercontext.Users.FirstOrDefault(p => p.Id == PropValue);
                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    result += objuser != null ? objuser.Email + "," : "";
            }
        }
        if(roles != null)
            result += getUserEmailidsFromRoles(roles,user);
        return result;
    }
    public string getUserEmailidsFromRoles(string[] roles, IUser user)
    {
        string result = "";
        if(roles[0] == "")
            return "";
        foreach(var item in roles)
        {
            var resultinfo = (from u in new ApplicationContext(user).T_Users
                              select new { u.Roles, u.Id }).ToList();
            foreach(var userId in resultinfo)
            {
                if(userId.Roles.Any(p => p.RoleId == item))
                {
                    var objuser = usercontext.Users.FirstOrDefault(p => p.Id == userId.Id);
                    if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                        result += objuser != null ? objuser.Email + "," : "";
                }
            }
        }
        return result;
    }
    public string getEmailIdValue(IUser user, string[] values, object Entity, string[] roles, string userName)
    {
        string result = "";
        var entityType = ObjectContext.GetObjectType(Entity.GetType());
        var EntityName = entityType.Name;
        var Ent = ModelReflector.Entities.First(p => p.Name == EntityName);
        var associations = Ent.Associations;
        //ApplicationContext db = new ApplicationContext(user);
        PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
        foreach(string obj in values)
        {
            var assocPropArray = obj.Split('.');
            if(obj == "Owner")
            {
                var objuser = usercontext.Users.FirstOrDefault(p => p.UserName == userName);
                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    //result += objuser != null ? objuser.Email + "," : "";
                    //result.Add(objuser);
                    result += objuser.Email + ",";
            }
            if(EntityName == assocPropArray[0])
            {
                //var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[1]);
                object PropValue = Property.GetValue(Entity, null);
                var propertyFormat = Property.GetCustomAttribute(typeof(EmailAddressAttribute));
                if(PropValue != null)
                {
                    var objuser = usercontext.Users.FirstOrDefault(p => p.Id == PropValue);
                    if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                    {
                        if(new EmailAddressAttribute().IsValid(objuser.Email))
                            //result += objuser != null ? objuser.Email + "," : "";
                            //result.Add(objuser);
                            result += objuser.Email + ",";
                    }
                }
            }
            else
            {
                var assoc = associations.Find(p => p.Name == assocPropArray[0]);
                if(assoc != null)
                {
                    var targetEnity = ModelReflector.Entities.Find(p => p.Name == assoc.Target);
                    var emailProperty = targetEnity.Properties.Find(p => p.Name == assocPropArray[1]);
                    // PropertyInfo[] properties = (Entity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == assocPropArray[0] + "ID");
                    object PropValue = Property.GetValue(Entity, null);
                    long targetvalue = 0;
                    if(PropValue != null)
                    {
                        targetvalue = Convert.ToInt64(PropValue);
                    }
                    try
                    {
                        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + targetEnity.Name + "Controller, GeneratorBase.MVC.Controllers");
                        Type controller = new CreateControllerType(targetEnity.Name).controllerType;
                        using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                        {
                            MethodInfo mc = controller.GetMethod("GetPropertyValueByEntityId");
                            object[] MethodParams = new object[] { targetvalue, emailProperty.Name,"" };
                            var obj1 = mc.Invoke(objController, MethodParams);
                            if(obj1 != null)
                            {
                                var data = Convert.ToString(((System.Web.Mvc.JsonResult)(obj1)).Data);
                                var objuser = usercontext.Users.FirstOrDefault(p => p.Id == data);
                                if(objuser != null && !string.IsNullOrEmpty(objuser.Email))
                                {
                                    if(new EmailAddressAttribute().IsValid(objuser.Email))
                                        //result += objuser != null ? objuser.Email + "," : "";
                                        //result.Add(objuser);
                                        result += objuser.Email + ",";
                                }
                                if(!string.IsNullOrEmpty(data))
                                {
                                    if(new EmailAddressAttribute().IsValid(data))
                                        result += data + ",";
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
        }
        //return result.Trim().TrimEnd(",".ToCharArray());
        return result;
    }
}
}
