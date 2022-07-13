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
/// <summary>A controller for handling documents.</summary>
[AuthorizationRequired]
[ApiVersion("1.0")]
public partial class DocumentController : ApiBaseControllerv1
{
    /// <summary>Gets a task&lt; i HTTP action result&gt; using the given identifier.</summary>
    ///
    /// <param name="options">Options for controlling the operation.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpGet]
    [Route("api/v{version:apiVersion}/Document")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(PaginatedList<Document>))]
    public async Task<IHttpActionResult> Get([FromUri]DocumentSearchOptions options)
    {
        var list = db.Documents.Select(s => s);
        list = SearchInternal(list, options);
        return await Page(db.Documents, options.Page.Value, options.PageSize.Value);
    }
    
    /// <summary>Gets a task&lt; i HTTP action result&gt; using the given identifier.</summary>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [Route("api/v{version:apiVersion}/Document/{id:long}", Name = "GetDocumentById")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(Document))]
    [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
    public async Task<IHttpActionResult> Get(int id)
    {
        var _document = await db.Documents.FirstOrDefaultAsync(s => s.Id == id);
        if(_document != null)
            return Ok(_document);
        return NotFound();
    }
    
    /// <summary>GET api/Document/display.</summary>
    ///
    /// <param name="q">   (Optional) The question text.</param>
    /// <param name="page">(Optional) The page.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpGet]
    [Route("api/v{version:apiVersion}/Document/display")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(PaginatedList<DisplayItem>))]
    [Description("A light-weight, paged list of entities whose DisplayValue starts with the given search term.")]
    public async Task<IHttpActionResult> Display(string q = null, int? page = 1)
    {
        var list = db.Documents.Where(f => String.IsNullOrEmpty(q) || f.FileName.StartsWith(q))
                   .OrderBy(f => f.DisplayValue)
                   .Select(s => new DisplayItem()
        {
            Id = s.Id, Text = s.DisplayValue
        });
        return await Page(list, page.Value);
    }
    
    /// <summary>(An Action that handles HTTP GET requests) counts the given options.</summary>
    ///
    /// <param name="options">Options for controlling the operation.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [HttpGet]
    [Route("api/v{version:apiVersion}/Document/count")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(int))]
    public async Task<IHttpActionResult> Count([FromUri]DocumentSearchOptions options)
    {
        var list = db.Documents.Select(s => s);
        list = SearchInternal(list, options);
        var count = await list.CountAsync();
        return Ok(count);
    }
    
    /// <summary>Searches for the first internal.</summary>
    ///
    /// <param name="list">The list.</param>
    /// <param name="opt"> The option.</param>
    ///
    /// <returns>The found internal.</returns>
    
    private IQueryable<Document> SearchInternal(IQueryable<Document> list, DocumentSearchOptions opt)
    {
        if(!String.IsNullOrEmpty(opt.Search))
        {
            list = Filter(list, opt.Search, opt.DeepSearch);
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
    /// <returns>An IQueryable&lt;Document&gt;</returns>
    
    private IQueryable<Document> Filter(IQueryable<Document> list, string searchString, bool? deepSearch)
    {
        if(String.IsNullOrEmpty(searchString)) return list;
        searchString = searchString.ToUpper().Trim();
        if(deepSearch ?? false)
            list = list.Where(s => (s.FileName != null && (s.FileName.ToUpper().Contains(searchString))));
        else
            list = list.Where(s => (s.FileName != null && (s.FileName.ToUpper().Contains(searchString))));
        return list;
    }
    
    /// <summary>POST api/DocumentApi.</summary>
    ///
    /// <param name="_document">The document.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [Route("api/v{version:apiVersion}/Document")]
    [SwaggerResponse(HttpStatusCode.Created, typeof(Document))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void))]
    [SwaggerResponse(HttpStatusCode.Conflict, typeof(void))]
    public async Task<IHttpActionResult> Post(Document _document)
    {
        if(_document == null) return BadRequest();
        if(_document.Id > 0) return Conflict();
        _document.DateCreated = _document.DateLastUpdated = DateTime.UtcNow;
        db.Documents.Add(_document);
        db.SaveChanges();//SaveChangesAsync will not call base.SaveChanges of context, it has lots of validations
        return CreatedAtRoute("GetDocumentById", new { id = _document.Id }, _document);
    }
    
    /// <summary>PUT api/DocumentApi/5.</summary>
    ///
    /// <param name="id">       The Identifier to delete.</param>
    /// <param name="_document">The document.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [Route("api/v{version:apiVersion}/Document/{id:long}")]
    [SwaggerResponse(HttpStatusCode.OK, typeof(Document))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void))]
    public async Task<IHttpActionResult> Put(int id, Document _document)
    {
        if(_document == null) return BadRequest();
        if(_document.Id == 0) return BadRequest();
        _document.DateLastUpdated = DateTime.UtcNow;
        db.Entry(_document).State = EntityState.Modified;
        db.SaveChanges();//SaveChangesAsync will not call base.SaveChanges of context, it has lots of validations
        return Ok(_document);
    }
    
    /// <summary>DELETE api/DocumentApi/5.</summary>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>An asynchronous result that yields an IHttpActionResult.</returns>
    
    [Route("api/v{version:apiVersion}/Document/{id:long}")]
    [SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void))]
    public async Task<IHttpActionResult> Delete(int id)
    {
        if(id == 0) return BadRequest();
        var _document = await db.Documents.FirstOrDefaultAsync(s => s.Id == id); // should be FindAsync
        if(_document == null) return StatusCode(HttpStatusCode.NoContent);
        db.Entry(_document).State = EntityState.Deleted;
        db.Documents.Remove(_document);
        db.SaveChanges();//SaveChangesAsync will not call base.SaveChanges of context, it has lots of validations
        return StatusCode(HttpStatusCode.NoContent);
    }
}
}