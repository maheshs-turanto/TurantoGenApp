using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace GeneratorBase.MVC.Models
{
[Table("tbl_Permission")]
public class Permission
{
    [Key]
    public long Id
    {
        get;
        set;
    }
    [DisplayName("EntityName")]
    public string EntityName
    {
        get;
        set;
    }
    [DisplayName("RoleName")]
    public string RoleName
    {
        get;
        set;
    }
    [DisplayName("CanEdit")]
    public bool CanEdit
    {
        get;
        set;
    }
    [DisplayName("CanDelete")]
    public bool CanDelete
    {
        get;
        set;
    }
    [DisplayName("CanView")]
    public bool CanView
    {
        get;
        set;
    }
    [DisplayName("CanAdd")]
    public bool CanAdd
    {
        get;
        set;
    }
    [DisplayName("NoEdit")]
    public string NoEdit
    {
        get;
        set;
    }
    [DisplayName("NoView")]
    public string NoView
    {
        get;
        set;
    }
    [DisplayName("Owner")]
    public Nullable<bool> IsOwner
    {
        get;
        set;
    }
    [DisplayName("UserAssociation")]
    public string UserAssociation
    {
        get;
        set;
    }
    //code for verb action security
    [DisplayName("Verbs")]
    public string Verbs
    {
        get;
        set;
    }
    //
    [DisplayName("ViewR")]
    public string ViewR
    {
        get;
        set;
    }
    [DisplayName("EditR")]
    public string EditR
    {
        get;
        set;
    }
    [DisplayName("DeleteR")]
    public string DeleteR
    {
        get;
        set;
    }
    [DisplayName("Self Registration")]
    public Nullable<bool> SelfRegistration
    {
        get;
        set;
    }
    [NotMapped]
    public string DisplayValue
    {
        get
        {
            return EntityName + "-" + RoleName;
        }
    }
}
public class USB
{
    public USB()
    {
        this.security = new List<UserBasedSecurity>();
    }
    public USB(List<UserBasedSecurity> list)
        : this()
    {
        foreach(var lst in list)
            this.security.Add(lst);
    }
    public List<UserBasedSecurity> security
    {
        get;
        set;
    }
}
public class USBSecurity
{
    public USBSecurity()
    {
        this.roles = new List<string>();
    }
    public USBSecurity(UserBasedSecurity ub):this()
    {
        this.UBS = ub;
    }
    public List<string> roles
    {
        get;
        set;
    }
    public UserBasedSecurity UBS
    {
        get;
        set;
    }
}
public class SelectEntityRolesViewModel
{
    public SelectEntityRolesViewModel()
    {
        this.Entities = new List<SelectPermissionEditorViewModel>();
        this.privileges = new List<PermissionAdminPrivilege>();
    }
    // Enable initialization with an instance of ApplicationRole:
    public SelectEntityRolesViewModel(string RoleName)
        : this()
    {
        this.RoleName = RoleName;
        var Db = new PermissionContext();
        var permissions = Db.Permissions.ToList().Where(p => p.RoleName == RoleName);
        var listprivileges = Db.AdminPrivileges.ToList().Where(p => p.RoleName == RoleName);
        var IsAppHeader = false;
        var IsDefaultHeader = false;
        var rowcnt = 0;
        foreach(var ent in GeneratorBase.MVC.ModelReflector.Entities.Where(p=>!p.IsAdminEntity))
        {
            if(ent.Name.ToUpper() == "PERMISSION") continue;
            if(!IsAppHeader && !ent.IsDefault && rowcnt == 0)
            {
                IsAppHeader = true;
                rowcnt++;
            }
            else
                IsAppHeader = false;
            if(!IsDefaultHeader && ent.IsDefault && rowcnt == 1)
            {
                IsDefaultHeader = true;
                rowcnt++;
            }
            else
                IsDefaultHeader = false;
            var rvm = new SelectPermissionEditorViewModel(ent.Name, IsAppHeader, IsDefaultHeader,RoleName);
            this.Entities.Add(rvm);
        }
        foreach(var perm in permissions)
        {
            var checkUserRole =
                this.Entities.ToList().Find(r => r.EntityName == perm.EntityName);
            if(checkUserRole == null) continue;
            if(perm.CanEdit)
                checkUserRole.CanEdit = true;
            if(perm.CanDelete)
                checkUserRole.CanDelete = true;
            if(perm.CanView)
                checkUserRole.CanView = true;
            if(perm.CanAdd)
                checkUserRole.CanAdd = true;
            checkUserRole.IsOwner = perm.IsOwner !=null ?perm.IsOwner.Value:false;
            checkUserRole.SelfRegistration = perm.SelfRegistration != null ? perm.SelfRegistration.Value : false;
            checkUserRole.UserAssociation = perm.UserAssociation;
            //code for verb action security
            checkUserRole.Verbs = perm.Verbs;
            //
            checkUserRole.DataSecurityAssociationsViewCustom =  checkUserRole.DataSecurityAssociationsViewValue = perm.ViewR;
            checkUserRole.DataSecurityAssociationsEditCustom = checkUserRole.DataSecurityAssociationsEditValue = perm.EditR;
            checkUserRole.DataSecurityAssociationsDeleteCustom = checkUserRole.DataSecurityAssociationsDeleteValue = perm.DeleteR;
        }
        //foreach (var item in Enum.GetValues(typeof(AdminFeatures)))
        foreach(var item in (new AdminFeaturesDictionary()).getDictionary())
        {
            var privilege = listprivileges.FirstOrDefault(p => p.AdminFeature == item.Key);
            if(privilege != null)
                this.privileges.Add(privilege);
            else
            {
                var obj = new PermissionAdminPrivilege();
                obj.RoleName = this.RoleName;
                obj.AdminFeature = item.Key;
                obj.IsAllow = false;
                obj.IsAdd = false;
                obj.IsEdit = false;
                obj.IsDelete = false;
                this.privileges.Add(obj);
            }
        }
    }
    public string RoleName
    {
        get;
        set;
    }
    public List<SelectPermissionEditorViewModel> Entities
    {
        get;
        set;
    }
    public List<PermissionAdminPrivilege> privileges
    {
        get;
        set;
    }
}
public class SelectPermissionEditorViewModel
{
    public SelectPermissionEditorViewModel() { }
    public SelectPermissionEditorViewModel(string EntityName, bool IsAppHeader, bool IsDefaultHeader,string rolename)
    {
        this.EntityName = EntityName;
        this.IsAssociatedWithUser = false;
        //code for verb action security
        this.IsHaveVerbs = false;
        this.IsSelfRegistrartion = false;
        this.IsAppHeader = IsAppHeader;
        this.IsDefaultHeader = IsDefaultHeader;
        this.IsRuleBasedRole = false;
        this.RuleBasedRoleBase = "";
        var roleobj = (new GeneratorBase.MVC.Models.ApplicationDbContext(new GeneratorBase.MVC.Models.SystemUser())).Roles.FirstOrDefault(p => p.Name == rolename);
        if(roleobj.RoleType == "RuleBased")
        {
            var dynamicrole = (new GeneratorBase.MVC.ApplicationContext(new GeneratorBase.MVC.Models.SystemUser())).DynamicRoleMappings.FirstOrDefault(p => p.RoleId == rolename);
            if(dynamicrole != null)
            {
                this.IsRuleBasedRole = true;
                this.RuleBasedRoleBase = dynamicrole.EntityName;
                this.DataSecurityAssociationsEdit = this.DataSecurityAssociationsDelete = this.DataSecurityAssociationsView = GetDataFiltersOfEntity(0,this.RuleBasedRoleBase, EntityName,dynamicrole.UserRelation,this.RuleBasedRoleBase);
            }
        }
        //
        var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).Select(p=>p.Name);
        var association = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Associations.Where(p => p.Target == "IdentityUser" || EntList.Contains(p.Target)).ToList();
        if(association != null && association.Count()>0)
        {
            this.IsAssociatedWithUser = true;
            UserAssociationList = association;
        }
        //code for verb action security
        var EntityVerbs = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Verbs.ToList();
        if(EntityVerbs != null && EntityVerbs.Count() > 0)
        {
            this.IsHaveVerbs = true;
            EntityVerbsList = EntityVerbs;
        }
        var userassociation = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Associations.Where(p => p.Target == "IdentityUser").ToList();
        if(userassociation != null && userassociation.Count()>0)
        {
            this.IsSelfRegistrartion = true;
        }
        //
    }
    private string ReverseString(string s, string separtator, string userrelation, int mode)
    {
        var dotseparator = ".";
        int temp;
        string result = "";
        string[] a = s.Split(separtator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        result = string.Join(".", a.Reverse());
        if(string.IsNullOrEmpty(result)) dotseparator = "";
        if(mode == 1) return result + dotseparator + userrelation;
        return result + dotseparator + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0";
    }
    private List<ModelReflector.Association> GetDataFiltersOfEntity(int mode, string Entity, string TargetEntity, string userrelation,string baseentity)
    {
        var modelentities = ModelReflector.Entities;
        var Ent = modelentities.First(p => p.Name == Entity);
        var BaseEntity = modelentities.First(p => p.Name == baseentity);
        var EntTarget = modelentities.First(p => p.Name == TargetEntity);
        var AssoList = new List<ModelReflector.Association>();
        if(mode != 1)
            foreach(var asso in BaseEntity.Associations.Where(p => p.Target != "IdentityUser"))
            {
                ModelReflector.Association newAsso1 = new ModelReflector.Association();
                newAsso1.Name = asso.Name.ToLower() + ".Any(" + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)";// assointernalname + "." + association1.Name.ToLower();
                newAsso1.DisplayName = newAsso1.Name;
                newAsso1.DisplayName = newAsso1.DisplayName.Replace("@0", "LoggedInUser");
                if(asso.Target == TargetEntity) AssoList.Add(newAsso1);
                foreach(var ent1 in ModelReflector.Entities)
                {
                    var Ent1 = modelentities.FirstOrDefault(p => p.Name == ent1.Name);
                    if(mode != 1 && Ent1.type == "Bridge" && Ent1.Associations.Any(p => p.Target == asso.Target))
                    {
                        var AssoTargetEntity = modelentities.FirstOrDefault(p => p.Name == asso.Target);
                        ModelReflector.Association newAsso11 = new ModelReflector.Association();
                        //newAsso11.Name = Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)";// assointernalname + "." + association1.Name.ToLower();
                        newAsso11.Name = Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + asso.Target.ToLower() + ".Any(" + newAsso1.Name + "))";
                        if(newAsso11.Name.ToLower().Contains((Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + asso.Target.ToLower() + ".Any(").ToLower()))
                            continue;
                        var targettopass = asso.Target.ToLower() + "." + newAsso1.Name;
                        newAsso11.DisplayName = Ent1.DisplayName + "." + ent1.DisplayName;
                        if(ent1.Name == EntTarget.Name)
                        {
                            newAsso11.DisplayName = newAsso11.Name;
                            newAsso11.DisplayName = newAsso11.DisplayName.Replace("@0", "LoggedInUser");
                            AssoList.Add(newAsso11);
                        }
                        if(ent1.Name != Ent.Name)
                            // AssoList.AddRange(GetDataFiltersOfEntity(0, ent1.Name, EntTarget.Name, Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)", baseentity));
                            AssoList.AddRange(GetDataFiltersOfEntity(0, ent1.Name, EntTarget.Name, targettopass, baseentity));
                    }
                    //if (ent1.Name != Ent.Name)
                    if(EntTarget.Associations.Any(p => p.Target == asso.Target))
                        // if(TargetEntity!=baseentity)
                        AssoList.AddRange(GetDataFiltersOfEntity(1, ent1.Name, TargetEntity, newAsso1.Name, baseentity));
                }
            }
        foreach(var ent in ModelReflector.Entities)
        {
            if(mode != 1 && Ent.type == "Bridge" && Ent.Associations.Any(p => p.Target == ent.Name))
            {
                ModelReflector.Association newAsso1 = new ModelReflector.Association();
                if(userrelation.Contains(".Any("))
                    newAsso1.Name = Ent.Name + "_" + ent.Name.ToLower() + ".Any("+ userrelation+")";
                else
                    newAsso1.Name = Ent.Name + "_" + ent.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)";// assointernalname + "." + association1.Name.ToLower();
                newAsso1.DisplayName = Ent.DisplayName + "." + ent.DisplayName;
                if(ent.Name == EntTarget.Name)
                {
                    newAsso1.DisplayName = newAsso1.Name;
                    newAsso1.DisplayName = newAsso1.DisplayName.Replace("@0", "LoggedInUser");
                    AssoList.Add(newAsso1);
                }
                if(ent.Name != Ent.Name)
                {
                    AssoList.AddRange(GetDataFiltersOfEntity(1, ent.Name, EntTarget.Name, newAsso1.Name, baseentity));
                    //    AssoList.AddRange(GetDataFiltersOfEntity(1, ent.Name, EntTarget.Name, Ent.Name + "_" + ent.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)", baseentity));
                }
            }
            if(ent.Associations.Any(p => p.Target == Ent.Name))
            {
                foreach(var association in ent.Associations.Where(p => p.Target == Ent.Name))
                {
                    ModelReflector.Association newAsso = new ModelReflector.Association();
                    newAsso.Name = association.Name.ToLower();
                    var assoname = association.AssociationProperty;// +"." + ent.DisplayName;
                    var assointernalname = association.Name.ToLower();
                    newAsso.Target = association.Target;
                    newAsso.DisplayName = association.AssociationProperty;// +"." + ent.DisplayName;
                    newAsso.AssociationProperty = association.AssociationProperty;
                    if(Ent.Name == association.Name)
                        newAsso.AssociationProperty = ent.Name + "." + association.AssociationProperty;
                    if(ent.Name == EntTarget.Name)
                    {
                        newAsso.DisplayName = newAsso.Name = ReverseString(newAsso.Name, ".", userrelation, mode);
                        newAsso.DisplayName = newAsso.DisplayName.Replace("@0", "LoggedInUser");
                        AssoList.Add(newAsso);
                    }
                    var Ent1 = modelentities.FirstOrDefault(p => p == ent);
                    foreach(var ent1 in ModelReflector.Entities)
                    {
                        if(mode != 1 && Ent1.type == "Bridge" && Ent1.Associations.Any(p => p.Target == ent1.Name))
                        {
                            ModelReflector.Association newAsso1 = new ModelReflector.Association();
                            newAsso1.Name = Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)";// assointernalname + "." + association1.Name.ToLower();
                            newAsso1.DisplayName = Ent1.DisplayName + "." + ent1.DisplayName;
                            if(ent1.Name == EntTarget.Name)
                            {
                                newAsso1.DisplayName = newAsso1.Name;
                                newAsso1.DisplayName = newAsso1.DisplayName.Replace("@0", "LoggedInUser");
                                AssoList.Add(newAsso1);
                            }
                            if(ent1.Name != Ent.Name)
                                AssoList.AddRange(GetDataFiltersOfEntity(1, ent1.Name, EntTarget.Name, Ent1.Name + "_" + ent1.Name.ToLower() + ".Any(" + Ent.Name.ToLower() + "." + userrelation.TrimEnd("ID".ToCharArray()).ToLower() + ".UserName" + "=@0)", baseentity));
                        }
                        if(ent1.Associations.Any(p => p.Target == Ent1.Name))
                            foreach(var association1 in ent1.Associations.Where(p => p.Target == Ent1.Name))
                            {
                                ModelReflector.Association newAsso1 = new ModelReflector.Association();
                                newAsso1.Name = assointernalname + "." + association1.Name.ToLower();
                                var assoname1 = assoname + "." + association1.AssociationProperty;
                                var assointernalname1 = assointernalname + "." + association1.Name.ToLower();
                                newAsso1.Target = association1.Target;
                                newAsso1.DisplayName = assoname + "." + association1.AssociationProperty;// +"." + ent1.DisplayName;
                                newAsso1.AssociationProperty = association1.AssociationProperty;
                                if(Ent1.Name == association1.Name)
                                    newAsso1.AssociationProperty = ent1.Name + "." + association1.AssociationProperty;
                                if(ent1.Name == EntTarget.Name)
                                {
                                    newAsso1.DisplayName = newAsso1.Name = ReverseString(newAsso1.Name, ".", userrelation, mode);
                                    newAsso1.DisplayName = newAsso1.DisplayName.Replace("@0", "LoggedInUser");
                                    AssoList.Add(newAsso1);
                                }
                                var Ent2 = modelentities.FirstOrDefault(p => p == ent1);
                                foreach(var ent2 in ModelReflector.Entities)
                                    if(ent2.Associations.Any(p => p.Target == Ent2.Name))
                                        foreach(var association2 in ent2.Associations.Where(p => p.Target == Ent2.Name))
                                        {
                                            ModelReflector.Association newAsso2 = new ModelReflector.Association();
                                            newAsso2.Name = assointernalname1 + "." + association2.Name.ToLower();
                                            var assoname2 = assoname1 + "." + association2.AssociationProperty;
                                            var assointernalname2 = assointernalname1 + "." + association2.Name.ToLower();
                                            newAsso2.Target = association2.Target;
                                            newAsso2.DisplayName = assoname1 + "." + association2.AssociationProperty;// +"." + ent2.DisplayName;
                                            newAsso2.AssociationProperty = association2.AssociationProperty;
                                            if(Ent2.Name == association2.Name)
                                                newAsso2.AssociationProperty = ent2.Name + "." + association2.AssociationProperty;
                                            if(ent2.Name == EntTarget.Name)
                                            {
                                                newAsso2.DisplayName = newAsso2.Name = ReverseString(newAsso2.Name, ".", userrelation, mode);
                                                newAsso2.DisplayName = newAsso2.DisplayName.Replace("@0", "LoggedInUser");
                                                AssoList.Add(newAsso2);
                                            }
                                            var Ent3 = modelentities.FirstOrDefault(p => p == ent2);
                                            foreach(var ent3 in ModelReflector.Entities)
                                                if(ent2.Associations.Any(p => p.Target == Ent3.Name))
                                                    foreach(var association3 in ent3.Associations.Where(p => p.Target == Ent3.Name))
                                                    {
                                                        ModelReflector.Association newAsso3 = new ModelReflector.Association();
                                                        newAsso3.Name = assointernalname2 + "." + association3.Name.ToLower();
                                                        newAsso3.Target = association3.Target;
                                                        newAsso3.DisplayName = assoname2 + "." + association3.AssociationProperty;// +"." + ent3.DisplayName;
                                                        newAsso3.AssociationProperty = association3.AssociationProperty;
                                                        var assoname3 = assoname2 + "." + association3.AssociationProperty;// +"." + ent3.DisplayName;
                                                        var assointernalname3 = assointernalname2 + "." + association3.Name.ToLower();
                                                        if(Ent2.Name == association3.Name)
                                                            newAsso3.AssociationProperty = ent3.Name + "." + association3.AssociationProperty;
                                                        if(ent3.Name == EntTarget.Name)
                                                        {
                                                            newAsso3.DisplayName = newAsso3.Name = ReverseString(newAsso3.Name, ".", userrelation, mode);
                                                            newAsso3.DisplayName = newAsso3.DisplayName.Replace("@0", "LoggedInUser");
                                                            AssoList.Add(newAsso3);
                                                        }
                                                        var Ent4 = modelentities.FirstOrDefault(p => p == ent3);
                                                        foreach(var ent4 in ModelReflector.Entities)
                                                            if(ent3.Associations.Any(p => p.Target == Ent4.Name))
                                                                foreach(var association4 in ent4.Associations.Where(p => p.Target == Ent4.Name))
                                                                {
                                                                    ModelReflector.Association newAsso4 = new ModelReflector.Association();
                                                                    newAsso4.Name = assointernalname3 + "." + association4.Name.ToLower();
                                                                    newAsso4.Target = association4.Target;
                                                                    newAsso4.DisplayName = assoname3 + "." + association4.AssociationProperty;// +"." + ent4.DisplayName;
                                                                    newAsso4.AssociationProperty = association4.AssociationProperty;
                                                                    if(Ent2.Name == association4.Name)
                                                                        newAsso4.AssociationProperty = ent4.Name + "." + association4.AssociationProperty;
                                                                    if(ent4.Name == EntTarget.Name)
                                                                    {
                                                                        newAsso4.DisplayName = newAsso4.Name = ReverseString(newAsso4.Name, ".", userrelation, mode);
                                                                        newAsso4.DisplayName = newAsso4.DisplayName.Replace("@0", "LoggedInUser");
                                                                        AssoList.Add(newAsso4);
                                                                    }
                                                                }
                                                    }
                                        }
                            }
                    }
                }
            }
        }
        if(baseentity == TargetEntity)
        {
            ModelReflector.Association basecriteria = new ModelReflector.Association();
            basecriteria.Name = "";
            basecriteria.Target = "IdentityUser";
            basecriteria.DisplayName =basecriteria.Name  = ReverseString(basecriteria.Name, ".", userrelation, mode);
            basecriteria.DisplayName = basecriteria.DisplayName.Replace("@0", "LoggedInUser");
            AssoList.Add(basecriteria);
        }
        List<ModelReflector.Association> finallist =   new List<ModelReflector.Association>();
        foreach(var test in  AssoList.OrderBy(p => p.Name.Length).Distinct().ToList())
        {
            bool result = HiearchicalSecurityHelper.HierarchicalSecurityCheck(TargetEntity, new SystemUser(), (new ApplicationContext(new SystemUser())), test.Name);
            if(result)
            {
                finallist.Add(test);
            }
        }
        return finallist.OrderBy(p => p.Name.Length).Distinct().ToList();
    }
    public bool CanEdit
    {
        get;
        set;
    }
    public bool CanDelete
    {
        get;
        set;
    }
    public bool CanView
    {
        get;
        set;
    }
    public bool CanAdd
    {
        get;
        set;
    }
    public bool IsOwner
    {
        get;
        set;
    }
    public bool SelfRegistration
    {
        get;
        set;
    }
    public bool IsAssociatedWithUser
    {
        get;
        set;
    }
    public bool IsAppHeader
    {
        get;
        set;
    }
    public bool IsDefaultHeader
    {
        get;
        set;
    }
    //code for verb action security
    public bool IsHaveVerbs
    {
        get;
        set;
    }
    public bool IsSelfRegistrartion
    {
        get;
        set;
    }
    //
    public string UserAssociation
    {
        get;
        set;
    }
    public string Verbs
    {
        get;
        set;
    }
    public List<GeneratorBase.MVC.ModelReflector.Association> UserAssociationList
    {
        get;
        set;
    }
    //code for verb action security
    public List<GeneratorBase.MVC.ModelReflector.Verb> EntityVerbsList
    {
        get;
        set;
    }
    //
    [Required]
    public string EntityName
    {
        get;
        set;
    }
    
    public string RuleBasedRoleBase
    {
        get;
        set;
    }
    public bool IsRuleBasedRole
    {
        get;
        set;
    }
    public string DataSecurityAssociationsEditValue
    {
        get;
        set;
    }
    public string DataSecurityAssociationsViewValue
    {
        get;
        set;
    }
    public string DataSecurityAssociationsDeleteValue
    {
        get;
        set;
    }
    public string DataSecurityAssociationsEditCustom
    {
        get;
        set;
    }
    public string DataSecurityAssociationsViewCustom
    {
        get;
        set;
    }
    public string DataSecurityAssociationsDeleteCustom
    {
        get;
        set;
    }
    public List<GeneratorBase.MVC.ModelReflector.Association> DataSecurityAssociationsView
    {
        get;
        set;
    }
    public List<GeneratorBase.MVC.ModelReflector.Association> DataSecurityAssociationsEdit
    {
        get;
        set;
    }
    public List<GeneratorBase.MVC.ModelReflector.Association> DataSecurityAssociationsDelete
    {
        get;
        set;
    }
}
public class SelectFlsViewModel
{
    public SelectFlsViewModel()
    {
        this.Properties = new List<SelectFlsEditorViewModel>();
    }
    // Enable initialization with an instance of ApplicationRole:
    public SelectFlsViewModel(string RoleName, string EntityName)
        : this()
    {
        this.RoleName = RoleName;
        this.RoleName = RoleName;
        var Db = new PermissionContext();
        var permissions = Db.Permissions.ToList().Where(p => p.RoleName == RoleName && p.EntityName == EntityName);
        var entList = GeneratorBase.MVC.ModelReflector.Entities.Where(p=>!p.IsAdminEntity);
        if(EntityName != null)
            entList = entList.Where(p => p.Name == EntityName).ToList();
        foreach(var ent in entList)
        {
            if(ent.Name.ToUpper() == "PERMISSION") continue;
            foreach(var prop in ent.Properties.Where(p => !(p.DisplayName.Contains("WorkFlowInstanceId")) && !(p.DisplayName.Contains("TenantId"))))
            {
                var rvm = new SelectFlsEditorViewModel(prop.Name, ent.Name);
                this.Properties.Add(rvm);
            }
        }
        foreach(var perm in permissions)
        {
            var checkUserRoleEdit =
                this.Properties.ToList().Where(r => perm.NoEdit != null && perm.NoEdit.Contains(r.PropertyName+","));
            var checkUserRoleView =
                this.Properties.ToList().Where(r => perm.NoView != null && perm.NoView.Contains(r.PropertyName + ","));
            foreach(var t in checkUserRoleEdit)
                t.NoEdit = true;
            foreach(var t in checkUserRoleView)
                t.NoView = true;
            this.CanAdd = perm.CanAdd;
            this.CanDelete = perm.CanDelete;
            this.CanView = perm.CanView;
            this.CanEdit = perm.CanEdit;
        }
    }
    public string RoleName
    {
        get;
        set;
    }
    public bool CanEdit
    {
        get;
        set;
    }
    public bool CanDelete
    {
        get;
        set;
    }
    public bool CanView
    {
        get;
        set;
    }
    public bool CanAdd
    {
        get;
        set;
    }
    // public string EntityName { get; set; }
    public List<SelectFlsEditorViewModel> Properties
    {
        get;
        set;
    }
}
public class SelectFlsEditorViewModel
{
    public SelectFlsEditorViewModel() { }
    public SelectFlsEditorViewModel(string PropertyName, string EntityName)
    {
        this.PropertyName = PropertyName;
        this.EntityName = EntityName;
    }
    public bool NoEdit
    {
        get;
        set;
    }
    public bool NoView
    {
        get;
        set;
    }
    [Required]
    public string PropertyName
    {
        get;
        set;
    }
    public string EntityName
    {
        get;
        set;
    }
}
}
