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
namespace GeneratorBase.MVC.ApiControllers
{
/// <summary>A controller for handling documents.</summary>
[AuthorizationRequired]
[Microsoft.Web.Http.ApiVersionNeutral]
public class DocumentController : ApiBaseController
{
    /// <summary>GET api/DocumentApi.</summary>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    [EnableQuery]
    public HttpResponseMessage Get()
    {
        var _Document = db.Documents;
        var objList = _Document as List<Document> ?? _Document.ToList();
        if(objList.Any())
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Document not found");
    }
    
    /// <summary>GET api/DocumentApi/5.</summary>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    public HttpResponseMessage Get(long id)
    {
        var obj = db.Documents.Find(id);
        if(obj != null)
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Document found for this id");
    }
    
    /// <summary>POST api/DocumentApi.</summary>
    ///
    /// <param name="Document">The document.</param>
    ///
    /// <returns>A HttpResponseMessage.</returns>
    
    public HttpResponseMessage Post([FromBody] Document Document)
    {
        Document.DateCreated = Document.DateLastUpdated = DateTime.UtcNow;
        db.Documents.Add(Document);
        db.SaveChanges();
        return Request.CreateResponse(HttpStatusCode.OK, Document.Id);
    }
    
    /// <summary>PUT api/DocumentApi/5.</summary>
    ///
    /// <param name="id">      The Identifier to delete.</param>
    /// <param name="Document">The document.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool Put(int id, [FromBody] Document Document)
    {
        if(id > 0)
        {
            Document.DateLastUpdated = DateTime.UtcNow;
            db.Entry(Document).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }
        return false;
    }
    
    /// <summary>DELETE api/DocumentApi/5.</summary>
    ///
    /// <param name="id">The Identifier to delete.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool Delete(int id)
    {
        if(id > 0)
        {
            Document Document = db.Documents.Find(id);
            db.Entry(Document).State = EntityState.Deleted;
            db.Documents.Remove(Document);
            db.SaveChanges();
            return true;
        }
        return false;
    }
    
    /// <summary>Deletes the document described by docID.</summary>
    ///
    /// <param name="docID">Identifier for the document.</param>
    
    protected void DeleteDocument(long? docID)
    {
        var document = db.Documents.Find(docID);
        db.Entry(document).State = EntityState.Deleted;
        db.Documents.Remove(document);
        db.SaveChanges();
    }
    
    /// <summary>Deletes the image gallery document described by docIDs.</summary>
    ///
    /// <param name="docIDs">The document i ds.</param>
    
    protected void DeleteImageGalleryDocument(string docIDs)
    {
        if(!String.IsNullOrEmpty(docIDs))
        {
            string[] strDocIds = docIDs.Split(',');
            foreach(string strDocId in strDocIds)
            {
                var document = db.Documents.Find(Convert.ToInt64(strDocId));
                db.Entry(document).State = EntityState.Deleted;
                db.Documents.Remove(document);
                db.SaveChanges();
            }
        }
    }
}
}
