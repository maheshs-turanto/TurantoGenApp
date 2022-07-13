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
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for Department actions (helper methods and other actions).</summary>
///
/// <remarks></remarks>
public partial class T_ExportDataDetailsController : BaseController
{
    [HttpGet]
    public ActionResult CreateCriteria(int? id, string EntityName)//mahesh
    {
        ViewBag.Id = id;
        var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => p.Name == EntityName).ToList();
        ViewBag.MainEntity = new SelectList(EntList, "Name", "DisplayName");
        Dictionary<string, string> list = new Dictionary<string, string>();
        ViewBag.Condition = new SelectList(list, "key", "value");
        Dictionary<string, string> PropertyList = new Dictionary<string, string>();
        PropertyList.Add("0", "--Select Property--");
        ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
        ViewBag.PropertyList7 = ViewBag.PropertyList1 = ViewBag.GroupList = new SelectList(PropertyList, "key", "value");
        ViewBag.AssociationList17 = new SelectList(PropertyList, "key", "value");
        ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                          = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                  = ViewBag.SuggestedDynamicValueInCondition7 = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(new Dictionary<string, string>(), "key", "value");
        ViewBag.SuggestedDynamicValue721 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
                                           = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.AssociationPropertyList7
                                                   = ViewBag.SuggestedDynamicValueInCondition72 = ViewBag.SuggestedDynamicValueInCondition721 = new SelectList(list, "key", "value");
        var ConditionItems = new ConditionContext().Conditions.Where(wh => wh.ExportDetailConditionsID == id).OrderBy(O => O.Id);
        if(ConditionItems.Count() > 0)
            ViewBag.ConditionList = ConditionItems.ToList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateCriteria(int? Id, string MainEntity, string lblrulecondition)
    {
        using(var appdb = new ApplicationContext(new SystemUser()))
        {
            using(var conditiondb = new ConditionContext())
            {
                if(!string.IsNullOrEmpty(lblrulecondition))
                {
                    var conditions = lblrulecondition.Split("?".ToCharArray());
                    foreach(var cond in conditions)
                    {
                        if(string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(",".ToCharArray());
                        Condition newcondition = new Condition();
                        newcondition.PropertyName = param[0];
                        newcondition.Operator = param[1];
                        if(param[1] == "Match")
                            newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                        else
                            newcondition.Value = param[2].Replace("&#44;", ",").Replace("&#63;", "?");
                        newcondition.LogicalConnector = param[3];
                        newcondition.DynamicRuleConditionsID = null;
                        newcondition.RuleConditionsID = null;
                        newcondition.ExportDetailConditionsID = Id;
                        conditiondb.Conditions.Add(newcondition);
                        conditiondb.SaveChanges();
                    }
                }
            }
        }
        return Json(new { success = true });
    }
    [HttpPost]
    public ActionResult DeleteCondition(int? Id)
    {
        using(var conditiondb = new ConditionContext())
        {
            var conditions = conditiondb.Conditions.Find(Id);
            conditiondb.Conditions.Remove(conditions);
            conditiondb.SaveChanges();
        }
        return Json(new { success = true });
    }
    
    public ICollection<T_ExportDataDetails> ExportDetails(System.Collections.Specialized.NameValueCollection collection, string EntityName)
    {
        var KeyData = new List<T_ExportDataDetails>();
        var chkcollection = collection.AllKeys.Where(wh => wh.StartsWith("chk_")).ToList();
        for(int i = 0; i < chkcollection.Count(); i++)
        {
            if(!chkcollection[i].StartsWith("chk_")) continue;
            var objdata = new T_ExportDataDetails();
            var keyval = collection[chkcollection[i]];
            var keys = keyval.Split('?').Where(wh => !(string.IsNullOrEmpty(wh))).ToList();
            if(keys.Count() > 3)
            {
                objdata.T_ParentEntity = keys[0].Trim();
                objdata.T_ChildEntity = keys[1].Trim();
                objdata.T_AssociationName = keys[2].Trim();
                objdata.T_IsNested = Convert.ToBoolean(keys[3]);
                objdata.T_Hierarchy = collection["hdn_" + objdata.T_AssociationName];
            }
            KeyData.Add(objdata);
        }
        return KeyData;
    }
}
}

