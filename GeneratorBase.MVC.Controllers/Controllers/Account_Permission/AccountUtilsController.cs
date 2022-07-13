using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
/// <summary>A controller for handling accounts.</summary>
[Authorize]
public partial class AccountController : IdentityBaseController
{


    /// <summary>Without blank row.</summary>
    ///
    /// <param name="ds">The ds.</param>
    ///
    /// <returns>A DataSet.</returns>
    
    private DataSet WithoutBlankRow(DataSet ds)
    {
        DataSet dsnew = new DataSet();
        DataTable dt = ds.Tables[0].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field).ToString().Trim(), string.Empty) == 0)).CopyToDataTable();
        dsnew.Tables.Add(dt);
        return dsnew;
    }
    
    /// <summary>Determine if we are all columns empty.</summary>
    ///
    /// <param name="dr">          The dr.</param>
    /// <param name="sheetColumns">The sheet columns.</param>
    ///
    /// <returns>True if all columns empty, false if not.</returns>
    
    bool AreAllColumnsEmpty(DataRow dr, List<string> sheetColumns)
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
    
    /// <summary>(An Action that handles HTTP POST requests) uploads a user.</summary>
    ///
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the UploadUser View.</returns>
    [HttpGet]
    public ActionResult UploadUser(string UrlReferrer, long? TenantId)
    {
        ViewBag.IsDefaultMapping = false;
        ViewBag.Title = "Upload File";
        ViewData["UrlReferrer"] = UrlReferrer;
        ViewData["TenantId"] = TenantId;
        return View();
    }
    /// <summary>Get next random number in range.</summary>
    ///
    /// <param name="random">System.Security.Cryptography.RandomNumberGenerator.</param>
    /// <param name="max">   Max value.</param>
    ///
    /// <returns>Generated random number.</returns>
    private int GetNextRandom(System.Security.Cryptography.RandomNumberGenerator random, int max)
    {
        if(max <= 0)
        {
            throw new ArgumentOutOfRangeException("max");
        }
        byte[] data = new byte[16];
        random.GetBytes(data);
        int value = BitConverter.ToInt32(data, 0) % max;
        if(value < 0)
        {
            value = -value;
        }
        return value;
    }
    private static int GetNextRandomNumber(System.Security.Cryptography.RandomNumberGenerator random, int max)
    {
        if(max <= 0)
        {
            throw new ArgumentOutOfRangeException("max");
        }
        byte[] data = new byte[16];
        random.GetBytes(data);
        int value = BitConverter.ToInt32(data, 0) % max;
        if(value < 0)
        {
            value = -value;
        }
        return value;
    }
    /// <summary>Creates random password.</summary>
    ///
    /// <returns>The new random password.</returns>
    public static string CreateRandomPassword()
    {
        ApplicationContext dbcontext = new ApplicationContext(new SystemUser());
        int pwdlength = Convert.ToInt32(dbcontext.AppSettings.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value);
        bool isalphanumeric = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireSpecialCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
        bool isdigit = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
        bool isUpperCase = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
        bool isLowerCase = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
        char[] charUpper = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        char[] charNumeric = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        char[] charAlphaNumeric = { '!', '@', '$', '?', '_', '&', '#', '*' };
        char[] charLower = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        System.Text.StringBuilder randomString = new System.Text.StringBuilder();
        //Random randomCharacter = new Random();
        System.Security.Cryptography.RandomNumberGenerator randomCharacter = new System.Security.Cryptography.RNGCryptoServiceProvider(); // Compliant for security-sensitive use cases
        int randomCharSelected;
        int stringlength = pwdlength;
        if(isUpperCase)
        {
            randomCharSelected = GetNextRandomNumber(randomCharacter, charUpper.Length);
            randomString.Append(charUpper[randomCharSelected]);
        }
        if(isalphanumeric)
        {
            randomCharSelected = GetNextRandomNumber(randomCharacter, charAlphaNumeric.Length);
            randomString.Append(charAlphaNumeric[randomCharSelected]);
        }
        if(isLowerCase)
        {
            for(uint i = 0; i < stringlength; i++)
            {
                int randomCharSelected1 = GetNextRandomNumber(randomCharacter, stringlength);
                randomString.Append(charLower[randomCharSelected1]);
            }
        }
        if(isdigit)
        {
            randomCharSelected = GetNextRandomNumber(randomCharacter, charNumeric.Length);
            randomString.Append(charNumeric[randomCharSelected]);
        }
        return randomString.ToString();
    }
    /// <summary>Generate Captcha method for show Captcha image on Register page.</summary>
    [AllowAnonymous]
    public void generateCaptcha()
    {
        CaptchaImage cap = new CaptchaImage();
        string randomString = cap.CreateRandomText(6);
        Bitmap bitmap = new Bitmap(200, 40, PixelFormat.Format32bppArgb);
        System.Security.Cryptography.RandomNumberGenerator random = new System.Security.Cryptography.RNGCryptoServiceProvider(); // Compliant for security-sensitive use cases
        // Create a graphics object for drawing.
        Graphics g = Graphics.FromImage(bitmap);
        g.PageUnit = GraphicsUnit.Pixel;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new Rectangle(0, 0, 200, 40);
        // Fill in the background.
        HatchBrush hatchBrush = new HatchBrush(HatchStyle.Shingle, Color.LightGray, Color.White);
        g.FillRectangle(hatchBrush, rect);
        // Set up the text font.
        SizeF size;
        float fontSize = rect.Height + 1;
        Font font;
        // Adjust the font size until the text fits within the image.
        do
        {
            fontSize--;
            font = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            size = g.MeasureString(randomString, font);
        }
        while(size.Width > rect.Width);
        // Set up the text format.
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        // Create a path using the text and warp it randomly.
        GraphicsPath path = new GraphicsPath();
        path.AddString(randomString, font.FontFamily, (int)font.Style, font.Size, rect, format);
        float v = 4F;
        PointF[] points =
        {
            new PointF(GetNextRandom(random,rect.Width) / v, GetNextRandom(random,rect.Height) / v),
            new PointF(rect.Width - GetNextRandom(random,rect.Width) / v, GetNextRandom(random,rect.Height) / v),
            new PointF(GetNextRandom(random,rect.Width) / v, rect.Height - GetNextRandom(random,rect.Height) / v),
            new PointF(rect.Width - GetNextRandom(random,rect.Width) / v, rect.Height - GetNextRandom(random,rect.Height) / v)
        };
        Matrix matrix = new Matrix();
        matrix.Translate(0F, 0F);
        path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
        // Draw the text.
        hatchBrush = new HatchBrush(HatchStyle.Shingle, Color.LightGray, Color.DarkGray);
        g.FillPath(hatchBrush, path);
        // Add some random noise.
        int m = Math.Max(rect.Width, rect.Height);
        for(int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
        {
            int x = GetNextRandom(random, rect.Width);
            int y = GetNextRandom(random, rect.Height);
            int w = GetNextRandom(random, m / 50);
            int h = GetNextRandom(random, m / 50);
            g.FillEllipse(hatchBrush, x, y, w, h);
        }
        captchastring = randomString;
        //this.ControllerContext.Controller.ViewBag.captchastring = randomString;
        HttpResponseBase response = this.ControllerContext.HttpContext.Response;
        response.ContentType = "image/jpeg";
        bitmap.Save(response.OutputStream, ImageFormat.Jpeg);
        bitmap.Dispose();
        // Clean up.
        font.Dispose();
        hatchBrush.Dispose();
        g.Dispose();
    }
    /// <summary>(An Action that handles HTTP POST requests) uploads a user.</summary>
    ///
    /// <param name="FileUpload"> The file upload.</param>
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the UploadUser View.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UploadUser([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection, string UrlReferrer, long? TenantId)
    {
        if(FileUpload != null)
        {
            ViewData["UrlReferrer"] = UrlReferrer;
            ViewData["TenantId"] = TenantId;
            string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();
            if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv" || fileExtension == ".all")
            {
                string rename = string.Empty;
                if(fileExtension == ".all")
                {
                    rename = System.IO.Path.GetFileName(FileUpload.FileName.ToLower().Replace(fileExtension, ".csv"));
                    fileExtension = ".csv";
                }
                else
                    rename = System.IO.Path.GetFileName(FileUpload.FileName);
                string fileLocation = string.Format("{0}\\{1}", Server.MapPath("~/ExcelFiles"), rename);
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                FileUpload.SaveAs(fileLocation);
                DataSet objDataSet = ExcelImportHelper.DataImport(fileExtension, fileLocation);
                var col = new List<SelectListItem>();
                if(objDataSet.Tables.Count > 0)
                {
                    int iCols = objDataSet.Tables[0].Columns.Count;
                    if(iCols > 0)
                    {
                        for(int i = 0; i < iCols; i++)
                        {
                            col.Add(new SelectListItem { Value = (i + 1).ToString(), Text = objDataSet.Tables[0].Columns[i].Caption });
                        }
                    }
                }
                col.Insert(0, new SelectListItem { Value = "", Text = "Select Column" });
                Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList> objColMapAssocProperties = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList>();
                Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>();
                // var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "ApplicationUser");
                var entList = new GeneratorBase.MVC.ModelReflector.Entity { Name = "User", DisplayName = "User" };
                entList.Properties = new List<ModelReflector.Property>();
                if(entList != null)
                {
                    entList.Properties.Add(new ModelReflector.Property { Name = "UserName", DisplayName = "User Name", DataType = "String", IsRequired = true });
                    entList.Properties.Add(new ModelReflector.Property { Name = "Password", DisplayName = "Password", DataType = "String", IsRequired = false });
                    entList.Properties.Add(new ModelReflector.Property { Name = "FirstName", DisplayName = "First Name", DataType = "String", IsRequired = true });
                    entList.Properties.Add(new ModelReflector.Property { Name = "LastName", DisplayName = "Last Name", DataType = "String", IsRequired = true });
                    entList.Properties.Add(new ModelReflector.Property { Name = "Email", DisplayName = "Email", DataType = "String", IsRequired = true });
                    entList.Properties.Add(new ModelReflector.Property { Name = "PhoneNumber", DisplayName = "Phone Number", DataType = "String", IsRequired = false });
                    entList.Properties.Add(new ModelReflector.Property { Name = "LockOutEnabled", DisplayName = "User's Status", DataType = "Boolean", IsRequired = false });
                    entList.Properties.Add(new ModelReflector.Property { Name = "NotifyForEmail", DisplayName = "Send Email Notification to Create Password", DataType = "Boolean", IsRequired = false });
                    entList.Properties.Add(new ModelReflector.Property { Name = "Roles", DisplayName = "Roles", DataType = "String", IsRequired = false });
                    if(!TenantId.HasValue)
                        entList.Properties.Add(new ModelReflector.Property { Name = "Tenant", DisplayName = "Tenant", DataType = "String", IsRequired = false });
                    foreach(var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                    {
                        long selectedVal = 0;
                        var colSelected = col.FirstOrDefault(p => p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                        if(colSelected != null)
                            selectedVal = long.Parse(colSelected.Value);
                        objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                    }
                }
                ViewBag.AssociatedProperties = objColMapAssocProperties;
                ViewBag.ColumnMapping = objColMap;
                ViewBag.FilePath = fileLocation;
                if(!string.IsNullOrEmpty(collection["ListOfMappings"]))
                {
                    string typeName = "";
                    string colKey = "";
                    string colDisKey = "";
                    string colListInx = "";
                    typeName = "User";
                    string FilePath = ViewBag.FilePath;
                    var columnlist = colKey;
                    var columndisplaynamelist = colDisKey;
                    var selectedlist = colListInx;
                    //string DefaultColumnMappingName = string.Empty;
                    string DetailMessage = "";
                    //string excelConnectionString = string.Empty;
                    DataTable tempdt = new DataTable();
                    if(selectedlist != null && columnlist != null)
                    {
                        var dtsheetColumns = selectedlist.Split(',').ToList();
                        var dttblColumns = columndisplaynamelist.Split(',').ToList();
                        for(int j = 0; j < dtsheetColumns.Count; j++)
                        {
                            string columntable = dttblColumns[j];
                            int columnSheet = 0;
                            if(string.IsNullOrEmpty(dtsheetColumns[j]))
                                continue;
                            else
                                columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                            tempdt.Columns.Add(columntable);
                        }
                        for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            var sheetColumns = selectedlist.Split(',').ToList();
                            if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                                continue;
                            var tblColumns = columndisplaynamelist.Split(',').ToList();
                            DataRow objdr = tempdt.NewRow();
                            for(int j = 0; j < sheetColumns.Count; j++)
                            {
                                string columntable = tblColumns[j];
                                int columnSheet = 0;
                                if(string.IsNullOrEmpty(sheetColumns[j]))
                                    continue;
                                else
                                    columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                                string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                                if(string.IsNullOrEmpty(columnValue))
                                    continue;
                                objdr[columntable] = columnValue;
                            }
                            tempdt.Rows.Add(objdr);
                        }
                    }
                    DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Users";
                    if(entList != null)
                    {
                        if(!string.IsNullOrEmpty(DetailMessage))
                            ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
                        ViewBag.ColumnMapping = null;
                        ViewBag.FilePath = FilePath;
                        ViewBag.ColumnList = columnlist;
                        ViewBag.SelectedList = selectedlist;
                        ViewBag.ConfirmImportData = tempdt;
                        if(ViewBag.ConfirmImportData != null)
                        {
                            ViewBag.Title = "Data Preview";
                            return View("UploadUser");
                        }
                        else
                            return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Plese select Excel File.");
            }
        }
        ViewBag.Title = "Column Mapping";
        return View("UploadUser");
    }
    
    /// <summary>(An Action that handles HTTP POST requests) confirm import data.</summary>
    ///
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>A response stream to send to the ConfirmImportData View.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmImportData(FormCollection collection, string UrlReferrer, long? TenantId)
    {
        ViewData["UrlReferrer"] = UrlReferrer;
        ViewData["TenantId"] = TenantId;
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["lblColumn"];
        var columndisplaynamelist = collection["lblColumnDisplayName"];
        var selectedlist = collection["colList"];
        var selectedAssocPropList = collection["colAssocPropList"];
        //bool SaveMapping = collection["SaveMapping"] == "on" ? true : false;
        //string mappingName = collection["MappingName"];
        string DetailMessage = "";
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        //string typename = "User";
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = ExcelImportHelper.DataImport(fileExtension, fileLocation);
            DataTable tempdt = new DataTable();
            if(selectedlist != null && columnlist != null)
            {
                var dtsheetColumns = selectedlist.Split(',').ToList();
                var dttblColumns = columndisplaynamelist.Split(',').ToList();
                for(int j = 0; j < dtsheetColumns.Count; j++)
                {
                    string columntable = dttblColumns[j];
                    int columnSheet = 0;
                    if(string.IsNullOrEmpty(dtsheetColumns[j]))
                        continue;
                    else
                        columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                    tempdt.Columns.Add(columntable);
                }
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    var sheetColumns = selectedlist.Split(',').ToList();
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    var tblColumns = columndisplaynamelist.Split(',').ToList();
                    DataRow objdr = tempdt.NewRow();
                    for(int j = 0; j < sheetColumns.Count; j++)
                    {
                        string columntable = tblColumns[j];
                        int columnSheet = 0;
                        if(string.IsNullOrEmpty(sheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                        string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                        if(string.IsNullOrEmpty(columnValue))
                            continue;
                        objdr[columntable] = columnValue;
                    }
                    tempdt.Rows.Add(objdr);
                }
            }
            DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Users";
            Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
            if(!string.IsNullOrEmpty(selectedAssocPropList))
            {
                var entitypropList = selectedAssocPropList.Split(',');
                foreach(var prop in entitypropList)
                {
                    var lst = prop.Split('-');
                    lstEntityProp.Add(lst[1], lst[0]);
                }
            }
            Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
            //var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_Employee");
            var entList = new GeneratorBase.MVC.ModelReflector.Entity { Name = "User", DisplayName = "User" };
            entList.Properties = new List<ModelReflector.Property>();
            entList.Properties.Add(new ModelReflector.Property { Name = "UserName", DisplayName = "User Name", DataType = "String", IsRequired = true });
            entList.Properties.Add(new ModelReflector.Property { Name = "Password", DisplayName = "Password", DataType = "String", IsRequired = false });
            entList.Properties.Add(new ModelReflector.Property { Name = "FirstName", DisplayName = "First Name", DataType = "String", IsRequired = true });
            entList.Properties.Add(new ModelReflector.Property { Name = "LastName", DisplayName = "Last Name", DataType = "String", IsRequired = true });
            entList.Properties.Add(new ModelReflector.Property { Name = "Email", DisplayName = "Email", DataType = "String", IsRequired = true });
            entList.Properties.Add(new ModelReflector.Property { Name = "PhoneNumber", DisplayName = "Phone Number", DataType = "String", IsRequired = false });
            entList.Properties.Add(new ModelReflector.Property { Name = "LockOutEnabled", DisplayName = "User's Status", DataType = "Boolean", IsRequired = false });
            entList.Properties.Add(new ModelReflector.Property { Name = "NotifyForEmail", DisplayName = "Send Email Notification to Create Password", DataType = "Boolean", IsRequired = false });
            entList.Properties.Add(new ModelReflector.Property { Name = "Roles", DisplayName = "Roles", DataType = "String", IsRequired = false });
            if(!TenantId.HasValue)
                entList.Properties.Add(new ModelReflector.Property { Name = "Tenant", DisplayName = "Tenant", DataType = "String", IsRequired = false });
            if(objAssoUnique.Count > 0)
                ViewBag.AssoUnique = objAssoUnique;
            if(!string.IsNullOrEmpty(DetailMessage))
                ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
            ViewBag.FilePath = FilePath;
            ViewBag.ColumnList = columnlist;
            ViewBag.SelectedList = selectedlist;
            ViewBag.ConfirmImportData = tempdt;
            ViewBag.colAssocPropList = selectedAssocPropList;
            if(ViewBag.ConfirmImportData != null)
            {
                ViewBag.Title = "Data Preview";
                return View("UploadUser");
            }
            else
                return RedirectToAction("Index");
        }
        return View();
    }
    
    /// <summary>(An Action that handles HTTP POST requests) import data.</summary>
    ///
    /// <param name="collection"> The collection.</param>
    /// <param name="UrlReferrer">The URL referrer.</param>
    /// <param name="TenantId">   Identifier for the tenant.</param>
    ///
    /// <returns>An asynchronous result that yields an ActionResult.</returns>
    
    [AcceptVerbs(HttpVerbs.Post)]
    [HttpPost]
    public async Task<ActionResult> ImportData(FormCollection collection, string UrlReferrer, long? TenantId)
    {
        ViewData["UrlReferrer"] = UrlReferrer;
        ViewData["TenantId"] = TenantId;
        string FilePath = collection["hdnFilePath"];
        var columnlist = collection["hdnColumnList"];
        var selectedlist = collection["hdnSelectedList"];
        string fileLocation = FilePath;
        //string excelConnectionString = string.Empty;
        string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
        var selectedAssocPropList = collection["hdnSelectedAssocPropList"];
        Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(selectedAssocPropList))
        {
            var entitypropList = selectedAssocPropList.Split(',');
            foreach(var prop in entitypropList)
            {
                var lst = prop.Split('-');
                lstEntityProp.Add(lst[1], lst[0]);
            }
        }
        if(fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
        {
            DataSet objDataSet = ExcelImportHelper.DataImport(fileExtension, fileLocation);
            string error = string.Empty;
            if(selectedlist != null && columnlist != null)
            {
                for(int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    var sheetColumns = selectedlist.Split(',').ToList();
                    if(AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                        continue;
                    ApplicationUser model = new ApplicationUser();
                    var tblColumns = columnlist.Split(',').ToList();
                    var rolestring = string.Empty;
                    var tenantstring = string.Empty;
                    for(int j = 0; j < sheetColumns.Count; j++)
                    {
                        string columntable = tblColumns[j];
                        int columnSheet = 0;
                        if(string.IsNullOrEmpty(sheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                        string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                        if(string.IsNullOrEmpty(columnValue))
                            continue;
                        switch(columntable)
                        {
                        case "Email":
                            model.Email = columnValue;
                            break;
                        case "PhoneNumber":
                            model.PhoneNumber = columnValue;
                            break;
                        case "FirstName":
                            model.FirstName = columnValue;
                            break;
                        case "LastName":
                            model.LastName = columnValue;
                            break;
                        case "UserName":
                            model.UserName = columnValue;
                            break;
                        case "Password":
                            model.PasswordHash = columnValue;
                            break;
                        case "NotifyForEmail":
                            model.NotifyForEmail = Boolean.Parse(columnValue);
                            break;
                        case "Roles":
                            rolestring = columnValue;
                            break;
                        case "Tenant":
                            tenantstring = columnValue;
                            break;
                        case "LockOutEnabled":
                        {
                            if(columnValue != null && columnValue.ToLower() == "locked")
                                model.LockoutEndDateUtc = DateTime.MaxValue;
                            break;
                        }
                        default:
                            break;
                        }
                    }
                    if(model.NotifyForEmail == null)
                        model.NotifyForEmail = false;
                    if(model.NotifyForEmail.HasValue && model.NotifyForEmail.Value)
                        model.PasswordHash = CreateRandomPassword();
                    var User = await UserManager.FindByEmailAsync(model.Email);
                    if(User != null)
                    {
                        if(ViewBag.ImportError == null)
                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + " Email already exists.";
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + " Email already exists.";
                        error += ((i + 1).ToString()) + ",";
                        continue;
                    }
                    var userExist = await UserManager.FindByNameAsync(model.UserName);
                    if(userExist == null)
                    {
                        try
                        {
                            var result = await UserManager.CreateAsync(model, model.PasswordHash);
                            if(result.Succeeded)
                            {
                                //Two factor change
                                string twofactorauthenticationenable = CommonFunction.Instance.TwoFactorAuthenticationEnabled();
                                if(twofactorauthenticationenable.ToLower() == "true")
                                {
                                    await UserManager.SetTwoFactorEnabledAsync(model.Id, true);
                                }
                                if(!string.IsNullOrEmpty(rolestring))
                                {
                                    var roles = rolestring.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                    var idManager = new IdentityManager();
                                    idManager.ClearUserRoles(LogggedInUser, model.Id);
                                    foreach(var role in roles)
                                    {
                                        var roleobj = Identitydb.Roles.FirstOrDefault(p => p.Name == role.Trim());
                                        if(roleobj != null)
                                            idManager.AddUserToRole(LogggedInUser, model.Id, roleobj.Id);
                                    }
                                }
                                else AssignDefaultRoleToNewUser(model.Id);
                                if(TenantId.HasValue)
                                    AssociateWithTenant(TenantId, model.Id);
                                else if(!string.IsNullOrEmpty(tenantstring))
                                {
                                    var tenantList = tenantstring.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                    if(tenantList.Contains("-1"))
                                    {
                                        var list = CommonFunction.Instance.getTenantList(((CustomPrincipal)LogggedInUser));
                                        foreach(var tenant in list)
                                        {
                                            long tenantId = 0;
                                            Int64.TryParse(tenant.Key, out tenantId);
                                            if(tenantId > 0)
                                                AssociateWithTenant(tenantId, model.Id);
                                        }
                                    }
                                    else
                                        foreach(var tenant in tenantList)
                                        {
                                            long tenantId = 0;
                                            Int64.TryParse(tenant, out tenantId);
                                            if(tenantId > 0)
                                            {
                                                AssociateWithTenant(tenantId, model.Id);
                                            }
                                        }
                                }
                                //SendEmailToUser(model);
                                var emailstatus = SendEmailToUser(model);
                                if(emailstatus != "EmailSent")
                                    ViewBag.ImportError = emailstatus;
                            }
                            else
                            {
                                if(ViewBag.ImportError == null)
                                    ViewBag.ImportError = "Row No : " + (i + 1) + " " + " Required value missing.";
                                else
                                    ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + " Required value missing.";
                                error += ((i + 1).ToString()) + ",";
                                continue;
                            }
                        }
                        catch
                        {
                            if(ViewBag.ImportError == null)
                                ViewBag.ImportError = "Row No : " + (i + 1) + " " + " Data validation error !";
                            else
                                ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + " Data validation error !";
                            error += ((i + 1).ToString()) + ",";
                            continue;
                        }
                    }
                    else
                    {
                        if(ViewBag.ImportError == null)
                            ViewBag.ImportError = "Row No : " + (i + 1) + " " + " UserName already exists.";
                        else
                            ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + " UserName already exists.";
                        error += ((i + 1).ToString()) + ",";
                        continue;
                    }
                }
            }
            if(ViewBag.ImportError != null)
            {
                ViewBag.FilePath = FilePath;
                ViewBag.ErrorList = error.Substring(0, error.Length - 1);
                ViewBag.Title = "Error List";
                return View("UploadUser");
            }
            else
            {
                if(System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                if(ViewBag.ImportError == null)
                {
                    ViewBag.ImportError = "success";
                    ViewBag.Title = "Upload List";
                    return View("UploadUser");
                }
                return RedirectToAction("Index");
            }
        }
        return View();
    }
}
}
