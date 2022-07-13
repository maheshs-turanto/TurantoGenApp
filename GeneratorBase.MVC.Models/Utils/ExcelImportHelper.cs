using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
/// <summary>An excel import helper.</summary>
public class ExcelImportHelper
{
    /// <summary>Determine if we are all columns empty.</summary>
    ///
    /// <param name="dr">          The dr.</param>
    /// <param name="sheetColumns">The sheet columns.</param>
    ///
    /// <returns>True if all columns empty, false if not.</returns>
    
    public static bool AreAllColumnsEmpty(DataRow dr, List<string> sheetColumns)
    {
        if(dr == null)
        {
            return true;
        }
        else
        {
            for(int i = 0; i < sheetColumns.Count(); i++)
            {
                if(string.IsNullOrEmpty(sheetColumns[i]))
                    continue;
                if(dr[Convert.ToInt32(sheetColumns[i]) - 1] != null && dr[Convert.ToInt32(sheetColumns[i]) - 1].ToString() != "")
                {
                    return false;
                }
            }
            return true;
        }
    }
    
    /// <summary>Data import.</summary>
    ///
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="fileLocation"> The file location.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    public static DataSet DataImport(string fileExtension, string fileLocation)
    {
        string excelConnectionString = string.Empty;
        DataSet objDataSet = new DataSet();
        DataTable dt = null;
        if(!System.IO.File.Exists(fileLocation))
        {
            return objDataSet;
        }
        if(fileExtension == ".xlsx")
        {
            dt = new DataTable();
            using(XLWorkbook workBook = new XLWorkbook(fileLocation))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);
                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach(IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if(firstRow)
                    {
                        foreach(IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        //Cursor in blank area
                        //foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                        //foreach(IXLCell cell in row.Cells())
                        //{
                        //    dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                        //    i++;
                        //}
                        for(var column = 0; column <= dt.Columns.Count-1; column++)
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = row.Cell(column+1).Value.ToString();
                            i++;
                        }
                    }
                }
                objDataSet.Tables.Add(dt);
            }
        }
        else if(fileExtension == ".xls" || fileExtension == ".csv")
        {
            if(fileExtension == ".xls")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            }
            else if(fileExtension == ".csv")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(fileLocation) + ";Extended Properties=\"Text;HDR=YES;FMT=Delimited\"";
            }
            if(fileExtension == ".csv")
            {
                //query = "SELECT * FROM [" + System.IO.Path.GetFileName(fileLocation) + "]";
                using(var parser = new TextFieldParser(new StringReader(System.IO.File.ReadAllText(fileLocation))))
                {
                    var headerRow = true;
                    var dtcsv = new DataTable();
                    int count = 0;
                    parser.Delimiters = new[] { "," };
                    while(!parser.EndOfData)
                    {
                        var currentRow = parser.ReadFields();
                        if(headerRow)
                        {
                            foreach(var field in currentRow)
                            {
                                dtcsv.Columns.Add(field, typeof(object));
                                count++;
                            }
                            headerRow = false;
                        }
                        else
                        {
                            currentRow = currentRow.Take(count).ToArray();
                            dtcsv.Rows.Add(currentRow);
                        }
                    }
                    objDataSet.Tables.Add(dtcsv);
                    return WithoutBlankRow(objDataSet);
                }
            }
            else
            {
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                string query = string.Empty;
                String[] excelSheets = null;
                dt = new DataTable();
                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if(dt == null)
                {
                    return null;
                }
                excelSheets = new String[dt.Rows.Count];
                int t = 0;
                foreach(DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                query = string.Format("Select * from [{0}]", excelSheets[0]);
                excelConnection.Close();
                using(OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                {
                    dataAdapter.Fill(objDataSet);
                }
                excelConnection1.Close();
            }
        }
        return WithoutBlankRow(objDataSet);
    }
    
    /// <summary>Without blank row.</summary>
    ///
    /// <param name="ds">The ds.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    public static DataSet WithoutBlankRow(DataSet ds)
    {
        if(ds.Tables[0].Rows.Count == 0) return ds;
        DataSet dsnew = new DataSet();
        DataTable dt = ds.Tables[0].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field).ToString().Trim(), string.Empty) == 0)).CopyToDataTable();
        dsnew.Tables.Add(dt);
        return dsnew;
    }
    
    /// <summary>Validates the model described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public static bool ValidateModel(object validate)
    {
        return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null, true);
    }
    
    /// <summary>Validates the model with errors described by validate.</summary>
    ///
    /// <param name="validate">The validate.</param>
    ///
    /// <returns>A list of.</returns>
    
    public static ICollection<ValidationResult> ValidateModelWithErrors(object validate)
    {
        ICollection<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), results, true);
        return results;
    }
}
}