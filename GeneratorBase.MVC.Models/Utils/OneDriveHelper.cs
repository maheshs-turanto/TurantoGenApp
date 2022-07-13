using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System;
using System.Net;

namespace GeneratorBase.MVC.Models
{
public class OneDriveHelper
{
    #region property
    /// <summary>
    /// clientId of you office 365 application
    /// </summary>
    public string ClientId
    {
        get;
    }
    /// <summary>
    /// Password/Public Key of you office 365 application
    /// </summary>
    public string ClientSecret
    {
        get;
    }
    /// <summary>
    /// use this token to request Office 365 API
    /// </summary>
    public string AccessToken
    {
        get;
        protected set;
    }
    /// <summary>
    /// when accessToken had expires, can use this token to refresh accessToken
    /// </summary>
    public string RefreshToken
    {
        get;
        protected set;
    }
    /// <summary>
    /// The last time of you get token
    /// </summary>
    public DateTime RefreshTime
    {
        get;
        protected set;
    }
    /// <summary>
    /// Refresh time span
    /// </summary>
    public TimeSpan RefreshTimeSpan
    {
        get;
        protected set;
    }
    /// <summary>
    /// The OneDrive REST API root address
    /// </summary>
    public string OneDriveApiRoot
    {
        get;
        set;
    } = "https://graph.microsoft.com/v1.0/";
    /// <summary>
    /// OneDrive TenantId
    /// </summary>
    public string OneDriveTenantId
    {
        get;
        set;
    }
    /// <summary>
    /// OneDrive Token Url
    /// </summary>
    public string OneDriveTokenUrl = "https://login.microsoftonline.com/";
    /// <summary>
    /// OneDrive UserName
    /// </summary>
    public string OneDriveUserName
    {
        get;
        set;
    }
    /// <summary>
    /// OneDrive Password
    /// </summary>
    public string OneDrivePassword
    {
        get;
        set;
    }
    /// <summary>
    /// OneDrive Folder
    /// </summary>
    public string OneDriveFolderName
    {
        get;
        set;
    }
    #endregion
    
    
    
    public OneDriveHelper(CompanyProfile companyProfile)
    {
        this.ClientId = companyProfile.OneDriveClientId;
        this.ClientSecret = companyProfile.OneDriveSecret;
        this.OneDriveTenantId = companyProfile.OneDriveTenantId;
        this.OneDriveUserName = companyProfile.OneDriveUserName;
        this.OneDrivePassword = companyProfile.OneDrivePassword;
        this.OneDriveFolderName = companyProfile.OneDriveFolderName;
    }
    
    public HttpClient CreateHttpClient(string bearerToken = null)
    {
        var httpClient = new HttpClient();
        if(!string.IsNullOrEmpty(bearerToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", bearerToken);
        }
        return httpClient;
    }
    
    public async Task<string> GetOneDriveToken()
    {
        var oneDriveUrl = $"{OneDriveTokenUrl}{OneDriveTenantId}/oauth2/v2.0/token";
        var responseString = string.Empty;
        using(var client = CreateHttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            using(var request = new HttpRequestMessage(HttpMethod.Post, oneDriveUrl))
            {
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "password" }, { "client_id", ClientId }, { "client_secret", ClientSecret }, { "scope", "files.readwrite.all" }, { "username", OneDriveUserName }, { "password", OneDrivePassword } });
                using(var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                        responseString = await response.Content.ReadAsStringAsync();
                }
            }
        }
        if(!string.IsNullOrEmpty(responseString))
        {
            TokenResult(responseString);
        }
        return AccessToken;
    }
    
    private void TokenResult(string result)
    {
        JObject jo = JObject.Parse(result);
        this.AccessToken = jo.SelectToken("access_token").Value<string>();
        this.RefreshTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(jo.SelectToken("expires_in").Value<string>()));
        this.RefreshTime = DateTime.Now;
    }
    
    public async Task<string> GetFilesList()
    {
        var oneDriveUrl = $"{OneDriveApiRoot}me/drive/root";
        var responseString = string.Empty;
        using(var client = CreateHttpClient(AccessToken))
        {
            using(var request = new HttpRequestMessage(HttpMethod.Get, oneDriveUrl))
            {
                using(var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                        responseString = await response.Content.ReadAsStringAsync();
                }
            }
        }
        return responseString;
    }
    public async Task<string> GetFilesList(System.Web.Mvc.FileContentResult filePath, string oneDriveFilePath)
    {
        var oneDriveUrl = $"{OneDriveApiRoot}me/drive/" + (string.IsNullOrEmpty(OneDriveFolderName) ? "root" : "root:/" + OneDriveFolderName);
        //var oneDriveUrl = $"{OneDriveApiRoot}me/drive/root";
        var responseString = string.Empty;
        using(var client = CreateHttpClient(AccessToken))
        {
            using(var request = new HttpRequestMessage(HttpMethod.Get, oneDriveUrl))
            {
                using(var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    if(response.IsSuccessStatusCode)
                        responseString = await response.Content.ReadAsStringAsync();
                }
            }
            if(!string.IsNullOrEmpty(responseString))
            {
                JObject joroot = JObject.Parse(responseString);
                var driveId = joroot.SelectToken("parentReference").SelectToken("driveId").Value<string>();
                var folderId = joroot.SelectToken("id").Value<string>();
                responseString = await GetUploadSession(filePath, oneDriveFilePath, driveId, folderId);
            }
        }
        return responseString;
    }
    
    public async Task<string> GetFolder(string OneDriveFolderName)
    {
        var oneDriveUrl = $"{OneDriveApiRoot}me/drive/root:/" + OneDriveFolderName;
        var responseString = string.Empty;
        using(var client = CreateHttpClient(AccessToken))
        {
            using(var request = new HttpRequestMessage(HttpMethod.Get, oneDriveUrl))
            {
                using(var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    if(response.IsSuccessStatusCode)
                        responseString = await response.Content.ReadAsStringAsync();
                }
            }
        }
        return responseString;
    }
    
    public async Task<string> GetUploadSession(System.Web.Mvc.FileContentResult filePath, string oneDriveFilePath, string rootresult, string folderid)
    {
        Stream fileStream = new MemoryStream(filePath.FileContents);
        var oneDriveUrl = string.Empty;
        if(!string.IsNullOrEmpty(OneDriveFolderName))
            oneDriveUrl = $"{OneDriveApiRoot}drives/{rootresult}/items/{folderid}:/{oneDriveFilePath}:/content";
        else
            oneDriveUrl = $"{OneDriveApiRoot}drives/{rootresult}/items/{folderid}/children/{oneDriveFilePath}/content";
        var responseString = string.Empty;
        using(var client = CreateHttpClient(AccessToken))
        {
            using(var content = new StreamContent(fileStream))
            {
                content.Headers.Add("Content-Type", "application/octet-stream");
                using(var request = new HttpRequestMessage(HttpMethod.Put, oneDriveUrl))
                {
                    request.Content = content;
                    using(var response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        if(response.IsSuccessStatusCode)
                            responseString = await response.Content.ReadAsStringAsync();
                    }
                }
                if(!string.IsNullOrEmpty(responseString))
                {
                    JObject jo = JObject.Parse(responseString);
                    string fileId = jo.SelectToken("id").Value<string>();
                    responseString = fileId;
                    //await GetShareLinkAsync(fileId);
                }
            }
        }
        return responseString;
    }
    
    public async Task<string> GetShareLinkAsync(string fileID)
    {
        var payload = new OneDriveRequestShare
        {
            type = "edit",
            scope = "organization"
        };
        var stringPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
        var oneDriveUrl = $"{OneDriveApiRoot}me/drive/items/{fileID}/createLink";
        var responseString = string.Empty;
        using(var client = CreateHttpClient(AccessToken))
        {
            using(var content = new StringContent(stringPayload, Encoding.UTF8, "application/json"))
            {
                using(var request = new HttpRequestMessage(HttpMethod.Post, oneDriveUrl))
                {
                    request.Content = content;
                    using(var response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        if(response.IsSuccessStatusCode)
                            responseString = await response.Content.ReadAsStringAsync();
                    }
                }
                if(!string.IsNullOrEmpty(responseString))
                {
                    JObject jodoc = JObject.Parse(responseString);
                    responseString = jodoc.SelectToken("link").SelectToken("webUrl").Value<string>();
                }
            }
        }
        return responseString;
    }
    
    public async Task<byte[]> DownloadFile(string fileID)
    {
        var oneDriveUrl = $"{OneDriveApiRoot}me/drive/items/{fileID}/content";
        using(var client = CreateHttpClient(AccessToken))
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using(var request = new HttpRequestMessage(HttpMethod.Get, oneDriveUrl))
            {
                using(var response = await client.GetAsync(oneDriveUrl).ConfigureAwait(false))
                {
                    if(response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsByteArrayAsync();
                        return responseString;
                    }
                }
            }
        }
        return null;
    }
}
public class OneDriveRequestShare
{
    public string type
    {
        get;
        set;
    }
    public string scope
    {
        get;
        set;
    }
}
}
