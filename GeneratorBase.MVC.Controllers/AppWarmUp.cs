using GeneratorBase.MVC.Models;
using System.Collections.Generic;
using System.Linq;
namespace GeneratorBase.MVC.Controllers
{
public class AppWarmUp : System.Web.Hosting.IProcessHostPreloadClient
{
    public void Preload(string[] parameters)
    {
        var counts = new List<int>();
        counts.Add(new ApplicationDbContext().Users.Count());
        counts.Add(new ReportsContext().Reportss.Count());
        counts.Add(new JournalEntryContext().JournalEntries.Count());
    }
}
}