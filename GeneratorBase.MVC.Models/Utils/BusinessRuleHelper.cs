using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
/// <summary>The business rule helper.</summary>
public class BusinessRuleHelper
{
    /// <summary>Gets mandatory properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The mandatory properties dictionary.</returns>
    
    public static Dictionary<string, string> GetMandatoryPropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                if(ruleType == "OnCreate")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                else if(ruleType == "OnEdit")
                    BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                var listruleactions = BR.Where(q => q.ruleaction.Any(p => p.associatedactiontype.TypeNo == 2));
                if(listruleactions.Count() == 0) return RequiredProperties;
                OModel.setCalculation();
                //var ResultOfBusinessRules = db.MandatoryPropertiesRule((object)OModel, BR, entityName);
                var ResultOfBusinessRules = MandatoryPropertiesRule((object)OModel, BR, entityName);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var ruleActions = new RuleActionContext().RuleActions.Where(p => p.AssociatedActionTypeID == 2).GetFromCache<IQueryable<RuleAction>, RuleAction>().Select(p => p.RuleActionID).ToList();
                var BRFail = BRAll.Except(BR);
                BRFail = BRFail.Where(p => ruleActions.Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    var listArgs = Args;
                    var entityModel = ModelReflector.Entities;
                    foreach(var parametersArgs in Args)
                    {
                        var dispName = "";
                        var paramName = parametersArgs.ParameterName;
                        var entity = entityModel.FirstOrDefault(p => p.Name == entityName);
                        var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                        if(property != null)
                            dispName = property.DisplayName;
                        else
                        {
                            if(paramName.Contains("."))
                            {
                                var arrparamName = paramName.Split('.');
                                var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);
                                var targetPropName = entityModel.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                            }
                        }
                        if(!RequiredProperties.ContainsKey(paramName))
                        {
                            var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
                            var FailureMessage = !(string.IsNullOrEmpty(objBR.FailureMessage)) ? objBR.FailureMessage : dispName;
                            RequiredProperties.Add(paramName, FailureMessage);
                            if(!RequiredProperties.ContainsKey("FailureMessage-" + objBR.Id))
                                RequiredProperties.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RequiredProperties.ContainsKey("InformationMessage-" + objBR.Id))
                            RequiredProperties.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return RequiredProperties;
    }
    
    /// <summary>Gets validate before save properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The validate before save properties dictionary.</returns>
    public static Dictionary<string, string> GetValidateBeforeSavePropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName.Trim()).ToList();
            EntityState state = EntityState.Added;
            if(ruleType == "OnEdit")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                state = EntityState.Modified;
            }
            if(ruleType == "OnCreate")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                state = EntityState.Added;
            }
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var listruleactions = BR.Where(q => q.ruleaction.Any(p => p.associatedactiontype.TypeNo == 10));
                if(listruleactions.Count() == 0) return RulesApplied;
                OModel.setCalculation();
                //var ResultOfBusinessRules = db.ValidateBeforeSavePropertiesRule((object)OModel, BR, entityName, state);
                var ResultOfBusinessRules = ValidateBeforeSavePropertiesRule((object)OModel, BR, entityName, state);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 10);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(10))
                {
                    var entityModel = ModelReflector.Entities;
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        if(rules.Key.TypeNo == 10)
                        {
                            var ruleconditionsdb = new ConditionContext().Conditions.Where(p => p.RuleConditionsID == rules.Value.BRID).GetFromCache<IQueryable<Condition>, Condition>();
                            foreach(var condition in ruleconditionsdb)
                            {
                                string conditionPropertyName = condition.PropertyName;
                                var Entity = entityModel.FirstOrDefault(p => p.Name == entityName);
                                var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName);
                                var AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == conditionPropertyName);
                                var propDispName = "";
                                if(conditionPropertyName.StartsWith("[") && conditionPropertyName.EndsWith("]"))
                                {
                                    conditionPropertyName = conditionPropertyName.TrimStart('[').TrimEnd(']').Trim();
                                    if(conditionPropertyName.Contains("."))
                                    {
                                        var targetProperties = conditionPropertyName.Split(".".ToCharArray());
                                        if(targetProperties.Length > 1)
                                        {
                                            AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                            if(AssociationInfo != null)
                                            {
                                                var EntityInfo1 = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                                conditionPropertyName = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]).DisplayName;
                                            }
                                        }
                                        propDispName = AssociationInfo.DisplayName + "." + conditionPropertyName;
                                    }
                                }
                                else
                                {
                                    propDispName = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName).DisplayName;
                                }
                                conditions += propDispName + " " + condition.Operator + " " + condition.Value + ", ";
                            }
                        }
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(','));
                        //RulesApplied.Add(rules.Value.BRID, conditions.Trim().TrimEnd(",".ToCharArray()));
                        var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                        foreach(var objBR in BRList)
                        {
                            if(!RulesApplied.ContainsKey(objBR.Id.ToString()))
                            {
                                if(!string.IsNullOrEmpty(objBR.FailureMessage))
                                {
                                    if(rules.Key.TypeNo == 10)
                                        RulesApplied.Add(objBR.Id.ToString(), objBR.FailureMessage);
                                }
                                else
                                    RulesApplied.Add(objBR.Id.ToString(), conditions.Trim().TrimEnd(",".ToCharArray()));
                            }
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return RulesApplied;
    }
    /// <summary>Gets validate before save properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="ruleType">  Type of the rule.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The validate before save properties dictionary.</returns>
    public static Dictionary<string, string> GetValidateBeforeSavePropertiesDictionaryForPopupConfirm(IUser User, ApplicationContext db, dynamic OModel, string ruleType, string entityName)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        var conditions = "";
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName.Trim()).ToList();
            EntityState state = EntityState.Added;
            if(ruleType == "OnEdit")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                state = EntityState.Modified;
            }
            if(ruleType == "OnCreate")
            {
                BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                state = EntityState.Added;
            }
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var listruleactions = BR.Where(q => q.ruleaction.Any(p => p.associatedactiontype.TypeNo == 15));
                if(listruleactions.Count() == 0) return RulesApplied;
                OModel.setCalculation();
                //var ResultOfBusinessRules = db.ValidateBeforeSavePropertiesRuleConfirmPop((object)OModel, BR, entityName, state);
                var ResultOfBusinessRules = ValidateBeforeSavePropertiesRuleConfirmPop((object)OModel, BR, entityName, state);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 15);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(15))
                {
                    var entityModel = ModelReflector.Entities;
                    foreach(var rules in ResultOfBusinessRules)
                    {
                        if(rules.Key.TypeNo == 15)
                        {
                            var ruleconditionsdb = new ConditionContext().Conditions.Where(p => p.RuleConditionsID == rules.Value.BRID).GetFromCache<IQueryable<Condition>, Condition>();
                            foreach(var condition in ruleconditionsdb)
                            {
                                string conditionPropertyName = condition.PropertyName;
                                var Entity = entityModel.FirstOrDefault(p => p.Name == entityName);
                                var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName);
                                var AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == conditionPropertyName);
                                var propDispName = "";
                                if(conditionPropertyName.StartsWith("[") && conditionPropertyName.EndsWith("]"))
                                {
                                    conditionPropertyName = conditionPropertyName.TrimStart('[').TrimEnd(']').Trim();
                                    if(conditionPropertyName.Contains("."))
                                    {
                                        var targetProperties = conditionPropertyName.Split(".".ToCharArray());
                                        if(targetProperties.Length > 1)
                                        {
                                            AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                            if(AssociationInfo != null)
                                            {
                                                var EntityInfo1 = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                                conditionPropertyName = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]).DisplayName;
                                            }
                                        }
                                        propDispName = AssociationInfo.DisplayName + "." + conditionPropertyName;
                                    }
                                }
                                else if(conditionPropertyName == "Id" && condition.Operator == ">")
                                    conditions += "Always";
                                else
                                {
                                    propDispName = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName).DisplayName;
                                }
                                conditions += propDispName + " " + condition.Operator + " " + condition.Value + ", ";
                            }
                        }
                        //RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(','));
                        //RulesApplied.Add("<span style=\"color:grey;font-size:11px;\">Warning(#" + rules.Value.BRID + ") :</span> ", conditions.Trim().TrimEnd(",".ToCharArray()));
                        var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                        foreach(var objBR in BRList)
                        {
                            if(!RulesApplied.ContainsKey(objBR.Id.ToString()))
                            {
                                if(!string.IsNullOrEmpty(objBR.FailureMessage))
                                {
                                    if(rules.Key.TypeNo == 15 && !RulesApplied.ContainsKey("Type15" + objBR.Id))
                                    {
                                        RulesApplied.Add("Type15" + objBR.Id, objBR.FailureMessage);
                                    }
                                }
                                else
                                    RulesApplied.Add(objBR.Id.ToString(), conditions.Trim().TrimEnd(",".ToCharArray()));
                            }
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return RulesApplied;
    }
    
    /// <summary>Gets lock business rules dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The lock business rules dictionary.</returns>
    
    public static Dictionary<string, string> GetLockBusinessRulesDictionary(IUser User, ApplicationContext db, dynamic OModel, string entityName)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                OModel.setCalculation();
                //var ResultOfBusinessRules = db.LockEntityRule((object)OModel, BR, entityName);
                var ResultOfBusinessRules = LockEntityRule((object)OModel, BR, entityName);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 2);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1) || ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                {
                    var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                    foreach(var objBR in BRList)
                    {
                        var FailureMessage = !(string.IsNullOrEmpty(objBR.FailureMessage)) ? objBR.FailureMessage : objBR.RuleName;
                        RulesApplied.Add(objBR.Id.ToString(), FailureMessage);
                        //if(!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
                        //    RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return RulesApplied;
    }
    
    /// <summary>Check mandatory properties.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    
    public static List<string> CheckMandatoryProperties(IUser User, ApplicationContext db, dynamic OModel, string entityName)
    {
        List<string> result = new List<string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if(BR != null && BR.Count > 0)
            {
                var modeltype = OModel.GetType();
                var propertyId = "Id";
                if(modeltype.GetProperty(propertyId) != null)
                {
                    var propertyvalue = modeltype.GetProperty(propertyId).GetValue(OModel, null);
                    if(propertyvalue == 0)
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                    if(propertyvalue > 0)
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                }
                //var ResultOfBusinessRules = db.MandatoryPropertiesRule((object)OModel, BR, entityName);
                var ResultOfBusinessRules = MandatoryPropertiesRule((object)OModel, BR, entityName);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    var entityModel = ModelReflector.Entities;
                    foreach(string paramName in Args.Select(p => p.ParameterName))
                    {
                        var type = OModel.GetType();
                        if(type.GetProperty(paramName) == null) continue;
                        var propertyvalue = type.GetProperty(paramName).GetValue(OModel, null);
                        if(propertyvalue == null)
                        {
                            var dispName = entityModel.FirstOrDefault(p => p.Name == entityName).Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            result.Add(dispName);
                        }
                    }
                }
            }
        }
        return result;
    }
    
    /// <summary>Gets read only properties dictionary.</summary>
    ///
    /// <param name="User">      The user.</param>
    /// <param name="db">        The database.</param>
    /// <param name="OModel">    The model.</param>
    /// <param name="entityName">Name of the entity.</param>
    ///
    /// <returns>The read only properties dictionary.</returns>
    
    public static Dictionary<string, string> GetReadOnlyPropertiesDictionary(IUser User, ApplicationContext db, dynamic OModel, string entityName, string ruleType = null)
    {
        Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
        if(User.businessrules != null)
        {
            var BR = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var listruleactions = BR.Where(q => q.ruleaction.Any(p => p.associatedactiontype.TypeNo == 4));
                var BrTypeID = 0;
                if(ruleType == "OnCreate")
                {
                    listruleactions = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                    BrTypeID = 1;
                }
                else if(ruleType == "OnEdit")
                {
                    listruleactions = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                    BrTypeID = 2;
                }
                if(listruleactions.Count() == 0) return RulesApplied;
                OModel.setCalculation();
                //var ResultOfBusinessRules = db.ReadOnlyPropertiesRule((object)OModel, BR, entityName);
                var ResultOfBusinessRules = ReadOnlyPropertiesRule((object)OModel, BR, entityName);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 4);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4) && (ResultOfBusinessRules.Values.Select(p => p.BRTID).Contains(BrTypeID) || ResultOfBusinessRules.Values.Select(p => p.BRTID).Contains(3)))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                    var listArgs = Args;
                    var entityModel = ModelReflector.Entities;
                    foreach(var parametersArgs in Args)
                    {
                        var dispName = "";
                        var paramName = parametersArgs.ParameterName;
                        var entity = entityModel.FirstOrDefault(p => p.Name == entityName);
                        var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                        //code for have mandatory property read only
                        var isIsRequired = false;
                        if(ruleType == "OnCreate")
                        {
                            var asso = entity.Associations.FirstOrDefault(p => p.AssociationProperty == paramName);
                            if(asso != null)
                                isIsRequired = asso.IsRequired;
                            else
                                isIsRequired = property.IsRequired;
                        }
                        if(isIsRequired)
                            continue;
                        //
                        if(property != null)
                            dispName = property.DisplayName;
                        else
                        {
                            if(paramName.Contains("."))
                            {
                                var arrparamName = paramName.Split('.');
                                var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);
                                var targetPropName = entityModel.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                            }
                        }
                        if(!RulesApplied.ContainsKey(paramName))
                        {
                            RulesApplied.Add(paramName, dispName);
                            var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
                            if(!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
                                RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                }
                if(BRFail != null && BRFail.Count() > 0)
                {
                    foreach(var objBR in BRFail)
                    {
                        if(!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
                            RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                    }
                }
            }
        }
        return RulesApplied;
    }
    
    /// <summary>Check hidden.</summary>
    ///
    /// <param name="User">           The user.</param>
    /// <param name="entityName">     Name of the entity.</param>
    /// <param name="brType">         Type of the line break.</param>
    /// <param name="isHiddenGroup">  True if is hidden group, false if not.</param>
    /// <param name="rbList">         List of rbs.</param>
    /// <param name="inlinesuffix">   (Optional) The inlinesuffix.</param>
    /// <param name="inlinedivsuffix">(Optional) The inlinedivsuffix.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string checkHidden(IUser User, string entityName, string brType, bool isHiddenGroup, string[] rbList, string inlinesuffix = "", string inlinedivsuffix = "")
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder hiddenBRString = new System.Text.StringBuilder();
        System.Text.StringBuilder chkHiddenGroup = new System.Text.StringBuilder();
        //RuleActionContext objRuleAction = new RuleActionContext();
        //ConditionContext objCondition = new ConditionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string selectCondition = "";
        string selectval = "";
        string propChangeEvnetdd = "";
        string AssociationName = "";
        bool IsAlways = false;
        {
            foreach(BusinessRule objBR in businessRules)
            {
                long ActionTypeId = 6;
                if(isHiddenGroup)
                    ActionTypeId = 12;
                //var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                var objRuleActionList = objBR.ruleaction.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId);
                if(objRuleActionList.Count() > 0)
                {
                    if(objBR.AssociatedBusinessRuleTypeID == 1 && brType != "OnCreate")
                        continue;
                    else if(objBR.AssociatedBusinessRuleTypeID == 2 && (brType != "OnEdit" && brType != "OnDetails"))
                        continue;
                    System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        //var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID).GetFromCache<IQueryable<Condition>, Condition>();
                        var objConditionList = objBR.ruleconditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                if(!User.CanView(entityName, inlinesuffix + objCon.PropertyName))
                                    continue;
                                IsAlways = false;
                                if(objCon.PropertyName == "Id")
                                {
                                    IsAlways = true;
                                }
                                if(string.IsNullOrEmpty(chkHidden.ToString()))
                                {
                                    if(objCon.PropertyName == "Id")
                                    {
                                        chkHidden.Append("<script type='text/javascript'>$(document).ready(function () {");
                                    }
                                    else
                                    {
                                        chkHidden.Append("<script type='text/javascript'>$(document).ready(function () {");
                                        //fnCondition = "hiddenCondition" + objCon.Id.ToString() + "()";
                                        fnCondition = "hiddenCondition" + objCon.Id.ToString() + "_" + ActionTypeId.ToString() + "(true)";
                                        chkHidden.Append(fnCondition + ";");
                                    }
                                }
                                string datatype = checkPropType(entityName, objCon.PropertyName, false);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy").Trim();
                                else
                                    condValue = objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?").Trim();
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                if(datatype == "Association")
                                {
                                    var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    if(!User.CanView(entityName, inlinesuffix + propertyName))
                                        continue;
                                    if(rbList != null && rbList.Contains(propertyName))
                                        rbcheck = true;
                                    var strText = ".text()";
                                    var strOptionSelected = "$('option:selected', '#" + inlinesuffix + propertyName + ":not(hidden)')";
                                    if(brType != "OnDetails")
                                        chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + inlinesuffix + propertyName + "')") + ".change(function() { " + fnCondition.Replace("(true)", "(false)") + "; });");
                                    else
                                    {
                                        propertyName = "lbl" + propertyName;
                                        strText = "[0].innerText";
                                        strOptionSelected = "$('#" + inlinesuffix + propertyName + "')";
                                    }
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "(" + strOptionSelected) + strText + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " " + logicalconnector + " (" + strOptionSelected) + strText + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(condValue.ToLower() == "null")
                                        {
                                            if(string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "(" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + "-- Select --" + "'.toLowerCase())" + "&&" + "(" + strOptionSelected + strText + ".toLowerCase() " + operand + " '" + "--None--" + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " " + logicalconnector + " (" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())" + "&&" + "(" + strOptionSelected + strText + ".toLowerCase() " + operand + " '" + "--None--" + "'.toLowerCase())";
                                            }
                                        }
                                        else
                                        {
                                            if(string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "(" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " " + logicalconnector + " (" + strOptionSelected) + strText + ".toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                    if(!rbcheck)
                                    {
                                        if(isHiddenGroup)
                                        {
                                            if(AssociationName != propertyName)
                                            {
                                                propChangeEvnetdd += " $('#" + inlinesuffix + propertyName + "').change(function() { CONDITION ";
                                                selectval += " var selected" + propertyName + " =  $('option:selected', '#" + inlinesuffix + propertyName + "') " + ".text().toLowerCase(); ";
                                                AssociationName = propertyName;
                                            }
                                            selectCondition += " selected" + propertyName + operand + " '" + condValue + "'.toLowerCase() ||";
                                        }
                                    }
                                }
                                else
                                {
                                    string propertyName = objCon.PropertyName;
                                    if(!User.CanView(entityName, inlinesuffix + propertyName))
                                        continue;
                                    string strVal = ".val()";
                                    string eventName = ".change";
                                    if(propertyAttribute.ToLower() == "currency")
                                        eventName = ".blur";
                                    if(brType != "OnDetails")
                                    {
                                        if(propertyName != "Id")
                                            chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + inlinesuffix + propertyName + "')") + eventName + "(function() { " + fnCondition.Replace("(true)", "(false)") + "; });");
                                    }
                                    else
                                    {
                                        if(propertyName != "Id")
                                        {
                                            propertyName = "lbl" + propertyName;
                                            strVal = "[0].innerText";
                                        }
                                    }
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            fnConditionValue = "($('#" + inlinesuffix + propertyName + "')" + strVal + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " ($('#" + inlinesuffix + propertyName + "')" + strVal + ".toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(propertyName != "Id")
                                        {
                                            string strLowerCase = ".toLowerCase() ";
                                            string strCondValue = " '" + condValue + "'";
                                            if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                            {
                                                strLowerCase = "";
                                                strCondValue = condValue;
                                            }
                                            if(datatype.ToLower() == "boolean")
                                            {
                                                strVal = ".prop('checked')";
                                                strCondValue = condValue.ToLower();
                                                strLowerCase = "";
                                            }
                                            if(string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                if(propertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && (brType == "OnEdit" || brType == "OnDetails"))
                                                    fnConditionValue = "($('#" + inlinesuffix + propertyName + "')" + strVal + " " + operand + " '" + objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?") + "')";
                                                else if(propertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                    fnConditionValue = "('true')";
                                                else
                                                {
                                                    if(strCondValue.ToLower().Trim() == "'null'")
                                                        fnConditionValue = "($('#" + inlinesuffix + propertyName + "')" + strVal + strLowerCase + operand + "''" + strLowerCase + ")";
                                                    else
                                                    {
                                                        var forDetailsPage = "";
                                                        if(datatype.ToLower() == "boolean")
                                                        {
                                                            forDetailsPage = " || $('#dv" + inlinesuffix + propertyName + "').find('select :selected').val()=='" + strCondValue + "'";
                                                        }
                                                        fnConditionValue = "($('#" + inlinesuffix + propertyName + "')" + strVal + strLowerCase + operand + strCondValue + strLowerCase + ")" + forDetailsPage;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                fnConditionValue += " " + logicalconnector + " ($('#" + inlinesuffix + propertyName + "')" + strVal + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                            }
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkHidden.ToString()))
                            {
                                if(!IsAlways)
                                    chkHidden.Append(" });");
                                var noncondFun = "";
                                var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    string fn = string.Empty;
                                    long? BrruleNo = 0;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        if(BrruleNo != objaa.ActionArgumentsID)
                                            fn += objaa.Id.ToString();
                                        BrruleNo = objaa.ActionArgumentsID;
                                        //change for inline association
                                        if(isHiddenGroup)
                                        {
                                            List<string> inlineId = getInlineAssociationsID(entityName);
                                            var indexof = objaa.ParameterName.IndexOf('|');
                                            var actionArg = "";
                                            if(indexof > -1)
                                                actionArg = objaa.ParameterName.Remove(objaa.ParameterName.IndexOf('|'));
                                            if(inlineId.Contains(actionArg))
                                            {
                                                fnProp += "$('#dv" + inlinedivsuffix + actionArg + "').css('display', type);";
                                                fnProp += "$('#li" + inlinedivsuffix + actionArg + "').css('display', type);";
                                                fnProp += "$('#wz" + inlinedivsuffix + actionArg + "').css('display', type);";
                                            }
                                            else
                                            {
                                                fnProp += "$('#dvGroup" + inlinedivsuffix + actionArg + "').css('display', type);";
                                                fnProp += "$('#liGroup" + inlinedivsuffix + actionArg + "').css('display', type=='block'?'':type);";
                                                fnProp += "$('#wzGroup" + inlinedivsuffix + actionArg + "').css('display', type);";
                                            }
                                            List<string> inlinegridId = getInlineGridAssociationsID(entityName);
                                            if(inlineId.Contains(actionArg))
                                            {
                                                fnProp += "$('#dv" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type);";
                                                fnProp += "$('#li" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type);";
                                                fnProp += "$('#wz" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type);";
                                            }
                                            else
                                            {
                                                fnProp += "$('#dvGroup" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type);";
                                                fnProp += "$('#liGroup" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type=='block'?'':type);";
                                                fnProp += "$('#wzGroup" + inlinedivsuffix + actionArg + "_InlineGrid" + "').css('display', type);";
                                            }
                                            System.Text.StringBuilder fnProp1 = new System.Text.StringBuilder();
                                            fnProp1.AppendLine("if (type == 'none' && !firstTime) {");
                                            // fnProp1.AppendLine("ResetToDefault('@Url.Action(\"ResetToDefault\", new { id = Model != null ? Model.Id : 0, groupName = \"" + actionArg + "\" })', '" + inlinedivsuffix + actionArg + "');");
                                            fnProp1.AppendLine("ResetToDefault(ResetBaseUrl.replace('_groupname','" + actionArg + "'), '" + inlinedivsuffix + actionArg + "');");
                                            fnProp1.AppendLine("}");
                                            //var inputValues = fnProp1.AppendLine("$('#dvGroup" + inlinedivsuffix + actionArg + "').find(':input').map(function() {");
                                            //fnProp1.AppendLine(" var ctrltype = $(this).prop('type');");
                                            //fnProp1.AppendLine(" if ((ctrltype == 'checkbox' || ctrltype == 'radio') && this.checked)");
                                            //fnProp1.AppendLine(" { } ");
                                            //fnProp1.AppendLine(" else if (ctrltype != 'button' && ctrltype != 'submit' && ctrltype != 'select-multiple') ");
                                            //fnProp1.AppendLine(" { ");
                                            //fnProp1.AppendLine(" $(this).val(''); ");
                                            //fnProp1.AppendLine(" } ");
                                            //fnProp1.AppendLine("})");
                                            fnProp += fnProp1.ToString();
                                        }
                                        else
                                        {
                                            fnProp += "$('#dv" + inlinedivsuffix + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
                                            fnProp += "$('#li" + inlinedivsuffix + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
                                            fnProp += "$('#wz" + inlinedivsuffix + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
                                            System.Text.StringBuilder fnProp1 = new System.Text.StringBuilder();
                                            var indexof = objaa.ParameterName.IndexOf('|');
                                            var actionArg = "";
                                            if(indexof > -1)
                                                actionArg = objaa.ParameterName.Remove(objaa.ParameterName.IndexOf('|'));
                                            //var inputValues = fnProp1.AppendLine("$('#dvGroup" + inlinedivsuffix + actionArg + "').find(':input').map(function() {");
                                            //fnProp1.AppendLine(" var ctrltype = $(this).prop('type');");
                                            //fnProp1.AppendLine(" if ((ctrltype == 'checkbox' || ctrltype == 'radio') && this.checked)");
                                            //fnProp1.AppendLine(" { } ");
                                            //fnProp1.AppendLine(" else if (ctrltype != 'button' && ctrltype != 'submit' && ctrltype != 'select-multiple') ");
                                            //fnProp1.AppendLine(" { ");
                                            //fnProp1.AppendLine(" $(this).val(''); ");
                                            //fnProp1.AppendLine(" } ");
                                            //fnProp1.AppendLine("})");
                                            fnProp1.AppendLine("if (type == 'none' && !firstTime) {");
                                            // fnProp1.AppendLine("ResetToDefault(ResetBaseUrl.replace('_groupname','" + actionArg + "'), '" + inlinedivsuffix + actionArg + "');");
                                            fnProp1.AppendLine("ResetToDefaultField(ResetBaseUrlField.replace('_fieldname','" + inlinedivsuffix + objaa.ParameterName.Replace('.', '_') + "'), '" + entityName + "');");
                                            fnProp1.AppendLine("}");
                                            fnProp += fnProp1.ToString();
                                        }
                                    }
                                    if(!string.IsNullOrEmpty(fn))
                                    {
                                        fnName = "hiddenProp" + fn;
                                    }
                                    if(IsAlways)
                                    {
                                        noncondFun += fnName + "('none', true);";
                                    }
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        string hdnElse = "";
                                        string showHideAllGroup = "";
                                        //if (!isHiddenGroup)
                                        {
                                            hdnElse = "else { " + fnName + "('block', firstTime); }";
                                        }
                                        if(!IsAlways && !string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            chkHidden.Append("function " + fnCondition.Replace("true", "firstTime") + " { if ( " + fnConditionValue + " ) { " + showHideAllGroup + " " + fnName + "('none', firstTime); } " + hdnElse + " }");
                                            chkFnHidden.Append("function " + fnName + "(type, firstTime) { " + fnProp + " }");
                                        }
                                        else
                                            chkFnHidden.Append("function " + fnName + "(type, firstTime) { " + fnProp + " }");
                                    }
                                }
                                if(IsAlways)
                                    chkHidden.Append(noncondFun + " });");
                            }
                            if(!string.IsNullOrEmpty(chkFnHidden.ToString()))
                            {
                                chkHidden.Append(chkFnHidden.ToString());
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(chkHidden.ToString()))
                    {
                        chkHidden.Append("</script> ");
                        hiddenBRString.Append(chkHidden);
                    }
                }
            }
        }
        //
        if(selectCondition != "" && selectval != "")
        {
            chkHiddenGroup.Append("<script type='text/javascript'> $(document).ready(function () {");
            selectCondition = selectCondition.Remove(selectCondition.Length - 2);
            string finalStr = selectval + "if (!(" + selectCondition + ")){//showalldivs();" + Environment.NewLine + "}});";
            chkHiddenGroup.Append(propChangeEvnetdd.Replace("CONDITION", finalStr));
            chkHiddenGroup.Append("}); ");
            chkHiddenGroup.Append("</script> ");
            selectval = "";
            propChangeEvnetdd = "";
            selectCondition = "";
            AssociationName = "";
            hiddenBRString.Append(chkHiddenGroup);
        }
        //
        //objRuleAction.Dispose();
        //objCondition.Dispose();
        objActionArgs.Dispose();
        return hiddenBRString.ToString();
    }
    
    /// <summary>Check set value user interface rule.</summary>
    ///
    /// <param name="User">          The user.</param>
    /// <param name="entityName">    Name of the entity.</param>
    /// <param name="brType">        Type of the line break.</param>
    /// <param name="idsOfBRType">   Type of the identifiers of line break.</param>
    /// <param name="rbList">        List of rbs.</param>
    /// <param name="inlineAssoList">List of inline assoes.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string checkSetValueUIRule(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder SetValueBRString = new System.Text.StringBuilder();
        //ConditionContext objCondition = new ConditionContext();
        //RuleActionContext objRuleAction = new RuleActionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string datatype = "";
        var paramValuetoday = "";
        if(businessRules != null && businessRules.Count > 0)
        {
            foreach(BusinessRule objBR in businessRules)
            {
                var ActionCount = 1;
                long ActionTypeId = 7;
                //var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id).GetFromCache<IQueryable<RuleAction>, RuleAction>().ToList();
                var objRuleActionList = objBR.ruleaction.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId).ToList();
                if(objRuleActionList.Count() > 0)
                {
                    if(Array.IndexOf(idsOfBRType, objBR.AssociatedBusinessRuleTypeID) < 0)
                        continue;
                    System.Text.StringBuilder chkSetValue = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkFnSetValue = new System.Text.StringBuilder();
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        if(ActionCount > 1)
                            continue;
                        //var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID).GetFromCache<IQueryable<Condition>, Condition>();
                        var objConditionList = objBR.ruleconditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                ActionCount++;
                                if(string.IsNullOrEmpty(chkSetValue.ToString()))
                                {
                                    //chkSetValue.Append("<script type='text/javascript'>$(document).ready(function () {");
                                    chkSetValue.Append("<script type='text/javascript'>$(document).ready(function () { ");
                                    fnCondition = "setvalueUIRule" + objCon.Id.ToString() + "()";
                                    chkSetValue.Append(fnCondition + ";");
                                }
                                datatype = "";
                                datatype = checkPropType(entityName, objCon.PropertyName, false);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                // check for [] then remove it in case of association
                                else if(objCon.Value.StartsWith("[") && objCon.Value.EndsWith("]"))
                                    condValue = objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?").Replace("[", "").Replace("]", "");
                                else
                                    condValue = objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?");
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                var rbcheckcond = false;
                                if(rbList != null && rbList.Contains(objCon.Value))
                                    rbcheckcond = true;
                                if(datatype == "Association")
                                {
                                    //var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    var propertyName = objCon.PropertyName;
                                    if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
                                    {
                                        var parameterSplit = propertyName.Substring(1, propertyName.Length - 2).Split('.');
                                        if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                            propertyName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                        else
                                            propertyName = parameterSplit[0];
                                    }
                                    if(rbList != null && rbList.Contains(propertyName))
                                        rbcheck = true;
                                    if(rbList != null && rbList.Contains(condValue))
                                        rbcheckcond = true;
                                    if(fnCondition == null)
                                        continue;
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                    //check if condvalue contains [] and then add another function statement(for assoication)
                                    if(objCon.Value.StartsWith("[") && objCon.Value.EndsWith("]"))
                                    {
                                        chkSetValue.Append((rbcheckcond ? " $('input:radio[name=" + condValue + "]')" : " $('#" + condValue + "')") + ".change(function() { " + fnCondition + "; });");
                                        condValue = "$('#" + condValue + "').val()";
                                    }
                                    else
                                        condValue = " '" + condValue + "'";
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf(" + condValue + ".toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase()" + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf(" + condValue + ".toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".val().toLowerCase() " + operand + "''.toLowerCase())";
                                            else
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " " + condValue + ".toLowerCase())";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase() " + operand + " " + condValue + ".toLowerCase())";
                                        }
                                    }
                                }
                                else
                                {
                                    string eventName = ".on('keyup keypress blur change',";
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + eventName + " function(event) { " + fnCondition + "; });");
                                    //if condValue contains [] then append another line
                                    //set condvalue here
                                    if(objCon.Value.StartsWith("[") && objCon.Value.EndsWith("]"))
                                    {
                                        chkSetValue.Append((rbcheckcond ? " $('input:radio[name=" + objCon.Value.Replace("[", "").Replace("]", "") + "]')" : " $('#" + objCon.Value.Replace("[", "").Replace("]", "") + "')") + eventName + " function(event) { " + fnCondition + "; });");
                                        //if (objCon.Value.StartsWith("[") && objCon.Value.EndsWith("]"))
                                        condValue = "$('#" + condValue + "').val()";
                                    }
                                    else
                                        condValue = " '" + condValue + "'";
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf(" + condValue + ".toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf(" + condValue + ".toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        string strLowerCase = ".toLowerCase() ";
                                        if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                        {
                                            strLowerCase = "";
                                            // strCondValue = condValue;
                                        }
                                        if(datatype.ToLower() == "boolean")
                                        {
                                            fnConditionValue = "($('#" + objCon.PropertyName + "').is(':checked').toString()" + strLowerCase + operand + condValue + strLowerCase + ")";
                                        }
                                        else if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnEdit")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val() " + operand + " '" + objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?") + "')";
                                            else if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                fnConditionValue = "('true')";
                                            else
                                            {
                                                if(condValue.ToLower().Trim() == "'null'")
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + "''" + strLowerCase + ")";
                                                else if(objCon.PropertyName != "Id")
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + condValue + strLowerCase + ")";
                                            }
                                        }
                                        else
                                        {
                                            if(objCon.PropertyName != "Id")
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + condValue + strLowerCase + ")";
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkSetValue.ToString()))
                            {
                                chkSetValue.Append(" });");
                                //chkSetValue.Append(" }); });");
                                long[] ids = objRuleActionList.Select(item => item.Id).ToArray();
                                var objActionArgsList = objActionArgs.ActionArgss.Include(r => r.actionarguments).Where(aa => ids.Any(x => x == aa.ActionArgumentsID)).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                                //var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    string stvalueTrue = string.Empty;
                                    string fn = string.Empty;
                                    bool IsElseAssoc = true;
                                    bool IsElseValue = true;
                                    bool IsInline = false;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        IsElseAssoc = objaa.actionarguments.IsElseAction;
                                        IsElseValue = objaa.actionarguments.IsElseAction;
                                        string paramValue = objaa.ParameterValue;
                                        string parameterNameInline = objaa.ParameterName;
                                        IsInline = false;
                                        //changes for radiobutton
                                        var rbcheck = false;
                                        if(parameterNameInline.StartsWith("[") && parameterNameInline.EndsWith("]"))
                                        {
                                            var parameterSplit = parameterNameInline.Substring(1, parameterNameInline.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                IsInline = true;
                                            //changes for radiobutton
                                            if(rbList != null && rbList.Contains(parameterSplit[0]))
                                                rbcheck = true;
                                        }
                                        string paramType = checkPropType(entityName, objaa.ParameterName, IsInline);
                                        string paramValueType = checkPropType(entityName, objaa.ParameterValue, IsInline);
                                        paramValuetoday = "";
                                        if(paramValue.ToLower().Trim().Contains("today") || paramType == "DateTime")
                                        {
                                            paramValuetoday = paramValue;
                                            Type type = Type.GetType("GeneratorBase.MVC.Models." + entityName + ", GeneratorBase.MVC.Models");
                                            //code changed to fix date today br issue
                                            var displayformatattributes = type.GetProperty(objaa.ParameterName) == null ? null : type.GetProperty(objaa.ParameterName).CustomAttributes.Where(a => a.ConstructorArguments.Count() > 0);
                                            if(displayformatattributes != null)
                                            {
                                                if(displayformatattributes.Count() > 0)
                                                {
                                                    var displayformatArgument = displayformatattributes.FirstOrDefault(p => p.AttributeType.Name == "CustomDisplayFormat").ConstructorArguments;
                                                    var formatstring = Convert.ToString(displayformatArgument[2].Value);
                                                    formatstring = formatstring.Substring((formatstring.LastIndexOf("0")) + 2, formatstring.Length - 4);
                                                    //paramValue = ApplyRule.EvaluateDateForActionInTarget(paramValue, formatstring, true);
                                                    var formatstr = "\"" + formatstring + "\".replace(\"MM/dd/yyyy\",\"MM/DD/YYYY\")";
                                                    paramValue = ApplyRule.EvaluateDateForActionInTargetOnLoading(formatstr, paramValuetoday, brType, objaa.ParameterName);
                                                    //paramValue = "moment(new Date()).format(" + formatstr + ")";
                                                }
                                                else
                                                    paramValue = ApplyRule.EvaluateDateForActionInTarget(paramValue);
                                            }
                                            fnProp += "$('#" + objaa.ParameterName + "').change();$('#" + objaa.ParameterName + "').val(" + paramValue + ");";
                                            stvalueTrue = "$('#" + objaa.ParameterName + "').attr('setvalue', true);";
                                        }
                                        else if(paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                                        {
                                            var parameterSplit = paramValue.Substring(1, paramValue.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                paramValue = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                            else
                                                paramValue = paramValue.TrimStart('[').TrimEnd(']').Trim();
                                            if(paramValueType == "Association" && paramType == "Association")
                                            {
                                                paramValue = parameterSplit[0];
                                                string objparamValue = "$('#" + paramValue + "').val()";
                                                string objparameterName = objaa.ParameterName;
                                                if(objparameterName.StartsWith("[") && objparameterName.EndsWith("]"))
                                                {
                                                    var objparameterSplit = objparameterName.Substring(1, objparameterName.Length - 2).Split('.');
                                                    if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(objparameterSplit[0].Trim()))
                                                        objparameterName = objparameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + objparameterSplit[1].ToString().Trim();
                                                    else
                                                        objparameterName = objparameterSplit[0];
                                                }
                                                fnProp += "$('#" + objparameterName + "').change();$('#" + objparameterName + "').val(" + objparamValue + ");";
                                                stvalueTrue = "$('#" + objparameterName + "').attr('setvalue', true);";
                                            }
                                            else
                                            {
                                                var parmstr = paramValue.Substring(0, paramValue.Length).Split('.');
                                                if(parmstr.Count() > 1)
                                                {
                                                    fnProp += " var setvalueUrl= $('#seturl" + entityName + "').attr('dataurl');";
                                                    fnProp += " var Idval = 0;";
                                                    fnProp += " if ($('#" + parmstr[0] + " :selected').val().length > 0)";
                                                    fnProp += " Idval = $('#" + parmstr[0] + " :selected').val();";
                                                    fnProp += " var propVal = GetValueOfProperty(Idval, setvalueUrl, '" + parmstr[0] + "', '" + parmstr[1] + "');";
                                                    fnProp += " $('#" + objaa.ParameterName + "').val(propVal);";
                                                    stvalueTrue = "$('#" + objaa.ParameterName + "').attr('setvalue', true);";
                                                    //paramValue = "$('#" + parmstr[0] + " :selected').text()";
                                                    //fnProp += "$('#" + objaa.ParameterName + "').change();$('#" + objaa.ParameterName + "').val(" + paramValue + ");";
                                                }
                                                else
                                                {
                                                    stvalueTrue = "$('#" + objaa.ParameterName + "').attr('setvalue', true);";
                                                    paramValue = "$('#" + paramValue + "').val()";
                                                    fnProp += "$('#" + objaa.ParameterName + "').change();$('#" + objaa.ParameterName + "').val(" + paramValue + ");";
                                                }
                                            }
                                        }
                                        if(paramType == "Association")
                                        {
                                            if(!IsElseAssoc)
                                            {
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                }
                                                if(rbcheck)
                                                {
                                                    fnProp += "$.map($('#dv" + parameterName + " span'), function(elem, index){";
                                                    fnProp += "if ($(elem).text() == '" + paramValue + "') $(elem).prev().attr('checked', true); });";
                                                    fnProp += "$('input:radio[name=" + parameterName + "]').trigger('change');";
                                                    stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                }
                                                else
                                                {
                                                    if(paramValue.ToLower().Trim() == "null")
                                                    {
                                                        fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                        fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).val() == '') return this; }).prop('selected', true);";
                                                        stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                    }
                                                    else
                                                    {
                                                        if(paramValueType == "Association")
                                                        {
                                                            var paramtext = "$('#" + paramValue + " option:selected').text()";
                                                            fnProp += "$('#" + parameterName + "_chosen').find('input').val(" + paramtext + ");";
                                                            fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                            fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).text() == " + paramtext + ") return this; }).prop('selected', true);";
                                                            stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                        }
                                                        else
                                                        {
                                                            fnProp += "$('#" + parameterName + "_chosen').find('input').val('" + paramValue + "');";
                                                            fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                            fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).text() == '" + paramValue + "') return this; }).prop('selected', true);";
                                                            stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                        }
                                                    }
                                                    fnProp += "$('#" + parameterName + "').trigger('click.chosen');";
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:updated');";
                                                    fnProp += "$('#" + parameterName + "').trigger('change');";
                                                }
                                            }
                                            else
                                            {
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                    //changes for radiobutton
                                                    if(rbList != null && rbList.Contains(parameterSplit[0]))
                                                        rbcheck = true;
                                                }
                                                fnProp += "}else {";
                                                if(rbcheck)
                                                {
                                                    fnProp += "$.map($('#dv" + parameterName + " span'), function(elem, index){";
                                                    fnProp += "if ($(elem).text() == '" + paramValue + "') $(elem).prev().attr('checked', true); });";
                                                    fnProp += "$('input:radio[name=" + parameterName + "]').trigger('change');";
                                                    stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                }
                                                else
                                                {
                                                    if(paramValue.ToLower().Trim() == "null")
                                                    {
                                                        fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                        fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).val() == '') return this; }).prop('selected', true);";
                                                        stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                    }
                                                    else
                                                    {
                                                        fnProp += "$('#" + parameterName + "_chosen').find('input').val('" + paramValue + "');";
                                                        fnProp += "$('#" + parameterName + "').trigger('chosen:open');";
                                                        fnProp += "$('#" + parameterName + " option').map(function () { if ($(this).text() == '" + paramValue + "') return this; }).prop('selected', true);";
                                                        stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                    }
                                                    fnProp += "$('#" + parameterName + "').trigger('click.chosen');";
                                                    fnProp += "$('#" + parameterName + "').trigger('chosen:updated');";
                                                    fnProp += "$('#" + parameterName + "').trigger('change');";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(!IsElseValue)
                                            {
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                }
                                                if(paramValue.ToLower().Trim() == "null")
                                                    paramValue = "";
                                                if(paramValue.Contains("$"))
                                                {
                                                    fnProp += "$('#" + parameterName + "').change();  $('#" + parameterName + "').val(" + paramValue + ");";
                                                    stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                }
                                                else
                                                {
                                                    if(paramType.ToLower() == "boolean")
                                                    {
                                                        if(paramValue.ToLower() == "true" || paramValue.ToLower() == "1" || paramValue.ToLower() == "yes")
                                                        {
                                                            fnProp += "$('#" + parameterName + "').change();$('#" + parameterName + "').click();$('#" + parameterName + "').attr('checked','checked');";
                                                            stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                        }
                                                        else
                                                        {
                                                            fnProp += "$('#" + parameterName + "').change();  $('#" + parameterName + "').removeAttr('checked');";
                                                            stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                        }
                                                    }
                                                    else if(!paramValuetoday.ToLower().Trim().Contains("today") && paramValueType != "Association")
                                                    {
                                                        fnProp += "$('#" + parameterName + "').change();  $('#" + parameterName + "').val('" + paramValue + "');";
                                                        stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if(paramValue.ToLower().Trim() == "null")
                                                    paramValue = "";
                                                fnProp += "}else {";
                                                string parameterName = objaa.ParameterName;
                                                if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                                {
                                                    var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                                    if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                        parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                                    else
                                                        parameterName = parameterSplit[0];
                                                    stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                }
                                                if(paramValue.Contains("$") && paramType != "DateTime")
                                                {
                                                    fnProp += " $('#" + parameterName + "').change(); $('#" + parameterName + "').val(" + paramValue + ");";
                                                    stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                }
                                                else
                                                {
                                                    if(paramType.ToLower() == "boolean")
                                                    {
                                                        stvalueTrue = "$('#" + parameterName + "').attr('setvalue', true);";
                                                        if(paramValue.ToLower() == "true" || paramValue.ToLower() == "1" || paramValue.ToLower() == "yes")
                                                            fnProp += "$('#" + parameterName + "').change();$('#" + parameterName + "').click();$('#" + parameterName + "').attr('checked','checked');";
                                                        else
                                                            fnProp += "$('#" + parameterName + "').change();  $('#" + parameterName + "').removeAttr('checked');";
                                                    }
                                                    else if(!paramValue.ToLower().Trim().Contains("today") && paramValueType != "Association" && paramType != "DateTime")
                                                        fnProp += "$('#" + parameterName + "').change();  $('#" + parameterName + "').val('" + paramValue + "');";
                                                }
                                            }
                                        }
                                        fn += objaa.Id.ToString();
                                    }
                                    //if (fnCondition == "")
                                    //    continue;
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "setvalueUIRuleProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        chkSetValue.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + stvalueTrue + "" + fnProp + " } }");
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkFnSetValue.ToString()))
                            {
                                chkSetValue.Append(chkFnSetValue.ToString());
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(chkSetValue.ToString()) && !string.IsNullOrEmpty(datatype))
                    {
                        chkSetValue.Append("</script> ");
                        SetValueBRString.Append(chkSetValue);
                    }
                }
            }
        }
        //objRuleAction.Dispose();
        // objCondition.Dispose();
        objActionArgs.Dispose();
        return SetValueBRString.ToString();
    }
    
    /// <summary>Check set value user interface rule.</summary>
    ///
    /// <param name="User">          The user.</param>
    /// <param name="entityName">    Name of the entity.</param>
    /// <param name="brType">        Type of the line break.</param>
    /// <param name="idsOfBRType">   Type of the identifiers of line break.</param>
    /// <param name="rbList">        List of rbs.</param>
    /// <param name="inlineAssoList">List of inline assoes.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string RestrictDropdownValueRule(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder SetValueBRString = new System.Text.StringBuilder();
        System.Text.StringBuilder SetValueBRStringlast = new System.Text.StringBuilder();
        //ConditionContext objCondition = new ConditionContext();
        //RuleActionContext objRuleAction = new RuleActionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string datatype = "";
        var paramValuetoday = "";
        var propertyNameuni = "";
        long ruletyp = 0;
        string fnConditionValueLast = string.Empty;
        if(businessRules != null && businessRules.Count > 0)
        {
            foreach(BusinessRule objBR in businessRules)
            {
                var ActionCount = 1;
                long ActionTypeId = 17;
                //var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id).GetFromCache<IQueryable<RuleAction>, RuleAction>().ToList();
                var objRuleActionList = objBR.ruleaction.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId).ToList();
                if(objRuleActionList.Count() > 0)
                {
                    ruletyp = 17;
                    if(Array.IndexOf(idsOfBRType, objBR.AssociatedBusinessRuleTypeID) < 0)
                        continue;
                    System.Text.StringBuilder chkSetValue = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkFnSetValue = new System.Text.StringBuilder();
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        if(ActionCount > 1)
                            continue;
                        var objConditionList = objBR.ruleconditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                ActionCount++;
                                if(string.IsNullOrEmpty(chkSetValue.ToString()))
                                {
                                    chkSetValue.Append("<script type='text/javascript'>$(document).ready(function () {");
                                    fnCondition = "RestrictDropdownValueRule" + objCon.Id.ToString() + "()";
                                    chkSetValue.Append(fnCondition + ";");
                                }
                                datatype = checkPropType(entityName, objCon.PropertyName, false);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                else
                                    condValue = objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?");
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                if(datatype == "Association")
                                {
                                    //var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    var propertyName = objCon.PropertyName;
                                    if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
                                    {
                                        var parameterSplit = propertyName.Substring(1, propertyName.Length - 2).Split('.');
                                        if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                            propertyName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                        else
                                            propertyName = parameterSplit[0];
                                    }
                                    if(rbList != null && rbList.Contains(propertyName))
                                        rbcheck = true;
                                    if(fnCondition == null)
                                        continue;
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase()" + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".val().toLowerCase() " + operand + "''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                                if(string.IsNullOrEmpty(propertyNameuni))
                                                {
                                                    fnConditionValueLast = ((rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())") + "||";
                                                    propertyNameuni = propertyName;
                                                }
                                                if(propertyNameuni == propertyName)
                                                {
                                                    fnConditionValueLast += ((rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())") + "||";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string eventName = ".on('blur',";
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + eventName + " function(event) { " + fnCondition + "; });");
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().length == 0)";
                                            else
                                                fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        string strLowerCase = ".toLowerCase() ";
                                        string strCondValue = " '" + condValue + "'";
                                        if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                        {
                                            strLowerCase = "";
                                            strCondValue = condValue;
                                        }
                                        if(datatype.ToLower() == "boolean")
                                        {
                                            fnConditionValue = "($('#" + objCon.PropertyName + "').is(':checked').toString()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                        else if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnEdit")
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val() " + operand + " '" + objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?") + "')";
                                            else if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                fnConditionValue = "('true')";
                                            else
                                            {
                                                if(strCondValue.ToLower().Trim() == "'null'")
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + "''" + strLowerCase + ")";
                                                else
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                            }
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " ($('#" + objCon.PropertyName + "').val()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkSetValue.ToString()))
                            {
                                chkSetValue.Append(" });");
                                long[] ids = objRuleActionList.Select(item => item.Id).ToArray();
                                var objActionArgsList = objActionArgs.ActionArgss.Include(r => r.actionarguments).Where(aa => ids.Any(x => x == aa.ActionArgumentsID)).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                                //var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    //Islah
                                    string fnPropElse = string.Empty;
                                    string fn = string.Empty;
                                    bool IsElseAssoc = true;
                                    bool IsElseValue = true;
                                    bool IsInline = false;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        IsElseAssoc = objaa.actionarguments.IsElseAction;
                                        IsElseValue = objaa.actionarguments.IsElseAction;
                                        string paramValue = objaa.ParameterValue;
                                        string parameterNameInline = objaa.ParameterName;
                                        IsInline = false;
                                        //changes for radiobutton
                                        var rbcheck = false;
                                        if(parameterNameInline.StartsWith("[") && parameterNameInline.EndsWith("]"))
                                        {
                                            var parameterSplit = parameterNameInline.Substring(1, parameterNameInline.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                IsInline = true;
                                            //changes for radiobutton
                                            if(rbList != null && rbList.Contains(parameterSplit[0]))
                                                rbcheck = true;
                                        }
                                        string paramType = checkPropType(entityName, objaa.ParameterName, IsInline);
                                        string paramValueType = checkPropType(entityName, objaa.ParameterValue, IsInline);
                                        if(paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                                        {
                                            var parameterSplit = paramValue.Substring(1, paramValue.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                paramValue = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                            else
                                                paramValue = paramValue.TrimStart('[').TrimEnd(']').Trim();
                                        }
                                        string parameterName = objaa.ParameterName;
                                        if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                        {
                                            var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                            else
                                                parameterName = parameterSplit[0];
                                        }
                                        fnProp += "$('#dv" + parameterName + "ID" + " select').attr('dataurl', $('#dv" + parameterName + "ID" + " select').attr('dataurl')+" + "'&RestrictDropdownVal=" + paramValue + "');";
                                        fnPropElse += "$('#dv" + parameterName + "ID" + " select').attr('dataurl', removeURLQueryParameter($('#dv" + parameterName + "ID" + " select').attr('dataurl'), 'RestrictDropdownVal'));";
                                        fn += objaa.Id.ToString();
                                    }
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "setvalueUIRuleProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        chkSetValue.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + fnPropElse + fnProp + " } }");
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkFnSetValue.ToString()))
                            {
                                chkSetValue.Append(chkFnSetValue.ToString());
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(chkSetValue.ToString()) && !string.IsNullOrEmpty(datatype))
                    {
                        chkSetValue.Append("</script> ");
                        SetValueBRString.Append(chkSetValue);
                    }
                }
            }
            if(propertyNameuni != "" && ruletyp == 17 && fnConditionValueLast != "")
            {
                var fnConditionval = "";
                SetValueBRStringlast.Append("<script type='text/javascript'>$(document).ready(function () { ");
                fnConditionval = "RestrictDropdownValueRuleNull" + propertyNameuni + "()";
                SetValueBRStringlast.Append("$('#" + propertyNameuni + "').change(function() {");
                SetValueBRStringlast.Append(fnConditionval + " });" + " });");
                SetValueBRStringlast.Append("function " + fnConditionval + "{");
                SetValueBRStringlast.Append("var Obval= $('option:selected', '#" + propertyNameuni + "').val();");
                SetValueBRStringlast.Append("if(!(" + fnConditionValueLast.Trim(new Char[] { ' ', '|', '|' }) + ") || Obval=='') {");
                SetValueBRStringlast.Append("$('#dv" + propertyNameuni + " select').attr('dataurl', removeURLQueryParameter($('#dv" + propertyNameuni + " select').attr('dataurl'), 'RestrictDropdownVal'));");
                SetValueBRStringlast.Append(" } } </script> ");
            }
        }
        if(SetValueBRStringlast.Length > 0)
            SetValueBRString.Append(SetValueBRStringlast);
        //objRuleAction.Dispose();
        //objCondition.Dispose();
        objActionArgs.Dispose();
        return SetValueBRString.ToString();
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="User"></param>
    /// <param name="entityName"></param>
    /// <param name="brType"></param>
    /// <param name="idsOfBRType"></param>
    /// <param name="rbList"></param>
    /// <param name="inlineAssoList"></param>
    /// <returns></returns>
    public static string RestrictDropdownValueRuleInLineEdit(IUser User, string entityName, string brType, long[] idsOfBRType, string[] rbList, string[] inlineAssoList)
    {
        var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
        if(businessRules == null || businessRules.Count() == 0) return "";
        System.Text.StringBuilder SetValueBRString = new System.Text.StringBuilder();
        System.Text.StringBuilder SetValueBRStringFun = new System.Text.StringBuilder();
        System.Text.StringBuilder SetValueBRStringlast = new System.Text.StringBuilder();
        //ConditionContext objCondition = new ConditionContext();
        //RuleActionContext objRuleAction = new RuleActionContext();
        ActionArgsContext objActionArgs = new ActionArgsContext();
        string datatype = "";
        var paramValuetoday = "";
        var propertyNameuni = "";
        long ruletyp = 0;
        string fnConditionValueLast = string.Empty;
        SetValueBRString.Append("<script type='text/javascript'>function FillDropInlineAll(clt) {");
        foreach(BusinessRule objBR in businessRules)
        {
            var ActionCount = 1;
            long ActionTypeId = 17;
            //var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id).GetFromCache<IQueryable<RuleAction>, RuleAction>().ToList();
            var objRuleActionList = objBR.ruleaction.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId).ToList();
            if(objRuleActionList.Count() > 0)
            {
                ruletyp = 17;
                if(Array.IndexOf(idsOfBRType, objBR.AssociatedBusinessRuleTypeID) < 0)
                    continue;
                System.Text.StringBuilder chkSetValue1 = new System.Text.StringBuilder();
                foreach(RuleAction objRA in objRuleActionList)
                {
                    //var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID).GetFromCache<IQueryable<Condition>, Condition>();
                    var objConditionList = objBR.ruleconditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                    if(objConditionList.Count() > 0)
                    {
                        string fnCondition = string.Empty;
                        bool morethenonecondition = true;
                        foreach(Condition objCon in objConditionList)
                        {
                            if(morethenonecondition)
                            {
                                fnCondition = "RestrictDropdownValueRule" + objCon.Id.ToString() + "(clt);";
                                chkSetValue1.Append(fnCondition);
                            }
                            if(objConditionList.Count() > 1)
                                morethenonecondition = false;
                        }
                    }
                }
                SetValueBRStringFun.Append(chkSetValue1);
            }
        }
        SetValueBRStringFun.Append("} </script>");
        SetValueBRString.Append(SetValueBRStringFun);
        SetValueBRString.Append("<script type='text/javascript'> function FillDropInline(clt) { FillDropInlineAll(clt); $('#' + clt).change(function() {");
        SetValueBRString.Append("FillDropInlineAll(clt);RestrictDropdownValueRuleNull(clt);");
        SetValueBRString.Append("});} </script>");
        SetValueBRString.Append("<script type='text/javascript'>");
        if(businessRules != null && businessRules.Count > 0)
        {
            foreach(BusinessRule objBR in businessRules)
            {
                var ActionCount = 1;
                long ActionTypeId = 17;
                //var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id).GetFromCache<IQueryable<RuleAction>, RuleAction>().ToList();
                var objRuleActionList = objBR.ruleaction.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId).ToList();
                if(objRuleActionList.Count() > 0)
                {
                    #region ActionListInformation
                    ruletyp = 17;
                    if(Array.IndexOf(idsOfBRType, objBR.AssociatedBusinessRuleTypeID) < 0)
                        continue;
                    System.Text.StringBuilder chkFnSetValue = new System.Text.StringBuilder();
                    System.Text.StringBuilder chkSetValue = new System.Text.StringBuilder();
                    foreach(RuleAction objRA in objRuleActionList)
                    {
                        if(ActionCount > 1)
                            continue;
                        //var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID).GetFromCache<IQueryable<Condition>, Condition>();
                        var objConditionList = objBR.ruleconditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                        if(objConditionList.Count() > 0)
                        {
                            string fnCondition = string.Empty;
                            string fnConditionValue = string.Empty;
                            foreach(Condition objCon in objConditionList)
                            {
                                ActionCount++;
                                if(string.IsNullOrEmpty(chkSetValue.ToString()))
                                {
                                    fnCondition = " RestrictDropdownValueRule" + objCon.Id.ToString() + "(clt)";
                                }
                                datatype = checkPropType(entityName, objCon.PropertyName, false);
                                string operand = checkOperator(objCon.Operator);
                                string propertyAttribute = getPropertyAttribute(entityName, objCon.PropertyName);
                                string logicalconnector = objCon.LogicalConnector.ToLower() == "and" ? "&&" : "||";
                                //check if today is used for datetime property
                                string condValue = "";
                                if(datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                    condValue = DateTime.UtcNow.Date.ToString("MM/dd/yyyy");
                                else
                                    condValue = objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?");
                                var rbcheck = false;
                                if(rbList != null && rbList.Contains(objCon.PropertyName))
                                    rbcheck = true;
                                if(datatype == "Association")
                                {
                                    //var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                    var propertyName = objCon.PropertyName;
                                    if(propertyName.StartsWith("[") && propertyName.EndsWith("]"))
                                    {
                                        var parameterSplit = propertyName.Substring(1, propertyName.Length - 2).Split('.');
                                        if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                            propertyName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                        else
                                            propertyName = parameterSplit[0];
                                    }
                                    if(rbList != null && rbList.Contains(propertyName))
                                        rbcheck = true;
                                    if(fnCondition == null)
                                        continue;
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : "\n"));
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#'+clt)") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#'+clt)") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#'+clt)") + ".val().toLowerCase()" + operand + " ''.toLowerCase())";
                                            else
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#'+clt)") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#'+clt)") + ".val().toLowerCase() " + operand + "''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#'+clt)") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                                if(string.IsNullOrEmpty(propertyNameuni))
                                                {
                                                    fnConditionValueLast = ((rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#'+clt)") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())") + "||";
                                                    propertyNameuni = propertyName;
                                                }
                                                if(propertyNameuni == propertyName)
                                                {
                                                    fnConditionValueLast += ((rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected','#'+clt)") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())") + "||";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "null")
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#'+clt)") + ".val().toLowerCase() " + operand + " ''.toLowerCase())";
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? " " + logicalconnector + " ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : logicalconnector + " ($('option:selected', '#'+clt)") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string eventName = ".on('blur',";
                                    chkSetValue.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : "\n"));
                                    if(operand.Length > 2)
                                    {
                                        if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue = "($('#'+clt).val().length == 0)";
                                            else
                                                fnConditionValue = "($('#'+clt).val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                        else
                                        {
                                            if(condValue.ToLower().Trim() == "'null'")
                                                fnConditionValue += " " + logicalconnector + " ($(''+clt).val().length == 0)";
                                            else
                                                fnConditionValue += " " + logicalconnector + " ($('#'+clt).val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                        }
                                    }
                                    else
                                    {
                                        string strLowerCase = ".toLowerCase() ";
                                        string strCondValue = " '" + condValue + "'";
                                        if(datatype.ToLower() == "decimal" || datatype.ToLower() == "double" || datatype.ToLower() == "int32")
                                        {
                                            strLowerCase = "";
                                            strCondValue = condValue;
                                        }
                                        if(datatype.ToLower() == "boolean")
                                        {
                                            fnConditionValue = "($('#" + objCon.PropertyName + "').is(':checked').toString()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                        else if(string.IsNullOrEmpty(fnConditionValue))
                                        {
                                            if(objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0")
                                            {
                                                fnConditionValue = "($('option:selected', '#'+clt).val() " + operand + " '" + objCon.Value.Replace("&#44;", ",").Replace("&#63;", "?") + "' && #PN#)";
                                                if(string.IsNullOrEmpty(propertyNameuni))
                                                {
                                                    fnConditionValueLast = ((rbcheck ? "" : "($('option:selected', '#'+clt)") + ".val()" + operand + " '" + condValue + "')") + "||";
                                                    propertyNameuni = "$('option:selected', '#'+clt).val()";
                                                }
                                                if(propertyNameuni == "$('option:selected', '#'+clt).val()")
                                                {
                                                    fnConditionValueLast += ((rbcheck ? "" : "($('option:selected','#'+clt)") + ".val() " + operand + " '" + condValue + "')") + "||";
                                                }
                                            }
                                            else
                                            {
                                                if(strCondValue.ToLower().Trim() == "'null'")
                                                    fnConditionValue = "(($('option:selected', '#'+clt).val()" + strLowerCase + operand + "''" + strLowerCase + ")";
                                                else
                                                    fnConditionValue = "(($('option:selected', '#'+clt)" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                            }
                                        }
                                        else
                                        {
                                            fnConditionValue += " " + logicalconnector + " (($('option:selected', '#'+clt).val()" + strLowerCase + operand + strCondValue + strLowerCase + ")";
                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkSetValue.ToString()))
                            {
                                long[] ids = objRuleActionList.Select(item => item.Id).ToArray();
                                var objActionArgsList = objActionArgs.ActionArgss.Include(r => r.actionarguments).Where(aa => ids.Any(x => x == aa.ActionArgumentsID)).GetFromCache<IQueryable<ActionArgs>, ActionArgs>();
                                //var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                if(objActionArgsList.Count() > 0)
                                {
                                    string fnName = string.Empty;
                                    string fnProp = string.Empty;
                                    //Islah
                                    string fnPropElse = string.Empty;
                                    string fn = string.Empty;
                                    bool IsElseAssoc = true;
                                    bool IsElseValue = true;
                                    bool IsInline = false;
                                    foreach(ActionArgs objaa in objActionArgsList)
                                    {
                                        IsElseAssoc = objaa.actionarguments.IsElseAction;
                                        IsElseValue = objaa.actionarguments.IsElseAction;
                                        string paramValue = objaa.ParameterValue;
                                        string parameterNameInline = objaa.ParameterName;
                                        IsInline = false;
                                        //changes for radiobutton
                                        var rbcheck = false;
                                        if(parameterNameInline.StartsWith("[") && parameterNameInline.EndsWith("]"))
                                        {
                                            var parameterSplit = parameterNameInline.Substring(1, parameterNameInline.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                IsInline = true;
                                            //changes for radiobutton
                                            if(rbList != null && rbList.Contains(parameterSplit[0]))
                                                rbcheck = true;
                                        }
                                        string paramType = checkPropType(entityName, objaa.ParameterName, IsInline);
                                        string paramValueType = checkPropType(entityName, objaa.ParameterValue, IsInline);
                                        if(paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                                        {
                                            var parameterSplit = paramValue.Substring(1, paramValue.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                paramValue = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                            else
                                                paramValue = paramValue.TrimStart('[').TrimEnd(']').Trim();
                                        }
                                        string parameterName = objaa.ParameterName;
                                        if(parameterName.StartsWith("[") && parameterName.EndsWith("]"))
                                        {
                                            var parameterSplit = parameterName.Substring(1, parameterName.Length - 2).Split('.');
                                            if(inlineAssoList != null && inlineAssoList.Length > 0 && inlineAssoList.Contains(parameterSplit[0].Trim()))
                                                parameterName = parameterSplit[0].Replace("ID", "").ToLower().Trim() + "_" + parameterSplit[1].ToString().Trim();
                                            else
                                                parameterName = parameterSplit[0];
                                        }
                                        //
                                        fnConditionValue = fnConditionValue.Replace("#PN#", "$(document.getElementById(clt)).attr('controlname')=='" + parameterName + "ID'");
                                        fnProp += "$('#'+clt).attr('dataurl', $('#'+clt).attr('dataurl')+" + "'&RestrictDropdownVal=" + paramValue + "');";
                                        fnPropElse += "$('#'+clt).attr('dataurl', removeURLQueryParameter($('#'+clt).attr('dataurl'), 'RestrictDropdownVal'));";
                                        fn += objaa.Id.ToString();
                                    }
                                    if(!string.IsNullOrEmpty(fn))
                                        fnName = "setvalueUIRuleProp" + fn;
                                    if(!string.IsNullOrEmpty(fnName))
                                    {
                                        chkSetValue.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + fnPropElse + fnProp + " } }");
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(chkFnSetValue.ToString()))
                            {
                                chkSetValue.Append(chkFnSetValue.ToString());
                            }
                        }
                    }
                    SetValueBRString.Append(chkSetValue);
                    #endregion
                }
            }
            SetValueBRStringlast.Append("function RestrictDropdownValueRuleNull(clt){");
            if(propertyNameuni != "" && ruletyp == 17 && fnConditionValueLast != "")
            {
                SetValueBRStringlast.Append("var Obval= $('option:selected', '#'+clt).val();");
                SetValueBRStringlast.Append("if(!(" + fnConditionValueLast.Trim(new Char[] { ' ', '|', '|' }) + ") || Obval=='') {");
                SetValueBRStringlast.Append("$('#'+clt).attr('dataurl', removeURLQueryParameter($('#'+clt).attr('dataurl'), 'RestrictDropdownVal'));");
                SetValueBRStringlast.Append(" }");
            }
            SetValueBRStringlast.Append("}");
        }
        if(SetValueBRStringlast.Length > 0)
            SetValueBRString.Append(SetValueBRStringlast);
        SetValueBRString.Append("</script>");
        //objRuleAction.Dispose();
        //objCondition.Dispose();
        objActionArgs.Dispose();
        return SetValueBRString.ToString();
    }
    
    /// <summary>Apply NoView.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="User">    The logged-in user.</param>
    /// <param name="resource">  The entity name.</param>
    public static void ApplyNoView(object entity, IUser User, string resource)
    {
        if(!User.IsAdmin && User.permissions.Any(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.NoView)))
        {
            var list = User.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && !string.IsNullOrEmpty(p.NoView)).NoView.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach(var item in list)
            {
                var propertyInfo = entity.GetType().GetProperty(item);
                if(propertyInfo == null) continue;
                Type targetType = propertyInfo.PropertyType;
                if(propertyInfo.PropertyType.IsGenericType)
                    targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                object safeValue = BusinessRuleHelper.GetDefault(targetType);
                propertyInfo.SetValue(entity, safeValue, null);
            }
        }
        HiddenPropertiesRule(entity, User.businessrules, resource);
    }
    
    /// <summary>Hidden properties rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    /// <param name="IsEdit"> Request came from edit page.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    
    public static List<string> HiddenPropertiesRule(object entity, List<BusinessRule> BR, string name, bool IsEdit = false)
    {
        List<string> properties = new List<string>();
        //using(var ruleactiondb = new RuleActionContext())
        foreach(var br in BR.Where(p => p.EntityName == name && (p.AssociatedBusinessRuleTypeID == 2 || p.AssociatedBusinessRuleTypeID == 3) && p.ActionTypeID.Contains(6)))
        {
            //Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var listruleactions = br.ruleaction.Where(p => p.associatedactiontype.TypeNo == 6);
            if(listruleactions.Count() == 0) continue;
            var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
            var ruleactions = listruleactions.Where(p => !p.IsElseAction);
            if(!ConditionResult)
                ruleactions = listruleactions.Where(p => p.IsElseAction);
            foreach(var act in ruleactions.Where(ra => ra.associatedactiontype.TypeNo == 6))
            {
                foreach(string propertyName in act.actionarguments.Select(p => p.ParameterName))
                {
                    var propertyInfo = entity.GetType().GetProperty(propertyName);
                    if(propertyInfo == null) continue;
                    Type targetType = propertyInfo.PropertyType;
                    if(propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    object safeValue = BusinessRuleHelper.GetDefault(targetType);
                    //when field is datetime
                    if(targetType.Name == "DateTime")
                        safeValue = null;
                    if(!IsEdit)
                        propertyInfo.SetValue(entity, safeValue, null);
                    properties.Add(propertyName);
                }
            }
            //using(ActionTypeContext atc = new ActionTypeContext())
            // foreach (var act in ruleactions)
            // {
            // GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
            // obj.ActionID = act.Id;
            // obj.BRID = br.Id;
            // var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
            // var typeno = typeobj != null ? typeobj.TypeNo : 0;
            // //if (!RuleDictionaryResult.ContainsKey(typeno))
            // if (typeobj != null)
            // RuleDictionaryResult.Add(typeobj, obj);
            // }
            // if (RuleDictionaryResult.Keys.Select(p => p.TypeNo).Contains(6))
            // {
            // var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(RuleDictionaryResult.Where(p => p.Key.TypeNo == 6).Select(v => v.Value.ActionID).ToList());
            // foreach (string propertyName in Args.Select(p => p.ParameterName))
            // {
            // var propertyInfo = entity.GetType().GetProperty(propertyName);
            // if (propertyInfo == null) continue;
            // Type targetType = propertyInfo.PropertyType;
            // if (propertyInfo.PropertyType.IsGenericType)
            // targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            // object safeValue = BusinessRuleHelper.GetDefault(targetType);
            // //when field is datetime
            // if (targetType.Name == "DateTime")
            // safeValue = null;
            // if (!IsEdit)
            // propertyInfo.SetValue(entity, safeValue, null);
            // properties.Add(propertyName);
            // }
            // }
        }
        return properties;
    }
    /// <summary>Hidden groups rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR"> The line break.</param>
    /// <param name="name"> The name.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    public static List<string> HiddenGroupsRule(object entity, List<BusinessRule> BR, string name)
    {
        List<string> properties = new List<string>();
        var brType = "OnEdit";
        foreach(var br in BR.Where(p => p.EntityName == name && p.ActionTypeID.Contains(12)))
        {
            if(br.AssociatedBusinessRuleTypeID == 1 && brType != "OnCreate")
                continue;
            else if(br.AssociatedBusinessRuleTypeID == 2 && (brType != "OnEdit" && brType != "OnDetails"))
                continue;
            //Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var listruleactions = br.ruleaction.Where(p => p.associatedactiontype.TypeNo == 12);
            if(listruleactions.Count() == 0) continue;
            var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
            var ruleactions = listruleactions.Where(p => !p.IsElseAction);
            if(!ConditionResult)
                ruleactions = listruleactions.Where(p => p.IsElseAction);
            var groupproperty = "";
            foreach(var act in ruleactions.Where(ra => ra.associatedactiontype.TypeNo == 12))
            {
                foreach(string propertyName in act.actionarguments.Select(p => p.ParameterName))
                {
                    var propertyname = propertyName.Split('|').ToList();
                    var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == name).Properties;
                    for(int i = 0; i < modelproperties.Count; i++)
                    {
                        if(modelproperties[i].PropText == propertyname.LastOrDefault() || modelproperties[i].DisplayName == propertyname.LastOrDefault())
                        {
                            groupproperty = modelproperties[i].Name;
                            var propertyInfo = entity.GetType().GetProperty(groupproperty);
                            propertyInfo.SetValue(entity, null);
                            properties.Add(groupproperty);
                        }
                    }
                }
            }
        }
        return properties;
    }
    /// <summary>Hidden verb rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>A List&lt;string&gt;</returns>
    
    public static List<string> HiddenVerbOnGridRule(object entity, List<BusinessRule> BR, string name)
    {
        List<string> properties = new List<string>();
        //using(var ruleactiondb = new RuleActionContext())
        foreach(var br in BR.Where(p => p.EntityName == name && (p.AssociatedBusinessRuleTypeID == 2 || p.AssociatedBusinessRuleTypeID == 3)))
        {
            //Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var listruleactions = br.ruleaction.Where(p => p.associatedactiontype.TypeNo == 16);
            if(listruleactions.Count() == 0) continue;
            var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
            var ruleactions = listruleactions.Where(p => !p.IsElseAction);
            if(!ConditionResult)
                ruleactions = listruleactions.Where(p => p.IsElseAction);
            foreach(var act in ruleactions.Where(ra => ra.associatedactiontype.TypeNo == 16))
            {
                foreach(string parameterValue in act.actionarguments.Select(p => p.ParameterValue))
                {
                    properties.Add(parameterValue);
                }
            }
            // using(ActionTypeContext atc = new ActionTypeContext())
            // foreach (var act in ruleactions)
            // {
            // GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
            // obj.ActionID = act.Id;
            // obj.BRID = br.Id;
            // var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
            // var typeno = typeobj != null ? typeobj.TypeNo : 0;
            // //if (!RuleDictionaryResult.ContainsKey(typeno))
            // if (typeobj != null)
            // RuleDictionaryResult.Add(typeobj, obj);
            // }
            // if (RuleDictionaryResult.Keys.Select(p => p.TypeNo).Contains(16))
            // {
            // var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(RuleDictionaryResult.Where(p => p.Key.TypeNo == 16).Select(v => v.Value.ActionID).ToList());
            // foreach (string parameterValue in Args.Select(p => p.ParameterValue))
            // {
            // properties.Add(parameterValue);
            // }
            // }
        }
        return properties;
    }
    
    /// <summary>Lock business rule.</summary>
    ///
    /// <param name="OModel">The entity row.</param>
    /// <param name="BR">    List of Business rules.</param>
    /// <param name="entityName"> Name of the entity</param>
    ///
    /// <returns>The lock business rules.</returns>
    
    public static Boolean GetLockBusinessRules(dynamic OModel, IUser User, List<BusinessRule> BR, string entityName)
    {
        Boolean RulesApplied = false;
        if(BR != null)
        {
            var BRAll = BR;
            if(BR != null && BR.Count > 0)
            {
                var listruleactions = BR.Where(q => q.ruleaction.Any(p => p.associatedactiontype.TypeNo == 1 || p.associatedactiontype.TypeNo == 11));
                if(listruleactions.Count() == 0) return RulesApplied;
                OModel.setCalculation();
                //var ResultOfBusinessRules = (new ApplicationContext(User)).LockEntityRule((object)OModel, BR, entityName);
                var ResultOfBusinessRules = LockEntityRule((object)OModel, BR, entityName);
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 2);
                if(ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1) || ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                {
                    var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                    foreach(var objBR in BRList)
                    {
                        var FailureMessage = !(string.IsNullOrEmpty(objBR.FailureMessage)) ? objBR.FailureMessage : objBR.RuleName;
                        return RulesApplied = true;
                    }
                }
            }
        }
        return RulesApplied;
    }
    
    /// <summary>Gets property attribute.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="PropName">  Name of the property.</param>
    ///
    /// <returns>The property attribute.</returns>
    
    public static string getPropertyAttribute(string EntityName, string PropName)
    {
        if(PropName == "Id")
            return "long";
        var entityModel = ModelReflector.Entities;
        var EntityInfo = entityModel.FirstOrDefault(p => p.Name == EntityName);
        if(EntityInfo == null) return string.Empty;
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(PropName.StartsWith("[") && PropName.EndsWith("]"))
        {
            PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        EntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                    }
                }
            }
        }
        if(PropInfo == null) return string.Empty;
        string DataTypeAttribute = PropInfo.DataTypeAttribute;
        if(AssociationInfo != null)
        {
            DataTypeAttribute = "Association";
        }
        return DataTypeAttribute;
    }
    
    /// <summary>Check operator.</summary>
    ///
    /// <param name="condition">The condition.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string checkOperator(string condition)
    {
        string opr = string.Empty;
        switch(condition)
        {
        case "=":
            opr = "==";
            break;
        case "Contains":
            opr = "Contains";
            break;
        default:
            opr = condition;
            break;
        }
        return opr;
    }
    
    /// <summary>Check property type.</summary>
    ///
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="PropName">  Name of the property.</param>
    /// <param name="IsInline">  True if is inline, false if not.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string checkPropType(string EntityName, string PropName, bool IsInline)
    {
        if(PropName == "Id")
            return "long";
        var entityModel = ModelReflector.Entities;
        var EntityInfo = entityModel.FirstOrDefault(p => p.Name == EntityName);
        if(EntityInfo == null) return string.Empty;
        var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
        if(PropName.StartsWith("[") && PropName.EndsWith("]"))
        {
            PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
            if(PropName.Contains("."))
            {
                var targetProperties = PropName.Split(".".ToCharArray());
                if(targetProperties.Length > 1)
                {
                    AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                    if(AssociationInfo != null)
                    {
                        EntityInfo = entityModel.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                        PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        if(PropInfo == null) return string.Empty;
                        if(IsInline)
                            AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[1]);
                    }
                }
            }
        }
        if(PropInfo == null) return string.Empty;
        string DataType = PropInfo.DataType;
        if(AssociationInfo != null)
        {
            DataType = "Association";
        }
        return DataType;
    }
    
    /// <summary>Gets a default.</summary>
    ///
    /// <param name="type">The type.</param>
    ///
    /// <returns>The default.</returns>
    
    public static object GetDefault(Type type)
    {
        if(type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }
    /// <summary>
    /// getInlineAssociationsID
    /// </summary>
    /// <param name="Entity">string</param>
    /// <returns></returns>
    public static List<string> getInlineAssociationsID(string Entity)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + Entity + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(Entity).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("getInlineAssociationsOfEntity");
        object[] MethodParams = new object[] { };
        var obj = mc.Invoke(objController, MethodParams);
        List<string> objStr = (List<string>)((System.Web.Mvc.JsonResult)(obj)).Data;
        return objStr;
    }
    /// <summary>
    /// getInlineGridAssociationsID
    /// </summary>
    /// <param name="Entity">string</param>
    /// <returns></returns>
    public static List<string> getInlineGridAssociationsID(string Entity)
    {
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + Entity + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(Entity).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("getInlineGridAssociationsOfEntity");
        object[] MethodParams = new object[] { };
        var obj = mc.Invoke(objController, MethodParams);
        List<string> objStr = (List<string>)((System.Web.Mvc.JsonResult)(obj)).Data;
        return objStr;
    }
    
    /// <summary>Lock business rule on entity.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> LockEntityRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 1 || p.associatedactiontype.TypeNo == 11));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    obj.BRTID = br.AssociatedBusinessRuleTypeID;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Business rule before save on properties.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    /// <param name="state"> The state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ValidateBeforeSavePropertiesRule(object entity, List<BusinessRule> BR, string name, EntityState state)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(state == EntityState.Modified && br.AssociatedBusinessRuleTypeID == 1)
                    continue;
                if(state == EntityState.Added && br.AssociatedBusinessRuleTypeID == 2)
                    continue;
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 10));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    // var typeobj = act.associatedactiontype;//atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeobj = new ActionType();
                    EntityCopy.CopyValuesForSameObjectType1(act.associatedactiontype, typeobj);
                    typeobj.BRid = br.Id;
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Business rule before save on properties.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    /// <param name="state"> The state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ValidateBeforeSavePropertiesRuleConfirmPop(object entity, List<BusinessRule> BR, string name, EntityState state)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(state == EntityState.Modified && br.AssociatedBusinessRuleTypeID == 1)
                    continue;
                if(state == EntityState.Added && br.AssociatedBusinessRuleTypeID == 2)
                    continue;
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 15));
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>UI Alert business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> UIAlertRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 13);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Hidden Verb business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> GetHiddenVerb(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using (var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 16);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using (ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Mandatory properties business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> MandatoryPropertiesRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 2);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    //  var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeobj = new ActionType();
                    EntityCopy.CopyValuesForSameObjectType1(act.associatedactiontype, typeobj);
                    typeobj.BRid = br.Id;
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Reads only properties business rule.</summary>
    ///
    /// <param name="entity">The entity.</param>
    /// <param name="BR">    The line break.</param>
    /// <param name="name">  The name.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ReadOnlyPropertiesRule(object entity, List<BusinessRule> BR, string name)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    obj.BRTID = br.AssociatedBusinessRuleTypeID;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Hidden Properties business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="IsEdit">      The IsEdit.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> HiddenPropertiesRuleDictionary(object entity, List<BusinessRule> BR, string name, bool IsEdit = false)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 6);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Hidden Groups business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="IsEdit">      The IsEdit.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public static Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> HiddenGroupsRule(object entity, List<BusinessRule> BR, string name, bool IsEdit = false)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                var listruleactions = br.ruleaction.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 12);
                if(listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype;// atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
    /// <summary>Set value business rule.</summary>
    ///
    /// <param name="entity">      The entity.</param>
    /// <param name="BR">          The line break.</param>
    /// <param name="name">        The name.</param>
    /// <param name="currentState">The current state.</param>
    ///
    /// <returns>List of action type with other information </returns>
    public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> SetValueRule(object entity, List<BusinessRule> BR, string name, EntityState currentState)
    {
        Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
        try
        {
            //using(var ruleactiondb = new RuleActionContext())
            foreach(var br in BR)
            {
                if(br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(currentState) != "Added")
                    continue;
                else if((br.AssociatedBusinessRuleTypeID == 5 || br.AssociatedBusinessRuleTypeID == 2) && Convert.ToString(currentState) != "Modified")
                    continue;
                else if(br.AssociatedBusinessRuleTypeID == 6 || br.AssociatedBusinessRuleTypeID == 7 || br.AssociatedBusinessRuleTypeID == 8)
                    continue;
                var ruleactions7 = br.ruleaction.Where(p => p.associatedactiontype.TypeNo == 7);
                if(ruleactions7.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = ruleactions7.Where(p => !p.IsElseAction);
                if(!ConditionResult)
                    ruleactions = ruleactions7.Where(p => p.IsElseAction);
                //using(ActionTypeContext atc = new ActionTypeContext())
                foreach(var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    var typeobj = act.associatedactiontype; //atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if(typeobj != null && !RuleDictionaryResult.ContainsKey(typeobj))
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        finally
        {
            RuleDictionaryResult = null;
        }
    }
}
}