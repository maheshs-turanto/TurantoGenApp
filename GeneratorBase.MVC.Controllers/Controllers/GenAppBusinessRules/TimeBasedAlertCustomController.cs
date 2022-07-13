using GeneratorBase.MVC.Models;
using RecurrenceGenerator;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{

/// <summary>A controller for handling time based alerts.</summary>
public partial class TimeBasedAlertController : Controller
{
    private Dictionary<string, string> CustomEmailBodyMapping(Dictionary<string, string> dictionary, ApplicationUser userinfo, object entityobj, string EntityName)
    {
        return dictionary;
    }
}
}