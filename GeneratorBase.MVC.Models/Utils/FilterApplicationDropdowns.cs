using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A filter application dropdowns.</summary>
public class FilterApplicationDropdowns
{
    /// <summary>Enumerates filter dropdown in this collection.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">      The user.</param>
    /// <param name="list">      The list.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="caller">    The caller.</param>
    /// <param name="RestrictDropdownVal">    The RestrictDropdownVal.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process filter dropdown in this
    /// collection.</returns>
    
    public IQueryable<T> FilterDropdown<T>(IUser User, IQueryable<T> list, string EntityName, string caller, string RestrictDropdownVal = "")
    {
        IQueryable<T> rejectedList = list;
        bool flag = false;
        bool flagIsElseAction = false;
        Expression<Func<T, bool>> finalLamda = null;
        if(!string.IsNullOrEmpty(RestrictDropdownVal))
        {
            var query = rejectedList;
            Type[] exprArgTypes = { query.ElementType };
            string[] valarr = RestrictDropdownVal.Split(',');
            var counter = 0;
            int CountCond = valarr.Count();
            foreach(string condval in valarr)
            {
                counter++;
                string val = condval.Trim();
                PropertyInfo[] properties = (typeof(T)).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == "DisplayValue");
                object PropValue = val;// filterCriteria.PropertyValue;
                string propToWhere = "DisplayValue";// filterCriteria.DropdownProperty;
                ParameterExpression p1 = Expression.Parameter(typeof(T), "p");
                MemberExpression member = Expression.PropertyOrField(p1, propToWhere);
                Type targetType = Property.PropertyType;
                dynamic PropertyValue = null;
                if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if(String.IsNullOrEmpty(Convert.ToString(PropValue)))
                        PropertyValue = null;
                    else
                        PropertyValue = Convert.ChangeType(PropValue, targetType.GetGenericArguments()[0]);
                }
                else
                {
                    PropertyValue = Convert.ChangeType(PropValue, targetType);
                }
                Expression<Func<T, bool>> expr1 = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                if(finalLamda == null)
                    finalLamda = expr1;
                else
                    finalLamda = Utility.OrElse<T>(finalLamda, expr1);
                if(CountCond == counter)
                {
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, finalLamda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    //var finalList = ((IQueryable<T>)q);
                    rejectedList = rejectedList.Except((IQueryable<T>)q);
                    flag = true;
                }
            }
        }
        var br = User.businessrules.Where(p => p.EntityName == EntityName);
        if(br != null && br.Count() > 0)
        {
            RuleActionContext ruleactiondb = new RuleActionContext();
            ActionArgsContext actionargs = new ActionArgsContext();
            ConditionContext ruleconditionsdb = new ConditionContext();
            var modelentities = ModelReflector.Entities;
            foreach(var _rule in br)
            {
                foreach(var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == _rule.Id && p.AssociatedActionTypeID == 5).GetFromCache<IQueryable<RuleAction>, RuleAction>())
                {
                    var arg = actionargs.ActionArgss.GetFromCache<IQueryable<ActionArgs>, ActionArgs>().FirstOrDefault(p => p.ActionArgumentsID == act.Id && (p.ParameterName == caller || p.ParameterName == "All"));
                    int CountCond = ruleconditionsdb.Conditions.Where(p => p.RuleConditionsID == _rule.Id).GetFromCache<IQueryable<Condition>, Condition>().Count();
                    int counter = 0;
                    if(arg != null)
                        foreach(var condition in ruleconditionsdb.Conditions.Where(p => p.RuleConditionsID == _rule.Id).GetFromCache<IQueryable<Condition>, Condition>())
                        {
                            var PropName = condition.PropertyName;
                            var EntityInfo = modelentities.FirstOrDefault(p => p.Name == EntityName);
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
                                            EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                            PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                                            PropName = targetProperties[0];
                                        }
                                    }
                                }
                            }
                            string DataType = PropInfo.DataType;
                            PropertyInfo[] properties = (typeof(T)).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                            var Property = properties.FirstOrDefault(p => p.Name == PropName);
                            counter++;
                            IQueryable query = list;
                            Type[] exprArgTypes = { query.ElementType };
                            if(AssociationInfo != null)
                            {
                                //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller, GeneratorBase.MVC.Controllers");
                                Type controller = new CreateControllerType(AssociationInfo.Target).controllerType;
                                using(var objController = (IDisposable)Activator.CreateInstance(controller, null))
                                {
                                    System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromPropertyValue");
                                    object[] MethodParams = new object[] { PropInfo.Name, condition.Value };
                                    var obj = mc.Invoke(objController, MethodParams);
                                    object PropValue = obj;
                                    string propToWhere = condition.PropertyName;// filterCriteria.DropdownProperty;
                                    ParameterExpression p1 = Expression.Parameter(typeof(T), "p");
                                    MemberExpression member = Expression.PropertyOrField(p1, PropName);
                                    Type targetType = typeof(System.Int64?);
                                    dynamic PropertyValue = PropValue;//Convert.ChangeType(PropValue, targetType);
                                    Expression<Func<T, bool>> expr1 = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    if(condition.Operator == ">")
                                    {
                                        expr1 = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if(condition.Operator == "<")
                                    {
                                        expr1 = Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if(condition.Operator == "<=")
                                    {
                                        expr1 = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if(condition.Operator == ">=")
                                    {
                                        expr1 = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if(condition.Operator == "!=")
                                    {
                                        expr1 = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if(finalLamda == null)
                                    {
                                        finalLamda = expr1;
                                    }
                                    else
                                    {
                                        if(condition.LogicalConnector.ToLower() == "and")
                                        {
                                            finalLamda = Utility.AndAlso<T>(finalLamda, expr1);
                                            //temp = expr1;
                                        }
                                        else
                                        {
                                            finalLamda = Utility.OrElse<T>(finalLamda, expr1);
                                            //temp = expr1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                object PropValue = condition.Value;// filterCriteria.PropertyValue;
                                string propToWhere = condition.PropertyName;// filterCriteria.DropdownProperty;
                                ParameterExpression p1 = Expression.Parameter(typeof(T), "p");
                                MemberExpression member = Expression.PropertyOrField(p1, propToWhere);
                                Type targetType = Property.PropertyType;
                                dynamic PropertyValue = null;
                                if(targetType.GetGenericArguments().Count() > 0)
                                    if(targetType.GetGenericArguments()[0].Name == "DateTime" && condition.Value.ToLower().Contains("today"))
                                        PropValue = ApplyRule.EvaluateDateTime("", condition.Value);
                                if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    if(String.IsNullOrEmpty(Convert.ToString(PropValue)))
                                        PropertyValue = null;
                                    else
                                        PropertyValue = Convert.ChangeType(PropValue, targetType.GetGenericArguments()[0]);
                                }
                                else
                                {
                                    PropertyValue = Convert.ChangeType(PropValue, targetType);
                                }
                                Expression<Func<T, bool>> expr1 = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                if(condition.Operator == ">")
                                {
                                    expr1 = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                }
                                if(condition.Operator == "<")
                                {
                                    expr1 = Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                }
                                if(condition.Operator == "<=")
                                {
                                    expr1 = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                }
                                if(condition.Operator == ">=")
                                {
                                    expr1 = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                }
                                if(condition.Operator == "!=")
                                {
                                    expr1 = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                }
                                if(finalLamda == null)
                                {
                                    finalLamda = expr1;
                                }
                                else
                                {
                                    if(condition.LogicalConnector.ToLower() == "and")
                                    {
                                        finalLamda = Utility.AndAlso<T>(finalLamda, expr1);
                                        //temp = expr1;
                                    }
                                    else
                                    {
                                        finalLamda = Utility.OrElse<T>(finalLamda, expr1);
                                        //temp = expr1;
                                    }
                                }
                            }
                            if(CountCond == counter)
                            {
                                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, finalLamda);
                                IQueryable q = query.Provider.CreateQuery(methodCall);
                                //finalList = ((IQueryable<T>)q);
                                rejectedList = rejectedList.Except((IQueryable<T>)q);
                                flag = true;
                            }
                        }
                    if(act.IsElseAction)
                        flagIsElseAction = true;
                }
            }
            ruleactiondb.Dispose();
            actionargs.Dispose();
            ruleconditionsdb.Dispose();
        }
        if(flag)
        {
            if(flagIsElseAction)
                return rejectedList;
            return list.Except(rejectedList);
        }
        else
            return list;
    }
    
    /// <summary>Enumerates filter dropdown in this collection.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="User">      The user.</param>
    /// <param name="list">      The list.</param>
    /// <param name="EntityName">Name of the entity.</param>
    /// <param name="caller">    The caller.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process filter dropdown in this
    /// collection.</returns>
    
    public IEnumerable<T> FilterDropdown<T>(IUser User, IEnumerable<T> list, string EntityName, string caller)
    {
        IEnumerable<T> rejectedList = list;
        return list;
    }
}
}