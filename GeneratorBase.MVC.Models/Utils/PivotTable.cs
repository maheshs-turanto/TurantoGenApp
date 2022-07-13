using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


/// <summary>
/// Create simple and advanced pivot reports.
/// </summary>
public class Pivot
{
    #region Variables
    
    private DataTable _DataTable;
    private string _CssTopHeading;
    private string _CssSubHeading;
    private string _CssLeftColumn;
    private string _CssItems;
    private string _CssTotals;
    private string _CssTable;
    
    #endregion Variables
    
    #region Constructors
    /// <summary>
    ///
    /// </summary>
    /// <param name="dataTable"></param>
    public Pivot(DataTable dataTable)
    {
        Init();
        _DataTable = dataTable;
    }
    
    #endregion Constructors
    
    #region Properties
    /// <summary>
    ///
    /// </summary>
    public DataTable ResultTable
    {
        get
        {
            return _DataTable;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssTopHeading
    {
        get
        {
            return _CssTopHeading;
        }
        set
        {
            _CssTopHeading = value;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssSubHeading
    {
        get
        {
            return _CssSubHeading;
        }
        set
        {
            _CssSubHeading = value;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssLeftColumn
    {
        get
        {
            return _CssLeftColumn;
        }
        set
        {
            _CssLeftColumn = value;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssItems
    {
        get
        {
            return _CssItems;
        }
        set
        {
            _CssItems = value;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssTotals
    {
        get
        {
            return _CssTotals;
        }
        set
        {
            _CssTotals = value;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public string CssTable
    {
        get
        {
            return _CssTable;
        }
        set
        {
            _CssTable = value;
        }
    }
    
    #endregion Properties
    
    #region Private Methods
    
    private string[] FindValues(string xAxisField, string xAxisValue, string yAxisField, string yAxisValue, string[,] zAxisFields)
    {
        int zAxis = zAxisFields.Length / 2;
        if(zAxis < 1)
            zAxis++;
        string[] zAxisValues = new string[zAxis];
        //set default values
        for(int i = 0; i <= zAxisValues.GetUpperBound(0); i++)
        {
            zAxisValues[i] = "0";
        }
        try
        {
            for(int z = 0; z < zAxis; z++)
            {
                double sum = 0;
                double count = 0;
                double avg = 0;
                double max = 0;
                double min = 0;
                foreach(DataRow row in _DataTable.Rows)
                {
                    //for (int z = 0; z < zAxis; z++)
                    //{
                    if(Convert.ToString(row[xAxisField]) == xAxisValue && Convert.ToString(row[yAxisField]) == yAxisValue)
                    {
                        if(zAxisFields[z, 1].Contains("Count"))
                        {
                            count++;
                            zAxisValues[z] = Convert.ToString(count);
                        }
                        if(zAxisFields[z, 1].Contains("Sum"))
                        {
                            sum += Convert.ToDouble(row[zAxisFields[z, 0]]);
                            zAxisValues[z] = Convert.ToString(sum);
                        }
                        if(zAxisFields[z, 1].Contains("Average"))
                        {
                            sum += Convert.ToDouble(row[zAxisFields[z, 0]]);
                            count++;
                            avg = sum / Convert.ToDouble(count);
                            zAxisValues[z] = String.Format("{0:0.##}", avg);
                        }
                        if(zAxisFields[z, 1].Contains("Max"))
                        {
                            if(max < Convert.ToDouble(row[zAxisFields[z, 0]]))
                            {
                                max = Convert.ToDouble(row[zAxisFields[z, 0]]);
                            }
                            zAxisValues[z] = Convert.ToString(max);
                        }
                        if(zAxisFields[z, 1].Contains("Min"))
                        {
                            if(min > Convert.ToDouble(row[zAxisFields[z, 0]]))
                            {
                                min = Convert.ToDouble(row[zAxisFields[z, 0]]);
                            }
                            zAxisValues[z] = Convert.ToString(min);
                        }
                        //for (int z = 0; z < zAxis; z++)
                        //{
                        //    zAxisValues[z] = Convert.ToString(row[zAxisFields[z]]);
                        //}
                        //break;
                    }
                }
                // zAxisValues[z] = zAxisFields[z, 1].Replace(",","=") + zAxisValues[z];
                //loop data
            }
        }
        catch
        {
            throw;
        }
        return zAxisValues;
    }
    
    private void Init()
    {
        _CssTopHeading = "";
        _CssSubHeading = "";
        _CssLeftColumn = "";
        _CssItems = "";
        _CssTotals = "";
        _CssTable = "";
    }
    
    private void TableStyle(HtmlTable table)
    {
        table.Attributes.Add("Class", "CssTable");
    }
    
    private void MainHeaderTopCellStyle(HtmlTableCell cell)
    {
        cell.Attributes.Add("Class", "CssTopHeading");
    }
    
    private void MainHeaderLeftCellStyle(HtmlTableCell cell)
    {
        cell.Attributes.Add("Class", "CssLeftColumn");
    }
    
    private void SubHeaderCellStyle(HtmlTableCell cell)
    {
        cell.Attributes.Add("Class", "CssSubHeading");
    }
    
    private void ItemCellStyle(HtmlTableCell cell)
    {
        cell.Attributes.Add("Class", "CssItems");
    }
    
    private void TotalCellStyle(HtmlTableCell cell)
    {
        cell.Attributes.Add("Class", "CssTotals");
    }
    
    #endregion Private Methods
    
    #region Public Methods
    /// <summary>
    /// Creates an advanced 3D Pivot table.
    /// </summary>
    /// <param name="xAxisField"></param>
    /// <param name="yAxisField"></param>
    /// <param name="zAxisFields"></param>
    /// <param name="table">Removed HtmlTable</param>
    /// <param name="tableBuilder"></param>
    
    public void PivotTable(string xAxisField, string yAxisField, string[,] zAxisFields, StringBuilder tableBuilder, string ReportName)
    {
        //style table
        tableBuilder.Append(string.Format("<table id=\"{0}\" class=\"{1}\">", "CrossTab", "CssTable"));
        //TableStyle(table);
        /*
         * The x-axis is the main horizontal row.
         * The z-axis is the sub horizontal row.
         * The y-axis is the left vertical column.
         */
        try
        {
            //get distinct xAxisFields
            ArrayList xAxis = new ArrayList();
            foreach(DataRow row in _DataTable.Rows)
            {
                if(!xAxis.Contains(row[xAxisField]))
                    xAxis.Add(row[xAxisField]);
            }
            //get distinct yAxisFields
            ArrayList yAxis = new ArrayList();
            foreach(DataRow row in _DataTable.Rows)
            {
                if(!yAxis.Contains(row[yAxisField]))
                    yAxis.Add(row[yAxisField]);
            }
            //create a 2D array for the y-axis/z-axis fields
            int zAxis = zAxisFields.Length / 2;
            if(zAxis < 1)
                zAxis = 1;
            string[,] matrix = new string[(xAxis.Count * zAxis), yAxis.Count];
            string[] zAxisValues = new string[zAxis];
            for(int y = 0; y < yAxis.Count; y++)  //loop thru y-axis fields
            {
                //rows
                for(int x = 0; x < xAxis.Count; x++)  //loop thru x-axis fields
                {
                    //main columns
                    //get the z-axis values
                    zAxisValues = FindValues(xAxisField, Convert.ToString(xAxis[x])
                                             , yAxisField, Convert.ToString(yAxis[y]), zAxisFields);
                    for(int z = 0; z < zAxis; z++)  //loop thru z-axis fields
                    {
                        //sub columns
                        matrix[(((x + 1) * zAxis - zAxis) + z), y] = zAxisValues[z];
                    }
                }
            }
            //calculate totals for the y-axis
            decimal[] yTotals = new decimal[(xAxis.Count * zAxis)];
            for(int col = 0; col < (xAxis.Count * zAxis); col++)
            {
                yTotals[col] = 0;
                for(int row = 0; row < yAxis.Count; row++)
                {
                    yTotals[col] += Convert.ToDecimal(matrix[col, row]);
                }
            }
            //calculate totals for the x-axis
            decimal[,] xTotals = new decimal[zAxis, (yAxis.Count + 1)];
            for(int y = 0; y < yAxis.Count; y++)  //loop thru the y-axis
            {
                int zCount = 0;
                for(int z = 0; z < (zAxis * xAxis.Count); z++)  //loop thru the z-axis
                {
                    xTotals[zCount, y] += Convert.ToDecimal(matrix[z, y]);
                    if(zCount == (zAxis - 1))
                        zCount = 0;
                    else
                        zCount++;
                }
            }
            for(int xx = 0; xx < zAxis; xx++)  //Grand Total
            {
                for(int xy = 0; xy < yAxis.Count; xy++)
                {
                    xTotals[xx, yAxis.Count] += xTotals[xx, xy];
                }
            }
            //Build HTML Table
            //Append main row (x-axis)
            //HtmlTableRow mainRow = new HtmlTableRow();
            //tableBuilder.Append(string.Format(titleHead));
            string exlimg = "~/icons/xls.png";
            int colspanForName = 0;
            if(yAxis.Count == 0)
                colspanForName = zAxis + 1;
            else
            {
                for(int x = 0; x <= xAxis.Count; x++)  //loop thru x-axis + 1
                {
                    colspanForName += zAxis;
                }
                colspanForName = colspanForName + 1;
            }
            string titleHead = "<tr><td style='text-align:center;' colspan=" + colspanForName + ">" +
                               "<div style='margin: 0 0 -19px -4px; float:left'><b id='exportexcel' onclick='ExpXls();'>" +
                               "<span class='fam-page-white-put' title='Export to Excel'></b></div>";
            if(ReportName != "")
            {
                titleHead += "<b>" + ReportName + "</b>";
            }
            else
            {
                titleHead += "<b>Blank Report</b>";
            }
            titleHead += "<div style='margin: 0 0 -19px -4px; float:right'><b id='exportexcel' onclick='ExpXls();'>" +
                         "<span class='fam-page-white-put' title='Export to Excel'></b></div></td></tr>";
            tableBuilder.Append(string.Format(titleHead));
            tableBuilder.Append(string.Format("<tr>"));
            tableBuilder.Append(string.Format("<td></td>"));
            //mainRow.Cells.Add(new HtmlTableCell());
            for(int x = 0; x <= xAxis.Count; x++)  //loop thru x-axis + 1
            {
                //HtmlTableCell cell = new HtmlTableCell();
                //cell.ColSpan = zAxis;
                if(x < xAxis.Count)
                {
                    //cell.InnerText = Convert.ToString(xAxis[x]);
                    tableBuilder.Append(string.Format("<td class=\"{0}\" colspan='" + zAxis + "'>" + Convert.ToString(xAxis[x]) + "</td>", "CssTopHeading"));
                }
                else
                {
                    //cell.InnerText = "Grand Totals";
                    tableBuilder.Append(string.Format("<td  class=\"{0}\" colspan='" + zAxis + "'>Grand Totals</td>", "CssTopHeading"));
                }
                //style cell
                //MainHeaderTopCellStyle(cell);
                //mainRow.Cells.Add(cell);
            }
            //table.Rows.Add(mainRow);
            tableBuilder.Append(string.Format("</tr>"));
            //Append sub row (z-axis)
            //HtmlTableRow subRow = new HtmlTableRow();
            tableBuilder.Append(string.Format("<tr>"));
            //subRow.Cells.Add(new HtmlTableCell());
            tableBuilder.Append(string.Format("<td  class=\"{0}\">" + yAxisField + "</td>", "CssSubHeading"));
            //subRow.Cells[0].InnerText = yAxisField;
            //style cell
            //SubHeaderCellStyle(subRow.Cells[0]);
            for(int x = 0; x <= xAxis.Count; x++)  //loop thru x-axis + 1
            {
                for(int z = 0; z < zAxis; z++)
                {
                    //HtmlTableCell cell = new HtmlTableCell();
                    //cell.Text = zAxisFields[z, 0];   for Count sum
                    //cell.InnerText = zAxisFields[z, 0] + " " + zAxisFields[z, 1];
                    tableBuilder.Append(string.Format("<td  class=\"{0}\">" + zAxisFields[z, 0] + "(" + zAxisFields[z, 1] + ")" + "</td>", "CssSubHeading"));
                    //style cell
                    //SubHeaderCellStyle(cell);
                    //subRow.Cells.Add(cell);
                }
            }
            //table.Rows.Add(subRow);
            tableBuilder.Append(string.Format("</tr>"));
            //Append table items from matrix
            for(int y = 0; y < yAxis.Count; y++)  //loop thru y-axis
            {
                //HtmlTableRow itemRow = new HtmlTableRow();
                tableBuilder.Append(string.Format("<tr>"));
                for(int z = 0; z <= (zAxis * xAxis.Count); z++)  //loop thru z-axis + 1
                {
                    //HtmlTableCell cell = new HtmlTableCell();
                    if(z == 0)
                    {
                        //cell.InnerText = Convert.ToString(yAxis[y]);
                        tableBuilder.Append(string.Format("<td class=\"{0}\">", "CssLeftColumn"));
                        tableBuilder.Append(string.Format(Convert.ToString(yAxis[y])));
                        //style cell
                        //MainHeaderLeftCellStyle(cell);
                    }
                    else
                    {
                        //cell.InnerText = Convert.ToString(matrix[(z - 1), y]);
                        tableBuilder.Append(string.Format("<td class=\"{0}\">", "CssItems"));
                        tableBuilder.Append(string.Format(Convert.ToString(matrix[(z - 1), y])));
                        //style cell
                        //ItemCellStyle(cell);
                    }
                    //itemRow.Cells.Add(cell);
                    tableBuilder.Append(string.Format("</td>"));
                }
                //append x-axis grand totals
                for(int z = 0; z < zAxis; z++)
                {
                    //HtmlTableCell cell = new HtmlTableCell();
                    tableBuilder.Append(string.Format("<td class=\"{0}\">", "CssTotals"));
                    //cell.InnerText = Convert.ToString(xTotals[z, y]);
                    tableBuilder.Append(string.Format(Convert.ToString(xTotals[z, y])));
                    //style cell
                    //TotalCellStyle(cell);
                    //itemRow.Cells.Add(cell);
                    tableBuilder.Append(string.Format("</td>"));
                }
                //table.Rows.Add(itemRow);
                tableBuilder.Append(string.Format("</tr>"));
            }
            //append y-axis totals
            //HtmlTableRow totalRow = new HtmlTableRow();
            tableBuilder.Append(string.Format("<tr>"));
            for(int x = 0; x <= (zAxis * xAxis.Count); x++)
            {
                //HtmlTableCell cell = new HtmlTableCell();
                tableBuilder.Append(string.Format("<td class=\"{0}\">", "CssTotals"));
                if(x == 0)
                {
                    //cell.InnerText = "Totals";
                    tableBuilder.Append("Totals");
                }
                else
                {
                    //cell.InnerText = Convert.ToString(yTotals[x - 1]);
                    tableBuilder.Append(Convert.ToString(yTotals[x - 1]));
                }
                //style cell
                //TotalCellStyle(cell);
                //totalRow.Cells.Add(cell);
                tableBuilder.Append(string.Format("</td>"));
            }
            //append x-axis/y-axis totals
            for(int z = 0; z < zAxis; z++)
            {
                tableBuilder.Append(string.Format("<td class=\"{0}\">", "CssTotals"));
                tableBuilder.Append(Convert.ToString(xTotals[z, xTotals.GetUpperBound(1)]));
                //style cell
                //totalRow.Cells.Add(cell);
                tableBuilder.Append(string.Format("</td>"));
            }
            //table.Rows.Add(totalRow);
            tableBuilder.Append(string.Format("</tr>"));
            tableBuilder.Append(string.Format("</table>"));
        }
        catch
        {
            throw;
        }
        //  return table;
    }
    
    
    
    
    #endregion Public Methods
    
}
