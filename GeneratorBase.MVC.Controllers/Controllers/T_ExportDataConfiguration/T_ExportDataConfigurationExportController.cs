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
public partial class T_ExportDataConfigurationController : BaseController
{
    [HttpGet]
    public ActionResult ExportData(string EntityName, int id = 0)
    {
        ViewData["EntityName"] = EntityName;
        ViewBag.ExportId = id;
        var objList = ExportAllData(EntityName, id);
        return View("~/Views/Shared/ExportData.cshtml", objList);
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
            var uniqueid = new String(chkcollection[i].Where(Char.IsDigit).ToArray());
            var keys = keyval.Split('?').Where(wh => !(string.IsNullOrEmpty(wh))).ToList();
            if(keys.Count() > 3)
            {
                objdata.T_ParentEntity = keys[0].Trim();
                objdata.T_ChildEntity = keys[1].Trim();
                objdata.T_AssociationName = keys[2].Trim();
                objdata.T_IsNested = Convert.ToBoolean(keys[3]);
                //objdata.T_Hierarchy = collection["hdn_" + objdata.T_AssociationName + uniqueid];
                objdata.T_Hierarchy = collection[chkcollection[i].Replace("chk_", "hdn_")];
            }
            KeyData.Add(objdata);
        }
        return KeyData;
    }
}
}

