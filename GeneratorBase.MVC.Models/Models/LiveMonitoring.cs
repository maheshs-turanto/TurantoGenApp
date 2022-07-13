using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
namespace GeneratorBase.MVC.Models
{
/// <summary>A live monitoring.</summary>
[HubName("iServerHub")]
public class LiveMonitoring : Hub
{
    /// <summary>Send this message.</summary>
    ///
    /// <param name="name">   The name.</param>
    /// <param name="message">The message.</param>
    
    public void Send(string name, string message)
    {
        // Call the broadcastMessage method to update clients.
        Clients.All.broadcastMessage(name, message);
        //Clients.All.addNewMessageToPage(name, message);
    }
    
    /// <summary>Sends a message.</summary>
    ///
    /// <param name="msg">The message.</param>
    
    public static void SendMessage(string msg)
    {
        var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveMonitoring>();
        hubContext.Clients.All.broadcastMessage(msg);
    }
    
    /// <summary>Called when the connection connects to this hub instance.</summary>
    ///
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
    
    public override Task OnConnected()
    {
        UserHandler.ConnectedIds.Add((Context.QueryString["userid"]));
        return base.OnConnected();
    }
    
    /// <summary>Called when a connection disconnects from this hub gracefully or due to a timeout.</summary>
    ///
    /// <param name="stopCalled">   true, if stop was called on the client closing the connection
    ///                             gracefully;
    ///                             false, if the connection has been lost for longer than the
    ///                             <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
    ///                             Timeouts can be caused by clients reconnecting to another SignalR
    ///                             server in scaleout.</param>
    ///
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
    
    public override Task OnDisconnected(bool stopCalled)
    {
        UserHandler.ConnectedIds.Remove((Context.QueryString["userid"]));
        return base.OnDisconnected(stopCalled);
    }
}

/// <summary>A user handler.</summary>
public static class UserHandler
{
    /// <summary>List of identifiers for the connected.</summary>
    public static HashSet<string> ConnectedIds = new HashSet<string>();
}
/// <summary>A data live monitoring.</summary>
public class DataLiveMonitoring
{
    /// <summary>Gets or sets the username.</summary>
    ///
    /// <value>The username.</value>
    
    public string username
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entityname.</summary>
    ///
    /// <value>The entityname.</value>
    
    public string entityname
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the action.</summary>
    ///
    /// <value>The action.</value>
    
    public string action
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the anchorid.</summary>
    ///
    /// <value>The anchorid.</value>
    
    public string anchorid
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entityid.</summary>
    ///
    /// <value>The entityid.</value>
    
    public string entityid
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the entitydisplayname.</summary>
    ///
    /// <value>The entitydisplayname.</value>
    
    public string entitydisplayname
    {
        get;
        set;
    }
    
    /// <summary>Gets or sets the datetime.</summary>
    ///
    /// <value>The datetime.</value>
    
    public string datetime
    {
        get;
        set;
    }
}
}