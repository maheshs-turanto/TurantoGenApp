using GeneratorBase.MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
[LocalDateTimeConverter]
public class JournalEntryController : JournalEntryBaseController
{
    // public  JournalEntryContext db = new JournalEntryContext(User);
    [Audit("ViewList")]
    public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsMobileRequest, bool? IsFilter, bool? RenderPartial, string FilterHostingEntityID, string FilterHostingEntity, bool? isHomePage, string ExtraIds, string EntityName, string Type, string RoleName, string Tenant, string DateTimeOfEntryFrom, string DateTimeOfEntryTo, string DateTimeOfEntryFromhdn, string DateTimeOfEntryTohdn, string RelatedEntityRecords)
    {
        // using (JournalEntryContext db = new JournalEntryContext(User))
        if(string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
        {
            isAsc = "ASC";
        }
        ViewBag.isAsc = isAsc;
        ViewBag.CurrentSort = sortBy;
        ViewData["RelatedEntityRecords"] = RelatedEntityRecords;
        ViewData["HostingEntity"] = HostingEntity;
        ViewData["HostingEntityID"] = HostingEntityID;
        ViewData["AssociatedType"] = AssociatedType;
        ViewData["FilterHostingEntity"] = FilterHostingEntity;
        ViewData["FilterHostingEntityID"] = FilterHostingEntityID;
        // EntityNameJournal = AssociatedType;
        ViewData["IsFilter"] = IsFilter.HasValue ? IsFilter.Value : false;
        if(searchString != null)
            page = null;
        else
            searchString = currentFilter;
        ViewBag.CurrentFilter = searchString;
        var lstJournal = from s in db.JournalEntries.AsNoTracking()
                         select s;
        if(HostingEntity != null && HostingEntityID != null)
        {
            if(isHomePage != null && isHomePage.Value)
            {
                lstJournal = lstJournal.Where(p => p.EntityName == HostingEntity && (p.Type == "Modified" || p.Type == "Added" || p.Type == "Deleted") && p.PropertyName != "T_RecordAddedInsertBy" && p.PropertyName != "T_RecordAddedInsertBy" && p.PropertyName != "T_RecordAddedInsertDate" && p.PropertyName != "T_RecordAdded");
                lstJournal = lstJournal.GroupBy(p => p.RecordId, (key, g) => g.OrderByDescending(e => e.DateTimeOfEntry).FirstOrDefault());
                lstJournal = sortRecords(lstJournal.Where(p => p.Type != "Deleted"), "DateTimeOfEntry", "desc");
                lstJournal = lstJournal.Take(5);
                ViewBag.IsHomePage = isHomePage.Value;
            }
            else
            {
                try
                {
                    Type controller = System.Type.GetType("GeneratorBase.MVC.Controllers." + HostingEntity + "Controller");
                    using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                    {
                        System.Reflection.MethodInfo mc = controller.GetMethod("GetExtraJournalEntry");
                        object[] MethodParams = new object[] { HostingEntityID, User, db, RelatedEntityRecords };
                        var obj = mc.Invoke(objController, MethodParams);
                        lstJournal = (IQueryable<JournalEntry>)obj;
                        lstJournal = lstJournal.Distinct().OrderByDescending(p => p.Id);
                        if(RelatedEntityRecords == HostingEntity || string.IsNullOrEmpty(RelatedEntityRecords))
                        {
                            var lstJournalMain =  db.JournalEntries.AsNoTracking();
                            lstJournalMain = new FilteredDbSet<JournalEntry>(db, d => d.Id > 0);
                            lstJournalMain = lstJournalMain.Where(p => p.EntityName == HostingEntity && p.RecordId == HostingEntityID).OrderByDescending(p => p.Id);
                            lstJournal = lstJournalMain.Union(lstJournal).Distinct().OrderByDescending(p => p.Id);
                        }
                    }
                }
                catch
                {
                    lstJournal = new FilteredDbSet<JournalEntry>(db, d => d.Id > 0);
                    lstJournal = lstJournal.Where(p => p.EntityName == HostingEntity && p.RecordId == HostingEntityID).OrderByDescending(p => p.Id);
                }
            }
        }
        if(FilterHostingEntity != null && FilterHostingEntityID != null)
        {
            var hostid = Convert.ToInt64(FilterHostingEntityID);
            lstJournal = lstJournal.Where(p => p.EntityName == FilterHostingEntity && p.RecordId == hostid).OrderByDescending(p => p.Id);
            ViewData["HostingEntity"] = FilterHostingEntity;
            ViewData["HostingEntityID"] = FilterHostingEntityID;
        }
        //
        if(!string.IsNullOrEmpty(ExtraIds))
        {
            List<long> ids = ExtraIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToList();
            lstJournal = db.JournalEntries.AsNoTracking().Where(p => ids.Contains(p.Id)).Union(lstJournal).Distinct().OrderByDescending(p => p.Id);
        }
        //
        //if ((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "EntityName")
        //{
        //    lstJournal = lstJournal.Where(p => p.EntityName == AssociatedType).OrderByDescending(p => p.Id);
        //}
        if((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "EntityName")
        {
            if(User.IsAdmin)
            {
                lstJournal = lstJournal.Where(p => p.EntityName == AssociatedType).OrderByDescending(p => p.Id);
            }
            else
            {
                if(User.CanView(AssociatedType))
                {
                    lstJournal = lstJournal.Where(d => !string.IsNullOrEmpty(d.EntityName) && d.EntityName == AssociatedType);
                    //lstJournal = new FilteredDbSet<JournalEntry>(db, d => !string.IsNullOrEmpty(d.EntityName) && d.EntityName == AssociatedType);
                    //var userList = db.T_Users.Select(p => p.UserName);
                    //lstJournal = lstJournal.Where(p => userList.Contains(p.UserName));
                }
                else
                {
                    lstJournal = lstJournal.Where(d => d.Id == 0);
                    //lstJournal = new FilteredDbSet<JournalEntry>(db, d => d.Id == 0);
                }
            }
        }
        if((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "Type")
        {
            lstJournal = lstJournal.Where(p => p.Type == AssociatedType).OrderByDescending(p => p.Id);
        }
        if((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "UserName")
        {
            lstJournal = lstJournal.Where(p => p.UserName == AssociatedType).OrderByDescending(p => p.Id);
        }
        if((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "PropertyName")
        {
            lstJournal = lstJournal.Where(p => p.PropertyName == AssociatedType).OrderByDescending(p => p.Id);
        }
        if(DateTimeOfEntryFrom != null || DateTimeOfEntryTo != null)
        {
            try
            {
                DateTime from = DateTimeOfEntryFrom == null ? Convert.ToDateTime("01/01/1900") :  TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(DateTimeOfEntryFrom),(new JournalEntry().m_Timezone));
                DateTime to = DateTimeOfEntryTo == null ? DateTime.MaxValue : TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(DateTimeOfEntryTo), (new JournalEntry().m_Timezone));
                lstJournal = lstJournal.Where(o => o.DateTimeOfEntry >= from && o.DateTimeOfEntry <= to);
                ViewBag.SearchResult += "\r\n DateTimeOfEntry= " + DateTimeOfEntryFromhdn + "-" + DateTimeOfEntryTohdn;
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        if(EntityName != null)
        {
            var ids = EntityName.Split(",".ToCharArray());
            List<string> ids1 = new List<string>();
            ViewBag.SearchResult += "\r\n Entity= ";
            foreach(var str in ids)
            {
                //Null Search
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        ViewBag.SearchResult += "";
                    }
                    else
                    {
                        ids1.Add(Convert.ToString(str));
                        var obj = getEntityDisplayName(str);
                        ViewBag.SearchResult += obj + ", ";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            //_JournalEntry = _JournalEntry.Where(p => ids1.Contains(p.EntityName));
            if(User.IsAdmin)
            {
                lstJournal = lstJournal.Where(p => ids1.Contains(p.EntityName)).OrderByDescending(p => p.Id);
            }
            else
            {
                lstJournal = lstJournal.Where(p => ids1.Contains(p.EntityName)).OrderByDescending(p => p.Id);
                //lstJournal = new FilteredDbSet<JournalEntry>(db, d => ids1.Contains(d.EntityName));
                //var userList = db.T_Users.Select(p => p.UserName);
                //lstJournal = lstJournal.Where(p => userList.Contains(p.UserName));
            }
        }
        if(Type != null)
        {
            var ids = Type.Split(",".ToCharArray());
            List<string> ids1 = new List<string>();
            ViewBag.SearchResult += "\r\n Type= ";
            foreach(var str in ids)
            {
                //Null Search
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        ViewBag.SearchResult += "";
                    }
                    else
                    {
                        ids1.Add((str));
                        ViewBag.SearchResult += str + ", ";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            lstJournal = lstJournal.Where(p => ids1.Contains(p.Type));
        }
        if(RoleName != null)
        {
            var ids = RoleName.Split(",".ToCharArray());
            List<string> ids1 = new List<string>();
            ViewBag.SearchResult += "\r\n RoleName= ";
            foreach(var str in ids)
            {
                //Null Search
                if(!string.IsNullOrEmpty(str))
                {
                    if(str == "NULL")
                    {
                        ids1.Add(null);
                        ViewBag.SearchResult += "";
                    }
                    else
                    {
                        ids1.Add(str);
                        ViewBag.SearchResult += str + ", ";
                    }
                }
                //
            }
            ids1 = ids1.ToList();
            lstJournal = lstJournal.Where(p => ids1.Contains(p.RoleName));
        }
        if(!String.IsNullOrEmpty(searchString))
        {
            lstJournal = searchRecords(lstJournal, searchString.ToUpper(), IsDeepSearch);
        }
        if(!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc) && isHomePage == null)
        {
            lstJournal = sortRecords(lstJournal, sortBy, isAsc);
        }
        else lstJournal = lstJournal.OrderByDescending(c => c.Id);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        ViewBag.Pages = page;
        if(itemsPerPage != null)
        {
            pageSize = (int)itemsPerPage;
            ViewBag.CurrentItemsPerPage = itemsPerPage;
        }
        var _JournalEntry = lstJournal;
        if(Convert.ToBoolean(IsExport))
        {
            pageNumber = 1;
            if(_JournalEntry.Count() > 0)
                pageSize = _JournalEntry.Count();
            return View("ExcelExport", _JournalEntry.ToPagedList(pageNumber, pageSize));
        }
        else
        {
            if(pageNumber > 1)
            {
                var totalListCount = _JournalEntry.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if(pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }
        }
        if(IsMobileRequest != true)
        {
            if(Request.AcceptTypes.Contains("text/html"))
            {
                if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                    return View(_JournalEntry.ToPagedList(pageNumber, pageSize));
                else
                    return PartialView("IndexPartial", _JournalEntry.ToPagedList(pageNumber, pageSize));
            }
            else if(Request.AcceptTypes.Contains("application/json"))
            {
                var Result = getJournalEntryList(_JournalEntry);
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            var Result = getJournalEntryList(_JournalEntry);
            return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        if(!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            return View(_JournalEntry.ToPagedList(pageNumber, pageSize));
        else
            return PartialView("IndexPartial", _JournalEntry.ToPagedList(pageNumber, pageSize));
    }
    public ActionResult SetFSearch(string searchString, string HostingEntity, string EntityName, string Type, string RoleName, string Tenant, bool? RenderPartial, bool ShowDeleted = false)
    {
        int Qcount = 0;
        if(Request.UrlReferrer != null)
            Qcount = Request.UrlReferrer.Query.Count();
        //For Reports
        if((RenderPartial == null ? false : RenderPartial.Value))
            Qcount = Request.QueryString.AllKeys.Count();
        ViewBag.CurrentFilter = searchString;
        // var EntityNamelist = db.JournalEntries.Select(p => p.EntityName).Distinct().OrderBy(p => p);
        var EntityNamelist = from x in db.JournalEntries.Select(p => p.EntityName).Distinct().OrderBy(p => p).ToList().Where(p => User.CanView(p))
                             select new { Id = x, Name = getEntityDisplayName(x) };
        var Typelist = db.JournalEntries.Select(p => p.Type).Distinct().OrderBy(p => p);
        var RoleNamelist = User.userroles;
        if(Qcount > 0)
        {
            if(EntityName != null)
            {
                var ids = EntityName.Split(",".ToCharArray());
                List<string> ids1 = new List<string>();
                ViewBag.SearchResult += "\r\n Entity= ";
                foreach(var str in ids)
                {
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            var obj = EntityNamelist.FirstOrDefault(p => p.Id == (str));
                            ViewBag.SearchResult += obj.Name + ", ";
                        }
                    }
                }
                ids1 = ids1.ToList();
                ViewBag.EntityName = new SelectList(EntityNamelist, "Id", "Name");
            }
            else
            {
                var list = EntityNamelist;
                ViewBag.EntityName = new SelectList(list, "Id", "Name");
            }
            if(Type != null)
            {
                var ids = Type.Split(",".ToCharArray());
                List<string> ids1 = new List<string>();
                ViewBag.SearchResult += "\r\n Type= ";
                foreach(var str in ids)
                {
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ViewBag.SearchResult += Typelist.FirstOrDefault(p => p == (str)) + ", ";
                        }
                    }
                }
                ids1 = ids1.ToList();
                ViewBag.Type = new SelectList(Typelist);
            }
            else
            {
                var list = Typelist;
                ViewBag.Type = new SelectList(list);
            }
            if(RoleName != null)
            {
                var ids = RoleName.Split(",".ToCharArray());
                List<string> ids1 = new List<string>();
                ViewBag.SearchResult += "\r\n RoleName= ";
                foreach(var str in ids)
                {
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ViewBag.SearchResult += RoleNamelist.FirstOrDefault(p => p == (str)) + ", ";
                        }
                    }
                }
                ids1 = ids1.ToList();
                ViewBag.RoleName = new SelectList(RoleNamelist);
            }
            else
            {
                var list = RoleNamelist;
                ViewBag.RoleName = new SelectList(list);
            }
        }
        else
        {
            ViewBag.EntityName = new SelectList(EntityNamelist, "Id", "Name");
            ViewBag.Type = new SelectList(Typelist);
            ViewBag.RoleName = new SelectList(RoleNamelist);
            //ViewBag.Tenant = new SelectList(Tenantlist);
        }
        var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
        if(lstFavoriteItem.Count() > 0)
            ViewBag.FavoriteItem = lstFavoriteItem;
        return View(new JournalEntry());
    }
    private Object getJournalEntryList(IQueryable<JournalEntry> journalEntry)
    {
        // var _journalEntry = from obj in journalEntry
        //                    select new
        //{
        //    DateTimeOfEntry = obj.DateTimeOfEntry,
        //    EntityName = obj.EntityName,
        //    NewValue = obj.NewValue,
        //    OldValue = obj.OldValue,
        //    PropertyName = obj.PropertyName,
        //    RecordId = obj.RecordId,
        //    RecordInfo = obj.RecordInfo,
        //    Type = obj.Type,
        //    UserName = obj.UserName,
        //    ID = obj.Id
        //};
        return journalEntry;
    }
    private Object getJournalItem(JournalEntry journalEntry)
    {
        return new
        {
            DateTimeOfEntry = journalEntry.DateTimeOfEntry,
            EntityName = journalEntry.EntityName,
            NewValue = journalEntry.NewValue,
            OldValue = journalEntry.OldValue,
            PropertyName = journalEntry.PropertyName,
            RecordId = journalEntry.RecordId,
            RecordInfo = journalEntry.RecordInfo,
            Type = journalEntry.Type,
            UserName = journalEntry.UserName,
            ID = journalEntry.Id
        };
    }
    private IQueryable<JournalEntry> searchRecords(IQueryable<JournalEntry> lstjournalEntry, string searchString, bool? IsDeepSearch)
    {
        using(JournalEntryContext db = new JournalEntryContext(User))
        {
            if(Convert.ToBoolean(IsDeepSearch))
                lstjournalEntry = lstjournalEntry.Where(s => (s.RecordId != null && SqlFunctions.StringConvert((double)s.RecordId).Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.NewValue) && s.NewValue.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.OldValue) && s.OldValue.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.RecordInfo) && s.RecordInfo.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.Type) && s.Type.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.RoleName) && s.RoleName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.Source) && s.Source.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
            else
                lstjournalEntry = lstjournalEntry.Where(s => (s.RecordId != null && SqlFunctions.StringConvert((double)s.RecordId).Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.NewValue) && s.NewValue.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.OldValue) && s.OldValue.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.RecordInfo) && s.RecordInfo.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.Type) && s.Type.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.RoleName) && s.RoleName.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.Source) && s.Source.ToUpper().Contains(searchString))
                                                        || (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
            try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstjournalEntry = lstjournalEntry.Union(db.JournalEntries.Where(s => (s.DateTimeOfEntry == datevalue)));
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return lstjournalEntry;
        }
    }
    private IQueryable<JournalEntry> sortRecords(IQueryable<JournalEntry> lstJournalEntry, string sortBy, string isAsc)
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
        ParameterExpression paramExpression = Expression.Parameter(typeof(JournalEntry), "journalentry");
        MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
        LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
        return (IQueryable<JournalEntry>)lstJournalEntry.Provider.CreateQuery(
                   Expression.Call(
                       typeof(Queryable),
                       methodName,
                       new Type[] { lstJournalEntry.ElementType, lambda.Body.Type },
                       lstJournalEntry.Expression,
                       lambda));
    }
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            if(db != null) db.Dispose();
        }
        base.Dispose(disposing);
    }
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string HostingEntityName, string HostingEntity, string HostingEntityID, string ExtraIds, string EntityNameJournal)
    {
        //using (JournalEntryContext db = new JournalEntryContext(User))
        {
            IQueryable<JournalEntry> list = db.JournalEntries;
            ViewData["HostingEntityID"] = HostingEntityID;
            ViewData["HostingEntity"] = HostingEntity;
            ViewBag.ExtraIds = ExtraIds;
            if(HostingEntity != null && HostingEntityID != null)
            {
                var hostid = Convert.ToInt64(HostingEntityID);
                list = list.Where(p => p.EntityName == HostingEntity && p.RecordId == hostid).OrderByDescending(p => p.Id);
            }
            if(!string.IsNullOrEmpty(ExtraIds))
            {
                List<long> ids = ExtraIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToList();
                list = db.JournalEntries.Where(p => ids.Contains(p.Id)).Union(list).Distinct().OrderByDescending(p => p.Id);
            }
            if(HostingEntityName == "EntityName")
            {
                if(HostingEntity != null)
                {
                    var query = from x in list.Where(x => x.EntityName == HostingEntity)
                                .Select(p => p.EntityName).Distinct().OrderBy(p => p).ToList()
                                select new { Id = x, Name = getEntityDisplayName(x) };
                    return Json(query, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = from x in list.Select(p => p.EntityName).Distinct().OrderBy(p => p).ToList()
                                select new { Id = x, Name = getEntityDisplayName(x) };
                    //.GroupBy(s => s.EntityName) // groups identical strings into an IGrouping
                    //.OrderByDescending(group1 => group1.Count()).ToList() // IGrouping is a collection, so you can count it
                    //            select new { Id = x.Key, Name = getEntityDisplayName(x.Key) };
                    return Json(query, JsonRequestBehavior.AllowGet);
                }
            }
            if(HostingEntityName == "Type")
            {
                var data = from x in list.Select(p => p.Type).Distinct().OrderBy(p => p).ToList()
                           select new { Id = x, Name = x };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if(HostingEntityName == "UserName")
            {
                //if (User.IsAdmin)
                //{
                var query = from x in list.Select(p => p.UserName).Distinct().OrderBy(p => p).ToList()
                            select new { Id = x, Name = x };
                return Json(query, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var query = from x in userList.OrderBy(p=>p) select new { Id = x, Name = x };
                //    return Json(query, JsonRequestBehavior.AllowGet);
                //}
            }
            if(HostingEntityName == "PropertyName")
            {
                //var data = from x in list.Select(p => p.PropertyName).Distinct().ToList()
                //           select new { Id = x, Name = x };
                var query = list.Where(x => x.PropertyName != null && x.EntityName == EntityNameJournal)
                            .GroupBy(x => new
                {
                    x.PropertyName, x.EntityName
                })
                .OrderByDescending(group1 => group1.Count()).ToList()
                .Select(grouped => new
                {
                    Id = grouped.Key.PropertyName, Name = getPropertyDisplayName(grouped.Key.PropertyName, EntityNameJournal)
                });
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
    private string getPropertyDisplayName(object p)
    {
        throw new NotImplementedException();
    }
    public string getPropertyDisplayName(string Property, string EntityName)
    {
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            prop = EntityReflector.Properties.FirstOrDefault(q => q.Name == Property).DisplayName;
        }
        catch(Exception ex)
        {
            prop = Property;
        }
        return prop;
    }
    public string getEntityDisplayName(string EntityName)
    {
        string prop = "";
        try
        {
            var EntityReflector = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            prop = EntityReflector.DisplayName;
        }
        catch(Exception ex)
        {
            prop = EntityName;
        }
        return prop;
    }
}
}

