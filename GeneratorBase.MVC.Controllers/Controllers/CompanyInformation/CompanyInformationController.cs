using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.ComponentModel.DataAnnotations;
using GeneratorBase.MVC.DynamicQueryable;
using System.Web.UI.DataVisualization.Charting;
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for CompanyInformation actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class CompanyInformationController : BaseController
{
    /// <summary>Loads view data for count.</summary>
    ///
    /// <param name="companyinformation">The CompanyInformation.</param>
    /// <param name="AssocType">Type of the associated.</param>
    private void LoadViewDataForCount(CompanyInformation companyinformation, string AssocType)
    {
    }
    /// <summary>Loads view data after on edit.</summary>
    ///
    /// <param name="companyinformation">The CompanyInformation.</param>
    private void LoadViewDataAfterOnEdit(CompanyInformation companyinformation)
    {
        LoadViewDataBeforeOnEdit(companyinformation, false);
        CustomLoadViewDataListAfterEdit(companyinformation);
    }
    /// <summary>Loads view data before on edit.</summary>
    ///
    /// <param name="companyinformation">         The CompanyInformation.</param>
    /// <param name="loadCustomViewData">(Optional) True to load custom view data.</param>
    private void LoadViewDataBeforeOnEdit(CompanyInformation companyinformation, bool loadCustomViewData = true)
    {
        ViewBag.CompanyInformationFooterSectionAssociationCount = db.FooterSections.Count(p => p.CompanyInformationFooterSectionAssociationID == companyinformation.Id);
        ViewBag.CompanyInformationIsHiddenRule = checkHidden(User,"CompanyInformation", "OnEdit", false,null);
        ViewBag.CompanyInformationIsGroupsHiddenRule = checkHidden(User,"CompanyInformation", "OnEdit", true,null);
        ViewBag.CompanyInformationIsSetValueUIRule = checkSetValueUIRule(User,"CompanyInformation", "OnEdit",new long[] { 6, 8 },null,null);
        ViewBag.CompanyInformationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "CompanyInformation", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        if(loadCustomViewData) CustomLoadViewDataListBeforeEdit(companyinformation);
    }
    /// <summary>Loads view data after on create.</summary>
    ///
    /// <param name="companyinformation">The CompanyInformation.</param>
    private void LoadViewDataAfterOnCreate(CompanyInformation companyinformation)
    {
        CustomLoadViewDataListAfterOnCreate(companyinformation);
        ViewBag.CompanyInformationIsHiddenRule = checkHidden(User,"CompanyInformation", "OnCreate", false,null);
        ViewBag.CompanyInformationIsGroupsHiddenRule = checkHidden(User,"CompanyInformation", "OnCreate", true,null);
        ViewBag.CompanyInformationIsSetValueUIRule = checkSetValueUIRule(User,"CompanyInformation", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.CompanyInformationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "CompanyInformation", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
    }
    /// <summary>Loads view data before on create.</summary>
    ///
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated entity.</param>
    private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
    {
        ViewBag.CompanyInformationIsHiddenRule = checkHidden(User,"CompanyInformation", "OnCreate", false,null);
        ViewBag.CompanyInformationIsGroupsHiddenRule = checkHidden(User,"CompanyInformation", "OnCreate", true,null);
        ViewBag.CompanyInformationIsSetValueUIRule = checkSetValueUIRule(User,"CompanyInformation", "OnCreate",new long[] { 6, 7},null,null);
        ViewBag.CompanyInformationRestrictDropdownValueRule = RestrictDropdownValueRule(User, "CompanyInformation", "OnCreate", new long[] { 6, 7 }, null, new string[] { "" });
        CustomLoadViewDataListBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
    }
    
    public object GetRecordById(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        using(var appcontext = (new ApplicationContext(new SystemUser(),true)))
        {
            appcontext.Configuration.LazyLoadingEnabled = false;
            var _Obj = appcontext.CompanyInformations.Find(Convert.ToInt64(id));
            return _Obj;
        }
    }
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal, string RestrictDropdownVal = "")
    {
        var result = DropdownHelper.GetAllValue<CompanyInformation>(User, db.CompanyInformations, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "CompanyInformation", RestrictDropdownVal);
        return Json(result.Select(x=> new
        {
            Id = x.Id, Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (used in business rule).</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForRB action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var result = DropdownHelper.GetAllValueForRB<CompanyInformation>(User, db.CompanyInformations, caller, key, AssoNameWithParent, AssociationID, ExtraVal, "CompanyInformation");
        return Json(result.Select(x => new
        {
            Id = x.Id, Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown for businessrule).</summary>
    ///
    /// <param name="propNameBR">The property name line break.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValueForBR action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
    {
        IQueryable<CompanyInformation> list = db.CompanyInformations;
        if(!string.IsNullOrEmpty(propNameBR))
        {
            var result = list.Select("new(Id," + propNameBR + " as value)");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    /// <summary>Creates a JSON result with the given data as its content (multiselect dropdown on UI).</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="caller">            The caller to filter items.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllMultiSelectValue action.</returns>
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllMultiSelectValue(string caller,string key, string AssoNameWithParent, string AssociationID)
    {
        caller = caller.Replace("?", "");
        var result  = DropdownHelper.GetAllMultiSelectValue(User, db.CompanyInformations, key, AssoNameWithParent, AssociationID, "CompanyInformation", caller);
        return Json(result.Select(x => new
        {
            Id = x.Id, Name = x.DisplayValue
        }), JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Audit(0)]
    public JsonResult GetLockBusinessRules(CompanyInformation OModel)
    {
        OModel = UpdateHiddenProperties(OModel, string.Join(",", User.CanNotView("CompanyInformation")));
        return Json(GetLockBusinessRulesDictionary(User,db,OModel,"CompanyInformation"), JsonRequestBehavior.AllowGet);
    }
    /// <summary>Order by list on column.</summary>
    ///
    /// <param name="lstCompanyInformation">The IQueryable list CompanyInformation.</param>
    /// <param name="sortBy">      Column used to sort list.</param>
    /// <param name="isAsc">       Is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    private IQueryable<CompanyInformation> sortRecords(IQueryable<CompanyInformation> lstCompanyInformation, string sortBy, string isAsc)
    {
        string methodName = "";
        switch(isAsc.ToLower())
        {
        case "asc":
            methodName = "OrderBy";
            break;
        case "desc":
            methodName = "OrderByDescending";
            break;
        }
        if(sortBy.Contains("."))
            return isAsc.ToLower() == "asc" ? lstCompanyInformation.Sort(sortBy, true) : lstCompanyInformation.Sort(sortBy, false);
        ParameterExpression paramExpression = Expression.Parameter(typeof(CompanyInformation), "companyinformation");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<CompanyInformation>)lstCompanyInformation.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstCompanyInformation.ElementType, lambda.Body.Type },
                       lstCompanyInformation.Expression,
                       lambda));
    }
    /// <summary>Append search conditions in IQueryable.</summary>
    ///
    /// <param name="lstCompanyInformation">The list CompanyInformation.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">Is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    private IQueryable<CompanyInformation> searchRecords(IQueryable<CompanyInformation> lstCompanyInformation, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lstCompanyInformation = lstCompanyInformation.Where(s => (!String.IsNullOrEmpty(s.CompanyName) && s.CompanyName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyEmail) && s.CompanyEmail.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyAddress) && s.CompanyAddress.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyCountry) && s.CompanyCountry.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyState) && s.CompanyState.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyCity) && s.CompanyCity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyZipCode) && s.CompanyZipCode.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ContactNumber1) && s.ContactNumber1.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ContactNumber2) && s.ContactNumber2.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LoginBackgroundWidth) && s.LoginBackgroundWidth.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LoginBackgroundHeight) && s.LoginBackgroundHeight.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LogoWidth) && s.LogoWidth.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LogoHeight) && s.LogoHeight.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.IconWidth) && s.IconWidth.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.IconHeight) && s.IconHeight.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SMTPUser) && s.SMTPUser.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SMTPServer) && s.SMTPServer.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SMTPPassword) && s.SMTPPassword.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.SMTPPort) && s.SMTPPort.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.AboutCompany) && s.AboutCompany.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Disclaimer) && s.Disclaimer.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        else
            lstCompanyInformation = lstCompanyInformation.Where(s => (!String.IsNullOrEmpty(s.CompanyName) && s.CompanyName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyEmail) && s.CompanyEmail.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyCountry) && s.CompanyCountry.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyState) && s.CompanyState.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyCity) && s.CompanyCity.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.CompanyZipCode) && s.CompanyZipCode.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ContactNumber1) && s.ContactNumber1.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ContactNumber2) && s.ContactNumber2.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
        bool boolvalue;
        if(Boolean.TryParse(searchString, out boolvalue))
            lstCompanyInformation = lstCompanyInformation.Union(db.CompanyInformations.Where(s => (s.SSL == boolvalue) || (s.UseAnonymous == boolvalue)));
        return lstCompanyInformation;
    }
    /// <summary>Appends where clause for HostingEntity (list inside tab or accordion).</summary>
    ///
    /// <param name="_CompanyInformation">IQueryable<CompanyInformation>.</param>
    /// <param name="HostingEntity">Name of Hosting Entity.</param>
    /// <param name="AssociatedType">Association Name.</param>
    /// <param name="HostingEntityID">Id of Hosting entity.</param>
    ///
    /// <returns>Modified LINQ IQueryable<CompanyInformation>.</returns>
    private IQueryable<CompanyInformation> FilterByHostingEntity(IQueryable<CompanyInformation> _CompanyInformation, string HostingEntity, string AssociatedType, int? HostingEntityID)
    {
        if(HostingEntity == "CompanyList" && AssociatedType == "CompanyInformationCompanyListAssociation_CompanyList")
        {
            if(HostingEntityID != null)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                var ids = db.CompanyInformationCompanyListAssociations.AsNoTracking().Where(p => p.CompanyListID == hostid).Select(p => p.CompanyInformationID).ToList();
                _CompanyInformation = _CompanyInformation.Where(p => ids.Contains(p.Id));
                ViewBag.HostingEntityIDData = 0;
            }
        }
        return _CompanyInformation;
    }
    /// <summary>UpdateHiddenProperties.</summary>
    ///
    /// <param name="OModel">  The model.</param>
    /// <param name="cannotview">Type of the cannotview.</param>
    ///
    /// <returns>CompanyInformation object.</returns>
    public CompanyInformation UpdateHiddenProperties(CompanyInformation OModel, string cannotview = null)
    {
        Dictionary<string, string> hiddenProperties = new Dictionary<string, string>();
        if(OModel.Id > 0 && !string.IsNullOrEmpty(cannotview))
            using(var context = (new ApplicationContext(new SystemUser(), true)))
            {
                var obj1 = context.CompanyInformations.Find(OModel.Id);
                if(obj1 != null)
                {
                    foreach(var item in cannotview.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                        var Property = properties.FirstOrDefault(p => p.Name == item);
                        var propertyInfo = OModel.GetType().GetProperty(item);
                        if(Property != null && propertyInfo != null)
                        {
                            object PropValue = Property.GetValue(obj1, null);
                            PropValue = PropValue == null ? 0 : PropValue;
                            Type targetType = propertyInfo.PropertyType;
                            if(propertyInfo.PropertyType.IsGenericType)
                                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                            try
                            {
                                object safeValue = (PropValue == null) ? null : Convert.ChangeType(PropValue, targetType);
                                propertyInfo.SetValue(OModel, safeValue, null);
                            }
                            catch
                            {
                                propertyInfo.SetValue(OModel, null, null);
                            }
                        }
                    }
                }
            }
        return OModel;
    }
    /// <summary>Sets view bag.</summary>
    ///
    /// <param name="user">    The user.</param>
    ///
    /// <returns>Dictionary<long, string></returns>
    public Dictionary<long, string> SetViewBagCompanyInformation(IUser user)
    {
        return null;
    }
}
}

