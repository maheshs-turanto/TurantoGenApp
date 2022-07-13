// file:	Controllers\Account_Permission\PermissionController.cs
//
// summary:	Implements the permission controller class

using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling permissions.</summary>
public class PermissionController : IdentityBaseController
{
    /// <summary>(An Action that handles HTTP POST requests) saves the ubs.</summary>
    ///
    /// <param name="model">The model.</param>
    ///
    /// <returns>A response stream to send to the SaveUBS View.</returns>
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult SaveUBS(USB model)
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("UserBasedSecurity"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            var Db = new UserBasedSecurityContext();
            Db.UserBasedSecurities.RemoveRange(Db.UserBasedSecurities);
            Db.SaveChanges();
            foreach(var _model in model.security)
            {
                Db.UserBasedSecurities.Add(_model);
            }
            Db.SaveChanges();
        }
        return RedirectToAction("Index", "Admin");
    }
    
    /// <summary>Clears the ubs.</summary>
    ///
    /// <returns>A response stream to send to the ClearUBS View.</returns>
    
    public ActionResult ClearUBS()
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("UserBasedSecurity"))
            return RedirectToAction("Index", "Home");
        var Db = new UserBasedSecurityContext();
        Db.UserBasedSecurities.RemoveRange(Db.UserBasedSecurities);
        Db.SaveChanges();
        return RedirectToAction("Index", "Admin");
    }
    
    /// <summary>User based security.</summary>
    ///
    /// <param name="key">The key.</param>
    ///
    /// <returns>A response stream to send to the UserBasedSecurity View.</returns>
    
    public ActionResult UserBasedSecurity(string key)
    {
        if(!((CustomPrincipal)User).CanViewAdminFeature("UserBasedSecurity"))
            return RedirectToAction("Index", "Home");
        string setEntity = key;
        var Db = new UserBasedSecurityContext();
        var Datalist = Db.UserBasedSecurities.ToList();
        var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !Enum.IsDefined(typeof(GeneratorBase.MVC.ModelReflector.IgnoreEntities), p.Name) && !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).ToList();
        // var EntList = GeneratorBase.MVC.ModelReflector.Entities.ToList();
        if(key == null && EntList.Count > 0)
        {
            if(Datalist != null && Datalist.Where(p => p.IsMainEntity).Count() > 0)
            {
                setEntity = Datalist.FirstOrDefault(p => p.IsMainEntity).EntityName;
                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == setEntity);
                if(Entity != null)
                    ViewData["AlreadySet"] = Entity.DisplayName;
                else ViewData["AlreadySet"] = setEntity;
            }
            else
                setEntity = EntList[0].Name;
        }
        List<UserBasedSecurity> Data = new List<Models.UserBasedSecurity>();
        if(Datalist != null && Datalist.Where(p => p.EntityName == setEntity && p.IsMainEntity).Count() > 0)
            Data = Datalist;
        else
        {
            List<string> entitiesAdded = new List<string>();
            entitiesAdded.Add(setEntity);
            if(setEntity != null)
                Data = GetGridData(Data, setEntity, setEntity, entitiesAdded);
        }
        ViewBag.EntityList = new SelectList(EntList, "Name", "DisplayName", setEntity);
        var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
        RoleList.Add("All");
        var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
        RoleList.Remove(adminString);
        ViewBag.Roles = new SelectList(RoleList, "", "");
        USB DataUSB = new USB(Data);
        if(setEntity == null)
            return View(DataUSB);
        return View(DataUSB);
    }
    
    /// <summary>Gets grid data.</summary>
    ///
    /// <param name="Data">         The data.</param>
    /// <param name="setEntity">    The set entity.</param>
    /// <param name="Entity">       The entity.</param>
    /// <param name="entitiesAdded">The entities added.</param>
    ///
    /// <returns>The grid data.</returns>
    
    private List<UserBasedSecurity> GetGridData(List<UserBasedSecurity> Data, string setEntity, string Entity, List<string> entitiesAdded)
    {
        //IEnumerable<string> SelectedEntity = Data.Select(op => op.EntityName);
        if(Data.Where(p => p.EntityName == setEntity && p.TargetEntityName == Entity).Count() > 0)
            //if (SelectedEntity.Contains(Entity))
            return Data;
        else
        {
            var Ent = ModelReflector.Entities.Where(p => p.Name == Entity).ToList()[0];
            if(setEntity == Entity)
            {
                UserBasedSecurity UBS = new UserBasedSecurity();
                UBS.EntityName = Entity;
                UBS.IsMainEntity = true;
                UBS.RolesToIgnore = "";
                UBS.TargetEntityName = "User";
                UBS.AssociationName = Ent.Associations.Where(p => p.Target == "IdentityUser").ToList()[0].AssociationProperty;
                Data.Add(UBS);
            }
            foreach(var _obj in ModelReflector.Entities)
            {
                foreach(var Next in _obj.Associations.Where(p => p.Target == Entity))
                {
                    UserBasedSecurity UBS = new UserBasedSecurity();
                    UBS.EntityName = _obj.Name;
                    UBS.IsMainEntity = false;
                    UBS.RolesToIgnore = "";
                    UBS.TargetEntityName = Entity;
                    UBS.AssociationName = Next.AssociationProperty;
                    UBS.Other1 = _obj.Name == Entity ? "false" : "";
                    Data.Add(UBS);
                    if(entitiesAdded.Contains(_obj.Name))
                        continue;
                    else
                    {
                        entitiesAdded.Add(_obj.Name);
                        GetGridData(Data, Entity, _obj.Name, entitiesAdded);
                        break;
                    }
                }
            }
        }
        return Data;
    }
    
    /// <summary>GET: /Permission/.</summary>
    ///
    /// <param name="RoleName">Name of the role.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string RoleName)
    {
        if(!((CustomPrincipal)User).CanViewAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        //var Db = new ApplicationDbContext();
        //var roles = Db.Roles;
        var model = new SelectEntityRolesViewModel(RoleName);
        if(!((CustomPrincipal)User).IsAdmin)
        {
            var roles = Identitydb.Roles.First(u => u.Name == model.RoleName);
            if(roles.RoleType == "Global")
                return RedirectToAction("Index", "Home");
        }
        //var adminfeaturelist = new List<PermissionAdminPrivilege>();
        //foreach (var item in Enum.GetValues(typeof(AdminFeatures)))
        //{
        //    var obj = new PermissionAdminPrivilege();
        //    obj.RoleName = RoleName;
        //    obj.IsAllow = false;
        //    obj.AdminFeature = item.ToString();
        //    adminfeaturelist.Add(obj);
        //}
        //model.privileges = adminfeaturelist;
        return View(model);
        //return View(db.Permissions.ToList());
    }
    
    /// <summary>GET: /Permission/.</summary>
    ///
    /// <param name="RoleName">Name of the role.</param>
    /// <param name="key">     The key.</param>
    ///
    /// <returns>A response stream to send to the Fls View.</returns>
    
    public ActionResult Fls(string RoleName, string key)
    {
        if(!((CustomPrincipal)User).CanViewAdminFeature("FieldLevelSecurity"))
            return RedirectToAction("Index", "Home");
        if(!((CustomPrincipal)User).IsAdmin)
        {
            var roles = Identitydb.Roles.First(u => u.Name == RoleName);
            if(roles.RoleType == "Global")
                return RedirectToAction("Index", "Home");
        }
        string setEntity = key;
        if(key == null)
            setEntity = GeneratorBase.MVC.ModelReflector.Entities[0].Name;
        var model = new SelectFlsViewModel(RoleName, setEntity);
        ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName", setEntity);
        return View(model);
    }
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    
    /// <summary>(An Action that handles HTTP POST requests) saves the fls.</summary>
    ///
    /// <param name="model">The model.</param>
    ///
    /// <returns>A response stream to send to the SaveFls View.</returns>
    
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult SaveFls(SelectFlsViewModel model)
    {
        PermissionContext db = new PermissionContext(LogggedInUser);
        if(!((CustomPrincipal)User).CanEditAdminFeature("FieldLevelSecurity"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            string EntName = "";
            if(model.Properties.Count() > 0)
            {
                EntName = model.Properties[0].EntityName;
                var list = db.Permissions.FirstOrDefault(q => q.RoleName == model.RoleName && EntName == q.EntityName);
                Permission permission = (list != null ? list : new Permission());
                string NoEdit = "";
                string NoView = "";
                permission.EntityName = EntName;
                permission.RoleName = model.RoleName;
                foreach(var ent in model.Properties)
                {
                    if(ent.NoEdit)
                        NoEdit += ent.PropertyName + ",";
                    if(ent.NoView)
                        NoView += ent.PropertyName + ",";
                }
                //NoEdit = NoEdit.TrimEnd(",".ToCharArray());
                //NoView = NoView.TrimEnd(",".ToCharArray());
                permission.NoEdit = NoEdit;
                permission.NoView = NoView;
                permission.CanAdd = model.CanAdd;
                permission.CanEdit = model.CanEdit;
                permission.CanDelete = model.CanDelete;
                permission.CanView = model.CanView;
                if(list == null)
                    db.Permissions.Add(permission);
                db.SaveChanges();
            }
            return RedirectToAction("Fls", "Permission", new { RoleName = model.RoleName, key = EntName });
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) saves an admin privilege.</summary>
    ///
    /// <param name="model">The model.</param>
    ///
    /// <returns>A response stream to send to the SaveAdminPrivilege View.</returns>
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult SaveAdminPrivilege(SelectEntityRolesViewModel model)
    {
        PermissionContext db = new PermissionContext(LogggedInUser);
        if(!((CustomPrincipal)User).IsAdmin)
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            foreach(var ent in model.privileges)
            {
                var list = db.AdminPrivileges.FirstOrDefault(q => q.RoleName == model.RoleName && ent.AdminFeature == q.AdminFeature);
                PermissionAdminPrivilege permission = (list != null ? list : new PermissionAdminPrivilege());
                permission.IsAllow = ent.IsAllow;
                permission.IsEdit = ent.IsEdit;
                permission.IsAdd = ent.IsAdd;
                permission.IsDelete = ent.IsDelete;
                permission.AdminFeature = ent.AdminFeature;
                permission.RoleName = ent.RoleName;
                if(list == null)
                    db.AdminPrivileges.Add(permission);
                db.SaveChanges();
            }
        }
        return RedirectToAction("Index", new { RoleName = model.RoleName });
        // return Json("FROMPAGE", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    
    
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    
    /// <summary>(An Action that handles HTTP POST requests) saves a permission.</summary>
    ///
    /// <param name="model">The model.</param>
    ///
    /// <returns>A response stream to send to the SavePermission View.</returns>
    
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult SavePermission(SelectEntityRolesViewModel model)
    {
        PermissionContext db = new PermissionContext(LogggedInUser);
        if(!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            //var idManager = new IdentityManager();
            foreach(var ent in model.Entities)
            {
                var list = db.Permissions.FirstOrDefault(q => q.RoleName == model.RoleName && ent.EntityName == q.EntityName);
                //db.Permissions.RemoveRange(list);
                Permission permission = (list != null ? list : new Permission());
                permission.CanAdd = ent.CanAdd;
                permission.ViewR = !string.IsNullOrEmpty(ent.DataSecurityAssociationsViewCustom) ? ent.DataSecurityAssociationsViewCustom : ent.DataSecurityAssociationsViewValue;
                permission.EditR = !string.IsNullOrEmpty(ent.DataSecurityAssociationsEditCustom) ? ent.DataSecurityAssociationsEditCustom : ent.DataSecurityAssociationsEditValue;
                permission.DeleteR = !string.IsNullOrEmpty(ent.DataSecurityAssociationsDeleteCustom) ? ent.DataSecurityAssociationsDeleteCustom : ent.DataSecurityAssociationsDeleteValue;
                permission.CanDelete = ent.CanDelete;
                permission.CanView = ent.CanView;
                permission.CanEdit = ent.CanEdit;
                if(!string.IsNullOrEmpty(permission.ViewR))
                    permission.CanView = true;
                if(!string.IsNullOrEmpty(permission.EditR))
                    permission.CanEdit = true;
                if(!string.IsNullOrEmpty(permission.DeleteR))
                    permission.CanDelete = true;
                permission.IsOwner = ent.IsOwner;
                permission.SelfRegistration = ent.SelfRegistration;
                permission.UserAssociation = ent.UserAssociation;
                permission.EntityName = ent.EntityName;
                permission.RoleName = model.RoleName;
                //code for verb action security
                permission.Verbs = ent.Verbs;
                //
                if(list == null)
                    db.Permissions.Add(permission);
                db.SaveChanges();
            }
            return Json("FROMPAGE", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", "Admin");
        }
        return View();
    }
    
    /// <summary>GET: /Permission/Details/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    
    public ActionResult Details(long? id)
    {
        PermissionContext db = new PermissionContext(LogggedInUser);
        if(!((CustomPrincipal)User).CanViewAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Permission permission = db.Permissions.Find(id);
        if(permission == null)
        {
            return HttpNotFound();
        }
        return View(permission);
    }
    
    /// <summary>GET: /Permission/Create.</summary>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create()
    {
        if(!((CustomPrincipal)User).CanAddAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        return View();
    }
    
    /// <summary>POST: /Permission/Create To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="permission">The permission.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,EntityName,RoleName,CanEdit,CanDelete,CanView,CanAdd")] Permission permission)
    {
        PermissionContext db = new PermissionContext(LogggedInUser);
        if(!((CustomPrincipal)User).CanAddAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            db.Permissions.Add(permission);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(permission);
    }
    
    /// <summary>GET: /Permission/Edit/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(long? id)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PermissionContext db = new PermissionContext(LogggedInUser);
        Permission permission = db.Permissions.Find(id);
        if(permission == null)
        {
            return HttpNotFound();
        }
        return View(permission);
    }
    
    /// <summary>POST: /Permission/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="permission">The permission.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,EntityName,RoleName,CanEdit,CanDelete,CanView,CanAdd")] Permission permission)
    {
        if(!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(ModelState.IsValid)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
            db.Entry(permission).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(permission);
    }
    
    /// <summary>GET: /Permission/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(long? id)
    {
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        PermissionContext db = new PermissionContext(LogggedInUser);
        Permission permission = db.Permissions.Find(id);
        if(permission == null)
        {
            return HttpNotFound();
        }
        return View(permission);
    }
    
    /// <summary>POST: /Permission/Delete/5.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(long id)
    {
        if(!((CustomPrincipal)User).CanDeleteAdminFeature("RoleEntityPermission"))
            return RedirectToAction("Index", "Home");
        PermissionContext db = new PermissionContext(LogggedInUser);
        Permission permission = db.Permissions.Find(id);
        db.Permissions.Remove(permission);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    
    /// <summary>
    ///
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="aftersaveuser"></param>
    ///
    public void AfterSave(Permission permission, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(UserManager != null) UserManager.Dispose();
            if(Identitydb != null) Identitydb.Dispose();
            if(RoleManager != null) RoleManager.Dispose();
        }
        base.Dispose(disposing);
    }
    
    
    
}


}

