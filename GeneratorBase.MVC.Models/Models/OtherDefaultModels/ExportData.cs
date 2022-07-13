using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorBase.MVC.Models
{
public class ExportData
{
    public string EntityName
    {
        get;
        set;
    }
    public string DisplayName
    {
        get;
        set;
    }
    public string AssociationName
    {
        get;
        set;
    }
    public string AssociationValue
    {
        get;
        set;
    }
    
    public bool IsNested
    {
        get;
        set;
    }
    public bool IsSelf
    {
        get;
        set;
    }
    public string FilterQuery
    {
        get;
        set;
    }
    public bool IsSelected
    {
        get;
        set;
    }
}

public class ExportDataKey
{
    public long Id
    {
        get;
        set;
    }
    public string ParentEntity
    {
        get;
        set;
    }
    public string ChildEntity
    {
        get;
        set;
    }
    public string AssociationName
    {
        get;
        set;
    }
    public bool IsNested
    {
        get;
        set;
    }
    public List<string> Hierarchy
    {
        get;
        set;
    }
}

public class ExportDataDeleteSet
{
    public long Id
    {
        get;
        set;
    }
    public string EntityName
    {
        get;
        set;
    }
    public IQueryable Query
    {
        get;
        set;
    }
}
}
