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
using System.Drawing.Imaging;
using System.Web.Helpers;
using RecurrenceGenerator;
namespace GeneratorBase.MVC.Controllers
{
public partial class T_ScheduleController : BaseController
{
    public ActionResult CreateEvent(string startDate, string slotID)
    {
        Dictionary<string,string> list = new Dictionary<string,string>();
        ViewData["startDate"] = startDate;
        ViewData["slotID"] = slotID;
        return View(list);
    }
    public void AfterSave(T_Schedule t_schedule, GeneratorBase.MVC.Models.IUser aftersaveuser, EntityState entityState) //mahesh
    {
        if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn == null && t_schedule.T_RepeatOn_t_schedule.Count() > 0)
        {
            return;
        }
        bool flagT_RepeatOn = false;
        using(ApplicationContext tempdb = new ApplicationContext(new SystemUser()))
        {
            foreach(var obj in tempdb.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id))
            {
                tempdb.T_RepeatOns.Remove(obj);
                flagT_RepeatOn = true;
            }
            if(flagT_RepeatOn)
                tempdb.SaveChanges();
            if(t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach(var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                    objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                    objT_RepeatOn.T_RecurrenceDaysID = pgs;
                    tempdb.T_RepeatOns.Add(objT_RepeatOn);
                }
                tempdb.SaveChanges();
            }
        }
    }
    public ActionResult getRegisteredEvents(DateTime currentDate, string currentView)
    {
        currentDate = currentDate.Date;
        List<TemplateEvents> result = new List<TemplateEvents>();
        DateTime start = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek); // prev sunday 00:00
        var end = start.AddDays(7);
        var selectedMonth = currentDate.Month;
        var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        jsonResult.MaxJsonLength = int.MaxValue;
        return jsonResult;
    }
    public ActionResult getAllMeetingEvents(DateTime currentDate, string currentView)
    {
        currentDate = currentDate.Date;
        List<TemplateEvents> result = new List<TemplateEvents>();
        DateTime start = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek); // prev sunday 00:00
        var end = start.AddDays(7);
        var selectedMonth = currentDate.Month;
        var NowDate = DateTime.UtcNow.Date;
        foreach(var item in db.T_Schedules.Where(p=>p.T_EndDate == null || (p.T_EndDate != null && p.T_EndDate >=DbFunctions.AddDays(DateTime.UtcNow,-1))).ToList())
        {
        }
        //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        jsonResult.MaxJsonLength = int.MaxValue;
        return jsonResult;
    }
    public ActionResult getEvents(string ids)//mahesh
    {
        List<TemplateEvents> result = new List<TemplateEvents>();
        if(!string.IsNullOrEmpty(ids))
            foreach(var item in ids.Split(",".ToCharArray()))
            {
                if(string.IsNullOrEmpty(item)) continue;
                var id = Convert.ToInt64(item);
                var scheduleobj = db.T_Schedules.Find(id);
            }
        // result.ForEach(p => p.schedule = null);
        //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        jsonResult.MaxJsonLength = int.MaxValue;
        return jsonResult;
    }
    public ActionResult EventsUpdate(string id,string EntityName)
    {
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        long ID = Convert.ToInt64(id);
        return View();
    }
    public ActionResult getTimeSlotsByIds(string ids)
    {
        List<TemplateEvents> result = new List<TemplateEvents>();
        //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        jsonResult.MaxJsonLength = int.MaxValue;
        return jsonResult;
    }
    public ActionResult getTimeSlots(string EntityName, long id)
    {
        List<TemplateEvents> result = new List<TemplateEvents>();
        //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        jsonResult.MaxJsonLength = int.MaxValue;
        return jsonResult;
    }
    public ActionResult ShowFullCalendar()
    {
        var objlist = db.T_Schedules.AsQueryable();
        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
        return View(objlist.ToList());
    }
}
public class UpdateButton
{
    public string ButtonText
    {
        get;
        set;
    }
    public string Target
    {
        get;
        set;
    }
    public string AssociatedType
    {
        get;
        set;
    }
    public string HostingEntityName
    {
        get;
        set;
    }
    public string PopupTitle
    {
        get;
        set;
    }
    public string Type
    {
        get;
        set;
    }
}
}

