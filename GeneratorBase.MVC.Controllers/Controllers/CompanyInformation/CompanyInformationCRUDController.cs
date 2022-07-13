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
using Z.EntityFramework.Plus;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A partial controller class for CompanyInformation actions (CRUD and others).</summary>
///
/// <remarks></remarks>

public partial class CompanyInformationController : BaseController
{


    /// <summary>CompanyInformation Index Action, renders items in different UI format based on viewtype</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="currentFilter">  Specifying the current Filter.</param>
    /// <param name="searchString">   The search string.</param>
    /// <param name="sortBy">         Describes the column sort this list.</param>
    /// <param name="isAsc">          Sort order.</param>
    /// <param name="page">           List page number.</param>
    /// <param name="itemsPerPage">   The items per page.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    /// <param name="AssociatedType"> The associated type (association name).</param>
    /// <param name="IsExport">       Is export.</param>
    /// <param name="IsDeepSearch">   Is search search.</param>
    /// <param name="IsFilter">       Specify filter.</param>
    /// <param name="RenderPartial">  Render partial.</param>
    /// <param name="BulkOperation">  Render Bulk View.</param>
    /// <param name="parent">         The parent.</param>
    /// <param name="Wfsearch">       The wfsearch.</param>
    /// <param name="caller">         The caller.</param>
    /// <param name="BulkAssociate">  The bulk associate.</param>
    /// <param name="viewtype">       The viewtype (grid, gallery, list or custom view).</param>
    /// <param name="isMobileHome">   The is mobile home.</param>
    /// <param name="IsHomeList">     Is called at home page.</param>
    /// <param name="IsDivRender">    (Optional) True if is div render, false if not.</param>
    /// <param name="ShowDeleted">    (Optional) True to show, false to hide the deleted.</param>
    /// <param name="ExportType">     (Optional) Type of the export.</param>
    ///
    /// <returns>A response stream to send to the Grid/Gallery/List view.</returns>
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome, bool? IsHomeList, bool IsDivRender = false, bool ShowDeleted = false, string ExportType = null)
    {
        IndexViewBag(currentFilter, searchString, sortBy, isAsc, page, itemsPerPage, HostingEntity, HostingEntityID, AssociatedType, IsExport, IsDeepSearch, IsFilter, RenderPartial, BulkOperation, parent, Wfsearch, caller, BulkAssociate, viewtype, IsDivRender);
        CustomLoadViewDataListOnIndex(HostingEntity, HostingEntityID, AssociatedType);
        var lstCompanyInformation = from s in db.CompanyInformations
                                    select s;
        if(!String.IsNullOrEmpty(searchString))
        {
            lstCompanyInformation = searchRecords(lstCompanyInformation, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(ViewBag.isAsc))
        {
            lstCompanyInformation = sortRecords(lstCompanyInformation, sortBy, ViewBag.isAsc);
        }
        else lstCompanyInformation = lstCompanyInformation.OrderByDescending(c => c.Id);
        lstCompanyInformation = CustomSorting(lstCompanyInformation, HostingEntity, AssociatedType, sortBy, ViewBag.isAsc);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        //Cookies for pagesize
        if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"] != null)
        {
            if(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"].Value != "null")
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"].Value);
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        pageSize = pageSize > 100 ? 100 : pageSize;
        //Cookies for pagination
        if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"] != null)
        {
            if(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"].Value != "null")
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "CompanyInformation"].Value);
            ViewBag.Pages = pageNumber;
        }
        // for Restrict Dropdown
        ViewBag.CompanyInformationRestrictDropdownValueRuleInLIneEdit = RestrictDropdownValueRuleInLineEdit(User, "CompanyInformation", "OnEdit", new long[] { 6, 8 }, null, new string[] { "" });
        //
        ViewBag.PageSize = pageSize;
        var _CompanyInformation = lstCompanyInformation;
        _CompanyInformation = FilterByHostingEntity(_CompanyInformation, HostingEntity, AssociatedType, HostingEntityID);
        if(pageNumber > 1)
        {
            var totalListCount = _CompanyInformation.Count();
            if(BulkOperation != null && BulkAssociate != null && (BulkOperation == "single" || BulkOperation == "multiple"))
            {
                totalListCount = lstCompanyInformation.Except(_CompanyInformation).Count();
            }
            int quotient = totalListCount / pageSize;
            var remainder = totalListCount % pageSize;
            var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
            if(pageNumber > maxpagenumber)
            {
                pageNumber = 1;
            }
        }
        if(string.IsNullOrEmpty(viewtype))
            viewtype = "IndexPartial";
        ViewBag.TemplatesName = GetTemplatesForList(User, "CompanyInformation", viewtype);
        TempData["CompanyInformationlist"] = null;
        if((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype && ViewBag.BuiltInPage == false)
            ViewBag.TemplatesName = viewtype;
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
        {
            var list = _CompanyInformation.ToCachedPagedList(pageNumber, pageSize);
            ViewBag.EntityCompanyInformationDisplayValue = new SelectList(list.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }), "ID", "DisplayValue");
            TempData["CompanyInformationlist"] = list.Select(z => new
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
                ViewBag.TemplatesName = "IndexPartial";
                ViewData["BulkAssociate"] = BulkAssociate;
                FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                _CompanyInformation = _fad.FilterDropdown<CompanyInformation>(User, _CompanyInformation, "CompanyInformation", caller);
                if(Convert.ToBoolean(BulkAssociate))
                {
                    if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                        return PartialView("BulkOperation", sortRecords(lstCompanyInformation.Except(_CompanyInformation), sortBy, isAsc).ToCachedPagedList(pageNumber, pageSize));
                    else
                        return PartialView("BulkOperation", lstCompanyInformation.Except(_CompanyInformation).OrderBy(q => q.DisplayValue).ToCachedPagedList(pageNumber, pageSize));
                }
                else
                    return PartialView("BulkOperation", _CompanyInformation.OrderBy(p => p.Id).ToCachedPagedList(pageNumber, pageSize));
            }
            else
            {
                if(ViewBag.TemplatesName == null)
                {
                    if(!string.IsNullOrEmpty(isMobileHome))
                    {
                        pageSize = _CompanyInformation.Count() == 0 ? 1 : _CompanyInformation.Count();
                    }
                    var list = _CompanyInformation.ToCachedPagedList(pageNumber, Convert.ToBoolean(IsHomeList) ? 5 : pageSize);
                    ViewBag.EntityCompanyInformationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["CompanyInformationlist"] = list.Select(z => new
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
                        pageSize = _CompanyInformation.Count() == 0 ? 1 : _CompanyInformation.Count();
                    }
                    var list = _CompanyInformation.ToCachedPagedList(pageNumber, pageSize);
                    ViewBag.EntityCompanyInformationDisplayValue = new SelectList(list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    }), "ID", "DisplayValue");
                    TempData["CompanyInformationlist"] = list.Select(z => new
                    {
                        ID = z.Id,
                        DisplayValue = z.DisplayValue
                    });
                    return PartialView(ViewBag.TemplatesName, list);
                }
            }
        }
    }
    /// <summary>Details (ReadOnly View)</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="defaultview">      The defaultview to render.</param>
    ///
    /// <returns>A response stream to send to the Details View.</returns>
    public ActionResult Details(int? id, string HostingEntityName, string AssociatedType, string defaultview)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        if(!CustomAuthorizationBeforeDetails(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        CompanyInformation companyinformation = db.CompanyInformations.Find(id);
        if(companyinformation == null)
        {
            return HttpNotFound();
        }
        companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id).Select(p => p.CompanyListID).ToList();
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Details";
        ViewBag.TemplatesName = GetTemplatesForDetails(User, "CompanyInformation", defaultview);
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        LoadViewDataBeforeOnEdit(companyinformation);
        if(!string.IsNullOrEmpty(AssociatedType))
            LoadViewDataForCount(companyinformation, AssociatedType);
        return View(ViewBag.TemplatesName, companyinformation);
    }
    /// <summary>Create (including details properties)</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="IsDDAdd">          The is dropdown add.</param>
    /// <param name="viewtype">         The viewtype.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype, bool RenderPartial = false, string viewmode = "create", string wizardstep = "")
    {
        if(!User.CanAdd("CompanyInformation") || !CustomAuthorizationBeforeCreate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        viewtype = "Create";
        ViewBag.TemplatesName = GetTemplatesForCreate(User, "CompanyInformation", viewtype);
        ViewData["CompanyInformationParentUrl"] = UrlReferrer;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["HostingEntityID"] = HostingEntityID;
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName);
    }
    
    
    /// <summary>(An Action that handles HTTP POST requests) Creates a new record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="companyinformationObj">The CompanyInformation object.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="IsDDAdd">          Add from dropdown.</param>
    /// <param name="AssociatedEntity"> The associated entity.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,CompanyName,CompanyEmail,CompanyAddress,CompanyCountry,CompanyState,CompanyCity,CompanyZipCode,ContactNumber1,ContactNumber2,LoginBackgroundWidth,LoginBackgroundHeight,LogoWidth,LogoHeight,IconWidth,IconHeight,SMTPUser,SMTPServer,SMTPPassword,SMTPPort,SSL,UseAnonymous,AboutCompany,Disclaimer,SelectedCompanyList_CompanyInformationCompanyListAssociation,OneDriveClientId,OneDriveSecret,OneDriveTenantId,OneDriveUserName,OneDrivePassword,OneDriveFolderName")] CompanyInformation companyinformationObj, HttpPostedFileBase LoginBg, String CamerafileUploadLoginBg, HttpPostedFileBase Logo, String CamerafileUploadLogo, HttpPostedFileBase Icon, String CamerafileUploadIcon, string UrlReferrer, bool? IsDDAdd, string AssociatedEntity, string HostingEntityName, string HostingEntityID, bool RenderPartial = false, string viewmode = "create")
    {
        string command = Request.Form["hdncommand"];
        CheckBeforeSave(companyinformationObj, command);
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(LoginBg != null || (Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != ""))
            IsFileTypeAndSizeAllowed(LoginBg);
        if(Logo != null || (Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != ""))
            IsFileTypeAndSizeAllowed(Logo);
        if(Icon != null || (Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != ""))
            IsFileTypeAndSizeAllowed(Icon);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(LoginBg != null)
            {
                long documentID = SaveDocument(LoginBg);
                companyinformationObj.LoginBg = documentID;
            }
            if(Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformationObj.LoginBg = documentIDCamara;
                }
            }
            if(Logo != null)
            {
                long documentID = SaveDocument(Logo);
                companyinformationObj.Logo = documentID;
            }
            if(Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformationObj.Logo = documentIDCamara;
                }
            }
            if(Icon != null)
            {
                long documentID = SaveDocument(Icon);
                companyinformationObj.Icon = documentID;
            }
            if(Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformationObj.Icon = documentIDCamara;
                }
            }
            bool customcreate_hasissues = false;
            if(!CustomSaveOnCreate(companyinformationObj, out customcreate_hasissues, command))
            {
                //Encryptdata
                companyinformationObj.SMTPPassword = Encryptdata(companyinformationObj.SMTPPassword);
                db.CompanyInformations.Add(companyinformationObj);
                db.SaveChanges();
            }
            bool flagCompanyInformationCompanyListAssociation = false;
            var dblistCompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformationObj.Id);
            if(companyinformationObj.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
                dblistCompanyInformationCompanyListAssociation = dblistCompanyInformationCompanyListAssociation.Where(a => !companyinformationObj.SelectedCompanyList_CompanyInformationCompanyListAssociation.Contains(a.CompanyListID));
            foreach(var obj in dblistCompanyInformationCompanyListAssociation)
            {
                db.CompanyInformationCompanyListAssociations.Remove(obj);
                flagCompanyInformationCompanyListAssociation = true;
            }
            if(flagCompanyInformationCompanyListAssociation)
                db.SaveChanges();
            flagCompanyInformationCompanyListAssociation = false;
            if(companyinformationObj.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
            {
                foreach(var pgs in companyinformationObj.SelectedCompanyList_CompanyInformationCompanyListAssociation)
                {
                    if(db.CompanyInformationCompanyListAssociations.FirstOrDefault(a => a.CompanyInformationID == companyinformationObj.Id && a.CompanyListID == pgs) == null)
                    {
                        CompanyInformationCompanyListAssociation objCompanyInformationCompanyListAssociation = new CompanyInformationCompanyListAssociation();
                        objCompanyInformationCompanyListAssociation.CompanyInformationID = companyinformationObj.Id;
                        objCompanyInformationCompanyListAssociation.CompanyListID = pgs;
                        objCompanyInformationCompanyListAssociation.TenantId = pgs;
                        db.Entry(objCompanyInformationCompanyListAssociation).State = EntityState.Added;
                        db.CompanyInformationCompanyListAssociations.Add(objCompanyInformationCompanyListAssociation);
                        flagCompanyInformationCompanyListAssociation = true;
                    }
                }
                if(flagCompanyInformationCompanyListAssociation)
                    db.SaveChanges();
            }
            else
            {
                CompanyInformationCompanyListAssociation objCompanyInformationCompanyListAssociation = new CompanyInformationCompanyListAssociation();
                objCompanyInformationCompanyListAssociation.CompanyInformationID = companyinformationObj.Id;
                objCompanyInformationCompanyListAssociation.CompanyListID = 0;
                objCompanyInformationCompanyListAssociation.TenantId = 0;
                db.Entry(objCompanyInformationCompanyListAssociation).State = EntityState.Added;
                db.CompanyInformationCompanyListAssociations.Add(objCompanyInformationCompanyListAssociation);
                flagCompanyInformationCompanyListAssociation = true;
                if(flagCompanyInformationCompanyListAssociation)
                    db.SaveChanges();
            }
            if(AssociatedEntity == "CompanyInformationCompanyListAssociation_CompanyList")
            {
                long hostingentityid;
                if(Int64.TryParse(HostingEntityID, out hostingentityid) && hostingentityid > 0)
                {
                    db.CompanyInformationCompanyListAssociations.Add(new CompanyInformationCompanyListAssociation { CompanyListID = hostingentityid, CompanyInformationID = companyinformationObj.Id });
                    db.SaveChanges();
                }
            }
            if(!customcreate_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(companyinformationObj, "Create", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(viewmode == "wizard")
                    return RedirectToAction("Edit", new { Id = companyinformationObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                if(command == "Create & Continue")
                {
                    if(RenderPartial)
                    {
                        var result = new { Result = "Success", Id = companyinformationObj.Id };
                        return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return RedirectToAction("Edit", new { Id = companyinformationObj.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                }
                if(command == "Create & Add another")
                    return Redirect(Convert.ToString(Request.UrlReferrer.PathAndQuery));
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
        }
        if(IsDDAdd != null)
            ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
        LoadViewDataAfterOnCreate(companyinformationObj);
        ViewData["CompanyInformationParentUrl"] = UrlReferrer;
        ViewData["wizardstep"] = "#step" + wizardstep;
        ViewData["viewmode"] = viewmode;
        return View(companyinformationObj);
    }
    /// <summary>(An Action that handles HTTP GET requests) Renders UI to edit a record on model popup.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="viewtype">         The viewtype.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype, bool RecordReadOnly = false)
    {
        if(!User.CanEdit("CompanyInformation") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        CompanyInformation companyinformation = db.CompanyInformations.Find(id);
        if(companyinformation == null)
        {
            return HttpNotFound();
        }
        companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id).Select(p => p.CompanyListID).ToList();
        if(UrlReferrer != null)
            ViewData["CompanyInformationParentUrl"] = UrlReferrer;
        if(ViewData["CompanyInformationParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation/Edit/" + companyinformation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation/Create"))
            ViewData["CompanyInformationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        LoadViewDataBeforeOnEdit(companyinformation);
        var objCompanyInformation = new List<CompanyInformation>();
        ViewBag.CompanyInformationDD = new SelectList(objCompanyInformation, "ID", "DisplayValue");
        return View(companyinformation);
    }
    /// <summary>(An Action that handles HTTP POST requests) Edits record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="companyinformation">       The CompanyInformation object.</param>
    /// <param name="UrlReferrer">     The URL referrer (return url after save).</param>
    /// <param name="IsAddPop">        Add popup.</param>
    /// <param name="AssociatedEntity">The associated entity.</param>
    ///
    /// <returns>A response stream to send to the EditQuick View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,CompanyName,CompanyEmail,CompanyAddress,CompanyCountry,CompanyState,CompanyCity,CompanyZipCode,ContactNumber1,ContactNumber2,LoginBg,LoginBackgroundWidth,LoginBackgroundHeight,Logo,LogoWidth,LogoHeight,Icon,IconWidth,IconHeight,SMTPUser,SMTPServer,SMTPPassword,SMTPPort,SSL,UseAnonymous,AboutCompany,Disclaimer,SelectedCompanyList_CompanyInformationCompanyListAssociation")] CompanyInformation companyinformation, HttpPostedFileBase File_LoginBg, String CamerafileUploadLoginBg, HttpPostedFileBase File_Logo, String CamerafileUploadLogo, HttpPostedFileBase File_Icon, String CamerafileUploadIcon, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        if(!string.IsNullOrEmpty(cannotviewproperties)) companyinformation = UpdateHiddenProperties(companyinformation, cannotviewproperties);
        CheckBeforeSave(companyinformation, command);
        if(File_LoginBg != null || (Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != ""))
            IsFileTypeAndSizeAllowed(File_LoginBg);
        if(File_Logo != null || (Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != ""))
            IsFileTypeAndSizeAllowed(File_Logo);
        if(File_Icon != null || (Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != ""))
            IsFileTypeAndSizeAllowed(File_Icon);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_LoginBg != null)
            {
                long documentID = 0;
                if(companyinformation.LoginBg != null)
                    documentID = UpdateDocument(File_LoginBg, companyinformation.LoginBg);
                else
                    documentID = SaveDocument(File_LoginBg);
                companyinformation.LoginBg = documentID;
            }
            if(Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.LoginBg != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.LoginBg);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.LoginBg = documentIDCamara;
                }
            }
            if(File_Logo != null)
            {
                long documentID = 0;
                if(companyinformation.Logo != null)
                    documentID = UpdateDocument(File_Logo, companyinformation.Logo);
                else
                    documentID = SaveDocument(File_Logo);
                companyinformation.Logo = documentID;
            }
            if(Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.Logo != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.Logo);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.Logo = documentIDCamara;
                }
            }
            if(File_Icon != null)
            {
                long documentID = 0;
                if(companyinformation.Icon != null)
                    documentID = UpdateDocument(File_Icon, companyinformation.Icon);
                else
                    documentID = SaveDocument(File_Icon);
                companyinformation.Icon = documentID;
            }
            if(Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.Icon != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.Icon);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.Icon = documentIDCamara;
                }
            }
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(companyinformation, out customsave_hasissues, command))
            {
                db.Entry(companyinformation).State = EntityState.Modified;
                db.SaveChanges();
            }
            bool flagCompanyInformationCompanyListAssociation = false;
            var dblistCompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id);
            if(companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
                dblistCompanyInformationCompanyListAssociation = dblistCompanyInformationCompanyListAssociation.Where(a => !companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation.Contains(a.CompanyListID));
            foreach(var obj in dblistCompanyInformationCompanyListAssociation)
            {
                db.CompanyInformationCompanyListAssociations.Remove(obj);
                flagCompanyInformationCompanyListAssociation = true;
            }
            if(flagCompanyInformationCompanyListAssociation)
                db.SaveChanges();
            flagCompanyInformationCompanyListAssociation = false;
            if(companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
            {
                foreach(var pgs in companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation)
                {
                    if(db.CompanyInformationCompanyListAssociations.FirstOrDefault(a => a.CompanyInformationID == companyinformation.Id && a.CompanyListID == pgs) == null)
                    {
                        CompanyInformationCompanyListAssociation objCompanyInformationCompanyListAssociation = new CompanyInformationCompanyListAssociation();
                        objCompanyInformationCompanyListAssociation.CompanyInformationID = companyinformation.Id;
                        objCompanyInformationCompanyListAssociation.CompanyListID = pgs;
                        db.Entry(objCompanyInformationCompanyListAssociation).State = EntityState.Added;
                        db.CompanyInformationCompanyListAssociations.Add(objCompanyInformationCompanyListAssociation);
                        flagCompanyInformationCompanyListAssociation = true;
                    }
                }
                if(flagCompanyInformationCompanyListAssociation)
                    db.SaveChanges();
            }
            var result = new { Result = "Succeed", UrlRefr = UrlReferrer };
            if(!customsave_hasissues)
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            var errors = "";
            foreach(ModelState modelState in ViewData.ModelState.Values)
            {
                foreach(ModelError error in modelState.Errors)
                {
                    errors += error.ErrorMessage + ".  ";
                }
            }
            var result = new { Result = "fail", UrlRefr = errors };
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if(db.Entry(companyinformation).State == EntityState.Detached)
            companyinformation = db.CompanyInformations.Find(companyinformation.Id);
        LoadViewDataAfterOnEdit(companyinformation);
        return View(companyinformation);
    }
    /// <summary>(An Action that handles HTTP GET requests) Renders UI to edit a record (full screen).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="id">               The identifier.</param>
    /// <param name="UrlReferrer">      The URL referrer (return url after save).</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="AssociatedType">   Association name.</param>
    /// <param name="defaultview">      The defaultview.</param>
    /// <param name="RenderPartial">    (Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview, bool RecordReadOnly = false, bool RenderPartial = false, string viewmode = "edit", string wizardstep = "")
    {
        if(!User.CanEdit("CompanyInformation") || !CustomAuthorizationBeforeEdit(id, HostingEntityName, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        CompanyInformation companyinformation = db.CompanyInformations.Find(id);
        if(companyinformation == null)
        {
            return HttpNotFound();
        }
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["CompanyInformationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.CompanyInformations.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityCompanyInformationDisplayValueEdit = TempData["CompanyInformationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["CompanyInformationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityCompanyInformationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id).Select(p => p.CompanyListID).ToList();
        if(string.IsNullOrEmpty(defaultview))
            defaultview = "Edit";
        ViewBag.TemplatesName = GetTemplatesForEdit(User, "CompanyInformation", defaultview);
        // NEXT-PREVIOUS DROP DOWN CODE
        SelectList selitm = new SelectList(lst, "ID", "DisplayValue");
        List<SelectListItem> newList = selitm.ToList();
        if(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation/Create"))
        {
            newList.Insert(0, (new SelectListItem { Text = companyinformation.DisplayValue, Value = companyinformation.Id.ToString() }));
            ViewBag.EntityCompanyInformationDisplayValueEdit = newList;
            TempData["CompanyInformationlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        else if(!newList.Exists(p => p.Value == Convert.ToString(companyinformation.Id)))
        {
            if(newList.Count > 0)
            {
                newList[0].Text = companyinformation.DisplayValue;
                newList[0].Value = companyinformation.Id.ToString();
            }
            else
            {
                newList.Insert(0, (new SelectListItem { Text = companyinformation.DisplayValue, Value = companyinformation.Id.ToString() }));
            }
            ViewBag.EntityCompanyInformationDisplayValueEdit = newList;
            TempData["CompanyInformationlist"] = newList.Select(z => new
            {
                ID = z.Value,
                DisplayValue = z.Text
            });
        }
        //
        if(UrlReferrer != null)
            ViewData["CompanyInformationParentUrl"] = UrlReferrer;
        if(ViewData["CompanyInformationParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation/Edit/" + companyinformation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation/Create"))
            ViewData["CompanyInformationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        ViewData["HostingEntityName"] = HostingEntityName;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["RecordReadOnly"] = RecordReadOnly;
        ViewData["viewmode"] = viewmode;
        ViewData["wizardstep"] = "#step" + wizardstep;
        LoadViewDataBeforeOnEdit(companyinformation);
        if(RenderPartial)
            ViewBag.IsPartial = true;
        return View(ViewBag.TemplatesName, companyinformation);
    }
    /// <summary>(An Action that handles HTTP POST requests) Action modifies a record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="companyinformation">    The CompanyInformation object.</param>
    /// <param name="UrlReferrer">  The URL referrer (return url after save).</param>
    /// <param name="RenderPartial">(Optional) The render partial.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,CompanyName,CompanyEmail,CompanyAddress,CompanyCountry,CompanyState,CompanyCity,CompanyZipCode,ContactNumber1,ContactNumber2,LoginBg,LoginBackgroundWidth,LoginBackgroundHeight,Logo,LogoWidth,LogoHeight,Icon,IconWidth,IconHeight,SMTPUser,SMTPServer,SMTPPassword,SMTPPort,SSL,UseAnonymous,AboutCompany,Disclaimer,SelectedCompanyList_CompanyInformationCompanyListAssociation,OneDriveClientId,OneDriveSecret,OneDriveTenantId,OneDriveUserName,OneDrivePassword,OneDriveFolderName")] CompanyInformation companyinformation, HttpPostedFileBase File_LoginBg, String CamerafileUploadLoginBg, HttpPostedFileBase File_Logo, String CamerafileUploadLogo, HttpPostedFileBase File_Icon, String CamerafileUploadIcon, string UrlReferrer, bool RenderPartial = false, string viewmode = "edit")
    {
        string command = Request.Form["hdncommand"];
        string cannotviewproperties = Request.Form["cannotviewproperties"];
        int wizardstep;
        Int32.TryParse(Request.Form["wizardstep"], out wizardstep);
        if(!string.IsNullOrEmpty(cannotviewproperties)) companyinformation = UpdateHiddenProperties(companyinformation, cannotviewproperties);
        CheckBeforeSave(companyinformation, command);
        if(File_LoginBg != null || (Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != ""))
            IsFileTypeAndSizeAllowed(File_LoginBg);
        if(File_Logo != null || (Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != ""))
            IsFileTypeAndSizeAllowed(File_Logo);
        if(File_Icon != null || (Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != ""))
            IsFileTypeAndSizeAllowed(File_Icon);
        if(ModelState.IsValid)
        {
            string path = Server.MapPath("~/Files/");
            string ticks = DateTime.UtcNow.Ticks.ToString();
            if(File_LoginBg != null)
            {
                long documentID = 0;
                if(companyinformation.LoginBg != null)
                    documentID = UpdateDocument(File_LoginBg, companyinformation.LoginBg);
                else
                    documentID = SaveDocument(File_LoginBg);
                companyinformation.LoginBg = documentID;
            }
            if(Request.Form["CamerafileUploadLoginBg"] != null && Request.Form["CamerafileUploadLoginBg"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLoginBg"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.LoginBg != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.LoginBg);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.LoginBg = documentIDCamara;
                }
            }
            if(File_Logo != null)
            {
                long documentID = 0;
                if(companyinformation.Logo != null)
                    documentID = UpdateDocument(File_Logo, companyinformation.Logo);
                else
                    documentID = SaveDocument(File_Logo);
                companyinformation.Logo = documentID;
            }
            if(Request.Form["CamerafileUploadLogo"] != null && Request.Form["CamerafileUploadLogo"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadLogo"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.Logo != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.Logo);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.Logo = documentIDCamara;
                }
            }
            if(File_Icon != null)
            {
                long documentID = 0;
                if(companyinformation.Icon != null)
                    documentID = UpdateDocument(File_Icon, companyinformation.Icon);
                else
                    documentID = SaveDocument(File_Icon);
                companyinformation.Icon = documentID;
            }
            if(Request.Form["CamerafileUploadIcon"] != null && Request.Form["CamerafileUploadIcon"] != "")
            {
                using(System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]))))
                {
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadIcon"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if(companyinformation.Icon != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, companyinformation.Icon);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    companyinformation.Icon = documentIDCamara;
                }
            }
            bool customsave_hasissues = false;
            if(!CustomSaveOnEdit(companyinformation, out customsave_hasissues, command))
            {
                if(companyinformation.SMTPPassword != "****")
                {
                    companyinformation.SMTPPassword = Encryptdata(companyinformation.SMTPPassword);
                    db.Entry(companyinformation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    companyinformation.SMTPPassword = db.CompanyInformations.AsNoTracking().Where(p => p.Id == companyinformation.Id).FirstOrDefault().SMTPPassword;
                    db.Entry(companyinformation).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            bool flagCompanyInformationCompanyListAssociation = false;
            var dblistCompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id);
            if(companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
                dblistCompanyInformationCompanyListAssociation = dblistCompanyInformationCompanyListAssociation.Where(a => !companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation.Contains(a.CompanyListID));
            foreach(var obj in dblistCompanyInformationCompanyListAssociation)
            {
                db.CompanyInformationCompanyListAssociations.Remove(obj);
                flagCompanyInformationCompanyListAssociation = true;
            }
            if(flagCompanyInformationCompanyListAssociation)
                db.SaveChanges();
            flagCompanyInformationCompanyListAssociation = false;
            if(companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
            {
                foreach(var pgs in companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation)
                {
                    if(db.CompanyInformationCompanyListAssociations.FirstOrDefault(a => a.CompanyInformationID == companyinformation.Id && a.CompanyListID == pgs) == null)
                    {
                        CompanyInformationCompanyListAssociation objCompanyInformationCompanyListAssociation = new CompanyInformationCompanyListAssociation();
                        objCompanyInformationCompanyListAssociation.CompanyInformationID = companyinformation.Id;
                        objCompanyInformationCompanyListAssociation.CompanyListID = pgs;
                        objCompanyInformationCompanyListAssociation.TenantId = pgs;
                        db.Entry(objCompanyInformationCompanyListAssociation).State = EntityState.Added;
                        db.CompanyInformationCompanyListAssociations.Add(objCompanyInformationCompanyListAssociation);
                        flagCompanyInformationCompanyListAssociation = true;
                    }
                }
                if(flagCompanyInformationCompanyListAssociation)
                    db.SaveChanges();
            }
            else
            {
                CompanyInformationCompanyListAssociation objCompanyInformationCompanyListAssociation = new CompanyInformationCompanyListAssociation();
                objCompanyInformationCompanyListAssociation.CompanyInformationID = companyinformation.Id;
                objCompanyInformationCompanyListAssociation.CompanyListID = 0;
                objCompanyInformationCompanyListAssociation.TenantId = 0;
                db.Entry(objCompanyInformationCompanyListAssociation).State = EntityState.Added;
                db.CompanyInformationCompanyListAssociations.Add(objCompanyInformationCompanyListAssociation);
                flagCompanyInformationCompanyListAssociation = true;
                if(flagCompanyInformationCompanyListAssociation)
                    db.SaveChanges();
            }
            if(!customsave_hasissues)
            {
                RedirectToRouteResult customRedirectAction = CustomRedirectUrl(companyinformation, "Edit", command, viewmode);
                if(customRedirectAction != null) return customRedirectAction;
                if(RenderPartial)
                {
                    var result = new { Result = "Success", Id = companyinformation.Id };
                    return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                if(command != "Save")
                {
                    if(command == "SaveNextPrev")
                    {
                        long NextPreId = Convert.ToInt64(Request.Form["hdnNextPrevId"]);
                        return RedirectToAction("Edit", new { Id = NextPreId, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
                    }
                    else
                        return RedirectToAction("Edit", new { Id = companyinformation.Id, UrlReferrer = UrlReferrer, viewmode = viewmode, wizardstep = wizardstep });
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
        }
        companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id).Select(p => p.CompanyListID).ToList();
        // NEXT-PREVIOUS DROP DOWN CODE
        TempData.Keep();
        string json = "";
        if(TempData["CompanyInformationlist"] == null)
        {
            json = Newtonsoft.Json.JsonConvert.SerializeObject(db.CompanyInformations.Select(z => new
            {
                ID = z.Id,
                DisplayValue = z.DisplayValue
            }).Take(10).ToList());
        }
        else
        {
            ViewBag.EntityCompanyInformationDisplayValueEdit = TempData["CompanyInformationlist"];
            json = Newtonsoft.Json.JsonConvert.SerializeObject(TempData["CompanyInformationlist"]);
        }
        Newtonsoft.Json.JsonSerializerSettings serSettings = new Newtonsoft.Json.JsonSerializerSettings();
        serSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<object>>(json, serSettings);
        ViewBag.EntityCompanyInformationDisplayValueEdit = new SelectList(lst, "ID", "DisplayValue");
        //
        if(db.Entry(companyinformation).State == EntityState.Detached)
            companyinformation = db.CompanyInformations.Find(companyinformation.Id);
        LoadViewDataAfterOnEdit(companyinformation);
        ViewData["CompanyInformationParentUrl"] = UrlReferrer;
        ViewData["viewmode"] = viewmode;
        return View(companyinformation);
    }
    
    /// <summary>Deletes the record for given ID.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <exception cref="Exception">Thrown when an exception error when id not found.</exception>
    ///
    /// <param name="id">The identifier.</param>
    ///
    /// <returns>A response stream to send to the Delete View.</returns>
    public ActionResult Delete(int id)
    {
        if(!User.CanDelete("CompanyInformation") || !CustomAuthorizationBeforeDelete(id))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        CompanyInformation companyinformation = db.CompanyInformations.Find(id);
        if(companyinformation == null)
        {
            throw(new Exception("Deleted"));
        }
        if(ViewData["CompanyInformationParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/CompanyInformation"))
            ViewData["CompanyInformationParentUrl"] = Request.UrlReferrer.PathAndQuery;
        return View(companyinformation);
    }
    /// <summary>(An Action that handles HTTP POST requests) (Defines the Delete Action) deletes the confirmed record.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="companyinformation">  The CompanyInformation object.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteConfirmed View.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(CompanyInformation companyinformation, string UrlReferrer)
    {
        if(!User.CanDelete("CompanyInformation"))
        {
            return RedirectToAction("Index", "Error");
        }
        companyinformation = db.CompanyInformations.Find(companyinformation.Id);
        if(CheckBeforeDelete(companyinformation))
        {
            bool customdelete_hasissues = false;
            if(!CustomDelete(companyinformation, out customdelete_hasissues, "Delete"))
            {
                db.Entry(companyinformation).State = EntityState.Deleted;
                //db.CompanyInformations.Remove(companyinformation); //issue delete with userbased security
                if(companyinformation.LoginBg != null)
                {
                    DeleteDocument(companyinformation.LoginBg);
                }
                if(companyinformation.Logo != null)
                {
                    DeleteDocument(companyinformation.Logo);
                }
                if(companyinformation.Icon != null)
                {
                    DeleteDocument(companyinformation.Icon);
                }
                db.SaveChanges();
            }
            if(!customdelete_hasissues)
            {
                if(!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if(ViewData["CompanyInformationParentUrl"] != null)
                {
                    string parentUrl = ViewData["CompanyInformationParentUrl"].ToString();
                    ViewData["CompanyInformationParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
        }
        return View(companyinformation);
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
    /// <summary>Bulk associate (Action associate multiple selected items with hosted entity).</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="ids">            The identifiers.</param>
    /// <param name="AssociatedType"> Type of the associated entity.</param>
    /// <param name="HostingEntity">  The hosting entity.</param>
    /// <param name="HostingEntityID">Identifier for the hosting entity.</param>
    ///
    /// <returns>A response stream to send to the BulkAssociate View.</returns>
    public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
    {
        var HostingID = Convert.ToInt64(HostingEntityID);
        if(HostingID == 0)
            return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Deletes selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="ids">        The identifiers.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteBulk View.</returns>
    public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
    {
        if(User != null && (!((CustomPrincipal)User).CanUseVerb("BulkDelete", "CompanyInformation", User) || !User.CanDelete("CompanyInformation")))
        {
            return RedirectToAction("Index", "Error");
        }
        bool customdelete_hasissues = false;
        foreach(var id in ids.Where(p => p > 0))
        {
            customdelete_hasissues = false;
            CompanyInformation companyinformation = db.CompanyInformations.Find(id);
            if(companyinformation != null)
            {
                if(CheckBeforeDelete(companyinformation))
                {
                    if(!CustomDelete(companyinformation, out customdelete_hasissues, "DeleteBulk"))
                    {
                        bool flagCompanyInformationCompanyListAssociation = false;
                        var dblistCompanyInformationCompanyListAssociation = db.CompanyInformationCompanyListAssociations.Where(a => a.CompanyInformationID == companyinformation.Id);
                        if(companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation != null)
                            dblistCompanyInformationCompanyListAssociation = dblistCompanyInformationCompanyListAssociation.Where(a => !companyinformation.SelectedCompanyList_CompanyInformationCompanyListAssociation.Contains(a.CompanyListID));
                        foreach(var obj in dblistCompanyInformationCompanyListAssociation)
                        {
                            db.CompanyInformationCompanyListAssociations.Remove(obj);
                            flagCompanyInformationCompanyListAssociation = true;
                        }
                        if(flagCompanyInformationCompanyListAssociation)
                            db.SaveChanges();
                        db.Entry(companyinformation).State = EntityState.Deleted;
                        db.CompanyInformations.Remove(companyinformation);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch(Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        }
                    }
                }
            }
        }
        return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Cancels the action and return back to previous page.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the Cancel View.</returns>
    public ActionResult Cancel(string UrlReferrer)
    {
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
    /// <summary>(An Action that handles HTTP POST requests) Renders UI to update records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="HostingEntityName">Name of the hosting entity.</param>
    /// <param name="HostingEntityID">  Identifier for the hosting entity.</param>
    /// <param name="AssociatedType">   Type of the associated.</param>
    /// <param name="IsDDUpdate">       Dropdown update.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpGet]
    public ActionResult BulkUpdate(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
    {
        if(!((CustomPrincipal)User).CanUseVerb("BulkUpdate", "CompanyInformation", User) || !User.CanEdit("CompanyInformation") || !CustomAuthorizationBeforeBulkUpdate(HostingEntityName, HostingEntityID, AssociatedType))
        {
            return RedirectToAction("Index", "Error");
        }
        BulkUpdateViewBag("CompanyInformation", UrlReferrer, HostingEntityName, HostingEntityID, AssociatedType, IsDDUpdate);
        LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
        return View();
    }
    /// <summary>(An Action that handles HTTP POST requests) Update selected records in bulk.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="companyinformation">  The CompanyInformation object.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the BulkUpdate View.</returns>
    [HttpPost]
    public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,IsDeleted,DeleteDateTime,CompanyName,CompanyEmail,CompanyAddress,CompanyCountry,CompanyState,CompanyCity,CompanyZipCode,ContactNumber1,ContactNumber2,LoginBg,LoginBackgroundWidth,LoginBackgroundHeight,Logo,LogoWidth,LogoHeight,Icon,IconWidth,IconHeight,SMTPUser,SMTPServer,SMTPPassword,SMTPPort,SSL,UseAnonymous,AboutCompany,Disclaimer,SelectedCompanyList_CompanyInformationCompanyListAssociation")] CompanyInformation companyinformation, HttpPostedFileBase File_LoginBg, String CamerafileUploadLoginBg, HttpPostedFileBase File_Logo, String CamerafileUploadLogo, HttpPostedFileBase File_Icon, String CamerafileUploadIcon, FormCollection collection, string UrlReferrer)
    {
        var bulkIds = collection["BulkUpdate"].Split(',').ToList();
        var chkUpdate = collection["chkUpdate"];
        if(!string.IsNullOrEmpty(chkUpdate))
        {
            bool customsave_hasissues = false;
            foreach(var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                CompanyInformation target = db.CompanyInformations.Find(objId);
                target.setDateTimeToClientTime();
                EntityCopy.CopyValuesForSameObjectType(companyinformation, target, chkUpdate);
                customsave_hasissues = false;
                CheckBeforeSave(target, "BulkUpdate");
                if(ValidateModel(target) && !CustomSaveOnEdit(target, out customsave_hasissues, "BulkUpdate"))
                {
                    db.Entry(target).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        db.Entry(target).State = EntityState.Detached;
                    }
                }
                else
                {
                    db.Entry(target).State = EntityState.Detached;
                }
            }
        }
        RedirectToRouteResult customRedirectAction = CustomRedirectUrl(companyinformation, "BulkUpdate", "");
        if(customRedirectAction != null) return customRedirectAction;
        if(!string.IsNullOrEmpty(UrlReferrer))
            return Redirect(UrlReferrer);
        else
            return RedirectToAction("Index");
    }
    
    public ActionResult DeleteImageGalleryDocumentAndUpdate(string ID, long recId, string PropName, string IDs)
    {
        var outputres = "";
        byte[] ConcurrencyKeyvalue = null;
        if(!String.IsNullOrEmpty(ID))
        {
            var document = db.Documents.Find(Convert.ToInt64(ID));
            db.Entry(document).State = EntityState.Deleted;
            db.Documents.Remove(document);
            db.SaveChanges();
            var output = string.Join(",", from v in IDs.Split(',')
                                     where v.Trim() != ID
                                     select v);
            outputres = output;
            using(var ctx = new ApplicationContext(new SystemUser()))
            {
                CompanyInformation companyinformation = ctx.CompanyInformations.Find(Convert.ToInt64(recId));
                if(PropName == "LoginBg")
                    companyinformation.LoginBg = null;
                if(PropName == "Logo")
                    companyinformation.Logo = null;
                if(PropName == "Icon")
                    companyinformation.Icon = null;
                ctx.Entry(companyinformation).State = EntityState.Modified;
                ctx.SaveChanges();
                ConcurrencyKeyvalue = ctx.CompanyInformations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
            }
        }
        return Json(new { result = "POP", output = outputres, ConcurrencyKey = ConcurrencyKeyvalue }, JsonRequestBehavior.AllowGet);
    }
    
    public ActionResult DeleteImageGalleryDocumentAndUpdateAll(long recId, string IDs, string PropName)
    {
        var outputres = "";
        byte[] ConcurrencyKeyvalue = null;
        DeleteImageGalleryDocument(IDs);
        using(var ctx = new ApplicationContext(new SystemUser()))
        {
            CompanyInformation companyinformation = ctx.CompanyInformations.Find(Convert.ToInt64(recId));
            ctx.Entry(companyinformation).State = EntityState.Modified;
            ctx.SaveChanges();
            ConcurrencyKeyvalue = ctx.CompanyInformations.Find(Convert.ToInt64(recId)).ConcurrencyKey;
        }
        return Json(new { result = "POP", ConcurrencyKey = ConcurrencyKeyvalue }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
    ///
    /// <remarks></remarks>
    ///
    /// <param name="disposing">    true to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null && CacheHelper.NoCache("CompanyInformation")) db.Dispose();
        }
        base.Dispose(disposing);
    }
    private string Decryptdata(string password)
    {
        string decryptpwd = string.Empty;
        UTF8Encoding encodepwd = new UTF8Encoding();
        Decoder Decode = encodepwd.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(password);
        int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        decryptpwd = new String(decoded_char);
        return decryptpwd;
    }
    private string Encryptdata(string password)
    {
        string strmsg = string.Empty;
        byte[] encode = new byte[password.Length];
        encode = Encoding.UTF8.GetBytes(password);
        strmsg = Convert.ToBase64String(encode);
        return strmsg;
    }
    
}
}
