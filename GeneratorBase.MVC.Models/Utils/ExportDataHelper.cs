using GeneratorBase.MVC.DynamicQueryable;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GeneratorBase.MVC.Models
{
public static class ExportDataHelper
{
    public static StringBuilder ReadmeLog(DataSet ds, string FileName)
    {
        var log = new StringBuilder();
        log.AppendLine(FileName);
        foreach(DataTable tbl in ds.Tables)
        {
            log.AppendLine(tbl.TableName + " - " + tbl.Rows.Count.ToString());
        }
        log.AppendLine("------------------------------------------------------------------------------------------");
        return log;
    }
    public static string conditioncriteria(long Id, string entity)
    {
        StringBuilder whereCondition = new StringBuilder();
        var conditions = (new ConditionContext()).Conditions.Where(p => p.ExportDetailConditionsID == Id).ToList();
        int iCnt = 1;
        foreach(var cond in conditions)
        {
            var PropertyName = SearchHelper.GetPropertyDP(entity, cond.PropertyName);
            var Operator = cond.Operator;
            var Value = cond.Value;
            var LogicalConnector = cond.LogicalConnector;
            var type = string.Empty;
            if(iCnt == conditions.Count())
                LogicalConnector = string.Empty;
            whereCondition.Append(PropertyName + " " + Operator + " " + Value + (!string.IsNullOrEmpty(LogicalConnector) ? " " + LogicalConnector + " " : string.Empty));
            whereCondition.Replace("[", "").Replace("]", "");
            iCnt++;
        }
        return whereCondition.ToString();
    }
    
    public static List<ExportDataKey> KeysList(List<T_ExportDataDetails> exportDataDetails)
    {
        var KeyData = new List<ExportDataKey>();
        foreach(var item in exportDataDetails)
        {
            var objdata = new ExportDataKey();
            objdata.Id = item.Id;
            objdata.ParentEntity = item.T_ParentEntity.Trim();
            objdata.ChildEntity = item.T_ChildEntity.Trim();
            objdata.AssociationName = item.T_AssociationName.Trim();
            objdata.IsNested = item.T_IsNested.Value;
            objdata.Hierarchy = item.T_Hierarchy.Split('?').Where(wh => !(string.IsNullOrEmpty(wh))).ToList();
            KeyData.Add(objdata);
        }
        return KeyData;
    }
    
    public static void SaveExcelFile(DataSet ds, T_ExportDataConfiguration objExportDataConfiguration, Dictionary<string, string> lstcondition, string UserName, string childfolderPath, string fileName, string EntityName, string RecordId, string RecordDisplayValue, string Notes,string Author)
    {
        using(ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            wb.Properties.Author = Author;
            wb.Properties.Title = EntityName;
            wb.Properties.Subject = EntityName + " data";
            wb.Properties.Comments = "Export Excel";
            wb.Properties.LastModifiedBy = UserName;
            wb.Worksheets.Add(ds);
            foreach(var wsitem in wb.Worksheets)
            {
                foreach(var dtrow in wsitem.Cells().Where(s => s.Value.ToString().StartsWith("=HYPERLINK")))
                {
                    var data = dtrow.Value.ToString().Replace("=HYPERLINK", "").Split(',');
                    dtrow.Value = data[1].ToString();
                    dtrow.Hyperlink = new ClosedXML.Excel.XLHyperlink(@"../" + data[0].ToString());
                }
            }
            var wrksht = wb.Worksheets.Add("Summary", 0);
            wrksht.Cell("A1").Value = "EXPORT DATA SUMMARY";
            wrksht.Range("A1:B1").Merge().Style.Font.SetBold().Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            wrksht.Columns("A").Style.Font.SetBold();
            wrksht.Cell("A2").Value = "Action";
            wrksht.Cell("B2").Value = objExportDataConfiguration.T_Name;
            wrksht.Cell("A3").Value = "Is Deleted";
            wrksht.Cell("B3").Value = (objExportDataConfiguration.T_EnableDelete.HasValue && objExportDataConfiguration.T_EnableDelete.Value) ? "Yes" : "No";
            wrksht.Cell("A4").Value = "Exported By";
            wrksht.Cell("B4").Value = UserName;
            wrksht.Cell("A5").Value = "Exported On";
            wrksht.Cell("B5").Value = DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss") + " UTC";
            wrksht.Cell("A7").Value = "Record Information";
            wrksht.Cell("B7").Value = RecordId + " - " + RecordDisplayValue;
            wrksht.Cell("A8").Value = "Note";
            wrksht.Cell("B8").Value = Notes;
            int clmcnt = 10;
            foreach(DataTable tbl in ds.Tables)
            {
                var condition = lstcondition.Count > 0 ? lstcondition.Where(wh => wh.Key == tbl.TableName).FirstOrDefault().Value : string.Empty;
                wrksht.Cell("A" + clmcnt).Value = tbl.TableName;
                wrksht.Cell("B" + clmcnt).Value = tbl.Rows.Count.ToString() + (!string.IsNullOrEmpty(condition) ? " (" + condition + ")" : string.Empty);
                clmcnt++;
            }
            wrksht.Rows().AdjustToContents();
            wrksht.Columns().AdjustToContents();
            wrksht.CellsUsed().Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            wrksht.CellsUsed().Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            wrksht.CellsUsed().Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            wrksht.CellsUsed().Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            wb.SaveAs(Path.Combine(childfolderPath, fileName));
        }
    }
}
}
