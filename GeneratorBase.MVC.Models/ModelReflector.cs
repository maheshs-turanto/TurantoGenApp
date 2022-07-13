using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
namespace GeneratorBase.MVC
{
/// <summary>Uses reflection to create lists of entities and their properties.</summary>
public class ModelReflector
{
    /// <summary>Gets or sets the entities.</summary>
    ///
    /// <value>The entities.</value>
    
    public static List<Entity> Entities
    {
        get;
        private set;
    }
    /// <summary>Static constructor.</summary>
    static ModelReflector()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = GetTypesWithTableAttribute(assembly);
        Entities = types.Select(t =>
                                new Entity()
        {
            Name = t.Name,
            DisplayName = GetDisplayName(t) ?? t.Name,
            IsDefault = GetDisplayName(t) == null || Enum.IsDefined(typeof(IgnoreEntities), t.Name) ? true : false,
            IsExternalEntity = GetDBTableAttribute(t),
            Properties = GetDisplayPropertiesForEntity(t).OrderBy(prop => prop.DisplayName).ToList(),
            Associations = GetAssociationsForEntity(t).OrderBy(asso => asso.DisplayName).ToList(),
            type = GetDescriptionAttr(t),
            //code for verb action security
            Verbs = GetVerbsForEntity(t).OrderBy(verb => verb.DisplayName).ToList(),
            //
            Groups = GetGroupsForEntity(t).OrderBy(g => g.Name).ToList(),
            //instruction feature
            Instructions = GetInstructionsForEntity(t).OrderBy(g => g.Name).ToList(),
            IsAdminEntity = (Enum.IsDefined(typeof(AdminEntities), t.Name)),
            IsnotificationEntity = (Enum.IsDefined(typeof(NotificationEntities), t.Name))
        }
                               ).OrderBy(p => p.IsDefault).ThenBy(p => p.DisplayName).ToList();
    }
    
    /// <summary>Gets database table attribute.</summary>
    ///
    /// <param name="t">A Type to process.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    private static bool GetDBTableAttribute(Type t)
    {
        var result = false;
        var attr = t.GetCustomAttribute<TableAttribute>(false);
        if(attr != null)
        {
            result = false;
        }
        else result = true;
        return result;
    }
    
    /// <summary>Gets display name.</summary>
    ///
    /// <param name="t">A Type to process.</param>
    ///
    /// <returns>The display name.</returns>
    
    private static string GetDisplayName(Type t)
    {
        string result = string.Empty;
        var attr = t.GetCustomAttribute<DisplayNameAttribute>(false);
        if(attr != null)
        {
            result = attr.DisplayName;
        }
        return string.IsNullOrEmpty(result) ? null : result;
    }
    
    /// <summary>Gets display name.</summary>
    ///
    /// <param name="t">A Type to process.</param>
    ///
    /// <returns>The display name.</returns>
    
    private static string GetDescriptionAttr(Type t)
    {
        string result = string.Empty;
        var attr = t.GetCustomAttribute<DescriptionAttribute>(false);
        if(attr != null)
        {
            result = attr.Description;
        }
        return string.IsNullOrEmpty(result) ? null : result;
    }
    
    /// <summary>Gets the types with table attributes in this collection.</summary>
    ///
    /// <param name="assembly">The assembly.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the types with table
    /// attributes in this collection.</returns>
    
    private static IEnumerable<Type> GetTypesWithTableAttribute(Assembly assembly)
    {
        //var tableAttr = typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute);
        foreach(Type type in assembly.GetTypes())
        {
            if(type.Name == "UserBasedSecurity" || type.Name == "UserDefinePages" || type.Name == "UserDefinePagesRole") continue;
            var attr = type.GetCustomAttribute<TableAttribute>(true);
            var attr1 = type.GetCustomAttribute<DisplayNameAttribute>(true);
            if(attr != null || attr1 != null)
            {
                yield return type;
            }
        }
    }
    
    /// <summary>Gets the display properties for entities in this collection.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the display properties for
    /// entities in this collection.</returns>
    
    private static IEnumerable<Property> GetDisplayPropertiesForEntity(Type type)
    {
        foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !p.Name.ToLower().EndsWith("workflowinstanceid") && p.PropertyType.Name != "ICollection`1"))
        {
            var attr = property.GetCustomAttribute<DisplayNameAttribute>(false);
            Attribute[] allattr = (Attribute[])Attribute.GetCustomAttributes(property, false);// property.GetCustomAttribute<System.ComponentModel.DataAnnotations.DataTypeAttribute>(false);
            var datatypeattr = allattr.FirstOrDefault(p => p.GetType() == typeof(System.ComponentModel.DataAnnotations.DataTypeAttribute));
            var Descriptionattr = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>(false);
            var regexattr = property.GetCustomAttribute<CustomRegularExpressionAttribute>(false);
            var formatattr = property.GetCustomAttribute<CustomDisplayFormat>(false);
            var Propattr = property.GetCustomAttribute<GeneratorBase.MVC.Models.PropertyInfoForEntity>(false);
            var PropTypeattr = property.GetCustomAttribute<GeneratorBase.MVC.Models.PropertyTypeForEntity>(false);
            if(attr != null)
            {
                yield return new Property()
                {
                    Name = property.Name,
                    DisplayName = attr.DisplayName,
                    DataType = (property.PropertyType.IsGenericType &&
                                property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? property.PropertyType.GetGenericArguments()[0].Name : property.PropertyType.Name,
                               IsRequired = property.IsDefined(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false),
                               IsEmailValidation = property.IsDefined(typeof(System.ComponentModel.DataAnnotations.EmailAddressAttribute), false),
                               DataTypeAttribute = datatypeattr != null ? ((System.ComponentModel.DataAnnotations.DataTypeAttribute)datatypeattr).DataType.ToString() : "",
                               Description = Descriptionattr == null ? "" : Descriptionattr.Description,
                               PropName = Propattr == null ? "" : Propattr.PropName,
                               PropText = Propattr == null ? "" : Propattr.PropText,
                               Proptype = Propattr == null ? "" : Propattr.Proptype,
                               PropCheck = PropTypeattr == null ? "" : PropTypeattr.PropCheck,
                               GroupInternalName = Propattr == null ? "" : Propattr.GroupInternalName,
                               IsDisplayProperty = PropTypeattr == null ? false : PropTypeattr.IsDisplayProperty,
                               RegExPattern = regexattr == null ? "" : regexattr.Pattern,
                               MaskPattern = regexattr == null ? "" : regexattr.maskpattern,
                               DisplayFormat = formatattr == null ? "" : formatattr.DataFormatString,
                               UIDisplayFormat = formatattr == null ? "" : formatattr.uidisplayformat,
                };
            }
        }
    }
    
    /// <summary>Gets the associations for entities in this collection.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the associations for entities
    /// in this collection.</returns>
    
    private static IEnumerable<Association> GetAssociationsForEntity(Type type)
    {
        var proplist = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach(var property in proplist.Where(p => p.GetCustomAttribute<DisplayNameAttribute>(false) == null))
        {
            if(property.Name == "Id") continue;
            var targetProperty = proplist.Where(p => p.Name.ToLower() == property.Name.ToLower() + "id").ToList();
            if(targetProperty.Count > 0)
            {
                var name = targetProperty[0].Name;
                yield return new Association()
                {
                    Name = name.TrimEnd("ID".ToCharArray()),
                    DisplayName = targetProperty[0].GetCustomAttribute<DisplayNameAttribute>(false).DisplayName,
                    IsRequired = targetProperty[0].IsDefined(typeof(System.ComponentModel.DataAnnotations.CustomValidationAttribute), false),
                    AssociationProperty = name,
                    Target = (property.PropertyType.Name == "ApplicationUser" ? "IdentityUser" : property.PropertyType.Name)
                };
            }
        }
        var tenantprop = proplist.FirstOrDefault(p => p.Name == "TenantId");
        if(tenantprop != null)
        {
            var name = "TenantId";
            var Propattr = tenantprop.GetCustomAttribute<GeneratorBase.MVC.Models.PropertyInfoForEntity>(false);
            if(Propattr != null)
                yield return new Association()
            {
                Name = name,
                DisplayName = tenantprop.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName,
                IsRequired = tenantprop.IsDefined(typeof(System.ComponentModel.DataAnnotations.CustomValidationAttribute), false),
                AssociationProperty = name,
                Target = Propattr == null ? "" : Propattr.PropName
            };
        }
    }
    
    /// <summary>code for verb action security.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the verbs for entities in
    /// this collection.</returns>
    
    private static IEnumerable<Verb> GetVerbsForEntity(Type type)
    {
        // string controllerName = "GeneratorBase.MVC.Controllers." + type.Name + "Controller, GeneratorBase.MVC.Controllers";
        // Type VerbTypeCls = Type.GetType(controllerName);
        Type VerbTypeCls = new GeneratorBase.MVC.Models.CreateControllerType(type.Name).controllerType;
        if(VerbTypeCls != null)
        {
            MethodInfo VerbTypeMethod = VerbTypeCls.GetMethod("getVerbsName");
            if(VerbTypeMethod != null)
            {
                using(var clsInstance = (IDisposable)Activator.CreateInstance(VerbTypeCls))
                {
                    string[] result = (string[])VerbTypeMethod.Invoke(clsInstance, null);
                    foreach(var verbName in result)
                    {
                        var name = verbName;
                        yield return new Verb()
                        {
                            Name = name,
                            DisplayName = name,
                        };
                    }
                }
            }
        }
    }
    
    /// <summary>code for getting groups of entity.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the groups for entities in
    /// this collection.</returns>
    
    private static IEnumerable<Group> GetGroupsForEntity(Type type)
    {
        //string controllerName = "GeneratorBase.MVC.Controllers." + type.Name + "Controller, GeneratorBase.MVC.Controllers";
        //Type GroupType = Type.GetType(controllerName);
        Type GroupType = new GeneratorBase.MVC.Models.CreateControllerType(type.Name).controllerType;
        if(GroupType != null)
        {
            MethodInfo GroupTypeMethod = GroupType.GetMethod("getGroupsName");
            if(GroupTypeMethod != null)
            {
                string[][] result = null;
                try
                {
                    using(var clsInstance = (IDisposable)Activator.CreateInstance(GroupType))
                    {
                        result = (string[][])GroupTypeMethod.Invoke(clsInstance, null);
                    }
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                if(result != null)
                {
                    foreach(string[] groupName in result)
                    {
                        var name = groupName[1];
                        var id = groupName[0];
                        yield return new Group()
                        {
                            Name = name,
                            Id = id
                        };
                    }
                }
            }
        }
    }
    //instruction feature
    /// <summary>code for getting instructions of entity.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process the instructions for entities in
    /// this collection.</returns>
    
    private static IEnumerable<Instruction> GetInstructionsForEntity(Type type)
    {
        //string controllerName = "GeneratorBase.MVC.Controllers." + type.Name + "Controller, GeneratorBase.MVC.Controllers";
        //Type InstuctionType = Type.GetType(controllerName);
        Type InstuctionType = new GeneratorBase.MVC.Models.CreateControllerType(type.Name).controllerType;
        if(InstuctionType != null)
        {
            MethodInfo InstuctionTypeMethod = InstuctionType.GetMethod("getInstructionLabelInfo");
            if(InstuctionTypeMethod != null)
            {
                string[][] result = null;
                try
                {
                    using(var clsInstance = (IDisposable)Activator.CreateInstance(InstuctionType))
                    {
                        result = (string[][])InstuctionTypeMethod.Invoke(clsInstance, null);
                    }
                }
                catch(Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                if(result != null)
                {
                    foreach(string[] instuctionLabel in result)
                    {
                        var name = instuctionLabel[1];
                        var id = instuctionLabel[0];
                        yield return new Instruction()
                        {
                            Name = name,
                            Id = id
                        };
                    }
                }
            }
        }
    }
    /// <summary>An entity.</summary>
    [DebuggerDisplay("{Name,nq}")]
    public class Entity
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        
        public string DisplayName
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
        
        public List<Property> Properties
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the associations.</summary>
        ///
        /// <value>The associations.</value>
        
        public List<Association> Associations
        {
            get;
            set;
        }
        
        /// <summary>code for verb action security.</summary>
        ///
        /// <value>The verbs.</value>
        
        public List<Verb> Verbs
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the groups.</summary>
        ///
        /// <value>The groups.</value>
        
        public List<Group> Groups
        {
            get;
            set;
        }
        //instruction feature
        /// <summary>Gets or sets the instructions.</summary>
        ///
        /// <value>The instructions.</value>
        
        public List<Instruction> Instructions
        {
            get;
            set;
        }
        /// <summary>Gets or sets a value indicating whether this object is admin entity.</summary>
        ///
        /// <value>True if this object is admin entity, false if not.</value>
        
        public bool IsAdminEntity
        {
            get;
            set;
        }
        /// <summary>Gets or sets a value indicating whether this object is admin entity.</summary>
        ///
        /// <value>True if this object is notification entity, false if not.</value>
        
        public bool IsnotificationEntity
        {
            get;
            set;
        }
        
        
        /// <summary>Gets or sets a value indicating whether this object is default.</summary>
        ///
        /// <value>True if this object is default, false if not.</value>
        
        public bool IsDefault
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets a value indicating whether this object is external entity.</summary>
        ///
        /// <value>True if this object is external entity, false if not.</value>
        
        public bool IsExternalEntity
        {
            get;
            set;
        }
        /// <summary>Gets or sets a type value of entity.</summary>
        ///
        /// <value>The type of entity.</value>
        
        public string type
        {
            get;
            set;
        }
    }
    /// <summary>A property.</summary>
    [DebuggerDisplay("{DisplayName,nq}")]
    public class Property
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        
        public string DisplayName
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the type of the data.</summary>
        ///
        /// <value>The type of the data.</value>
        
        public string DataType
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets a value indicating whether this object is required.</summary>
        ///
        /// <value>True if this object is required, false if not.</value>
        
        public bool IsRequired
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets a value indicating whether this object is email validation.</summary>
        ///
        /// <value>True if this object is email validation, false if not.</value>
        
        public bool IsEmailValidation
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the data type attribute.</summary>
        ///
        /// <value>The data type attribute.</value>
        
        public string DataTypeAttribute
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        
        public string Description
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the name of the property.</summary>
        ///
        /// <value>The name of the property.</value>
        
        public string PropName
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the property text.</summary>
        ///
        /// <value>The property text.</value>
        
        public string PropText
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the proptype.</summary>
        ///
        /// <value>The proptype.</value>
        
        public string Proptype
        {
            get;
            set;
        }
        public string GroupInternalName
        {
            get;
            set;
        }
        /// <summary>Gets or sets the propcheck.</summary>
        ///
        /// <value>The propcheck.</value>
        
        
        public string PropCheck
        {
            get;
            set;
        }
        public bool IsDisplayProperty
        {
            get;
            set;
        }
        /// <summary>Gets or sets the regex pattern.</summary>
        ///
        /// <value>The regex.</value>
        
        public string RegExPattern
        {
            get;
            set;
        }
        /// <summary>Gets or sets the mask pattern.</summary>
        ///
        /// <value>The regex.</value>
        
        public string MaskPattern
        {
            get;
            set;
        }
        /// <summary>Gets or sets the Display Format.</summary>
        ///
        /// <value>The regex.</value>
        
        public string DisplayFormat
        {
            get;
            set;
        }
        /// <summary>Gets or sets the UI Display Format.</summary>
        ///
        /// <value>The regex.</value>
        
        public string UIDisplayFormat
        {
            get;
            set;
        }
    }
    /// <summary>An association.</summary>
    public class Association
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        
        public string DisplayName
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets a value indicating whether this object is required.</summary>
        ///
        /// <value>True if this object is required, false if not.</value>
        
        public bool IsRequired
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the association property.</summary>
        ///
        /// <value>The association property.</value>
        
        public string AssociationProperty
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the Target for the.</summary>
        ///
        /// <value>The target.</value>
        
        public string Target
        {
            get;
            set;
        }
    }
    
    /// <summary>code for verb action security.</summary>
    public class Verb
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        
        public string DisplayName
        {
            get;
            set;
        }
        
    }
    /// <summary>A group.</summary>
    public class Group
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        
        public string Id
        {
            get;
            set;
        }
    }
    //instruction feature
    /// <summary>A Instruction.</summary>
    public class Instruction
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the identifier.</summary>
        ///
        ///  <summary>Gets or sets the identifier.</summary>
        
        public string Id
        {
            get;
            set;
        }
        ///  <summary>Gets or sets the Label.</summary>
        ///
        ///  <summary>Gets or sets the Label.</summary>
        public string Label
        {
            get;
            set;
        }
    }
    /// <summary>Information about the home tab.</summary>
    public class HomeTabInfo
    {
        /// <summary>Gets or sets the is tabed.</summary>
        ///
        /// <value>The is tabed.</value>
        
        public string IsTabed
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the has home.</summary>
        ///
        /// <value>The has home.</value>
        
        public string HasHome
        {
            get;
            set;
        }
        
        /// <summary>Gets or sets the tab order.</summary>
        ///
        /// <value>The tab order.</value>
        
        public string TabOrder
        {
            get;
            set;
        }
    }
    /// <summary>Values that represent admin entities.</summary>
    public enum AdminEntities
    {
        /// <summary>An enum constant representing the action Arguments option.</summary>
        ActionArgs,
        /// <summary>An enum constant representing the action type option.</summary>
        ActionType,
        /// <summary>An enum constant representing the business rule option.</summary>
        BusinessRule,
        /// <summary>An enum constant representing the business rule type option.</summary>
        BusinessRuleType,
        /// <summary>An enum constant representing the condition option.</summary>
        Condition,
        /// <summary>An enum constant representing the permission option.</summary>
        Permission,
        /// <summary>An enum constant representing the rule action option.</summary>
        RuleAction,
        CompanyInformation,
        CompanyInformationCompanyListAssociation,
        FooterSection
    }
    public enum NotificationEntities
    {
        PropertyHelpPage,
        Notification,
        Notificationtype,
        BackColor,
        IconType,
    }
    
    /// <summary>Values that represent ignore entities.</summary>
    public enum IgnoreEntities
    {
        /// <summary>An enum constant representing the property mapping option.</summary>
        PropertyMapping,
        /// <summary>An enum constant representing the recurrence days option.</summary>
        T_RecurrenceDays,
        /// <summary>An enum constant representing the recurring end type option.</summary>
        T_RecurringEndType,
        /// <summary>An enum constant representing the recurring frequency option.</summary>
        T_RecurringFrequency,
        /// <summary>An enum constant representing the recurring schedule detailstype option.</summary>
        T_RecurringScheduleDetailstype,
        /// <summary>An enum constant representing the repeat on option.</summary>
        T_RepeatOn,
        /// <summary>An enum constant representing the schedule option.</summary>
        T_Schedule,
        /// <summary>An enum constant representing the scheduletype option.</summary>
        T_Scheduletype,
        /// <summary>An enum constant representing the data source parameters option.</summary>
        DataSourceParameters,
        /// <summary>An enum constant representing the document option.</summary>
        //Document,
        /// <summary>An enum constant representing the entity data source option.</summary>
        EntityDataSource,
        /// <summary>An enum constant representing the monthly repeat type option.</summary>
        T_MonthlyRepeatType,
        /// <summary>An enum constant representing the API token option.</summary>
        ApiToken,
        /// <summary>An enum constant representing the multi tenant extra access option.</summary>
        MultiTenantExtraAccess,
        /// <summary>An enum constant representing the multi tenant login selected option.</summary>
        MultiTenantLoginSelected,
        /// <summary>An enum constant representing the LoginSelectedRoles option.</summary>
        LoginSelectedRoles,
        /// <summary>An enum constant representing the file document option.</summary>
        FileDocument,
        /// <summary>An enum constant representing the Entity Help Page option.</summary>
        EntityHelpPage,
        /// <summary>An enum constant representing the Entity Page option.</summary>
        EntityPage,
        /// <summary>An enum constant representing the Custom Report option.</summary>
        CustomReports,
        /// <summary>An enum constant representing the Report List option.</summary>
        ReportList,
        /// <summary>An enum constant representing the Report Group option.</summary>
        ReportsGroup,
        /// <summary>An enum constant representing the Property Help Page option.</summary>
        ///
        PropertyHelpPage,
        Notification,
        Notificationtype,
        BackColor,
        IconType, NotificationUserAssociation,
        NotificationRolestabAssociation,
        ApplicationUser,
        CompanyInformation,
        CompanyInformationCompanyListAssociation,
        FooterSection
        
        
    }
}
}