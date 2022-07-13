using GeneratorBase.MVC.DynamicQueryable;
using System;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Mvc;
using System.Web;
namespace GeneratorBase.MVC.Models
{
/// <summary>A graph helper.</summary>
public class GraphHelper
{
    /// <summary>Shows the graph.</summary>
    ///
    /// <param name="db">        The database.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="type">      The type.</param>
    /// <param name="inlarge">   The inlarge.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string ShowGraph(ApplicationContext db, string entityName, string type, int? inlarge)
    {
        string result = "";
        var entity = entityName;
        var entityModel = ModelReflector.Entities;
        var chartlist = db.Charts.Where(chrt => chrt.EntityName == entity && chrt.ShowInDashBoard).GetFromCache<IQueryable<T_Chart>, T_Chart>().ToList();
        if(type != "all")
            chartlist = chartlist.Where(chrt => chrt.Id == Convert.ToInt64(type)).ToList();
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(entityName).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("GetIQueryable");
        object[] MethodParams = new object[] { db };
        var entitylist = (IQueryable<IEntity>)mc.Invoke(objController, MethodParams);
        if(chartlist.Count > 0)
        {
            foreach(var charts in chartlist)
            {
                try
                {
                    var xaxis = charts.XAxis;
                    var yaxis = charts.YAxis;
                    var xaxisTitle = charts.XAxis;
                    var yaxisTitle = charts.YAxis;
                    var charttitle = charts.ChartTitle;
                    var entInfo = entityModel.FirstOrDefault(p => p.Name == charts.EntityName);
                    if(yaxis == null || yaxis == "0" || yaxis.ToLower() == "displayvalue")
                        yaxis = "id";
                    if(yaxis.ToLower() == "id")
                        yaxisTitle = entInfo.DisplayName;
                    var asso = entInfo.Associations.FirstOrDefault(p => p.AssociationProperty == xaxis);
                    if(asso != null)
                    {
                        xaxis = asso.Name.ToLower() + "." + "DisplayValue";
                        xaxisTitle = asso.DisplayName;
                    }
                    var gd = entitylist.AsQueryable().GroupBy(xaxis, "it");
                    var cntgrt10 = false;
                    if(gd.Count() > 10 && inlarge == null)
                    {
                        gd = gd.Take(10);
                        cntgrt10 = true;
                    }
                    if(yaxis.ToLower() == "id")
                        gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Count() as " + yaxis + ")");
                    else
                        gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Sum(" + yaxis + ") as " + yaxis + ")");
                    var chart = new System.Web.UI.DataVisualization.Charting.Chart
                    {
                        BorderlineDashStyle = ChartDashStyle.Solid,
                        BackSecondaryColor = System.Drawing.Color.White,
                        BackGradientStyle = GradientStyle.TopBottom,
                        BorderlineWidth = 1,
                        BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105),
                        RenderType = RenderType.ImageTag,
                        AntiAliasing = AntiAliasingStyles.All,
                        TextAntiAliasingQuality = TextAntiAliasingQuality.High
                    };
                    chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
                    if(cntgrt10)
                        chart.Titles.Add(CreateTitle(charttitle + " (Top 10)"));
                    else
                        chart.Titles.Add(CreateTitle(charttitle));
                    chart.ChartAreas.Add(CreateChartArea((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), "", xaxisTitle, yaxisTitle));
                    chart.Series.Add(CreateSeries((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), ""));
                    chart.Series[0].Points.DataBindXY(gd, xaxisTitle.Replace(" ", ""), gd, yaxis);
                    chart.Legends.Add(CreateLegend(""));
                    if(charts.ChartType.ToLower() == "pie")
                    {
                        chart.Series[0].LegendText = "#VALX";
                        chart.Series[0].Label = "#PERCENT{P2}";
                    }
                    if(inlarge != null)
                    {
                        chart.Width = 800;
                        chart.Height = 800;
                    }
                    byte[] bytes;
                    using(var chartimage = new MemoryStream())
                    {
                        chart.SaveImage(chartimage, ChartImageFormat.Png);
                        bytes = chartimage.GetBuffer();
                    }
                    if(cntgrt10)
                    {
                        var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + urlHelper.Action("ShowGraph", entity, new { type = charts.Id, inlarge = 1 }) + "')\"" : "") + "/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                    else
                    {
                        string img = "<div class=" + (inlarge == null ? "col-sm-4" : "col-sm-12") + " style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;' /></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        else
        {
            result = "No chart available to display.";
        }
        return result;
    }
    
    /// <summary>Show Graph For Entity Home Page.</summary>
    ///
    /// <param name="db">        The database.</param>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="type">      The type.</param>
    /// <param name="inlarge">   The inlarge.</param>
    /// <param name="XAxis">     The axis.</param>
    ///
    /// <returns>A string.</returns>
    
    public static string ShowGraphEntityHome(ApplicationContext db, string entityName, string type, int? inlarge, string XAxis)
    {
        string result = "";
        var entity = entityName;
        var chartlist = db.Charts.Where(chrt => chrt.EntityName == entity && chrt.ShowInDashBoard && chrt.XAxis == XAxis && chrt.YAxis == "Id").GetFromCache<IQueryable<T_Chart>, T_Chart>().ToList();
        if(type != "all")
            chartlist = chartlist.Where(chrt => chrt.Id == Convert.ToInt64(type)).ToList();
        //Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller, GeneratorBase.MVC.Controllers");
        Type controller = new CreateControllerType(entityName).controllerType;
        object objController = Activator.CreateInstance(controller, null);
        System.Reflection.MethodInfo mc = controller.GetMethod("GetIQueryable");
        object[] MethodParams = new object[] { db };
        var entitylist = (IQueryable<IEntity>)mc.Invoke(objController, MethodParams);
        if(chartlist.Count > 0)
        {
            var entityModel = ModelReflector.Entities;
            foreach(var charts in chartlist)
            {
                try
                {
                    var xaxis = charts.XAxis;
                    var yaxis = charts.YAxis;
                    var xaxisTitle = charts.XAxis;
                    var yaxisTitle = charts.YAxis;
                    var charttitle = charts.ChartTitle;
                    var entInfo = entityModel.FirstOrDefault(p => p.Name == charts.EntityName);
                    if(yaxis == null || yaxis == "0" || yaxis.ToLower() == "displayvalue")
                        yaxis = "id";
                    if(yaxis.ToLower() == "id")
                        yaxisTitle = entInfo.DisplayName;
                    var asso = entInfo.Associations.FirstOrDefault(p => p.AssociationProperty == xaxis);
                    if(asso != null)
                    {
                        xaxis = asso.Name.ToLower() + "." + "DisplayValue";
                        xaxisTitle = asso.DisplayName;
                    }
                    var gd = entitylist.AsQueryable().GroupBy(xaxis, "it");
                    var cntgrt10 = false;
                    if(gd.Count() > 10 && inlarge == null)
                    {
                        gd = gd.Take(10);
                        cntgrt10 = true;
                    }
                    if(yaxis.ToLower() == "id")
                        gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Count() as " + yaxis + ")");
                    else
                        gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Sum(" + yaxis + ") as " + yaxis + ")");
                    var chart = new System.Web.UI.DataVisualization.Charting.Chart
                    {
                        BorderlineDashStyle = ChartDashStyle.Solid,
                        BackSecondaryColor = System.Drawing.Color.White,
                        BackGradientStyle = GradientStyle.TopBottom,
                        BorderlineWidth = 1,
                        BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105),
                        RenderType = RenderType.ImageTag,
                        AntiAliasing = AntiAliasingStyles.All,
                        TextAntiAliasingQuality = TextAntiAliasingQuality.High
                    };
                    chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
                    if(cntgrt10)
                        chart.Titles.Add(CreateTitle(charttitle + " (Top 10)"));
                    else
                        chart.Titles.Add(CreateTitle(charttitle));
                    chart.ChartAreas.Add(CreateChartArea((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), "", xaxisTitle, yaxisTitle));
                    chart.Series.Add(CreateSeries((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), ""));
                    chart.Series[0].Points.DataBindXY(gd, xaxisTitle.Replace(" ", ""), gd, yaxis);
                    chart.Legends.Add(CreateLegend(""));
                    if(charts.ChartType.ToLower() == "pie")
                    {
                        chart.Series[0].LegendText = "#VALX";
                        chart.Series[0].Label = "#PERCENT{P2}";
                    }
                    if(inlarge != null)
                    {
                        chart.Width = 800;
                        chart.Height = 800;
                    }
                    byte[] bytes;
                    using(var chartimage = new MemoryStream())
                    {
                        chart.SaveImage(chartimage, ChartImageFormat.Png);
                        bytes = chartimage.GetBuffer();
                    }
                    if(cntgrt10)
                    {
                        var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        string img = "<div style='padding:0px; margin:0px 0px 0px 50px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + urlHelper.Action("ShowGraph", entity, new { type = charts.Id, inlarge = 1 }) + "')\"" : "") + "/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                    else
                    {
                        string img = "<div style='padding:0px; margin:0px 0px 0px 50px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;' /></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        else
        {
            result = "<div style='padding:0px; margin:0px 0px 0px 50px;'>No chart available to display.</div>";
        }
        return result;
    }
    //
    #region Chart Methods
    
    /// <summary>Creates a title.</summary>
    ///
    /// <param name="charttitle">The charttitle.</param>
    ///
    /// <returns>The new title.</returns>
    
    public static Title CreateTitle(string charttitle)
    {
        Title title = new Title();
        title.Text = charttitle;
        title.Font = new System.Drawing.Font("Helvetica", 10F, System.Drawing.FontStyle.Regular);
        title.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        return title;
    }
    
    /// <summary>Creates a legend.</summary>
    ///
    /// <param name="chartlegend">The chartlegend.</param>
    ///
    /// <returns>The new legend.</returns>
    
    public static Legend CreateLegend(string chartlegend)
    {
        Legend legend = new Legend();
        legend.Title = chartlegend;
        legend.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        legend.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        legend.Docking = Docking.Bottom;
        legend.Alignment = System.Drawing.StringAlignment.Center;
        return legend;
    }
    
    /// <summary>Creates the series.</summary>
    ///
    /// <param name="chartType">  Type of the chart.</param>
    /// <param name="chartseries">The chartseries.</param>
    ///
    /// <returns>The new series.</returns>
    
    public static Series CreateSeries(SeriesChartType chartType, string chartseries)
    {
        Series seriesDetail = new Series();
        seriesDetail.Name = chartseries;
        seriesDetail.IsValueShownAsLabel = false;
        if(chartType == SeriesChartType.Column)
            seriesDetail.IsVisibleInLegend = false;
        seriesDetail.ChartType = chartType;
        seriesDetail.BorderWidth = 2;
        seriesDetail.ChartArea = chartseries;
        return seriesDetail;
    }
    
    /// <summary>Creates chart area.</summary>
    ///
    /// <param name="chartType">Type of the chart.</param>
    /// <param name="chartarea">The chartarea.</param>
    /// <param name="xtitle">   The xtitle.</param>
    /// <param name="ytitle">   The ytitle.</param>
    ///
    /// <returns>The new chart area.</returns>
    
    public static ChartArea CreateChartArea(SeriesChartType chartType, string chartarea, string xtitle, string ytitle)
    {
        ChartArea chartArea = new ChartArea();
        chartArea.Name = chartarea;
        chartArea.BackColor = System.Drawing.Color.Transparent;
        chartArea.AxisX.IsLabelAutoFit = false;
        chartArea.AxisY.IsLabelAutoFit = false;
        chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
        chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        chartArea.AxisX.Interval = 1;
        if(chartType == SeriesChartType.Column)
            chartArea.AxisX.LabelStyle.Angle = -90;
        chartArea.AxisX.Title = xtitle;
        chartArea.AxisY.Title = ytitle;
        return chartArea;
    }
    #endregion
}
}