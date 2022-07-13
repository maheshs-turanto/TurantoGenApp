using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling property help pages.</summary>
public partial class PropertyHelpPageController : BaseController
{
    /// <summary>Indexes.</summary>
    ///
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string HostingEntity, int? HostingEntityID, string AssociatedType, bool? RenderPartial)
    {
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        var listentName = from s in db.EntityPages
                          select s;
        var entName = listentName.Where(p => p.Id == HostingEntityID).FirstOrDefault().EntityName;
        var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == entName);
        List<PropertyHelpPage> prophelpList = new List<PropertyHelpPage>();
        var listPropertyHelp = from s in db.PropertyHelpPages
                               select s;
        PropertyHelpPage prohelpObj = new PropertyHelpPage();
        foreach(var item in entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "IsDeleted" && p.Name != "DeleteDateTime" && p.Name != "TenantId"))
        {
            if(!User.CanView(entName, item.Name))
                continue;
            prohelpObj = new PropertyHelpPage();
            prohelpObj.EntityName = entName;
            prohelpObj.PropertyName = item.Name;
            prohelpObj.Disable = false;
            var assoc = entList.Associations.Where(p => p.AssociationProperty == item.Name).FirstOrDefault();
            if(assoc != null)
            {
                prohelpObj.PropertyDataType = "Association";
                prohelpObj.ObjectType = "Association";
            }
            else
            {
                prohelpObj.PropertyDataType = item.DataType;
                prohelpObj.ObjectType = "Property";
            }
            prohelpObj.EntityName = entName;
            var _PropertyHelp = listPropertyHelp.Where(p => p.EntityName == entName).ToList();
            if(_PropertyHelp.Count() > 0)
            {
                var recHelpText = _PropertyHelp.Where(p => p.PropertyName == item.Name).ToList();
                if(recHelpText.Count() > 0)
                {
                    prohelpObj.Id = recHelpText.FirstOrDefault().Id;
                    prohelpObj.HelpText = recHelpText.FirstOrDefault().HelpText;
                    prohelpObj.Tooltip = recHelpText.FirstOrDefault().Tooltip;
                    prohelpObj.Disable = recHelpText.FirstOrDefault().Disable;
                }
            }
            prophelpList.Add(prohelpObj);
        }
        //var GroupName = entList.Properties.Where(p => p.Proptype == "Group");
        var GroupName = entList.Groups;
        foreach(var item in GroupName)
        {
            prohelpObj = new PropertyHelpPage();
            prohelpObj.PropertyName = item.Id;
            prohelpObj.EntityName = entName;
            prohelpObj.PropertyDataType = "String";
            prohelpObj.ObjectType = "Group";
            prohelpObj.GroupId = string.IsNullOrEmpty(item.Id) ? 0 : Convert.ToInt32(item.Id);
            prohelpObj.GroupName = item.Name;
            prohelpObj.Disable = false;
            var _PropertyGroup = listPropertyHelp.Where(p => p.EntityName == entName).ToList();
            if(_PropertyGroup.Count() > 0)
            {
                var recGroup = _PropertyGroup.Where(p => p.PropertyName == item.Id).FirstOrDefault();
                if(recGroup != null)
                {
                    prohelpObj.PropertyName = item.Id;
                    prohelpObj.Id = recGroup.Id;
                    prohelpObj.HelpText = recGroup.HelpText;
                    prohelpObj.Tooltip = recGroup.Tooltip;
                    prohelpObj.Disable = recGroup.Disable;
                }
            }
            else
            {
                prohelpObj.PropertyName = item.Id;
                prohelpObj.ObjectType = "Group";
                prohelpObj.GroupId = string.IsNullOrEmpty(item.Id) ? 0 : Convert.ToInt32(item.Id);
            }
            prophelpList.Add(prohelpObj);
        }
        var Verbs = entList.Verbs.Where(p => p.Name != "BulkUpdate" && p.Name != "BulkDelete" && p.Name != "ImportExcel" && p.Name != "ExportExcel" && p.Name != "Recycle");
        foreach(var item in Verbs)
        {
            prohelpObj = new PropertyHelpPage();
            prohelpObj.PropertyName = item.Name;
            prohelpObj.EntityName = entName;
            prohelpObj.PropertyDataType = "Action";
            prohelpObj.ObjectType = "Verb";
            prohelpObj.Disable = false;
            var _PropertyVerb = listPropertyHelp.Where(p => p.EntityName == entName).ToList();
            if(_PropertyVerb.Count() > 0)
            {
                var recVerb = _PropertyVerb.Where(p => p.PropertyName == item.Name).ToList();
                if(recVerb.Count() > 0)
                {
                    prohelpObj.PropertyName = recVerb.FirstOrDefault().PropertyName;
                    prohelpObj.Id = recVerb.FirstOrDefault().Id;
                    prohelpObj.HelpText = recVerb.FirstOrDefault().HelpText;
                    prohelpObj.Tooltip = recVerb.FirstOrDefault().Tooltip;
                    prohelpObj.Disable = recVerb.FirstOrDefault().Disable;
                }
            }
            else
                prohelpObj.PropertyName = item.Name;
            prophelpList.Add(prohelpObj);
        }
        //instruction feature
        var Instructions = entList.Instructions;
        var lstPropertyHelpPages = db.PropertyHelpPages.Where(p => p.ObjectType == "Instruction" && p.EntityName == entName).ToList();
        foreach(var item in Instructions)
        {
            prohelpObj = new PropertyHelpPage();
            prohelpObj.PropertyName = item.Id;
            prohelpObj.EntityName = entName;
            prohelpObj.PropertyDataType = "Label";
            prohelpObj.ObjectType = "Instruction";
            prohelpObj.Disable = false;
            prohelpObj.HelpText = item.Name;
            var _PropertyInstruction = listPropertyHelp.Where(p => p.EntityName == entName && p.ObjectType == "Instruction").ToList();
            if(_PropertyInstruction.Count() > 0)
            {
                var recInstruction = _PropertyInstruction.Where(p => p.PropertyName == item.Id).ToList();
                if(recInstruction.Count() > 0)
                {
                    prohelpObj.PropertyName = recInstruction.FirstOrDefault().PropertyName;
                    prohelpObj.Id = recInstruction.FirstOrDefault().Id;
                    prohelpObj.HelpText = recInstruction.FirstOrDefault().HelpText;
                    prohelpObj.Tooltip = recInstruction.FirstOrDefault().Tooltip;
                    prohelpObj.Disable = recInstruction.FirstOrDefault().Disable;
                }
            }
            else
                prohelpObj.PropertyName = item.Id;
            prophelpList.Add(prohelpObj);
            var lstInstruction = lstPropertyHelpPages.Where(p => p.ObjectType == "Instruction" && p.PropertyName == item.Id);
            if(lstInstruction.Count() == 0)
            {
                db.PropertyHelpPages.Add(prohelpObj);
                db.SaveChanges();
            }
        }
        IPagedList<PropertyHelpPage> list = prophelpList.ToPagedList(1, 100);
        return PartialView(list);
    }
    
    /// <summary>Saves a property value.</summary>
    ///
    /// <param name="id">              The identifier.</param>
    /// <param name="property">        The property.</param>
    /// <param name="value">           The value.</param>
    /// <param name="EntityName">      Name of the entity.</param>
    /// <param name="PropertyDataType">Type of the property data.</param>
    /// <param name="PropertyName">    Name of the property.</param>
    /// <param name="ObjectType">      Type of the object.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyValue View.</returns>
    
    public ActionResult SavePropertyValue(long id, string property, string value, string EntityName, string PropertyDataType, string PropertyName, string ObjectType)
    {
        if(!User.CanEdit("PropertyHelpPage"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        if(id == 0)
        {
            var lstEntityPage = from s in db.EntityPages
                                select s;
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == EntityName);
            var propItm = entList.Properties.Where(p => p.Proptype == "Group" && p.Name == property).ToList();
            var entobj = lstEntityPage.Where(p => p.EntityName == EntityName).ToList();
            if(entobj.Count() > 0)
                id = entobj.FirstOrDefault().Id;
            PropertyHelpPage propertyhelppageForNewRec = new PropertyHelpPage();
            if(property == "HelpText")
                propertyhelppageForNewRec.HelpText = value;
            if(property == "Tooltip")
                propertyhelppageForNewRec.Tooltip = value;
            propertyhelppageForNewRec.PropertyHelpOfEntityID = id;
            propertyhelppageForNewRec.EntityName = EntityName;
            propertyhelppageForNewRec.PropertyName = PropertyName;
            propertyhelppageForNewRec.PropertyDataType = PropertyDataType;
            propertyhelppageForNewRec.ObjectType = ObjectType;
            propertyhelppageForNewRec.Disable = false;
            if(propItm.Count() > 0)
            {
                propertyhelppageForNewRec.GroupId = Convert.ToInt32(propItm.FirstOrDefault().PropName);
                propertyhelppageForNewRec.GroupName = propItm.FirstOrDefault().PropText;
            }
            db.PropertyHelpPages.Add(propertyhelppageForNewRec);
            db.SaveChanges();
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        PropertyHelpPage propertyhelppage = db.PropertyHelpPages.Find(id);
        if(propertyhelppage == null)
        {
            return HttpNotFound();
        }
        var propertyName = property;
        var propertyValue = value;
        var propertyInfo = propertyhelppage.GetType().GetProperty(propertyName);
        bool isSave = false;
        if(propertyInfo != null)
        {
            propertyhelppage.setDateTimeToClientTime();
            Type targetType = propertyInfo.PropertyType;
            if(propertyInfo.PropertyType.IsGenericType)
                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            object newValue = string.IsNullOrEmpty(value) ? null : value;
            isSave = true;
            try
            {
                object safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, targetType);
                propertyInfo.SetValue(propertyhelppage, safeValue, null);
                isSave = true;
            }
            catch(Exception ex)
            {
                var result = new { Result = "fail", data = ex.Message };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        if(isSave && ValidateModel(propertyhelppage))
        {
            db.Entry(propertyhelppage).State = EntityState.Modified;
            db.SaveChanges();
            var result = new { Result = "Success", data = value };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(var error in ValidateModelWithErrors(propertyhelppage))
                errors += error.ErrorMessage + ".  ";
            var result = new { Result = "fail", data = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    //instruction feature
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the ShowInstructionLabel action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult ShowInstructionLabel(string Entity)
    {
        var _Entity = db.EntityPages.Where(p => p.EntityName == Entity && p.Disable == false).GetFromCache<IQueryable<EntityPage>, EntityPage>().ToList();
        if(_Entity.Count > 0)
        {
            var _PropertyHelp = db.PropertyHelpPages.Where(p => p.EntityName == Entity && !p.Disable.Value && p.ObjectType == "Instruction").GetFromCache<IQueryable<PropertyHelpPage>, PropertyHelpPage>().ToList();
            var data = from x in _PropertyHelp.OrderBy(q => q.PropertyName).ToList()
                       select new
            {
                PropertyName = x.PropertyName,
                PropertyDataType = x.PropertyDataType,
                Tooltip = x.Tooltip,
                HelpText =System.Web.HttpUtility.HtmlDecode(x.HelpText),
                ObjectType = x.ObjectType,
                GroupId = x.GroupId,
                GroupName = x.GroupName,
                GroupInternalName = Entity + GetInternalName(x.GroupName),
                Disable = x.Disable
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        return Json(null, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="Entity">The entity.</param>
    ///
    /// <returns>A JSON response stream to send to the ShowHelpIcon action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult ShowHelpIcon(string Entity)
    {
        var _Entity = new ApplicationContext(new SystemUser()).EntityPages.AsNoTracking().GetFromCache<IQueryable<EntityPage>, EntityPage>().ToList().Where(p => p.EntityName == Entity && p.Disable == false).ToList();
        if(_Entity.Count > 0)
        {
            var _PropertyHelp = new ApplicationContext(new SystemUser()).PropertyHelpPages.Where(p => p.EntityName == Entity && !p.Disable.Value).GetFromCache<IQueryable<PropertyHelpPage>,PropertyHelpPage>().ToList();
            var data = from x in _PropertyHelp.OrderBy(q => q.PropertyName).ToList()
                       select new
            {
                PropertyName = x.PropertyName,
                PropertyDataType = x.PropertyDataType,
                Tooltip = x.Tooltip,
                HelpText = x.HelpText,
                ObjectType = x.ObjectType,
                GroupId = x.GroupId,
                GroupName = x.GroupName,
                GroupInternalName = Entity + GetInternalName(x.GroupName),
                Disable = x.Disable
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Shows the property help.</summary>
    ///
    /// <param name="Entity">       The entity.</param>
    /// <param name="propertyName"> Name of the property.</param>
    /// <param name="RenderPartial">The render partial.</param>
    ///
    /// <returns>A response stream to send to the ShowPropertyHelp View.</returns>
    [Audit(0)]
    public ActionResult ShowPropertyHelp(string Entity, string propertyName, bool? RenderPartial)
    {
        var listPropertyHelp = from s in db.PropertyHelpPages
                               select s;
        var _PropertyHelp = listPropertyHelp.Where(p => p.EntityName == Entity && !p.Disable.Value).GetFromCache<IQueryable<PropertyHelpPage>,PropertyHelpPage>().ToList();
        List<PropertyHelpPage> list = _PropertyHelp;
        if(propertyName != "")
        {
            list = _PropertyHelp.Where(p => p.PropertyName == propertyName).ToList();
        }
        if(list.Count() == 1)
        {
            var result = new
            {
                HelpText = list.FirstOrDefault().HelpText,
                PropertyDataType = list.FirstOrDefault().PropertyDataType,
                Tooltip = list.FirstOrDefault().Tooltip,
                ObjectType = list.FirstOrDefault().ObjectType,
                PropertyName = list.FirstOrDefault().PropertyName,
                GroupId = list.FirstOrDefault().GroupId,
                GroupName = list.FirstOrDefault().GroupName,
                Disable = list.FirstOrDefault().Disable
            };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var result = new { Result = list };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        //return PartialView(list);
    }
    
    /// <summary>Quick help.</summary>
    ///
    /// <param name="entName">   Name of the ent.</param>
    /// <param name="propName">  Name of the property.</param>
    /// <param name="ObjectType">Type of the object.</param>
    ///
    /// <returns>A response stream to send to the QuickHelp View.</returns>
    [Audit(0)]
    public ActionResult QuickHelp(string entName, string propName, string ObjectType)
    {
        //How Many entity are envolve help
        var lstEntity = from ent in db.EntityPages
                        where ent.Disable == false
                        select ent;
        //
        var _Entity = lstEntity.GetFromCache<IQueryable<EntityPage>,EntityPage>().ToList();
        Dictionary<string, string> dicAllEntityName = new Dictionary<string, string>();
        foreach(var item in _Entity)
        {
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == item.DisplayValue);
            if(Entity.Name == item.DisplayValue)
                dicAllEntityName.Add(item.DisplayValue, Entity.DisplayName);
        }
        ViewBag.AllEntityForHelp = new SelectList(dicAllEntityName.ToList().OrderBy(o => o.Value), "Key", "Value");
        if(_Entity.Where(p => p.EntityName == entName).Count() == 0)
            return View();
        // Get All Help Of an entity bysection
        var lstentityHelp = from entHelp in db.EntityHelpPages
                            where entHelp.Disable == false
                            select entHelp;
        //
        //Get All Help Of Property of an entity
        var listPropertyHelp = from propHelp in db.PropertyHelpPages
                               where propHelp.Disable == false && propHelp.EntityName == entName
                               select propHelp;
        //
        // Get Enabled Business Rules of Entity
        var lstBR = from br in User.businessrules
                    where br.Disable == false && br.EntityName == entName
                    select br;
        //
        ViewBag.ObjectType = ObjectType;
        var _PropertyHelp = listPropertyHelp.ToList();
        var _EntHelp = lstentityHelp.ToList();
        var _BR = lstBR.ToList();
        var brCount = _BR.Count();
        var entHelpCount = _EntHelp.Where(e => e.EntityName == entName).Count();
        var entCount = _Entity.Count();
        IPagedList<PropertyHelpPage> list = _PropertyHelp.ToPagedList(1, 1000);
        ViewBag.AllEntityForHelp = new SelectList(dicAllEntityName.ToList().OrderBy(o => o.Value), "Key", "Value");
        ViewBag.HasEntHelp = _EntHelp.All(p => (p.SectionText == null || p.SectionText == "<p><br></p>") && string.IsNullOrEmpty(p.SectionName));
        ViewBag.HasProperyHelp = _PropertyHelp.Where(p => p.ObjectType.ToLower() != "verb").All(p => (p.HelpText == null || p.HelpText == "<p><br></p>") && string.IsNullOrEmpty(p.Tooltip));
        ViewBag.HasActionHelp = _PropertyHelp.Where(p => p.ObjectType.ToLower() == "verb").All(p => (p.HelpText == null || p.HelpText == "<p><br></p>") && string.IsNullOrEmpty(p.Tooltip));
        ViewBag.BRCount = brCount;
        ViewBag.EntityCount = brCount;
        ViewBag.SummaryTabCount = entHelpCount;
        if(list.Count() == 0 || entCount == 0)
            return View();
        if(ObjectType == "Group")
        {
            int? propertyvalue = Convert.ToInt32(propName);
            propName = _PropertyHelp.Where(p => p.GroupId == propertyvalue).FirstOrDefault().GroupName.ToString();
        }
        ViewBag.propName = propName;
        return View(list);
    }
    
    /// <summary>Quick entity help.</summary>
    ///
    /// <param name="entName">Name of the ent.</param>
    ///
    /// <returns>A string.</returns>
    [Audit(0)]
    public string QuickEntityHelp(string entName)
    {
        var listEntityHelp = from s in db.EntityHelpPages
                             select s;
        var _EntityHelp = listEntityHelp.Where(p => p.EntityName == entName && p.Disable == false).GetFromCache<IQueryable<EntityHelpPage>,EntityHelpPage>().ToList().OrderBy(p => p.Order);
        string EntityHelpstr = "";
        foreach(var item in _EntityHelp)
        {
            EntityHelpstr += "<dl class='PropertyGroup' style='overflow-y:auto;'><dt>" + item.SectionName + "</dt><dd>" + item.SectionText + "</dd></dl>";
        }
        return EntityHelpstr;
    }
    
    /// <summary>Quick help line break rule.</summary>
    ///
    /// <param name="entName">Name of the ent.</param>
    ///
    /// <returns>A string.</returns>
    
    public string QuickHelpBRRule(string entName)
    {
        var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == entName);
        var EntityDisplayName = Entity.DisplayName;
        BusinessRuleContext br = new BusinessRuleContext();
        var _EntityBrObj = br.BusinessRules.Include(p => p.ruleaction).Where(p => p.EntityName == entName && !p.Disable).ToList().GroupBy(p => p.AssociatedBusinessRuleTypeID).OrderBy(g => g.Key);
        if(_EntityBrObj.Count() == 0)
            return "There is no business rules for entity " + Entity.DisplayName;
        string EntityBrstr = "";
        foreach(var groupBr in _EntityBrObj)
        {
            if(groupBr.Any(phpBr => !String.IsNullOrEmpty(phpBr.DisplayValue)))
            {
                var groupBrHelp = groupBr.FirstOrDefault(php => php.AssociatedBusinessRuleTypeID != 0);
                //string EntityBrstrGroupHeader = "<dt>" + groupBrHelp.associatedbusinessruletype.DisplayValue + "</dt>";
                string header = groupBrHelp.ruleaction.FirstOrDefault().ActionName;
                string EntityBrstrGroupHeader = "<dt>" + header + "</dt>";
                EntityBrstr += "<dl class='PropertyGroup' style='overflow-y:auto;'>";
                EntityBrstr += EntityBrstrGroupHeader;
                foreach(var item in groupBr)
                {
                    EntityBrstr += "<dd><b>Rule No.</b> : " + item.Id + "</dd>" + "<dd><b>Rule Name : </b>" + item.RuleName + "</dd><dd><b>Roles : </b>" + item.Roles + "</dd>";
                    EntityBrstr += "<dd><b>Description :</b>" + item.Description + "</dd><hr>";
                }
                EntityBrstr += "</dl>";
            }
        }
        return (EntityBrstr);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) saves a property help page for help
    /// text.</summary>
    ///
    /// <param name="propertyhelp">The propertyhelp.</param>
    ///
    /// <returns>A response stream to send to the SavePropertyHelpPageForHelpText View.</returns>
    
    [HttpPost]
    public ActionResult SavePropertyHelpPageForHelpText([Bind(Include = "Id,ConcurrencyKey,Tooltip,HelpText,PropertyName,PropertyDataType,ObjectType,EntityName,PropertyHelpOfEntityID")] PropertyHelpPage propertyhelp)
    {
        var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == propertyhelp.EntityName);
        //instruction feature
        if(propertyhelp.ObjectType == "Instruction")
        {
            var lstInstructionHelp = db.PropertyHelpPages.Where(p => p.ObjectType == "Instruction" && p.PropertyName == propertyhelp.PropertyName && p.PropertyHelpOfEntityID == propertyhelp.PropertyHelpOfEntityID);
            if(lstInstructionHelp.Count() > 0)
            {
                var lstPropertyPage = from s in db.PropertyHelpPages
                                      select s;
                var propObj = lstPropertyPage.Where(p => p.Id == propertyhelp.Id).FirstOrDefault();
                propObj.HelpText = propertyhelp.HelpText;
                db.Entry(propObj).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        var propItm = entList.Properties.Where(p => p.Proptype == "Group" && (p.Name == propertyhelp.PropertyName || p.PropName == propertyhelp.PropertyName)).ToList();
        if(propertyhelp.Id == 0)
        {
            var lstEntityPage = from s in db.EntityPages
                                select s;
            var entobj = lstEntityPage.Where(p => p.EntityName == propertyhelp.EntityName).ToList();
            if(entobj.Count() > 0)
                propertyhelp.Id = entobj.FirstOrDefault().Id;
            PropertyHelpPage propertyhelppageForNewRec = new PropertyHelpPage();
            propertyhelppageForNewRec.PropertyDataType = propertyhelp.PropertyDataType;
            propertyhelppageForNewRec.HelpText = propertyhelp.HelpText;
            propertyhelppageForNewRec.PropertyName = propertyhelp.PropertyName;
            propertyhelppageForNewRec.PropertyHelpOfEntityID = propertyhelp.Id;
            propertyhelppageForNewRec.EntityName = propertyhelp.EntityName;
            propertyhelppageForNewRec.Tooltip = propertyhelp.Tooltip;
            propertyhelppageForNewRec.ObjectType = propertyhelp.ObjectType;
            propertyhelppageForNewRec.Disable = false;
            if(propItm.Count() > 0)
            {
                propertyhelppageForNewRec.GroupId = Convert.ToInt32(propItm.FirstOrDefault().PropName);
                propertyhelppageForNewRec.GroupName = propItm.FirstOrDefault().PropText;
            }
            db.PropertyHelpPages.Add(propertyhelppageForNewRec);
            db.SaveChanges();
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var lstPropertyPage = from s in db.PropertyHelpPages
                                  select s;
            var propObj = lstPropertyPage.Where(p => p.Id == propertyhelp.Id).FirstOrDefault();
            propObj.HelpText = propertyhelp.HelpText;
            if(propItm.Count() > 0)
            {
                propObj.GroupId = Convert.ToInt32(propItm.FirstOrDefault().PropName);
                propObj.GroupName = propItm.FirstOrDefault().PropText;
            }
            db.Entry(propObj).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        var result = new { Result = "Faild", Id = propertyhelp.Id };
        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Enumerates validate model with errors in this collection.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process validate model with errors in
    /// this collection.</returns>
    
    private IEnumerable<ValidationResult> ValidateModelWithErrors(PropertyHelpPage validate)
    {
        ICollection<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), results, true);
        return results;
    }
    
    /// <summary>Validates the model described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ValidateModel(PropertyHelpPage validate)
    {
        return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null, true);
    }
    
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}