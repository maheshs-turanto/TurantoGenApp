using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
namespace GeneratorBase.MVC.Models
{
/// <summary>An excel import helper.</summary>
public static class ExcelExportHelper
{

    public static void SetColumnsOrder(this DataTable table, params String[] columnNames)
    {
        int columnIndex = 0;
        foreach(var columnName in columnNames)
        {
            var column = table.Columns[columnName];
            if(column != null)
            {
                column.SetOrdinal(columnIndex);
                columnIndex++;
            }
        }
    }
    public static void RemoveColumns(this DataTable table, params String[] columnNames)
    {
        foreach(var columnName in columnNames)
        {
            var column = table.Columns[columnName];
            if(column != null)
            {
                table.Columns.Remove(columnName);
            }
        }
        table.AcceptChanges();
    }
    
    public static DataTable ToDataTable<T>(IUser user, List<T> items, string entityName)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);
        var association = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName).Associations;
        //Get all the properties
        var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => user.CanView(entityName, prop.Name) && prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(false) != null && prop.PropertyType.Name != "ICollection`1" && prop.Name != "ExportDataLogId" && prop.Name != "IsDeleted" && prop.Name != "DeleteDateTime" && prop.Name != "TenantId" && prop.Name != "ConcurrencyKey" && prop.Name != "Id" && prop.Name != "m_Timezone" && prop.Name != "m_Timezonecustom" && prop.Name != "DisplayValue").ToArray();
        foreach(PropertyInfo prop in Props)
        {
            var attr = prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(false);
            //Defining type of data column gives proper data table
            var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
            //Setting column names as Property names
            //ModelReflector.Entities.FirstOrDefault().Properties.Where(prop => prop.Name).FirstOrDefault().DisplayName;
            var asso = association.FirstOrDefault(p => p.AssociationProperty == prop.Name);
            if(asso != null)
            {
                type = typeof(string);
            }
            if(dataTable.Columns[attr.DisplayName] == null)
                dataTable.Columns.Add(attr.DisplayName, type);
            else
                dataTable.Columns.Add(prop.Name, type);
        }
        foreach(T item in items)
        {
            var values = new object[Props.Length];
            for(int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                var attr = Props[i].CustomAttributes.Where(a => a.AttributeType.Name == "PropertyTypeForEntity").FirstOrDefault();
                if(attr != null)
                {
                    var tempCsd = attr.ConstructorArguments.Where(arg => arg.Value != null);
                    var tempcata = tempCsd.Where(arg => arg.Value.ToString() == "Encrypted");
                    if(tempcata.Count() > 0)
                    {
                        values[i] = "***";
                    }
                    else
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                }
                else
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                var asso = association.FirstOrDefault(p => p.AssociationProperty == Props[i].Name);
                if(asso != null && values[i] != null)
                {
                    if(asso.Target == "IdentityUser")
                        values[i] = EntityComparer.GetUserNameForAssociation(Convert.ToString(values[i]));
                    else
                        values[i] = GetAssociationDisplayValue(item, Props[i].Name.TrimEnd("ID".ToCharArray()).ToLower()); //EntityComparer.GetDisplayValueForAssociation(asso.Target, Convert.ToString(values[i]));
                }
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }
    public static DataTable ToDataTable<T>(IUser user, List<T> items, string entityname, string parentname, string associationname, string BasePath = null, string folderPath = null, bool IsSelf = false, bool IsSoftDelete = false)
    {
        if(entityname != null)
            entityname = entityname.Trim();
        if(parentname != null)
            parentname = parentname.Trim();
        if(associationname != null)
            associationname = associationname.Trim();
        var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityname);
        var parentEntity = ModelReflector.Entities.FirstOrDefault(p => p.Name == parentname);
        var entitydp = entity.DisplayName.Length > 31 ? entity.DisplayName.Substring(0, 31) : entity.DisplayName;
        var ParentChild = associationname != null ? entity.Associations.Where(wh => wh.Target == parentname && wh.Name.ToLower() == associationname.Replace("Self_", "").Trim()).FirstOrDefault() : null;
        DataTable dataTable = new DataTable();
        if(IsSelf)
            dataTable.TableName = ParentChild != null ? (ParentChild.DisplayName.Length > 31 ? ParentChild.DisplayName.Substring(0, 31) : ParentChild.DisplayName) : entitydp;
        else
            dataTable.TableName = entitydp;
        var association = entity.Associations;
        List<string> lstHiddenProp = new List<string>();
        //Get all the properties
        if(items.Count > 0)
        {
            var Props = items.FirstOrDefault().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => user.CanView(entityname, prop.Name) && prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(false) != null && prop.Name != "TenantId" && prop.Name != "ConcurrencyKey" && prop.Name != "Id" && prop.Name != "m_Timezone" && prop.Name != "m_Timezonecustom" && prop.Name != "DisplayValue" && prop.Name != "ExportDataLogId" && prop.Name != "IsDeleted" && prop.Name != "DeleteDateTime").ToArray();
            var docProps = entity.Properties.Where(p => !string.IsNullOrEmpty(p.PropCheck) && (p.PropCheck == "Document" || p.PropCheck == "Image"));
            foreach(PropertyInfo prop in Props)
            {
                var attr = prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>(false);
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                var asso = association.FirstOrDefault(p => p.AssociationProperty == prop.Name);
                if(asso != null)
                {
                    type = typeof(string);
                }
                if(docProps.Count() > 0 && docProps.Where(wh => wh.Name == prop.Name).Count() > 0)
                    dataTable.Columns.Add(attr.DisplayName == null ? attr.DisplayName : prop.Name, typeof(string));
                else if(dataTable.Columns[attr.DisplayName] == null)
                    dataTable.Columns.Add(attr.DisplayName, type);
                else
                    dataTable.Columns.Add(prop.Name, type);
            }
            foreach(var item in items)
            {
                lstHiddenProp.AddRange((item as Entity).ApplyHiddenRule(user.businessrules, entityname));
                var values = new object[Props.Length];
                for(int i = 0; i < Props.Length; i++)
                {
                    if(docProps.Count() > 0 && docProps.Where(wh => wh.Name == Props[i].Name).Count() > 0)
                    {
                        var itemval = Props[i].GetValue(item, null);
                        if(itemval != null)
                        {
                            var Idval = long.Parse(itemval.ToString());
                            var dbcontxt = (new ApplicationContext(user));
                            var doc = dbcontxt.Documents.Where(d => d.Id == Idval).FirstOrDefault();
                            if(doc == null) continue;
                            var DPParent = string.Empty;
                            if(ParentChild != null)
                            {
                                var ParentId = Props.FirstOrDefault(p => p.Name == ParentChild.AssociationProperty).GetValue(item, null);
                                //IEnumerable<dynamic> query = typeof(ApplicationContext).GetProperties().FirstOrDefault(p => p.Name == ParentChild.Target + "s").GetValue(dbcontxt, null) as IEnumerable<dynamic>;
                                //var selectQuery = (IQueryable<dynamic>)query;
                                //string whCondition = "Id =" + ParentId;
                                //selectQuery = (IQueryable<dynamic>)selectQuery.AsQueryable().Where(whCondition);
                                DPParent = ParentId.ToString(); // (selectQuery).ToList().FirstOrDefault().DisplayValue;
                            }
                            byte[] filedata = null;
                            var filename = doc.FileName;
                            if(doc.FileType.ToLower() == "onedrive")
                            {
                                filename = doc.DocumentName;
                                var OfficeAccessSession = CommonFunction.Instance.OneDrive(user);
                                string AccessToken = string.Empty;
                                if(string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
                                    AccessToken = OfficeAccessSession.GetOneDriveToken().GetAwaiter().GetResult();
                                if(AccessToken != null)
                                {
                                    filedata = OfficeAccessSession.DownloadFile(doc.FileName).GetAwaiter().GetResult();
                                }
                            }
                            else if(doc.FileType.ToLower() == "file")
                            {
                                filename = doc.DocumentName;
                                string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + doc.FileName;
                                filedata = System.IO.File.ReadAllBytes(filepath);
                            }
                            else
                            {
                                filedata = doc.Byte;
                            }
                            var dirPath = System.IO.Path.Combine(folderPath, entity.DisplayName, Convert.ToString(DPParent).Replace("/", "_"));
                            if(!System.IO.Directory.Exists(dirPath))
                            {
                                System.IO.Directory.CreateDirectory(dirPath);
                            }
                            System.IO.File.WriteAllBytes(dirPath + "/" + filename, filedata);
                            values[i] = "=HYPERLINK" + System.IO.Path.Combine(dirPath.Replace(BasePath, "").Trim(), filename) + ", " + filename + "";
                            if(IsSoftDelete && doc != null)
                            {
                                doc.IsDeleted = true;
                                dbcontxt.Entry(doc).State = EntityState.Modified;
                                dbcontxt.DirectSaveChanges();
                            }
                        }
                        else
                            values[i] = Props[i].GetValue(item, null);
                    }
                    else
                    {
                        var cellvalue = Props[i].GetValue(item, null);
                        var asso = association.FirstOrDefault(p => p.AssociationProperty == Props[i].Name);
                        if(asso != null && cellvalue != null)
                        {
                            if(asso.Target == "IdentityUser")
                                values[i] = EntityComparer.GetUserNameForAssociation(Convert.ToString(values[i]));
                            else
                                values[i] = GetAssociationDisplayValue(item, Props[i].Name.TrimEnd("ID".ToCharArray()).ToLower()); //EntityComparer.GetDisplayValueForAssociation(asso.Target, Convert.ToString(values[i]));
                        }
                        else
                            values[i] = cellvalue != null && cellvalue.ToString().Length > 32700 ? "Cell value is too long to display. Please refer record." : cellvalue;
                    }
                }
                dataTable.Rows.Add(values);
            }
        }
        if(lstHiddenProp.Count() > 0 && dataTable.Rows.Count > 0)
        {
            lstHiddenProp = lstHiddenProp.Distinct().ToList();
            var modelproperties = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityname).Properties;
            for(int i = 0; i < lstHiddenProp.Count; i++)
            {
                lstHiddenProp[i] = modelproperties.FirstOrDefault(q => q.Name == lstHiddenProp[i]).DisplayName;
            }
            dataTable.RemoveColumns((lstHiddenProp.Count > 0) ? lstHiddenProp.ToArray() : new string[] { });
        }
        return dataTable;
    }
    public static string GetAssociationDisplayValue(object item, string PropName)
    {
        var associationval = (Entity)item.GetType().GetProperty(PropName).GetValue(item, null);
        if(associationval != null)
            return associationval.DisplayValue;
        else
            return null;
    }
}
}