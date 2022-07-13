using GeneratorBase.MVC.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using System;
namespace GeneratorBase.MVC
{
public class JournalEntrySecurityFilter : IFilter<JournalEntryContext>
{
    IUser user;
    public JournalEntryContext DbContext
    {
        get;
        set;
    }
    public JournalEntrySecurityFilter(IUser user)
    {
        this.user = user;
    }
    public void ApplyBasicSecurity()
    {
    }
    public void ApplyUserBasedSecurity()
    {
    }
    public void CustomFilter()
    {
    }
    public void ApplyHierarchicalSecurity()
    {
    }
    public void ApplyMainSecurity()
    {
        if(string.IsNullOrEmpty(user.Name))
            return;
        //var userList = DbContext.T_Users.Select(p => p.UserName);
        Expression<Func<JournalEntry, bool>> predicateJournalEntry = d => (d.Id > 0);
        //Expression<Func<JournalEntry, bool>> predicateJournalEntry = d => (userList.Contains(d.UserName));
        if(!user.CanView("T_Customer"))
        {
            predicateJournalEntry = predicateJournalEntry.And(d => d.EntityName != "T_Customer");
        }
        if(!user.CanView("T_Schedule"))
        {
            predicateJournalEntry = predicateJournalEntry.And(d => d.EntityName != "T_Schedule");
        }
        DbContext.JournalEntries = new FilteredDbSet<JournalEntry>(DbContext, predicateJournalEntry);
    }
}
}

