using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
public partial class HomeController : BaseController
{
    private ApplicationDbContext userdb = new ApplicationDbContext(true);
    public ActionResult LiveMonitoring()
    {
        if(!User.CanView("DataMonitoring"))
        {
            return RedirectToAction("Index", "Error");
        }
        return View();
    }
    
    public ActionResult Index(string RegistrationEntity, string TokenId, bool? isThirdParty)
    {
        string HomePage = GetTemplatesForHome();
        TempData.Clear();
        if(string.IsNullOrEmpty(HomePage)) HomePage = "Index";
        if(!string.IsNullOrEmpty(RegistrationEntity) && (Request.UrlReferrer == null || (Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))))
        {
            ViewBag.RegistrationEntity = RegistrationEntity;
            ViewBag.UserId = TokenId;
            return View();
        }
        bool isItemZero = true;
        var roles = User.userroles;
        //var MultipleRoleSelection = CommonFunction.Instance.MultipleRoleSelection();
        var PromptForRoleSelection = CommonFunction.Instance.PromptForRoleSelection();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        //ApplicationDbContext userdb = new ApplicationDbContext(true);
        if(roles.Count() > 1 && Request.Cookies[AppUrl+"CurrentRole"]==null)
        {
            if(Convert.ToBoolean(PromptForRoleSelection))
            {
                var userroles = roles.ToList();
                if(!setPreviousCookie(userroles))
                {
                    ViewBag.PageRoles = userroles;
                    return View();
                }
                else
                {
                    var Segments = Request.UrlReferrer.Segments;
                    bool HasHomeseg = false;
                    foreach(var seg in Segments)
                    {
                        if(seg.ToUpper()=="HOME".ToUpper() || seg.ToUpper() == "LOGIN".ToUpper())
                        {
                            HasHomeseg = true;
                        }
                    }
                    if(Request.UrlReferrer != null && !HasHomeseg)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    else
                        return RedirectToAction("Index");
                };
            }
            else
            {
                setRoleCookie(String.Join(",",roles));
            }
        }
        else
        {
            if(roles.Count() > 0)
            {
                var userpagelist = (new UserDefinePagesRoleContext()).UserDefinePagesRoles;
                var role = roles.ToArray()[0];
                var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role);
                if(userpage != null)
                {
                    isItemZero = false;
                    var HomePageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent;
                    if(!string.IsNullOrEmpty(HomePageContent) && !string.IsNullOrWhiteSpace(HomePageContent))
                        ViewBag.PageContent = HomePageContent.Replace("Root_App_Path", GetBaseUrl());
                }
            }
            else
            {
                if(Request.Cookies[AppUrl + "CurrentRole"] != null)
                {
                    RemoveCookie(AppUrl + "CurrentRole");
                    return RedirectToAction("Index");
                }
                ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>";
            }
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
        {
            ViewBag.FavoriteItem = lstFavoriteItem;
            ViewBag.FavoriteCount = lstFavoriteItem.Count();
        }
        if(isItemZero || (ViewBag.PageRoles == null))
        {
            ViewBag.T_CustomerCount = db.T_Customers.Select(p=>p.Id).Count();
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            CompanyProfile cp = commonObj.getCompanyProfile(User);
            if(cp != null)
            {
                ViewBag.AboutCompany = cp.AboutCompany;
            }
        }
        var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
        ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
        ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
        if(userinfo!=null)
        {
            var loginattempt = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id);
            var lastlogin = loginattempt.Count() > 1 ? loginattempt.Skip(1).Take(1).First() : loginattempt.FirstOrDefault();
            //var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).FirstOrDefault();
            ViewBag.LastLoggedIn = lastlogin != null ? lastlogin.Date.ToString() : "";
        }
        ViewBag.LoginRoles = roles;
        SetViewBag(HomePage);
        if(!string.IsNullOrEmpty(HomePage) && HomePage.ToLower() == "customhomepage")   //added by rachana
        {
            return RedirectToAction("CustomHomePage");
        }
        if(!string.IsNullOrEmpty(HomePage) && HomePage.ToLower() == "dashboard")
        {
            return RedirectToAction("Dashboard");
        }
        return View(HomePage);
    }
    
    private bool setPreviousCookie(List<string> userroles)
    {
        var result = false;
        using(ApplicationDbContext ac = new ApplicationDbContext(true))
        {
            var oldAccess = ac.LoginSelectedRoles.FirstOrDefault(p => p.User == ((CustomPrincipal)User).Name);
            if(oldAccess != null)
            {
                string[] splittedarray = oldAccess.Roles.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                var  roles = userroles.Where(u => splittedarray.Contains(u.Trim())).ToArray();
                if(roles.Count() == 0)
                {
                    ac.LoginSelectedRoles.Remove(oldAccess);
                    ac.SaveChanges();
                }
                else
                {
                    string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
                    SetCookie(AppUrl + "CurrentRole", oldAccess.Roles);
                    result = true;
                }
            }
        }
        return result;
    }
    private void setRoleCookie(string key)
    {
        using(ApplicationDbContext ac = new ApplicationDbContext(true))
        {
            var oldAccess = ac.LoginSelectedRoles.AsNoTracking().FirstOrDefault(p => p.User == ((CustomPrincipal)User).Name);
            if(oldAccess != null)
            {
                key = oldAccess.Roles;
            }
            else
            {
                if(!string.IsNullOrEmpty(key))
                {
                    LoginSelectedRoles obj = new LoginSelectedRoles();
                    obj.Roles = key;
                    obj.User = ((CustomPrincipal)User).Name;
                    ac.LoginSelectedRoles.Add(obj);
                    ac.SaveChanges();
                }
            }
        }
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        SetCookie(AppUrl + "CurrentRole", key);
    }
    public JsonResult setRoleValue(string key)
    {
        setRoleCookie(key);
        bool result = true;
        return this.Json(result, JsonRequestBehavior.AllowGet);
    }
    public ActionResult About()
    {
        var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
        CompanyProfile cp = commonObj.getCompanyProfile(User);
        if(cp != null)
        {
            ViewBag.AboutCompany = cp.AboutCompany;
        }
        return View();
    }
    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";
        return View();
    }
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db!=null) db.Dispose();
            if(userdb != null) userdb.Dispose();
        }
        base.Dispose(disposing);
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult FavoriteSave(string Id, string Name, string FavoriteUrlEntityName, string FavoriteUrl)
    {
        string result = string.Empty;
        try
        {
            FavoriteItem objFs = new FavoriteItem();
            if(string.IsNullOrEmpty(Id))
            {
                objFs.Name = Name;
                objFs.LinkAddress =FavoriteUrl;
                objFs.EntityName = FavoriteUrlEntityName;
                objFs.LastUpdatedBy = DateTime.UtcNow;
                objFs.LastUpdatedByUser = User.Name;
                db.FavoriteItems.Add(objFs);
            }
            else
            {
                long objId = Int64.Parse(Id);
                objFs = db.FavoriteItems.Find(objId);
                objFs.Name = Name;
                objFs.EntityName = FavoriteUrlEntityName;
                db.Entry(objFs).State = EntityState.Modified;
            }
            db.SaveChanges();
            result = "success";
        }
        catch
        {
            result = "error";
        }
        return Json(result, JsonRequestBehavior.AllowGet);
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult FavoriteDelete(long Id)
    {
        FavoriteItem objFs = db.FavoriteItems.Find(Id);
        db.Entry(objFs).State = EntityState.Deleted;
        db.FavoriteItems.Remove(objFs);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult FavoriteDeleteUDF(long Id)
    {
        string result = string.Empty;
        FavoriteItem objFs = db.FavoriteItems.Find(Id);
        db.Entry(objFs).State = EntityState.Deleted;
        db.FavoriteItems.Remove(objFs);
        db.SaveChanges();
        result = "success";
        return Json(result, JsonRequestBehavior.AllowGet);
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult FavoriteDeleteHelp(long Id)
    {
        string result = string.Empty;
        FavoriteItem objFs = db.FavoriteItems.Find(Id);
        db.Entry(objFs).State = EntityState.Deleted;
        db.FavoriteItems.Remove(objFs);
        db.SaveChanges();
        result = "success";
        return Json(result, JsonRequestBehavior.AllowGet);
    }
    public JsonResult FacetedSearchDeleteHelp(long Id)
    {
        string result = string.Empty;
        T_FacetedSearch objFs = db.T_FacetedSearchs.Find(Id);
        db.Entry(objFs).State = EntityState.Deleted;
        db.T_FacetedSearchs.Remove(objFs);
        db.SaveChanges();
        result = "success";
        return Json(result, JsonRequestBehavior.AllowGet);
    }
    [Audit(0)]
    public ActionResult RedirectToEntity(string EntityName)
    {
        try
        {
            // var roles = ((CustomPrincipal)User).GetRoles();
            var roles = User.userroles;
            var defaultPages = db.DefaultEntityPages.Where(p => p.EntityName == EntityName);
            foreach(var defaultPage in defaultPages)
            {
                var pageRole = defaultPage.Roles.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                var isDisable = defaultPage.Flag == null ? false : defaultPage.Flag.Value;
                if(roles.Any(r => pageRole.Contains(r)) || pageRole.Contains("All") && !isDisable)
                {
                    var url = CommonFunction.Instance.getBaseUri();
                    if(defaultPage.PageType.ToLower() == "url" || defaultPage.PageType.ToLower() == "favorite")
                    {
                        return Redirect(url + defaultPage.PageUrl);
                    }
                    else
                    {
                        var _file = System.IO.File.Exists(Server.MapPath("~/Views//" + defaultPage.PageUrl.TrimEnd('/') + ".cshtml"));
                        if(_file)
                            return Redirect(url + defaultPage.PageUrl);
                        else
                            return RedirectToAction("Index", EntityName);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
        return RedirectToAction("Index", EntityName);
    }
    public ActionResult RedirectToEntityCreate(string EntityName)
    {
        try
        {
            // var roles = ((CustomPrincipal)User).GetRoles();
            var roles = User.userroles;
            var defaultPages = db.DefaultEntityPages.Where(p => p.EntityName == EntityName);
            foreach(var defaultPage in defaultPages)
            {
                var pageRole = defaultPage.Roles.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                var isDisable = defaultPage.Flag == null ? false : defaultPage.Flag.Value;
                if(roles.Any(r => pageRole.Contains(r)) || pageRole.Contains("All") && !isDisable)
                {
                    var url = CommonFunction.Instance.getBaseUri();
                    if(defaultPage.PageType.ToLower() == "url" || defaultPage.PageType.ToLower() == "favorite")
                    {
                        return Redirect(url + defaultPage.PageUrl);
                    }
                    else
                    {
                        var _file = System.IO.File.Exists(Server.MapPath("~/Views//" + defaultPage.PageUrl.TrimEnd('/') + ".cshtml"));
                        if(_file)
                            return Redirect(url + defaultPage.PageUrl);
                        else
                            return RedirectToAction("Create", EntityName);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
        return RedirectToAction("Create", EntityName);
    }
    public JsonResult isAdmin()
    {
        return this.Json(User.IsAdmin, JsonRequestBehavior.AllowGet);
    }
    public string GetTemplatesForHome()
    {
        string HomepageName = "";
        var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                   where s.EntityName == "Home" && s.Flag  !=true
                                   select s;
        bool IsInRoles = false;
        bool IsInRolesChk = false;
        DefaultEntityPage defentityObj = new DefaultEntityPage();
        if(lstDefaultEntityPage.Count() > 0)
        {
            foreach(DefaultEntityPage defentity in lstDefaultEntityPage)
            {
                string role = defentity.Roles.ToString();
                //lstDefaultEntityPage.Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach(var item in rolesArr)
                {
                    if(item.ToString() == "All")
                    {
                        IsInRoles = true;
                        defentityObj = defentity;
                        break;
                    }
                    else
                    {
                        IsInRoles = User.IsInRole(item.ToString());
                        if(IsInRoles)
                        {
                            defentityObj = defentity;
                            IsInRolesChk = true;
                            break;
                        }
                    }
                    if(IsInRolesChk)
                        break;
                }
                if(IsInRolesChk)
                    break;
            }
        }
        if(lstDefaultEntityPage.Count() > 0)
        {
            var defaultEntityPage = defentityObj.HomePage;
            if(defaultEntityPage != null)
                HomepageName = defaultEntityPage.ToString();
        }
        if(!String.IsNullOrEmpty(HomepageName) && IsInRoles)
            return HomepageName;
        return HomepageName;
    }
    public ActionResult Home1(string RegistrationEntity, string TokenId, bool? isThirdParty)
    {
        TempData.Clear();
        if(!string.IsNullOrEmpty(RegistrationEntity) && (Request.UrlReferrer == null || (Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))))
        {
            ViewBag.RegistrationEntity = RegistrationEntity;
            ViewBag.UserId = TokenId;
            return View();
        }
        bool isItemZero = true;
        // var roles = ((CustomPrincipal)User).GetRoles();
        var roles = User.userroles;
        //var MultipleRoleSelection = CommonFunction.Instance.MultipleRoleSelection();
        var PromptForRoleSelection = CommonFunction.Instance.PromptForRoleSelection();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        //ApplicationDbContext userdb = new ApplicationDbContext(true);
        if(roles.Count() > 1 && Request.Cookies[AppUrl+"CurrentRole"]==null)
        {
            if(Convert.ToBoolean(PromptForRoleSelection))
            {
                var userroles = roles.ToList();
                if(!setPreviousCookie(userroles))
                {
                    ViewBag.PageRoles = userroles;
                    return View();
                }
                else
                {
                    var Segments = Request.UrlReferrer.Segments;
                    bool HasHomeseg = false;
                    foreach(var seg in Segments)
                    {
                        if(seg.ToUpper()=="HOME".ToUpper() || seg.ToUpper() == "LOGIN".ToUpper())
                        {
                            HasHomeseg = true;
                        }
                    }
                    if(Request.UrlReferrer != null && !HasHomeseg)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    else
                        return RedirectToAction("Index");
                };
            }
            else
            {
                setRoleCookie(String.Join(",",roles));
            }
        }
        else
        {
            if(roles.Count() > 0)
            {
                var userpagelist = (new UserDefinePagesRoleContext()).UserDefinePagesRoles;
                var role = roles.ToArray()[0];
                var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role);
                if(userpage != null)
                {
                    isItemZero = false;
                    ViewBag.PageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent.Replace("Root_App_Path", GetBaseUrl());
                }
            }
            else
            {
                if(Request.Cookies[AppUrl + "CurrentRole"] != null)
                {
                    RemoveCookie(AppUrl + "CurrentRole");
                    return RedirectToAction("Index");
                }
                ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>";
            }
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
        {
            ViewBag.FavoriteItem = lstFavoriteItem;
            ViewBag.FavoriteCount = lstFavoriteItem.Count();
        }
        if(isItemZero || (ViewBag.PageRoles == null))
        {
            ViewBag.T_CustomerCount = db.T_Customers.Select(p=>p.Id).Count();
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            CompanyProfile cp = commonObj.getCompanyProfile(User);
            if(cp != null)
            {
                ViewBag.AboutCompany = cp.AboutCompany;
            }
        }
        var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
        ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
        ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
        if(userinfo!=null)
        {
            var loginattempt = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id);
            var lastlogin = loginattempt.Count() > 1 ? loginattempt.Skip(1).Take(1).First() : loginattempt.FirstOrDefault();
            //var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).FirstOrDefault();
            ViewBag.LastLoggedIn = lastlogin != null ? lastlogin.Date.ToString() : "";
        }
        ViewBag.LoginRoles = roles;
        SetViewBag("Home1");
        if(!User.IsAdmin)
        {
            string HomePage = GetTemplatesForHome();
            return View(HomePage);
        }
        return View();
    }
    public ActionResult Home3(string RegistrationEntity, string TokenId, bool? isThirdParty)
    {
        TempData.Clear();
        if(!string.IsNullOrEmpty(RegistrationEntity) && (Request.UrlReferrer == null || (Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))))
        {
            ViewBag.RegistrationEntity = RegistrationEntity;
            ViewBag.UserId = TokenId;
            return View();
        }
        bool isItemZero = true;
        // var roles = ((CustomPrincipal)User).GetRoles();
        var roles = User.userroles;
        //var MultipleRoleSelection = CommonFunction.Instance.MultipleRoleSelection();
        var PromptForRoleSelection = CommonFunction.Instance.PromptForRoleSelection();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        //ApplicationDbContext userdb = new ApplicationDbContext(true);
        if(roles.Count() > 1 && Request.Cookies[AppUrl+"CurrentRole"]==null)
        {
            if(Convert.ToBoolean(PromptForRoleSelection))
            {
                var userroles = roles.ToList();
                if(!setPreviousCookie(userroles))
                {
                    ViewBag.PageRoles = userroles;
                    return View();
                }
                else
                {
                    var Segments = Request.UrlReferrer.Segments;
                    bool HasHomeseg = false;
                    foreach(var seg in Segments)
                    {
                        if(seg.ToUpper()=="HOME".ToUpper() || seg.ToUpper() == "LOGIN".ToUpper())
                        {
                            HasHomeseg = true;
                        }
                    }
                    if(Request.UrlReferrer != null && !HasHomeseg)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    else
                        return RedirectToAction("Index");
                };
            }
            else
            {
                setRoleCookie(String.Join(",",roles));
            }
        }
        else
        {
            if(roles.Count() > 0)
            {
                var userpagelist = (new UserDefinePagesRoleContext()).UserDefinePagesRoles;
                var role = roles.ToArray()[0];
                var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role);
                if(userpage != null)
                {
                    isItemZero = false;
                    ViewBag.PageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent.Replace("Root_App_Path", GetBaseUrl());
                }
            }
            else
            {
                if(Request.Cookies[AppUrl + "CurrentRole"] != null)
                {
                    RemoveCookie(AppUrl + "CurrentRole");
                    return RedirectToAction("Index");
                }
                ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>";
            }
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
        {
            ViewBag.FavoriteItem = lstFavoriteItem;
            ViewBag.FavoriteCount = lstFavoriteItem.Count();
        }
        if(isItemZero || (ViewBag.PageRoles == null))
        {
            ViewBag.T_CustomerCount = db.T_Customers.Select(p=>p.Id).Count();
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            CompanyProfile cp = commonObj.getCompanyProfile(User);
            if(cp != null)
            {
                ViewBag.AboutCompany = cp.AboutCompany;
            }
        }
        var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
        ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
        ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
        if(userinfo!=null)
        {
            var loginattempt = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id);
            var lastlogin = loginattempt.Count() > 1 ? loginattempt.Skip(1).Take(1).First() : loginattempt.FirstOrDefault();
            //var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).FirstOrDefault();
            ViewBag.LastLoggedIn = lastlogin != null ? lastlogin.Date.ToString() : "";
        }
        ViewBag.LoginRoles = roles;
        SetViewBag("Home3");
        if(!User.IsAdmin)
        {
            string HomePage = GetTemplatesForHome();
            return View(HomePage);
        }
        return View();
    }
    public void SetViewBag(string actionName)
    {
        var list = db.T_DataMetrics.Where(p =>p.t_associateddatametrictype.T_Name == "Number" && !p.T_Hide.Value && p.T_DisplayOn.Contains(actionName)).OrderBy(p=>p.T_DisplayOrder).ToList();
        list = list.Where(p =>User.CanView(p.T_EntityName) && (string.IsNullOrEmpty(p.T_Roles) || User.IsInRole(User.userroles, p.T_Roles.Split(",".ToCharArray()))) && p.T_AssociatedFacetedSearchID.HasValue).ToList();
        ViewBag.Metrics = list;
        var listmetrics = db.T_DataMetrics.Where(p => p.t_associateddatametrictype.T_Name == "List" && !p.T_Hide.Value && p.T_DisplayOn.Contains(actionName)).OrderBy(p => p.T_DisplayOrder).ToList();
        listmetrics = listmetrics.Where(p =>User.CanView(p.T_EntityName) && (string.IsNullOrEmpty(p.T_Roles) || User.IsInRole(User.userroles, p.T_Roles.Split(",".ToCharArray()))) && p.T_AssociatedFacetedSearchID.HasValue).ToList();
        ViewBag.ListMetrics = listmetrics;
        var listGraph = db.T_DataMetrics.Where(p => p.t_associateddatametrictype.T_Name == "Graph" && !p.T_Hide.Value && p.T_DisplayOn.Contains(actionName)).OrderBy(p => p.T_DisplayOrder).ToList();
        listGraph = listGraph.Where(p =>User.CanView(p.T_EntityName) && (string.IsNullOrEmpty(p.T_Roles) || User.IsInRole(User.userroles, p.T_Roles.Split(",".ToCharArray()))) && p.T_AssociatedFacetedSearchID.HasValue).ToList();
        ViewBag.GraphMetrics = listGraph;
        var listHyperlink = db.T_DataMetrics.Where(p => p.t_associateddatametrictype.T_Name == "HyperLink" && !p.T_Hide.Value && p.T_DisplayOn.Contains(actionName)).OrderBy(p => p.T_DisplayOrder).ToList();
        if(listHyperlink != null)
        {
            listHyperlink = listHyperlink.Where(p => User.CanView(p.T_EntityName) && (string.IsNullOrEmpty(p.T_Roles) || User.IsInRole(User.userroles, p.T_Roles.Split(",".ToCharArray()))) && p.T_AssociatedFacetedSearchID.HasValue).ToList();
            ViewBag.MetricsHyperlink = listHyperlink;
        }
        ViewBag.ShowDashboard = list.Count() > 0 || listGraph.Count() > 0 || listmetrics.Count() > 0 || listHyperlink.Count() > 0;
    }
    public ActionResult Home2(string RegistrationEntity, string TokenId, bool? isThirdParty)
    {
        TempData.Clear();
        if(!string.IsNullOrEmpty(RegistrationEntity) && (Request.UrlReferrer == null || (Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))))
        {
            ViewBag.RegistrationEntity = RegistrationEntity;
            ViewBag.UserId = TokenId;
            return View();
        }
        bool isItemZero = true;
        // var roles = ((CustomPrincipal)User).GetRoles();
        var roles = User.userroles;
        //var MultipleRoleSelection = CommonFunction.Instance.MultipleRoleSelection();
        var PromptForRoleSelection = CommonFunction.Instance.PromptForRoleSelection();
        string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
        //ApplicationDbContext userdb = new ApplicationDbContext(true);
        if(roles.Count() > 1 && Request.Cookies[AppUrl+"CurrentRole"]==null)
        {
            if(Convert.ToBoolean(PromptForRoleSelection))
            {
                var userroles = roles.ToList();
                if(!setPreviousCookie(userroles))
                {
                    ViewBag.PageRoles = userroles;
                    return View();
                }
                else
                {
                    var Segments = Request.UrlReferrer.Segments;
                    bool HasHomeseg = false;
                    foreach(var seg in Segments)
                    {
                        if(seg.ToUpper()=="HOME".ToUpper() || seg.ToUpper() == "LOGIN".ToUpper())
                        {
                            HasHomeseg = true;
                        }
                    }
                    if(Request.UrlReferrer != null && !HasHomeseg)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    else
                        return RedirectToAction("Index");
                };
            }
            else
            {
                setRoleCookie(String.Join(",",roles));
            }
        }
        else
        {
            if(roles.Count() > 0)
            {
                var userpagelist = (new UserDefinePagesRoleContext()).UserDefinePagesRoles;
                var role = roles.ToArray()[0];
                var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role);
                if(userpage != null)
                {
                    isItemZero = false;
                    ViewBag.PageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent.Replace("Root_App_Path", GetBaseUrl());
                }
            }
            else
            {
                if(Request.Cookies[AppUrl + "CurrentRole"] != null)
                {
                    RemoveCookie(AppUrl + "CurrentRole");
                    return RedirectToAction("Index");
                }
                ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>";
            }
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
        {
            ViewBag.FavoriteItem = lstFavoriteItem;
            ViewBag.FavoriteCount = lstFavoriteItem.Count();
        }
        if(isItemZero || (ViewBag.PageRoles == null))
        {
            ViewBag.T_CustomerCount = db.T_Customers.Select(p=>p.Id).Count();
            var commonObj = GeneratorBase.MVC.Models.CommonFunction.Instance;
            CompanyProfile cp = commonObj.getCompanyProfile(User);
            if(cp != null)
            {
                ViewBag.AboutCompany = cp.AboutCompany;
            }
        }
        var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
        ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
        ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
        if(userinfo!=null)
        {
            var loginattempt = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id);
            var lastlogin = loginattempt.Count() > 1 ? loginattempt.Skip(1).Take(1).First() : loginattempt.FirstOrDefault();
            //var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).FirstOrDefault();
            ViewBag.LastLoggedIn = lastlogin != null ? lastlogin.Date.ToString() : "";
        }
        ViewBag.LoginRoles = roles;
        SetViewBag("Home2");
        if(!User.IsAdmin)
        {
            string HomePage = GetTemplatesForHome();
            return View(HomePage);
        }
        return View();
    }
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetChildren(string Id)
    {
        long mId = Convert.ToInt64(Id);
        var list = (new ApplicationContext(new SystemUser())).T_MenuItems.GetFromCache<IQueryable<T_MenuItem>, T_MenuItem>().ToList();
        var data = from x in list.Where(p => p.T_MenuItemMenuItemAssociationID == mId).OrderBy(q => q.T_DisplayOrder).ToList()
                   select new { Id = x.Id, T_Name = x.T_Name, T_Entity = x.T_Entity, T_DisplayOrder = x.T_DisplayOrder, T_LinkAddress = x.T_LinkAddress, T_ClassIcon = x.T_ClassIcon, T_Action = x.T_Action };
        return Json(data, JsonRequestBehavior.AllowGet);
    }
}
}
