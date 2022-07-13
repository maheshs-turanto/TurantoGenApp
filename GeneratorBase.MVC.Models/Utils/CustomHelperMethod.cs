using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeneratorBase.MVC.DynamicQueryable;
namespace GeneratorBase.MVC.Models
{
/// <summary>Cutom Helper methods class.</summary>
public class CustomHelperMethod
{
    // (Do not delete) class
    public static string sortDropDown(string entityName)
    {
        //if (entityName == "T_Month")
        //{
        //    return "T_MonthNumber";
        //}
        return string.Empty;
    }
    public static string sortMultiSelectDropDown(string entityName)
    {
        //if (entityName == "T_Month")
        //{
        //    return "T_MonthNumber";
        //}
        return string.Empty;
    }
    public static bool ShowHideAction(string entityName)
    {
        // custom code
        //var entName = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
        //return entName != null ? true : false;
        //
        return false;
    }
    public static string HideGroupsForFLSHiddenBR(System.Collections.Generic.List<string> NonViewableProperties, System.Collections.Generic.List<string> hiddenproperties,
            System.Collections.Generic.IEnumerable<System.Linq.IGrouping<string, ModelReflector.Property>> modelpropertiesbygroup, string entityName, IUser User, List<string> groupcontaininginlinegrid = null)
    {
        string lstHiddenGroup = string.Empty;
        foreach(var grp in modelpropertiesbygroup)
        {
            bool hideGroup = false;
            if(groupcontaininginlinegrid != null && groupcontaininginlinegrid.Contains(grp.Key)) continue;
            var proplistbygroup = grp.Select(p => p.Name);
            var propcount = NonViewableProperties.Intersect(proplistbygroup).Count();
            //foreach (var prop in grp)
            //{
            //    hideGroup = NonViewableProperties.Contains(prop.Name) || hiddenproperties.Contains(prop.Name) ? true : false;
            //}
            if(proplistbygroup.Count() == propcount)
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[;\\\\/:*?#$%!~^,@&()\"<>|&'\\[\\]]");
                var sourcenameOutput = r.Replace(grp.Key, " ");
                sourcenameOutput = sourcenameOutput.Replace(" ", "");
                sourcenameOutput = sourcenameOutput.Replace(".", "");
                sourcenameOutput = sourcenameOutput.Replace("+", "");
                sourcenameOutput = sourcenameOutput.Replace("-", "");
                lstHiddenGroup += "dvGroup" + entityName + sourcenameOutput + ",";
            }
        }
        lstHiddenGroup = lstHiddenGroup.Trim(',');
        return lstHiddenGroup;
    }
    
}
}