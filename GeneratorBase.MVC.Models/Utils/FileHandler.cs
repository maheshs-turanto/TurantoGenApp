using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
namespace GeneratorBase.MVC.Models
{
/// <summary>Document Helper class - has methods to perform some document(blob) related things.</summary>
public static class DocumentHelper
{
    /// <summary>Gets string to save in document table</summary>
    ///
    /// <returns>Id of Document table</returns>
    public static long SaveDocument(ApplicationContext db, string content, string filename, string extension, string MimeType)
    {
        Document document = new Document();
        document.DocumentName = filename;
        byte[] fileData = Encoding.ASCII.GetBytes(content); ;
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = extension;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.MIMEType = MimeType;
        document.Byte = fileData;
        db.Documents.Add(document);
        db.SaveChanges();
        return document.Id;
    }
    /// <summary>Saves file in document table</summary>
    ///
    /// <returns>Id of Document </returns>
    public static long SaveDocumentWithByte(ApplicationContext db, string ext, string camfilename, string filepath, string MimeType)
    {
        System.IO.FileStream stream = System.IO.File.OpenRead(filepath);
        byte[] filebyte = new byte[stream.Length];
        stream.Read(filebyte, 0, filebyte.Length);
        stream.Close();
        long ContentLength = filebyte.Length;
        int len = filebyte.Length;
        string fileExt = "";
        string filename = "";
        long fileSize = 0;
        Document document = new Document();
        document.DocumentName = filename;
        filename = camfilename;
        fileExt = ext;
        fileSize = ContentLength;
        byte[] fileData = null;
        using(var binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(filebyte)))
        {
            fileData = binaryReader.ReadBytes(len);
        }
        document.DocumentName = filename;
        document.DateCreated = System.DateTime.UtcNow.Date;
        document.DateLastUpdated = System.DateTime.UtcNow.Date;
        document.FileExtension = fileExt;
        document.DisplayValue = filename;
        document.FileName = filename;
        document.FileSize = fileSize;
        document.MIMEType = MimeType;
        document.Byte = fileData;
        db.Documents.Add(document);
        db.SaveChanges();
        return document.Id;
    }
    /// <summary>Gets a dictionary for DocumentId and DocumentName (used in index.cshtml pages)</summary>
    ///
    /// <returns>Dictionary<long, string></returns>
    public static Dictionary<long, string> GetDocumentNames(List<long> ids, GeneratorBase.MVC.Models.IUser user)
    {
        using(var db = (new ApplicationContext(new SystemUser())))
        {
            Dictionary<long, string> doc = db.Documents.Where(p => ids.Contains(p.Id)).Select(p => new
            {
                p.Id, p.DocumentName
            }).AsEnumerable().ToDictionary(kvp => kvp.Id, kvp => kvp.DocumentName); ;
            return doc;
        }
    }
    public static bool IsAlphaNumeric(this string str,string RegEx)
    {
        if(string.IsNullOrEmpty(str))
            return false;
        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(RegEx);//^[a-zA-Z0-9]*$ --without whitespace
        return r.IsMatch(str);
    }
}
/// <summary>A file handler, Handler to restrict file downloading based on user's permission.</summary>
public class FileHandler : IHttpHandler, IReadOnlySessionState
{
    /// <summary>Gets a value indicating whether another request can use the
    /// <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
    ///
    /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise,
    /// false.</value>
    bool IHttpHandler.IsReusable
    {
        get
        {
            return false;
        }
    }
    /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the
    /// <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
    ///
    /// <param name="context">  An <see cref="T:System.Web.HttpContext" /> object that provides
    ///                         references to the intrinsic server objects (for example, Request,
    ///                         Response, Session, and Server) used to service HTTP requests.</param>
    void IHttpHandler.ProcessRequest(HttpContext context)
    {
        if(!context.User.Identity.IsAuthenticated)
            return;
        var url = context.Request.RawUrl;
        if(!GetAuthorizedFiles((IUser)context.User).Contains(url.Substring(url.LastIndexOf("/") + 1)))
        {
            var context1 = new System.Web.Routing.RequestContext(
                new HttpContextWrapper(System.Web.HttpContext.Current),
                new System.Web.Routing.RouteData());
            var urlHelper = new System.Web.Mvc.UrlHelper(context1);
            var url1 = urlHelper.Action("Index", "Error");
            System.Web.HttpContext.Current.Response.Redirect(url1);
            return;
        }
        //var filExtension = GettingExtension(context.Request.RawUrl);
        context.Response.ClearContent();
        context.Response.ClearHeaders();
        //context.Response.ContentType = MIMEType.Get(filExtension);
        context.Response.AddHeader("Content-Disposition", "attachment");
        context.Response.WriteFile(context.Request.RawUrl);
        context.Response.Flush();
    }
    /// <summary>Getting extension.</summary>
    ///
    /// <param name="rawUrl">Raw URL.</param>
    ///
    /// <returns>A string.</returns>
    public string GettingExtension(string rawUrl)
    {
        return rawUrl.Substring(rawUrl.LastIndexOf(".", System.StringComparison.Ordinal));
    }
    /// <summary>Gets authorized files.</summary>
    ///
    /// <param name="user">The user.</param>
    ///
    /// <returns>The authorized files.</returns>
    private List<string> GetAuthorizedFiles(IUser user)
    {
        List<string> list = new List<string>();
        using(ApplicationContext db = new ApplicationContext(user))
        {
            //list.AddRange(db.FileDocuments.Select(p => p.AttachDocument));
        }
        using(ApplicationDbContext dbUser = new ApplicationDbContext(user))
        {
        }
        return list;
    }
}
/// <summary>
/// Summary description for MIMEType,
/// This code is the organized version of the answer posted by Sameul Neff
/// on stack over flow Check here http://stackoverflow.com/a/4259616/1142645
/// </summary>
public class MIMEType
{
    #region MIME type list
    private static readonly Dictionary<String, String> MimeTypeDict = new Dictionary<String, String>()
    {
        {      "bin", "application/octet-stream" },
        {      "png", "image/png" },
        {      "pdf", "application/pdf" },
        {     "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        {     "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };
    #endregion
    #region Get
    /// <summary>
    /// Returns the mime type for the requested file extension. Returns
    /// the default application/octet-stream if the extension is not found.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static String Get(String extension)
    {
        return Get(extension, MimeTypeDict["bin"]);
    }
    /// <summary>
    /// Returns the mime type for the requested file extension. Returns the
    /// specified defaultMimeType if the extension is not found.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="defaultMimeType"></param>
    /// <returns></returns>
    public static String Get(String extension, String defaultMimeType)
    {
        if(extension.StartsWith("."))
            extension = extension.Remove(0, 1);
        if(MimeTypeDict.ContainsKey(extension))
            return MimeTypeDict[extension];
        else
            return defaultMimeType;
    }
    #endregion Get
}
}

