using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
namespace GeneratorBase.MVC
{
/// <summary>Application context (Derived class of DbContext), interacts with database through EF.</summary>
public partial class ApplicationContext : DbContext
{
    /// <summary>The user.</summary>
    IUser user;
    /// <summary>The journal entry user.</summary>
    IUser journaluser;
    /// <summary>1 means bypass everthing and savechanges, 0 means ignore dirty property check,
    /// similarly you can create different modes as per requirement.</summary>
    
    int mode = -1;
    /// <summary>mark it as true for Recycling deleted records.</summary>
    bool disableIsdeletedFilter = false;
    /// <summary>to capture source for change in audit log.</summary>
    string JournalSource = "";
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">         The user.</param>
    /// <param name="JournalSource">(Optional) The journal source.</param>
    /// <param name="parameter">     (Optional) parameter.</param>
    public ApplicationContext(IUser user, string JournalSource = null, long parameter = 0) : base("DefaultConnection")
    {
        this.user = user;
        this.journaluser = user;
        if(!string.IsNullOrEmpty(JournalSource))
            this.JournalSource = JournalSource;
        else
        {
            try
            {
                if(HttpContext.Current != null)
                    this.JournalSource = VirtualPathUtility.MakeRelative("~", ((HttpContext.Current.Request.RequestContext.HttpContext).Request).Url.AbsolutePath);
                else
                    this.JournalSource = "";
            }
            catch
            {
                this.JournalSource = "";
            }
        }
        //this.Database.Log = MvcApplication.LogToConsole;
        if(this.user != null && !this.user.IsAdmin)
            this.ApplyFilters(new List<IFilter<ApplicationContext>>()
        {
            new ApplicationSecurityFilter(user)
        });
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">         The user.</param>
    /// <param name="journaluser">The journal user.</param>
    /// <param name="JournalSource">(Optional) The journal source.</param>
    public ApplicationContext(IUser user, IUser journaluser,string JournalSource = null)
        : base("DefaultConnection")
    {
        this.user = user;
        if(journaluser == null)
            this.journaluser = user;
        else this.journaluser = journaluser;
        if(!string.IsNullOrEmpty(JournalSource))
            this.JournalSource = JournalSource;
        else
        {
            try
            {
                if(HttpContext.Current != null)
                    this.JournalSource = VirtualPathUtility.MakeRelative("~", ((HttpContext.Current.Request.RequestContext.HttpContext).Request).Url.AbsolutePath);
                else
                    this.JournalSource = "";
            }
            catch
            {
                this.JournalSource = "";
            }
        }
        //this.Database.Log = MvcApplication.LogToConsole;
        if(this.user != null && (!this.user.IsAdmin))
            this.ApplyFilters(new List<IFilter<ApplicationContext>>()
        {
            new ApplicationSecurityFilter(user)
        });
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">         The user.</param>
    /// <param name="mode">         The mode.</param>
    /// <param name="JournalSource">(Optional) The journal source.</param>
    public ApplicationContext(IUser user, int mode, string JournalSource = null) : base("DefaultConnection")
    {
        this.user = user;
        this.journaluser = user;
        this.mode = mode;
        if(!string.IsNullOrEmpty(JournalSource))
            this.JournalSource = JournalSource;
        else
        {
            try
            {
                if(HttpContext.Current != null)
                    this.JournalSource = VirtualPathUtility.MakeRelative("~", ((HttpContext.Current.Request.RequestContext.HttpContext).Request).Url.AbsolutePath);
                else
                    this.JournalSource = "";
            }
            catch
            {
                this.JournalSource = "";
            }
        }
    }
    /// <summary>Constructor.</summary>
    ///
    /// <param name="user">                  The user.</param>
    /// <param name="disableIsdeletedFilter">   True to disable, false to enable the isdeleted filter.</param>
    /// <param name="JournalSource">         (Optional) The journal source.</param>
    public ApplicationContext(IUser user, bool disableIsdeletedFilter, string JournalSource = null) : base("DefaultConnection")
    {
        this.user = user;
        this.journaluser = user;
        this.disableIsdeletedFilter = disableIsdeletedFilter;
        if(!string.IsNullOrEmpty(JournalSource))
            this.JournalSource = JournalSource;
        else
        {
            try
            {
                if(HttpContext.Current != null)
                    this.JournalSource = VirtualPathUtility.MakeRelative("~", ((HttpContext.Current.Request.RequestContext.HttpContext).Request).Url.AbsolutePath);
                else
                    this.JournalSource = "";
            }
            catch
            {
                this.JournalSource = "";
            }
        }
        if(this.user != null && !this.user.IsAdmin)
            this.ApplyFilters(new List<IFilter<ApplicationContext>>()
        {
            new ApplicationSecurityFilter(user)
        });
    }
    /// <summary>Gets or sets the Document list.</summary>
    ///
    /// <value>The Document IQueryable list.</value>
    public IDbSet<FileDocument> FileDocuments
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Customer list.</summary>
    ///
    /// <value>The Customer IQueryable list.</value>
    public IDbSet<T_Customer> T_Customers
    {
        get;
        set;
    }
    
    //Default DbSet for Application
    /// <summary>Gets or sets the Export Data Configuration list.</summary>
    ///
    /// <value>The Export Data Configuration IQueryable list.</value>
    public IDbSet<T_ExportDataConfiguration> T_ExportDataConfigurations
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Export Data Details list.</summary>
    ///
    /// <value>The Export Data Details IQueryable list.</value>
    public IDbSet<T_ExportDataDetails> T_ExportDataDetailss
    {
        get;
        set;
    }
//Default DbSet for Application
    /// <summary>Gets or sets the Export Data Log list.</summary>
    ///
    /// <value>The Export Data Log IQueryable list.</value>
    public IDbSet<T_ExportDataLog> T_ExportDataLogs
    {
        get;
        set;
    }
//Default DbSet for Application
    /// <summary>Gets or sets the Export Data Log status list.</summary>
    ///
    /// <value>The Export Data Log status IQueryable list.</value>
    public IDbSet<T_ExportDataLogstatus> T_ExportDataLogstatuss
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the Document list.</summary>
    ///
    /// <value>The Documents IQueryable list.</value>
    public IDbSet<Document> Documents
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Bar list.</summary>
    ///
    /// <value>The Menu Bar IQueryable list.</value>
    public IDbSet<T_MenuBar> T_MenuBars
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Item list.</summary>
    ///
    /// <value>The Menu Item IQueryable list.</value>
    public IDbSet<T_MenuItem> T_MenuItems
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Menu Bar Menu Item Association list.</summary>
    ///
    /// <value>The Menu Bar Menu Item Association IQueryable list.</value>
    public IDbSet<T_MenuBarMenuItemAssociation> T_MenuBarMenuItemAssociations
    {
        get;
        set;
    }
    /// <summary>Gets or sets the TimeZone list.</summary>
    ///
    /// <value>The TimeZone IQueryable list.</value>
    public IDbSet<T_TimeZone> T_TimeZones
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ImportConfigurations list.</summary>
    ///
    /// <value>The ImportConfigurations IQueryable list.</value>
    public IDbSet<ImportConfiguration> ImportConfigurations
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FavoriteItems list.</summary>
    ///
    /// <value>The FavoriteItems IQueryable list.</value>
    public IDbSet<FavoriteItem> FavoriteItems
    {
        get;
        set;
    }
    /// <summary>Gets or sets the DefaultEntityPages list.</summary>
    ///
    /// <value>The DefaultEntityPages IQueryable list.</value>
    public IDbSet<DefaultEntityPage> DefaultEntityPages
    {
        get;
        set;
    }
    /// <summary>Gets or sets the DynamicRoleMappings list.</summary>
    ///
    /// <value>The DynamicRoleMappings IQueryable list.</value>
    public IDbSet<DynamicRoleMapping> DynamicRoleMappings
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ApplicationFeedbacks list.</summary>
    ///
    /// <value>The ApplicationFeedbacks IQueryable list.</value>
    public IDbSet<ApplicationFeedback> ApplicationFeedbacks
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ApplicationFeedbackTypes list.</summary>
    ///
    /// <value>The ApplicationFeedbackTypes IQueryable list.</value>
    public IDbSet<ApplicationFeedbackType> ApplicationFeedbackTypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ApplicationFeedbackStatuss list.</summary>
    ///
    /// <value>The ApplicationFeedbackStatuss IQueryable list.</value>
    public IDbSet<ApplicationFeedbackStatus> ApplicationFeedbackStatuss
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FeedbackPriority list.</summary>
    ///
    /// <value>The FeedbackPriority IQueryable list.</value>
    public IDbSet<FeedbackPriority> FeedbackPrioritys
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FavoriteItems list.</summary>
    ///
    /// <value>The FavoriteItems IQueryable list.</value>
    public IDbSet<FeedbackSeverity> FeedbackSeveritys
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FeedbackResource list.</summary>
    ///
    /// <value>The FeedbackResource IQueryable list.</value>
    public IDbSet<FeedbackResource> FeedbackResources
    {
        get;
        set;
    }
    /// <summary>Gets or sets the AppSettingGroup list.</summary>
    ///
    /// <value>The AppSettingGroup IQueryable list.</value>
    public IDbSet<AppSettingGroup> AppSettingGroups
    {
        get;
        set;
    }
    /// <summary>Gets or sets the AppSetting list.</summary>
    ///
    /// <value>The AppSetting IQueryable list.</value>
    public IDbSet<AppSetting> AppSettings
    {
        get;
        set;
    }
    /// <summary>Gets or sets the EmailTemplateType list.</summary>
    ///
    /// <value>The EmailTemplateType IQueryable list.</value>
    public IDbSet<EmailTemplateType> EmailTemplateTypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the EmailTemplate list.</summary>
    ///
    /// <value>The EmailTemplate IQueryable list.</value>
    public IDbSet<EmailTemplate> EmailTemplates
    {
        get;
        set;
    }
    /// <summary>Gets or sets the EntityDataSource list.</summary>
    ///
    /// <value>The EntityDataSource IQueryable list.</value>
    public IDbSet<EntityDataSource> EntityDataSources
    {
        get;
        set;
    }
    /// <summary>Gets or sets the DataSourceParameters list.</summary>
    ///
    /// <value>The DataSourceParameters IQueryable list.</value>
    public IDbSet<DataSourceParameters> DataSourceParameterss
    {
        get;
        set;
    }
    /// <summary>Gets or sets the PropertyMapping list.</summary>
    ///
    /// <value>The PropertyMapping IQueryable list.</value>
    public IDbSet<PropertyMapping> PropertyMappings
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Chart list.</summary>
    ///
    /// <value>The Chart IQueryable list.</value>
    public IDbSet<T_Chart> Charts
    {
        get;
        set;
    }
    
    #region Appointment/Scheduler
    /// <summary>Gets or sets the Schedule list.</summary>
    ///
    /// <value>The Schedule IQueryable list.</value>
    public IDbSet<T_Schedule> T_Schedules
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Scheduletype list.</summary>
    ///
    /// <value>The Scheduletype IQueryable list.</value>
    public IDbSet<T_Scheduletype> T_Scheduletypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FavoriteItems list.</summary>
    ///
    /// <value>The FavoriteItems IQueryable list.</value>
    public IDbSet<T_RecurringScheduleDetailstype> T_RecurringScheduleDetailstypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the RecurringFrequency list.</summary>
    ///
    /// <value>The RecurringFrequency IQueryable list.</value>
    public IDbSet<T_RecurringFrequency> T_RecurringFrequencys
    {
        get;
        set;
    }
    /// <summary>Gets or sets the RecurringEndType list.</summary>
    ///
    /// <value>The RecurringEndType IQueryable list.</value>
    public IDbSet<T_RecurringEndType> T_RecurringEndTypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the RecurrenceDays list.</summary>
    ///
    /// <value>The RecurrenceDays IQueryable list.</value>
    public IDbSet<T_RecurrenceDays> T_RecurrenceDayss
    {
        get;
        set;
    }
    /// <summary>Gets or sets the MonthlyRepeatType list.</summary>
    ///
    /// <value>The MonthlyRepeatType IQueryable list.</value>
    public IDbSet<T_MonthlyRepeatType> T_MonthlyRepeatTypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the RepeatOn list.</summary>
    ///
    /// <value>The RepeatOn IQueryable list.</value>
    public IDbSet<T_RepeatOn> T_RepeatOns
    {
        get;
        set;
    }
    #endregion
    /// <summary>Gets or sets the ApiToken list.</summary>
    ///
    /// <value>The ApiToken IQueryable list.</value>
    public IDbSet<ApiToken> ApiTokens
    {
        get;    //will be used in case of webapi only
        set;
    }
    /// <summary>Gets or sets the CustomReports list.</summary>
    ///
    /// <value>The CustomReports IQueryable list.</value>
    public IDbSet<CustomReports> CustomReportss
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ReportsInRole list.</summary>
    ///
    /// <value>The ReportsInRole IQueryable list.</value>
    public IDbSet<ReportsInRole> ReportsInRoles
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ApplicationUser list.</summary>
    ///
    /// <value>The ApplicationUser IQueryable list.</value>
    public IDbSet<ApplicationUser> T_Users
    {
        get;
        set;
    }
    /// <summary>Gets or sets the JournalEntry list.</summary>
    ///
    /// <value>The JournalEntry IQueryable list.</value>
    public IDbSet<JournalEntry> JournalEntries
    {
        get;
        set;
    }
    /// <summary>Gets or sets the FacetedSearch list.</summary>
    ///
    /// <value>The FacetedSearch IQueryable list.</value>
    public IDbSet<T_FacetedSearch> T_FacetedSearchs
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Data Metric list.</summary>
    ///
    /// <value>The Data Metric IQueryable list.</value>
    public IDbSet<T_DataMetric> T_DataMetrics
    {
        get;
        set;
    }
    /// <summary>Gets or sets the DataMetric Type list.</summary>
    ///
    /// <value>The DataMetric Type IQueryable list.</value>
    public IDbSet<T_DataMetrictype> T_DataMetrictypes
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Document Template list.</summary>
    ///
    /// <value>The Document Template IQueryable list.</value>
    public IDbSet<T_DocumentTemplate> T_DocumentTemplates
    {
        get;
        set;
    }
    /// <summary>Gets or sets the EntityPage list.</summary>
    ///
    /// <value>The EntityPage IQueryable list.</value>
    public IDbSet<EntityPage> EntityPages
    {
        get;
        set;
    }
    /// <summary>Gets or sets the EntityHelpPage list.</summary>
    ///
    /// <value>The EntityHelpPage IQueryable list.</value>
    public IDbSet<EntityHelpPage> EntityHelpPages
    {
        get;
        set;
    }
    /// <summary>Gets or sets the PropertyHelpPage list.</summary>
    ///
    /// <value>The PropertyHelpPage IQueryable list.</value>
    public IDbSet<PropertyHelpPage> PropertyHelpPages
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ReportsGroup list.</summary>
    ///
    /// <value>The ReportsGroup IQueryable list.</value>
    public IDbSet<ReportsGroup> ReportsGroups
    {
        get;
        set;
    }
    /// <summary>Gets or sets the ReportList list.</summary>
    ///
    /// <value>The ReportList IQueryable list.</value>
    public IDbSet<ReportList> ReportLists
    {
        get;
        set;
    }
    //
    
    
    //For Company Infromation
    /// <summary>Gets or sets the CompanyInformation list.</summary>
    ///
    /// <value>The CompanyInformation IQueryable list.</value>
    public IDbSet<CompanyInformation> CompanyInformations
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Footer Section  list.</summary>
    ///
    /// <value>The Footer Section  IQueryable list.</value>
    public IDbSet<FooterSection> FooterSections
    {
        get;
        set;
    }
    /// <summary>Gets or sets the CompanyInformation CompanyList Association list.</summary>
    ///
    /// <value>The CompanyInformation CompanyList Association IQueryable list.</value>
    public IDbSet<CompanyInformationCompanyListAssociation> CompanyInformationCompanyListAssociations
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Verb Group list.</summary>
    ///
    /// <value>The Verb Group IQueryable list.</value>
    public IDbSet<VerbGroup> VerbGroups
    {
        get;
        set;
    }
    /// <summary>Gets or sets the Verbs Name list.</summary>
    ///
    /// <value>The Verbs Name IQueryable list.</value>
    public IDbSet<VerbsName> VerbsNames
    {
        get;
        set;
    }
    
    /// <summary>Direct save changes (Bypass internal flow (journalentry, businessrules, permission etc.) and save changes directly to db).</summary>
    ///
    /// <returns>An int (returns 1 if saved successfully).</returns>
    public int DirectSaveChanges()
    {
        foreach(var entry in this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                e.State.HasFlag(EntityState.Modified) ||
                e.State.HasFlag(EntityState.Deleted)))
        {
            CacheHelper.RemoveCache(ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
        }
        return  base.SaveChanges();
    }
    /// <summary>Direct save changes (Bypass internal flow (businessrules, permission etc.) and save changes directly to db).</summary>
    ///
    /// <returns>An int (returns 1 if saved successfully).</returns>
    public int DirectSaveChangesWithJournaling()
    {
        var result = 0;
        var originalStates = new Dictionary<DbEntityEntry, EntityState>();
        var jedb = new JournalEntryContext(new SystemUser());
        foreach(var entry in this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                e.State.HasFlag(EntityState.Modified) ||
                e.State.HasFlag(EntityState.Deleted)))
        {
            DbPropertyValues OriginalObj = null;
            if(entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                OriginalObj = entry.GetDatabaseValues();
            MakeUpdateJournalEntry(entry, jedb, OriginalObj); //Added in list, but not committed into journal entry
            if(!originalStates.ContainsKey(entry))
                originalStates.Add(entry, entry.State);
            CacheHelper.RemoveCache(ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
        }
        result = base.SaveChanges();//Save Changes
        if(result > 0)   //check if everything gets saved or not
        {
            jedb.DirectSaveChanges(); //commit changes in journal entry
            if(originalStates.Any(p => p.Value.HasFlag(EntityState.Added)))
            {
                MakeAddJournalEntry(originalStates);
                SetAutoProperty(originalStates);
            }
        }
        return result;
    }
    /// <summary>Saves all changes made in this context to the underlying database.
    /// The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Permissions, Businessrules and other implementation before, during and after
    /// save changes. Calls code hooks. Makes journal entries.</summary>
    ///
    /// <returns>An int (returns 1 if saved successfully).</returns>
    public override int SaveChanges()
    {
        var result = 0;
        var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                      e.State.HasFlag(EntityState.Modified) ||
                      e.State.HasFlag(EntityState.Deleted));
        bool SoftDeleteEnabled = false;
        if(this.mode == 1)
        {
            return base.SaveChanges();
        }
        var originalStates = new Dictionary<DbEntityEntry, EntityState>();
        var jedb = new JournalEntryContext(new SystemUser());
        DbPropertyValues OriginalObj = null;
        foreach(var entry in entries)
        {
            OriginalObj = null;
            if(entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                OriginalObj = entry.GetDatabaseValues();
            if(this.mode != 0) //0 to ignore dirty properties check, and force calling save
                if(!CheckIfModified(entry, OriginalObj))
                {
                    CancelChanges(entry);
                    continue;
                }
            if(ValidateBeforeSave(entry, OriginalObj) == false)
            {
                CancelChanges(entry);
                continue;
            }
            OnSaving(entry, jedb, OriginalObj,SoftDeleteEnabled);
            MakeUpdateJournalEntry(entry, jedb, OriginalObj); //Added in list, but not committed into journal entry
            //Add in list for business logic after successfully);
            if(!originalStates.ContainsKey(entry))
                originalStates.Add(entry, entry.State);
            if(CheckExternalAPISave(entry))
            {
                CancelChanges(entry);
                continue;
            }
            CacheHelper.RemoveCache(ObjectContext.GetObjectType(entry.Entity.GetType()).Name);
        }
        result = base.SaveChanges();//Save Changes
        if(result > 0)  //check if everything gets saved or not
        {
            jedb.DirectSaveChanges(); //commit changes in journal entry
            ApplyBusinessLogicAfterSave(originalStates, OriginalObj); //Apply business rule after save
            if(originalStates != null && originalStates.Any(p => p.Value.HasFlag(EntityState.Deleted)))
            {
                AfterDelete(originalStates, OriginalObj);
            }
        }
        return result;
    }
    /// <summary>Executes the on saving action.</summary>
    ///
    /// <param name="entry">            The entry (object to be saved).</param>
    /// <param name="jedb">             The journal entry context.</param>
    /// <param name="OriginalObj">      The original object.</param>
    /// <param name="SoftDeleteEnabled">True to enable, false to disable the soft delete.</param>
    private void OnSaving(DbEntityEntry entry, JournalEntryContext jedb, DbPropertyValues OriginalObj, bool SoftDeleteEnabled)
    {
        // changes 11 april 2018
        if(entry.State != EntityState.Deleted)
        {
            OnSavingCustom(entry, this);
            SetDateTimeToUTC(entry);
            SetCalculatedValue(entry);
            SetAutoProperty(entry);
            SetDisplayValue(entry);
            CheckFieldLevelSecurity(entry, OriginalObj);
            if(entry.State == EntityState.Modified)
            {
                AssignOneToManyCurrentOnUpdate(entry, OriginalObj);
                //MakeUpdateJournalEntry(entry);
            }
            OrderedListCheck(entry, OriginalObj);
            TimeBasedAlert(entry,true); //to avoid duplicate emails onupdate/onadd bizrules
            InvokeWebHook(entry);
        }
        else
        {
            OnDeletingCustom(entry, OriginalObj,this);
            MakeDeleteJournalEntry(entry, jedb, OriginalObj);
        }
    }
    /// <summary>Applies the business logic after save.</summary>
    ///
    /// <param name="originalStates">List of original states.</param>
    private void ApplyBusinessLogicAfterSave(Dictionary<DbEntityEntry, EntityState> originalStates, DbPropertyValues OriginalObj)
    {
        if(originalStates != null)
        {
            if(originalStates.Any(p => p.Value.HasFlag(EntityState.Added)))
            {
                AssignOneToManyCurrentOnAdd(originalStates);
                MakeAddJournalEntry(originalStates);
                SetAutoProperty(originalStates);
            }
        }
        if(originalStates != null && originalStates.Any(p => p.Value.HasFlag(EntityState.Added) || p.Value.HasFlag(EntityState.Modified)))
        {
            InvokeActionRule(originalStates);
            EncryptValue(originalStates, OriginalObj);
            TimeBasedAlert(originalStates);
            AfterSave(originalStates);
        }
    }
    /// <summary>Check external API save (external web-api call).</summary>
    ///
    /// <param name="entry">The entry.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool CheckExternalAPISave(DbEntityEntry entry)
    {
        var result = false;
        var entity = (IEntity)entry.Entity;
        var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
        var EntityName = entityType.Name;
        var state = entry.State;
        if(IsExternalEntity(entity,EntityName,state).Result)
            result = true;
        return result;
    }
    /// <summary>Validates the before save.</summary>
    ///
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///                                         illegal values.</exception>
    ///
    /// <param name="entry">      The entry.</param>
    /// <param name="OriginalObj">The original object.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ValidateBeforeSave(DbEntityEntry entry, DbPropertyValues OriginalObj)
    {
        if((entry.State == EntityState.Deleted || entry.State == EntityState.Modified) && OriginalObj == null)
            return false;
        var result = true;
        var entity = (IEntity)entry.Entity;
        var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
        var EntityName = entityType.Name;
        if(!CheckPermissionOnEntity(EntityName, entry.State))
            return false;
        if(CheckOwnerPermission(entry, OriginalObj))
            return false;
        if(ViolatingBusinessRules(entry))
            return false;
        if(CheckLockCondition(entry, OriginalObj))
            return false;
        if(!CheckHierarchicalPermissionOnRecord(EntityName, entry, OriginalObj))
            return false;
        if((entry.State == EntityState.Added || entry.State == EntityState.Modified))
        {
            if(!Check1MThresholdCondition(entry))
                return false;
            string strChkBeforeSave = CheckBeforeSave(entity, EntityName);
            if(!string.IsNullOrEmpty(strChkBeforeSave))
            {
                throw new ArgumentException(strChkBeforeSave);
                return false;
            }
        }
        if(entry.State == EntityState.Deleted && !CheckBeforeDelete(entity, EntityName))
        {
            throw new ArgumentException("Validation Alert - Before Delete !! Record not deleted.");
            return false;
        }
        return result;
    }
}
}

