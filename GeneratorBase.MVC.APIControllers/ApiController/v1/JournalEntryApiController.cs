using GeneratorBase.MVC.Controllers;
using GeneratorBase.MVC.Models;
using GeneratorBase.MVC.Models.SearchOptions;
using Microsoft.Web.Http;
using NSwag.Annotations;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
namespace GeneratorBase.MVC.ApiControllers.v1
{
/// <summary>A controller for handling journal entries.</summary>
[AuthorizationRequired]
[ApiVersion("1.0")]
public class JournalEntryController : ApiBaseControllerv1
{
    /// <summary>Gets a task&lt; i HTTP action result&gt; using the given identifier.</summary>
    ///
    /// <param name="options">The options to get.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpGet]
    [Route("api/v{version:apiVersion}/JournalEntry")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(PaginatedList<JournalEntry>))]
    public async Task<IHttpActionResult> Get([FromUri]JournalEntrySearchOptions options)
    {
        var list = db.JournalEntries.Select(s => s);
        list = SearchInternal(list, options);
        return await Page(db.JournalEntries, options.Page.Value, options.PageSize.Value);
    }
    
    /// <summary>Gets a task&lt; i HTTP action result&gt; using the given identifier.</summary>
    ///
    /// <param name="id">The Identifier to get.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [Route("api/v{version:apiVersion}/JournalEntry/{id:long}")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(JournalEntry))]
    [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
    public async Task<IHttpActionResult> Get(int id)
    {
        var _journalentry = await db.JournalEntries.FirstOrDefaultAsync(s => s.Id == id);
        if(_journalentry != null)
            return Ok(_journalentry);
        return NotFound();
    }
    
    /// <summary>Gets.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    [Route("api/v{version:apiVersion}/JournalEntry")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(JournalEntry))]
    [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
    public IHttpActionResult Get(string EntityName)
    {
        var _journalentry = db.JournalEntries.Where(p => p.EntityName == EntityName).ToList();
        if(_journalentry != null)
            return Ok(_journalentry);
        return NotFound();
    }
    
    /// <summary>Searches for the first internal.</summary>
    ///
    /// <param name="list">The list.</param>
    /// <param name="opt"> The option.</param>
    ///
    /// <returns>The found internal.</returns>
    
    private IQueryable<JournalEntry> SearchInternal(IQueryable<JournalEntry> list, JournalEntrySearchOptions opt)
    {
        if(!String.IsNullOrEmpty(opt.Search))
        {
            list = Filter(list, opt.Search, opt.DeepSearch);
        }
        if(!String.IsNullOrEmpty(opt.entityname))
        {
            list = list.Where(p=>p.EntityName == opt.entityname);
        }
        if(opt.recordid.HasValue)
        {
            list = list.Where(p => p.RecordId == opt.recordid);
        }
        if(!String.IsNullOrEmpty(opt.propertyname))
        {
            list = list.Where(p => p.PropertyName == opt.propertyname);
        }
        if(!String.IsNullOrEmpty(opt.Order))
        {
            list = Sort(list, opt.Order, opt.Ascending.Value);
        }
        else list = list.OrderByDescending(c => c.Id);
        return list;
    }
    
    /// <summary>Filters.</summary>
    ///
    /// <param name="list">        The list.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="deepSearch">  The deep search.</param>
    ///
    /// <returns>An IQueryable&lt;JournalEntry&gt;</returns>
    
    private IQueryable<JournalEntry> Filter(IQueryable<JournalEntry> list, string searchString, bool? deepSearch)
    {
        if(String.IsNullOrEmpty(searchString)) return list;
        searchString = searchString.ToUpper().Trim();
        if(deepSearch ?? false)
            list = list.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (s.UserName != null && (s.UserName.ToUpper().Contains(searchString))));
        else
            list = list.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (s.UserName != null && (s.UserName.ToUpper().Contains(searchString))));
        return list;
    }
    
}
}