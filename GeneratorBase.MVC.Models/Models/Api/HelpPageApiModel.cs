using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Web.Http.Description;

namespace GeneratorBase.MVC.Models
{

/// <summary>The model that represents an API displayed on the help page.</summary>
public class HelpPageApiModel
{
    /// <summary>Gets or sets the parameter models.</summary>
    ///
    /// <value>The parameter models.</value>
    
    public IDictionary<string, TypeDocumentation> ParameterModels
    {
        get;
        set;
    }
    /// <summary>Initializes a new instance of the <see cref="HelpPageApiModel"/> class.</summary>
    public HelpPageApiModel()
    {
        SampleRequests = new Dictionary<MediaTypeHeaderValue, object>();
        SampleResponses = new Dictionary<MediaTypeHeaderValue, object>();
        ErrorMessages = new Collection<string>();
    }
    
    /// <summary>Gets or sets the <see cref="ApiDescription"/> that describes the API.</summary>
    ///
    /// <value>Information describing the API.</value>
    
    public ApiDescription ApiDescription
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sample requests associated with the API.</summary>
    ///
    /// <value>The sample requests.</value>
    
    public IDictionary<MediaTypeHeaderValue, object> SampleRequests
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the sample responses associated with the API.</summary>
    ///
    /// <value>The sample responses.</value>
    
    public IDictionary<MediaTypeHeaderValue, object> SampleResponses
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the error messages associated with this model.</summary>
    ///
    /// <value>The error messages.</value>
    
    public Collection<string> ErrorMessages
    {
        get;
        private set;
    }
    
    /// <summary>Gets or sets the version.</summary>
    ///
    /// <value>The version.</value>
    
    public string Version
    {
        get;
        set;
    }
}
}