using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
public class VerbGroupIndexArgsOption : IndexArgsOption
{




    public string DisplayOrderFrom
    {
        get;
        set;
    } public string DisplayOrderTo
    {
        get;
        set;
    } public string Flag1
    {
        get;
        set;
    } public string GroupIdFrom
    {
        get;
        set;
    } public string GroupIdTo
    {
        get;
        set;
    }
    
    
    public string FsearchId
    {
        get;
        set;
    }
}
public class VerbGroupIndexViewModel : VerbGroupIndexArgsOption
{
    public IPagedList<VerbGroup> list
    {
        get;
        set;
    }
    public VerbGroupIndexViewModel(IUser User, VerbGroupIndexArgsOption args)
    {
        args.EntityName = "VerbGroup";
        if(string.IsNullOrEmpty(args.IsAsc) && !string.IsNullOrEmpty(args.sortBy))
        {
            args.IsAsc = "ASC";
        }
        args.CurrentSort = args.sortBy;
        if(!string.IsNullOrEmpty(args.viewtype))
        {
            args.viewtype = args.viewtype.Replace("?IsAddPop=true", "");
        }
        if(args.searchString != null)
            args.page = null;
        else
            args.searchString = args.currentFilter;
        if(args.parent != null && args.parent == "Home")
        {
            args.HomeVal = args.searchString;
        }
        if(string.IsNullOrEmpty(args.viewtype))
            args.viewtype = "IndexPartial";
        if(args.IsdrivedTab != null)
            args.IsdrivedTab = true;
        var templatename = EntityDefaultPageHelper.GetTemplatesForList(User, "VerbGroup", args.viewtype);
        if((templatename != null && args.viewtype != null) && templatename != args.viewtype && args.BuiltInPage == false)
            args.viewtype = templatename;
        args.TemplatesName = args.viewtype;
        args.currentFilter = args.searchString;
        args.page = args.page ?? 1;
        if(!string.IsNullOrEmpty(args.searchString) && args.FSFilter == null)
            args.FSFilter = "Fsearch";
        if(!string.IsNullOrEmpty(args.SortOrder))
        {
            args.SearchResult += " \r\n Sort Order = ";
            var sortString = "";
            var sortProps = args.SortOrder.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == args.EntityName);
            foreach(var prop in sortProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                if(prop.Contains("."))
                {
                    sortString += prop + ",";
                    continue;
                }
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    sortString += asso.DisplayName + ">";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    sortString += propInfo.DisplayName + ">";
                }
            }
            args.SearchResult += sortString.TrimEnd(">".ToCharArray());
        }
        if(!string.IsNullOrEmpty(args.GroupByColumn))
        {
            args.SearchResult += " \r\n Group By = ";
            var groupbyString = "";
            var GroupProps = args.GroupByColumn.Split(",".ToCharArray());
            var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == args.EntityName);
            foreach(var prop in GroupProps)
            {
                if(string.IsNullOrEmpty(prop)) continue;
                var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                if(asso != null)
                {
                    groupbyString += asso.DisplayName + " > ";
                }
                else
                {
                    var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                    groupbyString += propInfo.DisplayName + " > ";
                }
            }
            args.SearchResult += groupbyString.TrimEnd(" > ".ToCharArray());
        }
        SetProperties(this, args);
    }
}
}
