using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
/// <summary>A theme.</summary>
public class Theme
{
    /// <summary>Default constructor.</summary>
    public Theme()
    {
        this.Id = 0;
        this.IsActive = false;
        this.IsDefault = false;
        this.Name = "";
        this.sidebarLinkColor = "#ffffff";
        this.backgroundColorbody = "#62a8d1";
        this.navlistBackground = "#62a8d1";
        this.footerbgcolor = "#000000";
        this.HeaderBackgroundColor = "#000000";
        this.HeaderAppLabel = "#ffffff";
        this.HeaderLinkReportBG = "#2e8965";
        this.HeaderLinkReportText = "#fffffff";
        this.HeaderLinkReportTextHover = "#2e8965";
        this.DashboardBoxBG = "#eeeeee";
        this.DashboardIconBG = "#62a8d1";
        this.DashboardBoxBorder = "#d8d8d8";
        this.DashboardBoxHover = "#eeeeee";
        this.DashboardBoxLink = "#428bca";
        this.DashboardBoxLinkHover = "#777777";
        this.TrackedBoxBorderColor = "#dddddd";
        this.sidebarLinkHoverBG = "#ffffff";
        this.sidebarLinkHoverColor = "#1963aa";
        this.PageTitleBG = "#edf5fa";
        this.PageTitleBorder1 = "#c3ddec";
        this.PanelHeadingBG = "#edf5fa";
        this.PanelHeadingForeColor1 = "#3784b1";
        this.PanelHeadingBorder = "#c3ddec";
        this.PageTitleForeColor = "#333333";
        this.BtnDefaultBG1 = "#edf5fa";
        this.BtnDefaultForecolor = "#3784b1";
        this.BtnDefaultHoverBG = "#e6e6e6";
        this.BtnDefaultHoverForecolor = "#4d4d4d";
        this.Borderbutton = "#c3ddec";
        this.Borderbutton1 = "#dadada";
        this.Btn2DefaultBG = "#428bca";
        this.Btn2DefaultForecolor = "#ffffff";
        this.Btn2DefaultHoverBG = "#3276b1";
        this.Btn2DefaultHoverForecolor = "#ffffff";
        this.Borderbutton2 = "#357ebd";
        this.Borderbutton3 = "#285e8e";
        this.TableHeaderBG = "#edf5fa";
        this.TableForecolor = "#3784b1";
        this.TableBorder = "#c3ddec";
        this.TabsActiveforecolor = "#5b98c2";
        this.TabsActiveBorder = "#c5d0dc";
        this.TabsActiveTopborder = "#4c8fbd";
        this.TabsActiveBackground = "#ffffff";
        this.TabsForecolor = "#999999";
        this.TabsBackground = "#f9f9f9";
        this.TabsBorder = "#c5d0dc";
        this.TabsHeadingBG = "#edf5fa";
        this.TabsHeadingBorder = "#c5d0dc";
        this.TabsForecolorHover = "#4c8fbd";
        this.TabsBackgroundHover = "#ffffff";
        this.TabsBorderHover = "#c5d0dc";
        this.SidebarActiveforcolor = "#2b7dbc";
        this.SidebarActiveBG = "#ffffff";
        this.SidebarHoverBorderLeft = "#3382af";
        this.FooterLinkcolor = "#999999";
        this.FooterForecolor = "#2a6496";
        this.rightwrapper = "#ffffff";
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="id">                       The identifier.</param>
    /// <param name="name">                     The name.</param>
    /// <param name="CssEditor">                The CSS editor.</param>
    /// <param name="IsActive">                 True if this object is active, false if not.</param>
    /// <param name="IsDefault">                True if this object is default, false if not.</param>
    /// <param name="sidebarLinkColor">         The color of the sidebar link.</param>
    /// <param name="backgroundColorbody">      The background colorbody.</param>
    /// <param name="navlistBackground">        The navlist background.</param>
    /// <param name="footerbgcolor">            The footerbgcolor.</param>
    /// <param name="HeaderBackgroundColor">    The color of the header background.</param>
    /// <param name="HeaderAppLabel">           The header application label.</param>
    /// <param name="HeaderLinkReportBG">       The header link report background.</param>
    /// <param name="HeaderLinkReportText">     The header link report text.</param>
    /// <param name="HeaderLinkReportTextHover">The header link report text hover.</param>
    /// <param name="HeaderLinkAdminBG">        The header link admin background.</param>
    /// <param name="HeaderLinkAdminText">      The header link admin text.</param>
    /// <param name="HeaderLinkAdminTextHover"> The header link admin text hover.</param>
    /// <param name="HeaderLinkUserBG">         The header link user background.</param>
    /// <param name="HeaderLinkUserText">       The header link user text.</param>
    /// <param name="HeaderLinkUserTextHover">  The header link user text hover.</param>
    /// <param name="DashboardBoxBG">           The dashboard box background.</param>
    /// <param name="DashboardIconBG">          The dashboard icon background.</param>
    /// <param name="DashboardBoxBorder">       The dashboard box border.</param>
    /// <param name="DashboardBoxHover">        The dashboard box hover.</param>
    /// <param name="DashboardBoxLink">         The dashboard box link.</param>
    /// <param name="DashboardBoxLinkHover">    The dashboard box link hover.</param>
    /// <param name="TrackedBoxBorderColor">    The color of the tracked box border.</param>
    /// <param name="sidebarLinkHoverBG">       The sidebar link hover background.</param>
    /// <param name="sidebarLinkHoverColor">    The color of the sidebar link hover.</param>
    /// <param name="PageTitleBG">              The page title background.</param>
    /// <param name="PageTitleBorder1">         The page title border 1.</param>
    /// <param name="PanelHeadingBG">           The panel heading background.</param>
    /// <param name="PanelHeadingForeColor1">   The panel heading foreground color 1.</param>
    /// <param name="PanelHeadingBorder">       The panel heading border.</param>
    /// <param name="PageTitleForeColor">       The color of the page title foreground.</param>
    /// <param name="BtnDefaultBG1">            The button default background 1.</param>
    /// <param name="BtnDefaultForecolor">      The button default forecolor.</param>
    /// <param name="BtnDefaultHoverBG">        The button default hover background.</param>
    /// <param name="BtnDefaultHoverForecolor"> The button default hover forecolor.</param>
    /// <param name="Borderbutton">             The borderbutton.</param>
    /// <param name="Borderbutton1">            The borderbutton 1.</param>
    /// <param name="Btn2DefaultBG">            The button 2 default background.</param>
    /// <param name="Btn2DefaultForecolor">     The button 2 default forecolor.</param>
    /// <param name="Btn2DefaultHoverBG">       The button 2 default hover background.</param>
    /// <param name="Btn2DefaultHoverForecolor">The button 2 default hover forecolor.</param>
    /// <param name="Borderbutton2">            The borderbutton 2.</param>
    /// <param name="Borderbutton3">            The borderbutton 3.</param>
    /// <param name="TableHeaderBG">            The table header background.</param>
    /// <param name="TableForecolor">           The table forecolor.</param>
    /// <param name="TableBorder">              The table border.</param>
    /// <param name="TabsActiveforecolor">      The tabs activeforecolor.</param>
    /// <param name="TabsActiveBorder">         The tabs active border.</param>
    /// <param name="TabsActiveTopborder">      The tabs active topborder.</param>
    /// <param name="TabsActiveBackground">     The tabs active background.</param>
    /// <param name="TabsForecolor">            The tabs forecolor.</param>
    /// <param name="TabsBackground">           The tabs background.</param>
    /// <param name="TabsBorder">               The tabs border.</param>
    /// <param name="TabsHeadingBG">            The tabs heading background.</param>
    /// <param name="TabsHeadingBorder">        The tabs heading border.</param>
    /// <param name="TabsForecolorHover">       The tabs forecolor hover.</param>
    /// <param name="TabsBackgroundHover">      The tabs background hover.</param>
    /// <param name="TabsBorderHover">          The tabs border hover.</param>
    /// <param name="SidebarActiveforcolor">    The sidebar activeforcolor.</param>
    /// <param name="SidebarActiveBG">          The sidebar active background.</param>
    /// <param name="SidebarHoverBorderLeft">   The sidebar hover border left.</param>
    /// <param name="FooterLinkcolor">          The footer linkcolor.</param>
    /// <param name="FooterForecolor">          The footer forecolor.</param>
    /// <param name="rightwrapper">             The rightwrapper.</param>
    
    public Theme(int id, string name, string CssEditor, bool IsActive,bool IsDefault, string sidebarLinkColor, string backgroundColorbody, string navlistBackground,
                 string footerbgcolor, string HeaderBackgroundColor, string HeaderAppLabel,
                 string HeaderLinkReportBG, string HeaderLinkReportText, string HeaderLinkReportTextHover,
                 string HeaderLinkAdminBG, string HeaderLinkAdminText, string HeaderLinkAdminTextHover,
                 string HeaderLinkUserBG, string HeaderLinkUserText, string HeaderLinkUserTextHover,
                 
                 string DashboardBoxBG,string DashboardIconBG, string DashboardBoxBorder, string DashboardBoxHover, string DashboardBoxLink, string DashboardBoxLinkHover, string TrackedBoxBorderColor, string sidebarLinkHoverBG, string sidebarLinkHoverColor,
                 string PageTitleBG, string PageTitleBorder1, string PanelHeadingBG, string PanelHeadingForeColor1, string PanelHeadingBorder, string PageTitleForeColor, string BtnDefaultBG1, string BtnDefaultForecolor,
                 string BtnDefaultHoverBG, string BtnDefaultHoverForecolor, string Borderbutton, string Borderbutton1,
                 string Btn2DefaultBG, string Btn2DefaultForecolor, string Btn2DefaultHoverBG, string Btn2DefaultHoverForecolor, string Borderbutton2, string Borderbutton3,
                 string TableHeaderBG, string TableForecolor, string TableBorder, string TabsActiveforecolor, string TabsActiveBorder, string TabsActiveTopborder,
                 string TabsActiveBackground, string TabsForecolor, string TabsBackground, string TabsBorder, string TabsHeadingBG, string TabsHeadingBorder,
                 string TabsForecolorHover, string TabsBackgroundHover, string TabsBorderHover, string SidebarActiveforcolor, string SidebarActiveBG, string SidebarHoverBorderLeft,
                 string FooterLinkcolor, string FooterForecolor, string rightwrapper)
    {
        this.Id = id;
        this.Name = name;
        this.CssEditor = CssEditor;
        this.sidebarLinkColor = sidebarLinkColor;
        this.backgroundColorbody = backgroundColorbody;
        this.navlistBackground = navlistBackground;
        this.footerbgcolor = footerbgcolor;
        this.HeaderBackgroundColor = HeaderBackgroundColor;
        this.HeaderAppLabel = HeaderAppLabel;
        this.HeaderLinkReportBG = HeaderLinkReportBG;
        this.HeaderLinkReportText = HeaderLinkReportText;
        this.HeaderLinkReportTextHover = HeaderLinkReportTextHover;
        this.HeaderLinkAdminBG = HeaderLinkAdminBG;
        this.HeaderLinkAdminText = HeaderLinkAdminText;
        this.HeaderLinkAdminTextHover = HeaderLinkAdminTextHover;
        this.HeaderLinkUserBG = HeaderLinkUserBG;
        this.HeaderLinkUserText = HeaderLinkUserText;
        this.HeaderLinkUserTextHover = HeaderLinkUserTextHover;
        this.DashboardBoxBG = DashboardBoxBG;
        this.DashboardIconBG = DashboardIconBG;
        this.DashboardBoxBorder = DashboardBoxBorder;
        this.DashboardBoxHover = DashboardBoxHover;
        this.DashboardBoxLink = DashboardBoxLink;
        this.DashboardBoxLinkHover = DashboardBoxLinkHover;
        this.TrackedBoxBorderColor = TrackedBoxBorderColor;
        this.sidebarLinkHoverBG = sidebarLinkHoverBG;
        this.sidebarLinkHoverColor = sidebarLinkHoverColor;
        this.PageTitleBG = PageTitleBG;
        this.PageTitleBorder1 = PageTitleBorder1;
        this.PanelHeadingBG = PanelHeadingBG;
        this.PanelHeadingForeColor1 = PanelHeadingForeColor1;
        this.PanelHeadingBorder = PanelHeadingBorder;
        this.PageTitleForeColor = PageTitleForeColor;
        this.BtnDefaultBG1 = BtnDefaultBG1;
        this.BtnDefaultForecolor = BtnDefaultForecolor;
        this.BtnDefaultHoverBG = BtnDefaultHoverBG;
        this.BtnDefaultHoverForecolor = BtnDefaultHoverForecolor;
        this.Borderbutton = Borderbutton;
        this.Borderbutton1 = Borderbutton1;
        this.Btn2DefaultBG = Btn2DefaultBG;
        this.Btn2DefaultForecolor = Btn2DefaultForecolor;
        this.Btn2DefaultHoverBG = Btn2DefaultHoverBG;
        this.Btn2DefaultHoverForecolor = Btn2DefaultHoverForecolor;
        this.Borderbutton2 = Borderbutton2;
        this.Borderbutton3 = Borderbutton3;
        this.TableHeaderBG = TableHeaderBG;
        this.TableForecolor = TableForecolor;
        this.TableBorder = TableBorder;
        this.TabsActiveforecolor = TabsActiveforecolor;
        this.TabsActiveBorder = TabsActiveBorder;
        this.TabsActiveTopborder = TabsActiveTopborder;
        this.TabsActiveBackground = TabsActiveBackground;
        this.TabsForecolor = TabsForecolor;
        this.TabsBackground = TabsBackground;
        this.TabsBorder = TabsBorder;
        this.TabsHeadingBG = TabsHeadingBG;
        this.TabsHeadingBorder = TabsHeadingBorder;
        this.TabsForecolorHover = TabsForecolorHover;
        this.TabsBackgroundHover = TabsBackgroundHover;
        this.TabsBorderHover = TabsBorderHover;
        this.SidebarActiveforcolor = SidebarActiveforcolor;
        this.SidebarActiveBG = SidebarActiveBG;
        this.SidebarHoverBorderLeft = SidebarHoverBorderLeft;
        this.FooterLinkcolor = FooterLinkcolor;
        this.FooterForecolor = FooterForecolor;
        this.rightwrapper = rightwrapper;
    }
    //[RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid PhoneNo")]
    
    /// <summary>Gets or sets the identifier.</summary>
    ///
    /// <value>The identifier.</value>
    
    [DisplayName("Id")]
    public long Id
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the name.</summary>
    ///
    /// <value>The name.</value>
    
    [DisplayName("Theme Name")]
    [Required]
    public string Name
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the CSS editor.</summary>
    ///
    /// <value>The CSS editor.</value>
    
    [DisplayName("CSS Editor")]
    public string CssEditor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is active.</summary>
    ///
    /// <value>True if this object is active, false if not.</value>
    
    [DisplayName("IsActive")]
    public bool IsActive
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets a value indicating whether this object is default.</summary>
    ///
    /// <value>True if this object is default, false if not.</value>
    
    [DisplayName("IsDefault")]
    public bool IsDefault
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the color of the sidebar link.</summary>
    ///
    /// <value>The color of the sidebar link.</value>
    
    [DisplayName("Side Bar link")]
    [Required]
    public string sidebarLinkColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the background colorbody.</summary>
    ///
    /// <value>The background colorbody.</value>
    
    [DisplayName("Back Ground Color")]
    [Required]
    public string backgroundColorbody
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the navlist background.</summary>
    ///
    /// <value>The navlist background.</value>
    
    [DisplayName("Side Bar Background")]
    [Required]
    public string navlistBackground
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the footerbgcolor.</summary>
    ///
    /// <value>The footerbgcolor.</value>
    
    [DisplayName("Footer Color")]
    [Required]
    public string footerbgcolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the color of the header background.</summary>
    ///
    /// <value>The color of the header background.</value>
    
    [DisplayName("Header Background Color")]
    [Required]
    public string HeaderBackgroundColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header application label.</summary>
    ///
    /// <value>The header application label.</value>
    
    [DisplayName("HeaderAppLabel")]
    [Required]
    public string HeaderAppLabel
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link report background.</summary>
    ///
    /// <value>The header link report background.</value>
    
    [DisplayName("HeaderLinkReportBG")]
    [Required]
    public string HeaderLinkReportBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link report text.</summary>
    ///
    /// <value>The header link report text.</value>
    
    [DisplayName("HeaderLinkReportText")]
    [Required]
    public string HeaderLinkReportText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link report text hover.</summary>
    ///
    /// <value>The header link report text hover.</value>
    
    [DisplayName("HeaderLinkReportTextHover")]
    [Required]
    public string HeaderLinkReportTextHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link admin background.</summary>
    ///
    /// <value>The header link admin background.</value>
    
    [DisplayName("HeaderLinkAdminBG")]
    [Required]
    public string HeaderLinkAdminBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link admin text.</summary>
    ///
    /// <value>The header link admin text.</value>
    
    [DisplayName("HeaderLinkAdminText")]
    [Required]
    public string HeaderLinkAdminText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link admin text hover.</summary>
    ///
    /// <value>The header link admin text hover.</value>
    
    [DisplayName("HeaderLinkAdminTextHover")]
    [Required]
    public string HeaderLinkAdminTextHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link user background.</summary>
    ///
    /// <value>The header link user background.</value>
    
    [DisplayName("HeaderLinkUserBG")]
    [Required]
    public string HeaderLinkUserBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link user text.</summary>
    ///
    /// <value>The header link user text.</value>
    
    [DisplayName("HeaderLinkUserText")]
    [Required]
    public string HeaderLinkUserText
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the header link user text hover.</summary>
    ///
    /// <value>The header link user text hover.</value>
    
    [DisplayName("HeaderLinkUserTextHover")]
    [Required]
    public string HeaderLinkUserTextHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard box background.</summary>
    ///
    /// <value>The dashboard box background.</value>
    
    [DisplayName("DashboardBoxBG")]
    [Required]
    public string DashboardBoxBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard icon background.</summary>
    ///
    /// <value>The dashboard icon background.</value>
    
    [DisplayName("DashboardIconBG")]
    [Required]
    public string DashboardIconBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard box border.</summary>
    ///
    /// <value>The dashboard box border.</value>
    
    [DisplayName("DashboardBoxBorder")]
    [Required]
    public string DashboardBoxBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard box hover.</summary>
    ///
    /// <value>The dashboard box hover.</value>
    
    [DisplayName("DashboardBoxHover")]
    [Required]
    public string DashboardBoxHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard box link.</summary>
    ///
    /// <value>The dashboard box link.</value>
    
    [DisplayName("DashboardBoxLink")]
    [Required]
    public string DashboardBoxLink
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the dashboard box link hover.</summary>
    ///
    /// <value>The dashboard box link hover.</value>
    
    [DisplayName("DashboardBoxLinkHover")]
    [Required]
    public string DashboardBoxLinkHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the color of the tracked box border.</summary>
    ///
    /// <value>The color of the tracked box border.</value>
    
    [DisplayName("TrackedBoxBorderColor")]
    [Required]
    public string TrackedBoxBorderColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sidebar link hover background.</summary>
    ///
    /// <value>The sidebar link hover background.</value>
    
    [DisplayName("sidebarLinkHoverBG")]
    [Required]
    public string sidebarLinkHoverBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the color of the sidebar link hover.</summary>
    ///
    /// <value>The color of the sidebar link hover.</value>
    
    [DisplayName("sidebarLinkHoverColor")]
    [Required]
    public string sidebarLinkHoverColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page title background.</summary>
    ///
    /// <value>The page title background.</value>
    
    [DisplayName("PageTitleBG")]
    [Required]
    public string PageTitleBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the page title border 1.</summary>
    ///
    /// <value>The page title border 1.</value>
    
    [DisplayName("PageTitleBorder1")]
    [Required]
    public string PageTitleBorder1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the panel heading background.</summary>
    ///
    /// <value>The panel heading background.</value>
    
    [DisplayName("PanelHeadingBG")]
    [Required]
    public string PanelHeadingBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the panel heading foreground color 1.</summary>
    ///
    /// <value>The panel heading foreground color 1.</value>
    
    [DisplayName("PanelHeadingForeColor1")]
    [Required]
    public string PanelHeadingForeColor1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the panel heading border.</summary>
    ///
    /// <value>The panel heading border.</value>
    
    [DisplayName("PanelHeadingBorder")]
    [Required]
    public string PanelHeadingBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the color of the page title foreground.</summary>
    ///
    /// <value>The color of the page title foreground.</value>
    
    [DisplayName("PageTitleForeColor")]
    [Required]
    public string PageTitleForeColor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button default background 1.</summary>
    ///
    /// <value>The button default background 1.</value>
    
    [DisplayName("BtnDefaultBG1")]
    [Required]
    public string BtnDefaultBG1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button default forecolor.</summary>
    ///
    /// <value>The button default forecolor.</value>
    
    [DisplayName("BtnDefaultForecolor")]
    [Required]
    public string BtnDefaultForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button default hover background.</summary>
    ///
    /// <value>The button default hover background.</value>
    
    [DisplayName("BtnDefaultHoverBG")]
    [Required]
    public string BtnDefaultHoverBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button default hover forecolor.</summary>
    ///
    /// <value>The button default hover forecolor.</value>
    
    [DisplayName("BtnDefaultHoverForecolor")]
    [Required]
    public string BtnDefaultHoverForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the borderbutton.</summary>
    ///
    /// <value>The borderbutton.</value>
    
    [DisplayName("Borderbutton")]
    [Required]
    public string Borderbutton
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the borderbutton 1.</summary>
    ///
    /// <value>The borderbutton 1.</value>
    
    [DisplayName("Borderbutton1")]
    [Required]
    public string Borderbutton1
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button 2 default background.</summary>
    ///
    /// <value>The button 2 default background.</value>
    
    [DisplayName("Btn2DefaultBG")]
    [Required]
    public string Btn2DefaultBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button 2 default forecolor.</summary>
    ///
    /// <value>The button 2 default forecolor.</value>
    
    [DisplayName("Btn2DefaultForecolor")]
    [Required]
    public string Btn2DefaultForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button 2 default hover background.</summary>
    ///
    /// <value>The button 2 default hover background.</value>
    
    [DisplayName("Btn2DefaultHoverBG")]
    [Required]
    public string Btn2DefaultHoverBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the button 2 default hover forecolor.</summary>
    ///
    /// <value>The button 2 default hover forecolor.</value>
    
    [DisplayName("Btn2DefaultHoverForecolor")]
    [Required]
    public string Btn2DefaultHoverForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the borderbutton 2.</summary>
    ///
    /// <value>The borderbutton 2.</value>
    
    [DisplayName("Borderbutton2")]
    [Required]
    public string Borderbutton2
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the borderbutton 3.</summary>
    ///
    /// <value>The borderbutton 3.</value>
    
    [DisplayName("Borderbutton3")]
    [Required]
    public string Borderbutton3
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the table header background.</summary>
    ///
    /// <value>The table header background.</value>
    
    [DisplayName("TableHeaderBG")]
    [Required]
    public string TableHeaderBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the table forecolor.</summary>
    ///
    /// <value>The table forecolor.</value>
    
    [DisplayName("TableForecolor")]
    [Required]
    public string TableForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the table border.</summary>
    ///
    /// <value>The table border.</value>
    
    [DisplayName("TableBorder")]
    [Required]
    public string TableBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs activeforecolor.</summary>
    ///
    /// <value>The tabs activeforecolor.</value>
    
    [DisplayName("TabsActiveforecolor")]
    [Required]
    public string TabsActiveforecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs active border.</summary>
    ///
    /// <value>The tabs active border.</value>
    
    [DisplayName("TabsActiveBorder")]
    [Required]
    public string TabsActiveBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs active topborder.</summary>
    ///
    /// <value>The tabs active topborder.</value>
    
    [DisplayName("TabsActiveTopborder")]
    [Required]
    public string TabsActiveTopborder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs active background.</summary>
    ///
    /// <value>The tabs active background.</value>
    
    [DisplayName("TabsActiveBackground")]
    [Required]
    public string TabsActiveBackground
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs forecolor.</summary>
    ///
    /// <value>The tabs forecolor.</value>
    
    [DisplayName("TabsForecolor")]
    [Required]
    public string TabsForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs background.</summary>
    ///
    /// <value>The tabs background.</value>
    
    [DisplayName("TabsBackground")]
    public string TabsBackground
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs border.</summary>
    ///
    /// <value>The tabs border.</value>
    
    [DisplayName("TabsBorder")]
    [Required]
    public string TabsBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs heading background.</summary>
    ///
    /// <value>The tabs heading background.</value>
    
    [DisplayName("TabsHeadingBG")]
    [Required]
    public string TabsHeadingBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs heading border.</summary>
    ///
    /// <value>The tabs heading border.</value>
    
    [DisplayName("TabsHeadingBorder")]
    [Required]
    public string TabsHeadingBorder
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs forecolor hover.</summary>
    ///
    /// <value>The tabs forecolor hover.</value>
    
    [DisplayName("TabsForecolorHover")]
    [Required]
    public string TabsForecolorHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs background hover.</summary>
    ///
    /// <value>The tabs background hover.</value>
    
    [DisplayName("TabsBackgroundHover")]
    [Required]
    public string TabsBackgroundHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the tabs border hover.</summary>
    ///
    /// <value>The tabs border hover.</value>
    
    [DisplayName("TabsBorderHover")]
    [Required]
    public string TabsBorderHover
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sidebar activeforcolor.</summary>
    ///
    /// <value>The sidebar activeforcolor.</value>
    
    [DisplayName("SidebarActiveforcolor")]
    [Required]
    public string SidebarActiveforcolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sidebar active background.</summary>
    ///
    /// <value>The sidebar active background.</value>
    
    [DisplayName("SidebarActiveBG")]
    [Required]
    public string SidebarActiveBG
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the sidebar hover border left.</summary>
    ///
    /// <value>The sidebar hover border left.</value>
    
    [DisplayName("SidebarHoverBorderLeft")]
    [Required]
    public string SidebarHoverBorderLeft
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the footer linkcolor.</summary>
    ///
    /// <value>The footer linkcolor.</value>
    
    [DisplayName("FooterLinkcolor")]
    [Required]
    public string FooterLinkcolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the footer forecolor.</summary>
    ///
    /// <value>The footer forecolor.</value>
    
    [DisplayName("FooterForecolor")]
    [Required]
    public string FooterForecolor
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the rightwrapper.</summary>
    ///
    /// <value>The rightwrapper.</value>
    
    [DisplayName("rightwrapper")]
    [Required]
    public string rightwrapper
    {
        get;
        set;
    }
    /// <summary>Sets calculated value (to save in db).</summary>
    
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
/// <summary>Interface for theme repository.</summary>
public interface IThemeRepository
{
    /// <summary>Edit theme.</summary>
    ///
    /// <param name="theme">The theme.</param>
    
    void EditTheme(Theme theme);
    
    /// <summary>Gets the theme.</summary>
    ///
    /// <returns>The theme.</returns>
    
    Theme GetTheme();
}
}