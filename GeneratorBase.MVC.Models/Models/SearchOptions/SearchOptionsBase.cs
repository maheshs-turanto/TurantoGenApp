using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneratorBase.MVC.Models.SearchOptions
{
/// <summary>A search options base (used for web-api actions).</summary>
public class SearchOptionsBase
{
    /// <summary>Gets or sets the search.</summary>
    ///
    /// <value>The search.</value>
    
    public string Search
    {
        get;
        set;
    }
    /// <summary>The deep search.</summary>
    private bool? _DeepSearch;
    
    /// <summary>Gets or sets the deep search.</summary>
    ///
    /// <value>The deep search.</value>
    
    public bool? DeepSearch
    {
        get
        {
            return _DeepSearch ?? false;
        }
        set
        {
            _DeepSearch = value;
        }
    }
    // public bool? DeepSearch { get; set; } = false;
    
    /// <summary>Gets or sets the order.</summary>
    ///
    /// <value>The order.</value>
    
    public string Order
    {
        get;
        set;
    }
    /// <summary>The ascending.</summary>
    private bool? _Ascending;
    
    /// <summary>Gets or sets the ascending.</summary>
    ///
    /// <value>The ascending.</value>
    
    public bool? Ascending
    {
        get
        {
            return _Ascending ?? false;
        }
        set
        {
            _Ascending = value;
        }
    }
    // public bool? Ascending { get; set; } = true;
    /// <summary>The page.</summary>
    private int? _Page;
    
    /// <summary>Gets or sets the page.</summary>
    ///
    /// <value>The page.</value>
    
    public int? Page
    {
        get
        {
            return _Page ?? 1;
        }
        set
        {
            _Page = value;
        }
    }
    
    private int? _PageSize;
    
    /// <summary>Gets or sets the page.</summary>
    ///
    /// <value>The page.</value>
    
    public int? PageSize
    {
        get
        {
            return _PageSize ?? 20;
        }
        set
        {
            _PageSize = value;
        }
    }
    
    /// <summary>Gets or sets the hosting entity.</summary>
    ///
    /// <value>The hosting entity.</value>
    
    public string HostingEntity
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the identifier of the hosting entity.</summary>
    ///
    /// <value>The identifier of the hosting entity.</value>
    
    public long? HostingEntityID
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the type of the associated.</summary>
    ///
    /// <value>The type of the associated.</value>
    
    public string AssociatedType
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the filter condition.</summary>
    ///
    /// <value>The filter condition.</value>
    
    public string FilterCondition
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sort order.</summary>
    ///
    /// <value>The sort order.</value>
    
    public string SortOrder
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