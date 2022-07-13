using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AttributeRouting.Web.Http;
using GeneratorBase.MVC.Models;
using GeneratorBase.MVC.Controllers;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System;
using System.Web.Http.OData;
using System.Data.Entity.SqlServer;
namespace GeneratorBase.MVC.ApiControllers
{
/// <summary>A controller for handling journal entries.</summary>
[AuthorizationRequired]
[Microsoft.Web.Http.ApiVersionNeutral]
public class JournalEntryController : ApiBaseController
{
    //private JournalEntryContext JournalEntryDB = new JournalEntryContext();
    
    /// <summary>Gets.</summary>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    [EnableQuery]
    public HttpResponseMessage Get()
    {
        //JournalEntryContext JournalEntryDB = new JournalEntryContext(User);
        var list = db.JournalEntries.ToList();
        var objList = list as List<JournalEntry> ?? list.ToList();
        if(objList.Any())
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "JournalEntry not found");
    }
    
    /// <summary>Gets.</summary>
    ///
    /// <param name="id">The Identifier to get.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    public HttpResponseMessage Get(long id)
    {
        //JournalEntryContext JournalEntryDB = new JournalEntryContext(User);
        var obj = db.JournalEntries.Find(id);
        if(obj != null)
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No JournalEntry found for this id");
    }
    
    /// <summary>Gets.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    public HttpResponseMessage Get(string EntityName)
    {
        //JournalEntryContext JournalEntryDB = new JournalEntryContext(User);
        var objList = db.JournalEntries.Where(p => p.EntityName == EntityName).ToList();
        if(objList.Any())
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No JournalEntry found");
    }
    
    /// <summary>Gets by record identifier.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="recordid">  The recordid.</param>
    ///
    /// <returns>The by record identifier.</returns>
    
    public HttpResponseMessage GetByRecordId(string EntityName,long recordid)
    {
        // JournalEntryContext JournalEntryDB = new JournalEntryContext(User);
        var objList = db.JournalEntries.Where(p => p.EntityName == EntityName && p.RecordId == recordid).ToList();
        if(objList.Any())
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No JournalEntry found");
    }
    
    /// <summary>Gets.</summary>
    ///
    /// <param name="skip">           The skip.</param>
    /// <param name="take">           The take.</param>
    /// <param name="orderKey">       The order key.</param>
    /// <param name="searchKey">      The search key.</param>
    /// <param name="hostingentity">  The hostingentity.</param>
    /// <param name="hostingentityid">The hostingentityid.</param>
    /// <param name="associatedtype"> The associatedtype.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    public HttpResponseMessage Get(Int32 skip, Int32 take, string orderKey, string searchKey, string hostingentity, string hostingentityid, string associatedtype)
    {
        // JournalEntryContext JournalEntryDB = new JournalEntryContext(User);
        var list = db.JournalEntries.OrderByDescending(o => o.Id).Skip(skip).Take(take);
        var objList = list.ToList();
        if(!string.IsNullOrEmpty(searchKey))
            objList = searchRecords(db.JournalEntries, searchKey, true).OrderByDescending(o => o.Id).Skip(skip).Take(take).ToList();
        if(objList.Any())
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No City found");
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lst">         The list.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="IsDeepSearch">The is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<JournalEntry> searchRecords(IQueryable<JournalEntry> lst, string searchString, bool? IsDeepSearch)
    {
        if(string.IsNullOrEmpty(searchString)) return lst;
        searchString = searchString.Trim();
        if(Convert.ToBoolean(IsDeepSearch))
            lst = lst.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (s.UserName != null && (s.UserName.ToUpper().Contains(searchString))));
        else
            lst = lst.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (s.UserName != null && (s.UserName.ToUpper().Contains(searchString))));
        return lst;
    }
}
}

