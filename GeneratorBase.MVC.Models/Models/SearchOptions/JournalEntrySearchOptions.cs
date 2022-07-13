using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models.SearchOptions
{
/// <summary>A journal entry search options.</summary>
public class JournalEntrySearchOptions : SearchOptionsBase
{
    /// <summary>Gets or sets the recordid.</summary>
    ///
    /// <value>The recordid.</value>
    
    public long? recordid
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entityname.</summary>
    ///
    /// <value>The entityname.</value>
    
    public string entityname
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the propertyname.</summary>
    ///
    /// <value>The propertyname.</value>
    
    public string propertyname
    {
        get;
        set;
    }
    public void setCalculation()
    {
        try { }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
    /// <summary>Sets date time to client time (calls with entity object).</summary>
    
    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}
