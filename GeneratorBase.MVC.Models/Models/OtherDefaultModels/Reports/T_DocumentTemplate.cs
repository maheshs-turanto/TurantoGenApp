using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
namespace GeneratorBase.MVC.Models
{
/// <summary>Document Template model class: Document Template.</summary>
[Table("tbl_DocumentTemplate"),CustomDisplayName("T_DocumentTemplate", "T_DocumentTemplate.resx", "Document Template")]
public partial class T_DocumentTemplate : Entity
{
    /// <summary>Default constructor for Document Template.</summary>
    public T_DocumentTemplate()
    {
        if(!this.T_Disable.HasValue)
            this.T_Disable = false;
        if(!this.T_EnableDownload.HasValue)
            this.T_EnableDownload = false;
        if(!this.T_EnablePreview.HasValue)
            this.T_EnablePreview = false;
    }
    /// <summary>Gets or sets the Auto No..</summary>
    ///
    /// <value>The T_AutoNo.</value>
    [CustomDisplayName("T_AutoNo","T_DocumentTemplate.resx","Auto No."), Column("AutoNo")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public Nullable<long> T_AutoNo
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Entity Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Entity Name.</summary>
    ///
    /// <value>The T_EntityName.</value>
    [CustomDisplayName("T_EntityName","T_DocumentTemplate.resx","Entity Name"), Column("EntityName")] [Required]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_EntityName
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Name.</summary>
    ///
    /// <value>The T_Name.</value>
    [CustomDisplayName("T_Name","T_DocumentTemplate.resx","Name"), Column("Name")] [Required]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Description.</summary>
    ///
    /// <value>The T_Description.</value>
    [CustomDisplayName("T_Description","T_DocumentTemplate.resx","Description"), Column("Description")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_Description
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Document.</summary>
    ///
    /// <value>The T_Document.</value>
    [CustomDisplayName("T_Document","T_DocumentTemplate.resx","Document"), Column("Document")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public Nullable<long> T_Document
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Document Type cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Document Type.</summary>
    ///
    /// <value>The T_DocumentType.</value>
    [CustomDisplayName("T_DocumentType","T_DocumentTemplate.resx","Template Type"), Column("DocumentType")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_DocumentType
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Action Type cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Action Type.</summary>
    ///
    /// <value>The T_ActionType.</value>
    [CustomDisplayName("T_ActionType","T_DocumentTemplate.resx","Action Type"), Column("ActionType")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_ActionType
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Default Output Format cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Default Output Format.</summary>
    ///
    /// <value>The T_DefaultOutputFormat.</value>
    [CustomDisplayName("T_DefaultOutputFormat","T_DocumentTemplate.resx","Default Output Format"), Column("DefaultOutputFormat")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_DefaultOutputFormat
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Allowed Roles.</summary>
    ///
    /// <value>The T_AllowedRoles.</value>
    [CustomDisplayName("T_AllowedRoles","T_DocumentTemplate.resx","Allowed Roles"), Column("AllowedRoles")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_AllowedRoles
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Allowed Tenants.</summary>
    ///
    /// <value>The T_Tenants.</value>
    [CustomDisplayName("T_Tenants", "T_DocumentTemplate.resx", "Allowed Tenants"), Column("Tenants")]
    [PropertyInfoForEntity("55873", "Basic Information", "Group")]
    public string T_Tenants
    {
        get;
        set;
    }
    
    [StringLength(255, ErrorMessage = "Attach Document  To cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Attach Document  To.</summary>
    ///
    /// <value>The T_AttachDocumentTo.</value>
    [CustomDisplayName("T_AttachDocumentTo","T_DocumentTemplate.resx","Attach Document  To"), Column("AttachDocumentTo")]
    [PropertyInfoForEntity("55873","Basic Information","Group")]
    public string T_AttachDocumentTo
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Display Type cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Display Type.</summary>
    ///
    /// <value>The T_DisplayType.</value>
    [CustomDisplayName("T_DisplayType","T_DocumentTemplate.resx","Display Type"), Column("DisplayType")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    public string T_DisplayType
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Allow Download.</summary>
    ///
    /// <value>The T_AllowDownload.</value>
    [CustomDisplayName("T_EnableDownload", "T_DocumentTemplate.resx", "Enable Download"), Column("EnableDownload")]
    [PropertyInfoForEntity("55873", "Basic Information", "Group")]
    public Boolean? T_EnableDownload
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Allow Preview.</summary>
    ///
    /// <value>The T_AllowPreview.</value>
    [CustomDisplayName("T_EnablePreview", "T_DocumentTemplate.resx", "Enable Preview"), Column("EnablePreview")]
    [PropertyInfoForEntity("55873", "Basic Information", "Group")]
    public Boolean? T_EnablePreview
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Display Order.</summary>
    ///
    /// <value>The T_DisplayOrder.</value>
    [CustomDisplayName("T_DisplayOrder","T_DocumentTemplate.resx","Display Order"), Column("DisplayOrder")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    public Nullable<Int32> T_DisplayOrder
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "ToolTip cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the ToolTip.</summary>
    ///
    /// <value>The T_ToolTip.</value>
    [CustomDisplayName("T_ToolTip","T_DocumentTemplate.resx","ToolTip"), Column("ToolTip")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    public string T_ToolTip
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Background Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Background Color.</summary>
    ///
    /// <value>The T_BackGroundColor.</value>
    [CustomDisplayName("T_BackGroundColor","T_DocumentTemplate.resx","Background Color"), Column("BackGroundColor")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    [PropertyUIInfoType("bgcolor")]
    public string T_BackGroundColor
    {
        get;
        set;
    }
    [StringLength(255, ErrorMessage = "Font Color cannot be longer than 255 characters.")]
    /// <summary>Gets or sets the Font Color.</summary>
    ///
    /// <value>The T_FontColor.</value>
    [CustomDisplayName("T_FontColor","T_DocumentTemplate.resx","Font Color"), Column("FontColor")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    [PropertyUIInfoType("fgcolor")]
    public string T_FontColor
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Disable?.</summary>
    ///
    /// <value>The T_Disable.</value>
    [CustomDisplayName("T_Disable","T_DocumentTemplate.resx","Disable?"), Column("Disable")]
    [PropertyInfoForEntity("55877","UI Information","Group")]
    public Boolean? T_Disable
    {
        get;
        set;
    }
    
    [NotMapped]
    public DateTime? m_T_RecordAdded
    {
        get;
        set;
    }[DataType(DataType.DateTime)][CustomDisplayFormat("T_RecordAdded", "T_DocumentTemplate", "{0:MM/dd/yyyy hh:mm tt}", "MM/DD/YYYY hh:mm A", true,"DateTime")]
    /// <summary>Gets or sets the Record On (TimeStamp property).</summary>
    ///
    /// <value>The T_RecordAdded.</value>
    //[CustomDisplayName("T_RecordAdded","T_DocumentTemplate.resx","Updated By"), Column("RecordAdded")]
    [CustomDisplayName("T_RecordAdded","T_DocumentTemplate.resx","Last Updated On"), Column("RecordAdded")]
    [PropertyInfoForEntity("55876","For Internal Use Only","Group")]
    public Nullable<DateTime> T_RecordAdded
    {
        get;
        set;
    }
    string m_T_RecordAddedUser = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "System";
    //[CustomDisplayName("T_RecordAddedUser","T_DocumentTemplate.resx","Updated By User"), Column("RecordAddedUser")]
    [CustomDisplayName("T_RecordAddedUser","T_DocumentTemplate.resx","Last Updated By"), Column("RecordAddedUser")]
    [PropertyInfoForEntity("55876","For Internal Use Only","Group")]
    /// <summary>Gets or sets the Record On (TimeStamp User).</summary>
    ///
    /// <value>The T_RecordAdded.</value>
    public string T_RecordAddedUser
    {
        get
        {
            return m_T_RecordAddedUser;
        }
        set
        {
            m_T_RecordAddedUser = value;
        }
    }
    /// <summary>Gets or sets the m_T_RecordAdded (TimeStamp property).</summary>
    ///
    /// <value>The m_T_RecordAdded.</value>
    [NotMapped]
    public Nullable<DateTime> m_T_RecordAddedInsertDate
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Record On (TimeStamp property).</summary>
    ///
    /// <value>The T_RecordAdded.</value>
    [DataType(DataType.DateTime)]
    //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
    [CustomDisplayFormat("T_RecordAdded", "T_DocumentTemplate", "{0:MM/dd/yyyy hh:mm:ss tt}", "MM/DD/YYYY hh:mm:ss A", true,"DateTime")]
    //[CustomDisplayName("T_RecordAddedInsertDate","T_DocumentTemplate.resx","Inserted By"), Column("RecordAddedInsertDate")]
    [CustomDisplayName("T_RecordAddedInsertDate","T_DocumentTemplate.resx","Created On"), Column("RecordAddedInsertDate")]
    [PropertyInfoForEntity("55876","For Internal Use Only","Group")]
    public Nullable<DateTime> T_RecordAddedInsertDate
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Record On (TimeStamp User).</summary>
    ///
    /// <value>The T_RecordAdded.</value>
    //[CustomDisplayName("T_RecordAddedInsertBy","T_DocumentTemplate.resx","Inserted By User"), Column("RecordAddedInsertBy")]
    [CustomDisplayName("T_RecordAddedInsertBy","T_DocumentTemplate.resx","Created By"), Column("RecordAddedInsertBy")]
    [PropertyInfoForEntity("55876","For Internal Use Only","Group")]
    public string T_RecordAddedInsertBy
    {
        get;
        set;
    }
    /// <summary>Gets display value (SaveChanges DbContext sets DisplayValue before Save).</summary>
    ///
    /// <returns>The display value.</returns>
    public  string getDisplayValue()
    {
        try
        {
            var dispValue = (this.T_EntityName != null ?Convert.ToString(this.T_EntityName)+"-" : Convert.ToString(this.T_EntityName))+(this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name));
            //this.m_DisplayValue = dispValue;
            this.m_DisplayValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
            return dispValue;
        }
        catch
        {
            return "";
        }
    }
/// <summary>Gets display value model (Actual value to render on UI).</summary>
///
/// <returns>The display value model.</returns>
    public override string getDisplayValueModel()
    {
        try
        {
            if(!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = (this.T_EntityName != null ?Convert.ToString(this.T_EntityName)+"-" : Convert.ToString(this.T_EntityName))+(this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name));
            return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
        }
        catch
        {
            return "";
        }
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
    public void setCalculation()
    {
        try {  }
        catch(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
    }
/// <summary>Sets date time to client time (calls with entity object).</summary>

    public void setDateTimeToClientTime() //call this method when you have to update record from code (not from html form). e.g. BulkUpdate
    {
        this.T_RecordAdded = this.T_RecordAdded.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(this.T_RecordAdded.Value, this.m_Timezone) : this.T_RecordAdded;
        this.T_RecordAddedInsertDate = this.T_RecordAddedInsertDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(this.T_RecordAddedInsertDate.Value, this.m_Timezone) : this.T_RecordAddedInsertDate;
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
        //this.T_RecordAdded =  this.T_RecordAdded.HasValue ? TimeZoneInfo.ConvertTimeToUtc(this.T_RecordAdded.Value, this.m_Timezone) :  this.T_RecordAdded;
    }
}
}

