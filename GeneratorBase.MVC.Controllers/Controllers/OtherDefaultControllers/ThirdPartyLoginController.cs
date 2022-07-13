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
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling third party logins.</summary>
public class ThirdPartyLoginController : Controller
{
    /// <summary>The repository.</summary>
    private IThirdPartyLoginRepository _repository;
    /// <summary>Default constructor.</summary>
    public ThirdPartyLoginController()
        : this(new ThirdPartyLoginRepository())
    {
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="repository">The repository.</param>
    
    public ThirdPartyLoginController(IThirdPartyLoginRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits the given cp.</summary>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    public ActionResult Edit()
    {
        if(((CustomPrincipal)User).IsAdmin)
        {
            ThirdPartyLogin cp = _repository.GetThirdPartyLogin();
            if(cp == null)
                return RedirectToAction("Index");
            return View(cp);
        }
        else return RedirectToAction("Index", "Home");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) edits the given cp.</summary>
    ///
    /// <param name="cp">The cp.</param>
    ///
    /// <returns>A response stream to send to the Edit View.</returns>
    
    [HttpPost]
    public ActionResult Edit(ThirdPartyLogin cp)
    {
        if(((CustomPrincipal)User).IsAdmin)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _repository.EditThirdPartyLogin(cp);
                    return RedirectToAction("Index","Admin");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Error editing record. " + ex.Message);
                }
            }
        }
        return RedirectToAction("Index", "Home");
    }
}
}

