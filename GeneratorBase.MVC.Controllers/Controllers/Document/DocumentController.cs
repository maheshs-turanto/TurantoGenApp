using GeneratorBase.MVC.Models;
using ICSharpCode.SharpZipLib.Zip;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MimeDetective;
using Z.EntityFramework.Plus;

namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling documents.</summary>
public partial class DocumentController : BaseController
{
    /// <summary>GET: /Document/Edit/5.</summary>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="defaultview">      The defaultview.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
    {
        if(!User.CanEdit("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Document Document = db.Documents.Find(id);
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["Documentlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.Documents.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityDocumentDisplayValueEdit = TempData["Documentlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["Documentlist"]);
        }
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json.Replace(@"\", ""));
        ViewBag.EntityDocumentDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(Document == null)
        {
            return HttpNotFound();
        }
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/Document/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = Document.DisplayValue, Value = Document.Id.ToString() }));
            ViewBag.EntityDocumentDisplayValueEdit = newList;
            TempData["Documentlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(Document.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = Document.DisplayValue;
                newList[0].Value = Document.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = Document.DisplayValue, Value = Document.Id.ToString() }));
            }
            ViewBag.EntityDocumentDisplayValueEdit = newList;
            TempData["Documentlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["DocumentParentUrl"] = UrlReferrer;
        if(ViewData["DocumentParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/Document") && !Request.UrlReferrer.AbsolutePath.EndsWith("/Document/Edit/" + Document.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/Document/Create"))
            ViewData["DocumentParentUrl"] = Request.UrlReferrer;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        return View(Document);
    }
    
    /// <summary>POST: /Document/Edit/5 To protect from overposting attacks, please enable the
    /// specific properties you want to bind to, for more details see
    /// http://go.microsoft.com/fwlink/?LinkId=317598.</summary>
    ///
    /// <param name="Document">                      The document.</param>
    /// <param name="File_Byte">                     The file byte.</param>
    /// <param name="CamerafileUploadAttachDocument">The camerafile upload attach document.</param>
    /// <param name="UrlReferrer">                   The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] Document Document, HttpPostedFileBase File_Byte, String CamerafileUploadAttachDocument, string UrlReferrer)
    {
        if(File_Byte != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
            IsFileTypeAndSizeAllowed(File_Byte);
        if(ModelState.IsValid)
        {
            string command = Request.Form["hdncommand"];
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_Byte != null)
            {
                Document.FileName = System.IO.Path.GetFileName(File_Byte.FileName.Trim().Replace(" ", ""));
                //UpdateDocument(File_Byte, Document.Id);
                string fileExt = "";
                string filename = "";
                long fileSize = 0;
                //Document document = new Document();
                Document.DocumentName = File_Byte.FileName;
                filename = System.IO.Path.GetFileName(File_Byte.FileName);
                fileExt = System.IO.Path.GetExtension(File_Byte.FileName);
                fileSize = File_Byte.ContentLength;
                byte[] fileData = null;
                using(var binaryReader = new BinaryReader(File_Byte.InputStream))
                {
                    fileData = binaryReader.ReadBytes(File_Byte.ContentLength);
                }
                Document.DocumentName = filename;
                Document.DateCreated = System.DateTime.UtcNow.Date;
                Document.DateLastUpdated = System.DateTime.UtcNow.Date;
                Document.FileExtension = fileExt;
                Document.DisplayValue = filename;
                Document.FileName = filename;
                Document.FileSize = fileSize;
                Document.MIMEType = File_Byte.ContentType;
                Document.Byte = fileData;
            }
            if(Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"])));
                image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                Document.FileName = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
            }
            db.Entry(Document).State = EntityState.Modified;
            db.SaveChanges();
            if(command != "Save")
            {
                if(command == "SaveNextPrev")
                {
                    long NextPreId = Convert.ToInt64(Request.Form["hdnNextPrevId"]);
                    return RedirectToAction("Edit", new { Id = NextPreId, UrlReferrer = UrlReferrer });
                }
                else
                    return RedirectToAction("Edit", new { Id = Document.Id, UrlReferrer = UrlReferrer });
            }
            if(!string.IsNullOrEmpty(UrlReferrer))
            {
                var query = HttpUtility.ParseQueryString(UrlReferrer);
                if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                    return RedirectToAction("Index");
                else
                    return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["Documentlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.Documents.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityDocumentDisplayValueEdit = TempData["Documentlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["Documentlist"]);
        }
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json.Replace(@"\", ""));
        ViewBag.EntityDocumentDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        ViewData["DocumentParentUrl"] = UrlReferrer;
        return View(Document);
    }
    
    /// <summary>GET: /Document/Delete/5.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Document Document = db.Documents.Find(id);
        if(Document == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["DocumentParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/Document"))
            ViewData["DocumentParentUrl"] = Request.UrlReferrer;
        return View(Document);
    }
    
    /// <summary>POST: /Document/Delete/5.</summary>
    ///
    /// <param name="Document">   The document.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(Document Document, string UrlReferrer)
    {
        if(!User.CanDelete("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        //Delete Document
        db.Entry(Document).State = EntityState.Deleted;
        db.Documents.Remove(Document);
        db.SaveChanges();
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        if(ViewData["DocumentParentUrl"] != null)
        {
            string parentUrl = ViewData["DocumentParentUrl"].ToString();
            ViewData["DocumentParentUrl"] = null;
            return Redirect(parentUrl);
        }
        else return RedirectToAction("Index");
        return View(Document);
    }
    
    /// <summary>Sort records.</summary>
    ///
    /// <param name="lstFileDocument">The list file document.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    ///
    /// <returns>The sorted records.</returns>
    
    private IQueryable<Document> sortRecords(IQueryable<Document> lstFileDocument, string sortBy, string isAsc)
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
        ParameterExpression paramExpression = Expression.Parameter(typeof(Document), "document");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<Document>)lstFileDocument.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstFileDocument.ElementType, lambda.Body.Type },
                       lstFileDocument.Expression,
                       lambda));
    }
    
    /// <summary>Searches for the first records.</summary>
    ///
    /// <param name="lstFileDocument">The list file document.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="IsDeepSearch">   The is deep search.</param>
    ///
    /// <returns>The found records.</returns>
    
    private IQueryable<Document> searchRecords(IQueryable<Document> lstFileDocument, string searchString, bool? IsDeepSearch)
    {
        searchString = searchString.Trim();
        lstFileDocument = lstFileDocument.Where(s => (!String.IsNullOrEmpty(s.FileName) && s.FileName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DocumentName) && s.DocumentName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FileType) && s.FileType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)));
        return lstFileDocument;
    }
    
    /// <summary>Indexes.</summary>
    ///
    /// <param name="currentFilter">  A filter specifying the current.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="sortBy">         Describes who sort this object.</param>
    /// <param name="isAsc">          The is ascending.</param>
    /// <param name="page">           The page.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> Type of the associated.</param>
    /// <param name="IsExport">       The is export.</param>
    /// <param name="IsDeepSearch">   The is deep search.</param>
    /// <param name="IsFilter">       A filter specifying the is.</param>
    /// <param name="RenderPartial">  The render partial.</param>
    /// <param name="BulkOperation">  The bulk operation.</param>
    /// <param name="parent">         The parent.</param>
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype.</param>
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     List of is homes.</param>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome, bool? IsHomeList, string HostingEntityName, string FileType)
    {
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["IsFilter"] = IsFilter;
        ViewData["BulkOperation"] = BulkOperation;
        ViewData["caller"] = caller;
        if(!string.IsNullOrEmpty(viewtype))
        {
            viewtype = viewtype.Replace("?IsAddPop=true", "");
            ViewBag.TemplatesName = viewtype;
        }
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstFileDocument = from s in db.Documents
                              select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstFileDocument = searchRecords(lstFileDocument, searchString.ToUpper(), IsDeepSearch);
        }
        if(parent != null && parent == "Home")
        {
            ViewBag.Homeval = searchString;
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
        {
            lstFileDocument = sortRecords(lstFileDocument, sortBy, isAsc);
        }
        else lstFileDocument = lstFileDocument.OrderByDescending(c => c.Id);
        if(!string.IsNullOrEmpty(HostingEntityName))
        {
            string hostName = Convert.ToString(HostingEntityName);
            if(hostName == "NA")
                lstFileDocument = lstFileDocument.Where(p => string.IsNullOrEmpty(p.EntityName));
            else
                lstFileDocument = lstFileDocument.Where(p => p.EntityName == hostName);
        }
        if(!string.IsNullOrEmpty(FileType))
        {
            string hostName = Convert.ToString(FileType);
            lstFileDocument = lstFileDocument.Where(p => p.FileType == hostName);
        }
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "Document"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "Document"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "Document"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "Document"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "Document"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "Document"].Value);
            ViewBag.Pages = pageNumber;
        }
        //
        //
        ViewBag.PageSize = pageSize;
        var _FileDocument = lstFileDocument;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_FileDocument.Count() > 0)
                pageSize = _FileDocument.Count();
            return View("ExcelExport", _FileDocument.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _FileDocument.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            if(string.IsNullOrEmpty(viewtype))
                viewtype = "IndexPartial";
            if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                ViewBag.TemplatesName = viewtype;
            var list = _FileDocument.ToPagedList(pageNumber, pageSize);
            ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["Documentlist"] = list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            });
            return View(list);
        }
        else
        {
            if(BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
            {
                ViewData["BulkAssociate"] = BulkAssociate;
                return View();
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _FileDocument.Count() == 0 ? 1 : _FileDocument.Count();
                    }
                    ViewData["HomePartialList"] = IsHomeList;
                    var list = _FileDocument.ToPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["Documentlist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(Convert.ToBoolean(IsHomeList) ? "HomePartialList" : "IndexPartial", list);
                }
                else
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _FileDocument.Count() == 0 ? 1 : _FileDocument.Count();
                    }
                    var list = _FileDocument.ToPagedList(pageNumber, pageSize);
                    ViewBag.EntityFileDocumentDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["Documentlist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    
    /// <summary>Downloads the given ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileResult.</returns>
    
    public ActionResult Download(long id)
    {
        //check download permission;
        return ExportCode(id);
    }
    public FileResult DownloadAll(string Ids)
    {
        //check download permission;
        if(!String.IsNullOrEmpty(Ids))
        {
            //string[] strDocIds = Ids.Split(',');
            return ExportCode1(Ids);
            //foreach (string strDocId in strDocIds)
            //{
            //    FileContentResult file = ExportCode(Convert.ToInt64(strDocId));
            //    return file;
            //}
        }
        return null;
    }
    /// <summary>Creates a FileContentResult with the given content.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileContentResult.</returns>
    
    protected ActionResult ExportCode(long id)
    {
        var doc = db.Documents.Find(id);
        if(doc == null)
            return null;
        if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "onedrive")
        {
            var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
            string AccessToken = string.Empty;
            if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
            if(AccessToken != null)
            {
                byte[] filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
                return File(filedata, System.Net.Mime.MediaTypeNames.Application.Octet, doc.DocumentName);
            }
            else
                return null;
        }
        else if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "file")
        {
            string filename = doc.FileName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            return File(filedata, "application/force-download", Path.GetFileName(doc.FileName));
        }
        else
        {
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
    private FileContentResult ExportCode1(string Ids)
    {
        BinaryWriter Writer = null;
        byte[] result = null;
        int nRead = 0;
        string[] strDocIds = Ids.Split(',');
        MemoryStream raw = new MemoryStream();
        System.Web.UI.Page sender;
        using(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(raw))
        {
            foreach(string strDocId in strDocIds)
            {
                var doc = db.Documents.Find(Convert.ToInt64(strDocId));
                if(doc == null)
                    return null;
                byte[] fileBytes = doc.Byte;
                string fileName = doc.DisplayValue;
                ZipEntry entry = new ZipEntry(fileName);
                zStream.PutNextEntry(entry);
                zStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }
        var zipped = raw.ToArray();
        return File(zipped, System.Net.Mime.MediaTypeNames.Application.Octet, "Archive.zip");
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A JSON response stream to send to the GetDocumentName action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    [Audit(0)]
    public JsonResult GetDocumentName(long? id)
    {
        var doc = db.Documents.Find(id);
        if(doc == null)
            return Json("NA", JsonRequestBehavior.AllowGet);
        else
            return Json(doc.DocumentName, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Displays an image.</summary>
    ///
    /// <param name="id">       The identifier.</param>
    /// <param name="maxSize">  The maximum size of the.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <param name="maxWidth"> The maximum width.</param>
    
    public void DisplayImage(long id, int? maxSize, int? maxHeight, int? maxWidth)
    {
        //maxSize = 30;
        int height = Math.Min(maxSize ?? Int32.MaxValue, maxHeight ?? Int32.MaxValue);
        int width = Math.Min(maxSize ?? Int32.MaxValue, maxWidth ?? Int32.MaxValue);
        var doc = db.Documents.Find(id);
        if(doc == null)
            return;
        byte[] filedata = null;
        if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "onedrive")
        {
            var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
            string AccessToken = string.Empty;
            if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
            if(AccessToken != null)
            {
                filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
            }
        }
        else if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "file")
        {
            string filename = doc.FileName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
            filedata = System.IO.File.ReadAllBytes(filepath);
        }
        else
        {
            filedata = doc.Byte;
        }
        var wi = new WebImage(filedata);
        wi.Resize(width, height, preventEnlarge: true);
        wi.Write();
    }
    /// <summary>Displays an image.</summary>
    ///
    /// <param name="id">       The identifier.</param>
    /// <param name="maxSize">  The maximum size of the.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <param name="maxWidth"> The maximum width.</param>
    ///
    /// <returns>A FileResult.</returns>
    public FileResult DisplayImageFast(long id, int? maxSize, int? maxHeight, int? maxWidth) //Mahesh
    {
        System.Drawing.ImageConverter imageconverter = new System.Drawing.ImageConverter();
        maxSize = 30;
        int height = Math.Min(maxSize ?? Int32.MaxValue, maxHeight ?? Int32.MaxValue);
        int width = Math.Min(maxSize ?? Int32.MaxValue, maxWidth ?? Int32.MaxValue);
        var doc = db.Documents.Find(id);
        if(doc == null)
            return null;
        System.Drawing.Bitmap wi = (System.Drawing.Bitmap)imageconverter.ConvertFrom(doc.Byte);
        System.Drawing.Bitmap resized = new System.Drawing.Bitmap(wi, new System.Drawing.Size(width, height));
        System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
        var resizedbyte = (byte[])converter.ConvertTo(resized, typeof(byte[]));
        return File(resizedbyte, doc.MIMEType.ToString());
    }
    /// <summary>Displays an image old described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileResult.</returns>
    
    public FileResult DisplayImage_old(long id)
    {
        var doc = db.Documents.Find(id);
        if(doc == null)
            return null;
        byte[] fileBytes = doc.Byte;
        //string fileName = doc.DisplayValue;
        return File(fileBytes, doc.MIMEType.ToString());
    }
    
    /// <summary>Displays an image afterhover described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileResult.</returns>
    
    public FileResult DisplayImageAfterhover(long id)
    {
        var doc = db.Documents.Find(id);
        if(doc == null)
            return null;
        if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "onedrive")
        {
            var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
            string AccessToken = string.Empty;
            if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
            if(AccessToken != null)
            {
                byte[] filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
                return File(filedata, System.Net.Mime.MediaTypeNames.Application.Octet, doc.DocumentName);
            }
            else
                return null;
        }
        else if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "file")
        {
            string filename = doc.FileName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            return File(filedata, "application/force-download", Path.GetFileName(doc.FileName));
        }
        else
        {
            byte[] fileBytes = doc.Byte;
            return File(fileBytes, doc.MIMEType.ToString());
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [Audit(0)]
    public FileResult DisplayProfileImage(long id)
    {
        using(var context = new ApplicationContext(new SystemUser()))
        {
            var doc = context.Documents.Where(d => d.Id == id).GetFromCache<IQueryable<Document>, Document>().FirstOrDefault();
            if(doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            //string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
    }
    /// <summary>Displays an image after click described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileResult.</returns>
    
    public FileResult DisplayImageAfterClick(long id)
    {
        var doc = db.Documents.Find(id);
        if(doc == null)
            return null;
        byte[] fileBytes = doc.Byte;
        //string fileName = doc.DisplayValue;
        return File(fileBytes, doc.MIMEType.ToString());
    }
    
    /// <summary>Displays an image mobile described by ID.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A FileResult.</returns>
    [Audit(0)]
    public FileResult DisplayImageMobile(long id)
    {
        using(var context = new ApplicationContext(new SystemUser()))
        {
            var doc = context.Documents.Where(d => d.Id == id).GetFromCache<IQueryable<Document>, Document>().FirstOrDefault();
            if(doc == null)
                return null;
            if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "onedrive")
            {
                var OfficeAccessSession = CommonFunction.Instance.OneDrive(User);
                string AccessToken = string.Empty;
                if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                    AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
                if(AccessToken != null)
                {
                    byte[] filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
                    return File(filedata, System.Net.Mime.MediaTypeNames.Application.Octet, doc.DocumentName);
                }
                else
                    return null;
            }
            else if(!string.IsNullOrEmpty(doc.FileType) && doc.FileType.ToLower() == "file")
            {
                string filename = doc.FileName;
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                return File(filedata, "application/force-download", Path.GetFileName(doc.FileName));
            }
            else
            {
                byte[] fileBytes = doc.Byte;
                return File(fileBytes, doc.MIMEType.ToString());
            }
        }
    }
    
    /// <summary>Displays an image mobile list.</summary>
    ///
    /// <param name="id">       The identifier.</param>
    /// <param name="maxSize">  The maximum size of the.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <param name="maxWidth"> The maximum width.</param>
    
    public void DisplayImageMobileList(long id, int? maxSize, int? maxHeight, int? maxWidth)
    {
        maxSize = 85;
        int height = Math.Min(maxSize ?? Int32.MaxValue, maxHeight ?? Int32.MaxValue);
        int width = Math.Min(maxSize ?? Int32.MaxValue, maxWidth ?? Int32.MaxValue);
        var doc = db.Documents.Find(id);
        if(doc == null)
            return;
        var wi = new WebImage(doc.Byte);
        wi.Resize(width, height, preventEnlarge: true);
        wi.Write();
    }
    
    /// <summary>Gets display value.</summary>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>The display value.</returns>
    
    public string GetDisplayValue(string id)
    {
        if(string.IsNullOrEmpty(id)) return "";
        long idvalue = Convert.ToInt64(id);
        using(var context = (new ApplicationContext(new SystemUser())))
        {
            var _Obj = context.Documents.FirstOrDefault(p => p.Id == idvalue);
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
        //return _Obj;
    }
    
    /// <summary>Deletes the document described by docID.</summary>
    ///
    /// <param name="docID">Identifier for the document.</param>
    public ActionResult DocumentDeassociate(long? docid)
    {
        if(!User.CanDelete("Document"))
        {
            return RedirectToAction("Index", "Error");
        }
        var document = db.Documents.Find(docid);
        db.Entry(document).State = EntityState.Deleted;
        db.Documents.Remove(document);
        db.SaveChanges();
        return Json("Success", JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Delete Export Data (used when soft-delete enabled), mark records for deletion.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <returns>A response stream to send to the Delete Export Data View.</returns>
    [Audit("DeleteExportData")]
    public bool? DeleteExportData(long ExportDataLogId, IQueryable<Document> objdocument)
    {
        if(User != null && (!User.CanDelete("Document")))
        {
            return null;
        }
        try
        {
            using(var dbDoc = new ApplicationContext((CustomPrincipal)User, true))
            {
                var objDoc = dbDoc.Documents.Where(wh => wh.IsDeleted.HasValue && wh.IsDeleted.Value && !(wh.ExportDataLogId.HasValue));
                objDoc.Update(p => new Document
                {
                    IsDeleted = true,
                    DeleteDateTime = DateTime.UtcNow,
                    ExportDataLogId = ExportDataLogId
                });
            }
        }
        catch(Exception ex)
        {
            return false;
        }
        return true;
    }
    
    /// <summary>Check before save.</summary>
    ///
    /// <param name="document">The Order.</param>
    /// <param name="command">  (Optional) The command.</param>
    ///
    /// <returns>A string.</returns>
    public string CheckBeforeSave(Document document, string command = "")
    {
        var AlertMsg = "";
        var fileType = new ApplicationContext(new SystemUser()).AppSettings.Where(p => p.Key == "FileTypes").FirstOrDefault();
        if(fileType != null)
        {
            var allowed = fileType.Value.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            if(!(fileType.Value.ToLower().Contains(document.FileExtension.Substring(1).ToLower())))
            {
                AlertMsg = AlertMsg + "File type is not proper. Accepts only [" + fileType.Value + "] file types)";
                ModelState.AddModelError("CustomError", AlertMsg);
                ViewBag.ApplicationError = AlertMsg;
            }
            else
            {
                FileType mimeType2 = null;
                mimeType2 = document.Byte.GetFileType();
                if(mimeType2 != null)
                {
                    if(document.FileType.ToLower() != "byte")
                        document.Byte = null;
                    var existing = mimeType2.Extension.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                    if(allowed.Intersect(existing).Count() == 0)
                    {
                        AlertMsg = AlertMsg + "File type is not proper. Accepts only [" + fileType.Value + "] file types)";
                        ModelState.AddModelError("CustomError", AlertMsg);
                        ViewBag.ApplicationError = AlertMsg;
                    }
                }
            }
        }
        return AlertMsg;
    }
    /// <summary>Check before delete.</summary>
    ///
    /// <param name="Document">The Document.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool CheckBeforeDelete(Document document, string command = "")
    {
        var result = true;
        // Write your logic here
        if(!result)
        {
            var AlertMsg = "Validation Alert - Before Delete !! Information not deleted.";
            ModelState.AddModelError("CustomError", AlertMsg);
            ViewBag.ApplicationError = AlertMsg;
        }
        return result;
    }
    /// <summary>Executes the deleting action.</summary>
    ///
    /// <param name="document">The document.</param>
    /// <param name="unitdb">   The unitdb.</param>
    /// <param name="ondeletinguser">The application user.</param>
    public void OnDeleting(Document document, ApplicationContext unitdb, GeneratorBase.MVC.Models.IUser ondeletinguser)
    {
        // Write your logic here
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="document"></param>
    /// <param name="aftersaveuser"></param>
    ///
    
    public void AfterSave(Document document, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="document"></param>
    /// <param name="aftersaveuser"></param>
    ///
    
    public void OnSaving(Document document, ApplicationContext db, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState)
    {
        // Write your logic here
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilter action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var entitylist = db.Documents.Select(p => p.EntityName).Distinct();
        IDictionary<string, string> finallist = new Dictionary<string, string>();
        foreach(var ent in entitylist)
        {
            var displaynameObj = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == ent);
            var displayname = displaynameObj != null ? displaynameObj.DisplayName : "N/A";
            if(string.IsNullOrEmpty(ent))
                finallist.Add(new KeyValuePair<string, string>("NA", displayname));
            else
                finallist.Add(new KeyValuePair<string, string>(ent, displayname));
        }
        return Json(finallist, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValueForFilterFileDataType action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValueForFilterFileDataType(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        var FileTypes = db.Documents.Select(p => p.FileType).Distinct();
        IDictionary<string, string> finallist = new Dictionary<string, string>();
        foreach(var filetype in FileTypes)
        {
            var filetypedp = string.Empty;
            if(!string.IsNullOrEmpty(filetype))
            {
                if(filetype.ToLower() == "file")
                {
                    filetypedp = "Physical File";
                }
                else if(filetype.ToLower() == "onedrive")
                {
                    filetypedp = "OneDrive";
                }
                else if(filetype.ToLower() == "byte")
                {
                    filetypedp = "Byte";
                }
            }
            finallist.Add(new KeyValuePair<string, string>(filetype, filetypedp));
        }
        return Json(finallist, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Correct old byte data.</summary>
    public ActionResult UpdateEntity()
    {
        using(var db = new ApplicationContext(new SystemUser(), true))
        {
            var documentlist = db.Documents.Where(wh => string.IsNullOrEmpty(wh.EntityName)).ToList();
            foreach(var doc in documentlist)
            {
                var id = doc.Id;
                doc.EntityName = DoAuditEntry.EntityNameForDocument(id);
                if(doc.EntityName != null)
                {
                    doc.FileType = "byte";
                    db.Entry(doc).State = EntityState.Modified;
                }
            }
            db.DirectSaveChanges();
        }
        return RedirectToAction("Index", "Document");
    }
    
    /// <summary
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