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
/// <summary>Menu Bar Menu Item Association model class: Bridge Entity for M:M association between MenuBar and MenuItem..</summary>
[Table("tbl_MenuBarMenuItemAssociation"),CustomDisplayName("T_MenuBarMenuItemAssociation", "T_MenuBarMenuItemAssociation.resx", "Menu Bar Menu Item Association")]
[Description("Bridge")]
public partial class T_MenuBarMenuItemAssociation : Entity
{
    /// <summary>Gets or sets the Order Number.</summary>
    ///
    /// <value>The T_OrderNumber.</value>
    [CustomDisplayName("T_OrderNumber", "T_MenuBarMenuItemAssociation.resx", "Order Number"), Column("OrderNumber")]
    public Nullable<Int32> T_OrderNumber
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Bar.</summary>
    ///
    /// <value>The T_MenuBar.</value>
    
    
    
    [CustomDisplayName("T_MenuBarID","T_MenuBarMenuItemAssociation.resx","Menu Bar"), Column("MenuBarID")]
    public Nullable<long> T_MenuBarID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Bar navigation property.</summary>
    ///
    /// <value>The t_menubar object.</value>
    
    public virtual T_MenuBar t_menubar
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Item.</summary>
    ///
    /// <value>The T_MenuItem.</value>
    
    
    
    [CustomDisplayName("T_MenuItemID","T_MenuBarMenuItemAssociation.resx","Menu Item"), Column("MenuItemID")]
    public Nullable<long> T_MenuItemID
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Item navigation property.</summary>
    ///
    /// <value>The t_menuitem object.</value>
    
    public virtual T_MenuItem t_menuitem
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
            using(var context = (new ApplicationContext(new SystemUser(),true)))
            {
                var dispValue="";
                dispValue =(this.T_MenuBarID != null ? context.T_MenuBars.FirstOrDefault(p=>p.Id == this.T_MenuBarID).DisplayValue + "-" : "")+(this.T_MenuItemID != null ? context.T_MenuItems.FirstOrDefault(p=>p.Id == this.T_MenuItemID).DisplayValue + "-" : "");
                //this.m_DisplayValue = dispValue;
                this.m_DisplayValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
                return dispValue;
            }
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
            var dispValue="";
            dispValue =(this.t_menubar != null ?t_menubar.DisplayValue + "-" : "")+(this.t_menuitem != null ?t_menuitem.DisplayValue + "-" : "");
            return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
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
    }
    /// <summary>Sets date time to UTC (calls before saving entity).</summary>
    public void setDateTimeToUTC()
    {
    }
}
}

