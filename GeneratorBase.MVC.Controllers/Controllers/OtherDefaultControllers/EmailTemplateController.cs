using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Linq.Expressions;
using System.Reflection;
namespace GeneratorBase.MVC.Controllers.OtherDefaultControllers
{
/// <summary>A controller for handling email templates.</summary>
public class EmailTemplateController : BaseController
{
    /// <summary>Gets the index.</summary>
    ///
    /// <returns>A response stream to send to the Index View.</returns>
    
    public ActionResult Index()
    {
        var lstEmailTemplate = from s in db.EmailTemplates
                               select s;
        var _EmailTemplate = lstEmailTemplate.Include(t => t.associatedemailtemplatetype);
        ViewBag.EmailTemplateType = db.EmailTemplateTypes.ToList();
        if(!Request.IsAjaxRequest())
            return View(_EmailTemplate);
        else
            return PartialView("IndexPartial", _EmailTemplate);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a new ActionResult.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    public ActionResult Create(string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("EmailTemplate"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateParentUrl"] = UrlReferrer;
        var objAssociatedEmailTemplateType = db.EmailTemplateTypes.OrderBy(p => p.Name).ToList();
        ViewBag.AssociatedEmailTemplateTypeID = new SelectList(objAssociatedEmailTemplateType, "ID", "DisplayValue");
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates a new ActionResult.</summary>
    ///
    /// <param name="emailtemplate">The emailtemplate.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="IsAddPop">     The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Create View.</returns>
    
    [HttpPost]
    [ValidateInput(false)]
    public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,AssociatedEmailTemplateTypeID,EmailContent")] EmailTemplate emailtemplate, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.EmailTemplates.Add(emailtemplate);
            db.SaveChanges();
        }
        return RedirectToAction("Index", "EmailTemplate");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("EmailTemplate"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EmailTemplate emailtemplate = db.EmailTemplates.Find(id);
        if(emailtemplate == null)
        {
            return HttpNotFound();
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateParentUrl"] = UrlReferrer;
        var objAssociatedEmailTemplateType =  db.EmailTemplateTypes.OrderBy(p => p.Name).ToList();
        objAssociatedEmailTemplateType.Add(emailtemplate.associatedemailtemplatetype);
        ViewBag.AssociatedEmailTemplateTypeID = new SelectList(objAssociatedEmailTemplateType, "ID", "DisplayValue", emailtemplate.AssociatedEmailTemplateTypeID);
        return View(emailtemplate);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits.</summary>
    ///
    /// <param name="emailtemplate">The emailtemplate.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    /// <param name="IsAddPop">     The is add pop.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,AssociatedEmailTemplateTypeID,EmailContent,EmailSubject")] EmailTemplate emailtemplate, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.Entry(emailtemplate).State = EntityState.Modified;
            db.SaveChanges();
        }
        return RedirectToAction("Index", "EmailTemplate");
    }
    
    /// <summary>Deletes the email template.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the DeleteEmailTemplate View.</returns>
    
    public ActionResult DeleteEmailTemplate(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanDelete("EmailTemplate"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EmailTemplate emailtemplate = db.EmailTemplates.Find(id);
        if(emailtemplate == null)
        {
            throw(new Exception("Deleted"));
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateParentUrl"] = UrlReferrer;
        return View(emailtemplate);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) deletes the email template confirmed.</summary>
    ///
    /// <param name="emailtemplate">The emailtemplate.</param>
    /// <param name="UrlReferrer">  The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteEmailTemplateConfirmed View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteEmailTemplateConfirmed(EmailTemplate emailtemplate, string UrlReferrer)
    {
        if(!User.CanDelete("EmailTemplate"))
        {
            return RedirectToAction("Index", "Error");
        }
        db.Entry(emailtemplate).State = EntityState.Deleted;
        db.EmailTemplates.Remove(emailtemplate);
        db.SaveChanges();
        return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates email template type.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateEmailTemplateType View.</returns>
    
    public ActionResult CreateEmailTemplateType(string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("EmailTemplateType"))
        {
            return RedirectToAction("Index", "Error");
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) creates email template type.</summary>
    ///
    /// <param name="emailtemplatetype">The emailtemplatetype.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the CreateEmailTemplateType View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateEmailTemplateType([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] EmailTemplateType emailtemplatetype, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.EmailTemplateTypes.Add(emailtemplatetype);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit email template type.</summary>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditEmailTemplateType View.</returns>
    
    public ActionResult EditEmailTemplateType(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanAdd("EmailTemplateType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EmailTemplateType emailtemplatetype = db.EmailTemplateTypes.Find(id);
        if(emailtemplatetype == null)
        {
            return HttpNotFound();
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
        return View(emailtemplatetype);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edit email template type.</summary>
    ///
    /// <param name="emailtemplatetype">The emailtemplatetype.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    /// <param name="IsAddPop">         The is add pop.</param>
    ///
    /// <returns>A response stream to send to the EditEmailTemplateType View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditEmailTemplateType([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] EmailTemplateType emailtemplatetype, string UrlReferrer, bool? IsAddPop)
    {
        if(ModelState.IsValid)
        {
            db.Entry(emailtemplatetype).State = EntityState.Modified;
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
            return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        return View();
    }
    
    /// <summary>Deletes the email template type.</summary>
    ///
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    ///
    /// <param name="id">         The identifier.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="IsAddPop">   The is add pop.</param>
    ///
    /// <returns>A response stream to send to the DeleteEmailTemplateType View.</returns>
    
    public ActionResult DeleteEmailTemplateType(long? id, string UrlReferrer, bool? IsAddPop)
    {
        if(!User.CanDelete("EmailTemplateType"))
        {
            return RedirectToAction("Index", "Error");
        }
        if(id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        EmailTemplateType emailtemplatetype = db.EmailTemplateTypes.Find(id);
        if(emailtemplatetype == null)
        {
            throw(new Exception("Deleted"));
        }
        ViewBag.IsAddPop = IsAddPop;
        ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
        return View(emailtemplatetype);
    }
    
    /// <summary>(An Action that handles HTTP POST requests) deletes the email template type
    /// confirmed.</summary>
    ///
    /// <param name="emailtemplatetype">The emailtemplatetype.</param>
    /// <param name="UrlReferrer">      The URL referrer.</param>
    ///
    /// <returns>A response stream to send to the DeleteEmailTemplateTypeConfirmed View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteEmailTemplateTypeConfirmed(EmailTemplateType emailtemplatetype, string UrlReferrer)
    {
        if(!User.CanDelete("EmailTemplateType"))
        {
            return RedirectToAction("Index", "Error");
        }
        var emailtemplate = db.EmailTemplates.Where(et => et.AssociatedEmailTemplateTypeID == emailtemplatetype.Id);
        if(emailtemplate.Count() > 0)
        {
            var AlertMsg = "Email Template Type " + emailtemplate.FirstOrDefault().DisplayValue + " can not be delete before deleteing its Email Template";
            ModelState.AddModelError("CustomError", AlertMsg);
            ViewBag.ApplicationError = AlertMsg;
            return Json(AlertMsg, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        else
        {
            db.Entry(emailtemplatetype).State = EntityState.Deleted;
            db.EmailTemplateTypes.Remove(emailtemplatetype);
            db.SaveChanges();
        }
        return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
    }
    
    /// <summary>Cancels.</summary>
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
    
    /// <summary>Creates a JSON result with the given data as its content.</summary>
    ///
    /// <param name="caller">            The caller.</param>
    /// <param name="key">               The key.</param>
    /// <param name="AssoNameWithParent">The asso name with parent.</param>
    /// <param name="AssociationID">     Identifier for the association.</param>
    /// <param name="ExtraVal">          The extra value.</param>
    ///
    /// <returns>A JSON response stream to send to the GetAllValue action.</returns>
    
    [AcceptVerbs(HttpVerbs.Get)]
    public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
    {
        IQueryable<EmailTemplateType> list = db.EmailTemplateTypes;
        if(AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
        {
            Nullable<long> AssoID = Convert.ToInt64(AssociationID);
            if(AssoID != null && AssoID > 0)
            {
                IQueryable query = db.EmailTemplateTypes;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                ParameterExpression p = Expression.Parameter(typeof(EmailTemplateType), "p");
                MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                LambdaExpression lambda = Expression.Lambda<Func<EmailTemplateType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<EmailTemplateType>)q);
            }
        }
        if(list.Count() > 0)
            list = list.Where(p => !(db.EmailTemplates.Select(et => et.AssociatedEmailTemplateTypeID).Contains(p.Id)));
        if(key != null && key.Length > 0)
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            if(ExtraVal != null && ExtraVal.Length > 0)
            {
                long? val = Convert.ToInt64(ExtraVal);
                var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
    
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
