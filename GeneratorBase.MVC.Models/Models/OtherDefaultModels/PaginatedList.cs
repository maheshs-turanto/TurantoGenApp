using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneratorBase.MVC.Models
{
/// <summary>Paginated List.</summary>
///
/// <typeparam name="T">Generic type parameter.</typeparam>

public class PaginatedList<T>
{
    /// <summary>Gets or sets the results.</summary>
    ///
    /// <value>The results.</value>
    
    [JsonProperty("results")]
    public IEnumerable<T> Results
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the current page.</summary>
    ///
    /// <value>The current page.</value>
    
    [JsonProperty("currentPage")]
    public long CurrentPage
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the total number of pages.</summary>
    ///
    /// <value>The total number of pages.</value>
    
    [JsonProperty("totalPages")]
    public long TotalPages
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the total number of results.</summary>
    ///
    /// <value>The total number of results.</value>
    
    [JsonProperty("totalResults")]
    public long TotalResults
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the pagination.</summary>
    ///
    /// <value>The pagination.</value>
    
    [JsonProperty("pagination")]
    public PaginationInformation Pagination
    {
        get;
        set;
    }
}

/// <summary>Information about the pagination.</summary>
public class PaginationInformation
{
    /// <summary>Gets or sets a value indicating whether the more.</summary>
    ///
    /// <value>True if more, false if not.</value>
    
    [JsonProperty("more")]
    public bool More
    {
        get;
        set;
    }
}
}