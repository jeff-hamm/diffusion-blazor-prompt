using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ButtsBlazor.Client.Shared;
public class ClusterConfig
{
    static readonly string DefaultClientId = Assembly.GetEntryAssembly()?.FullName ?? "ButtsBlazor";
    public string PublicUrl { get; set; } = "http://localhost:5023";
    public Uri PublicUri => new Uri("http://localhost:5023"); 
    public string MqttServer { get; set; } = "localhost";
    public string ClientId { get; set; } = DefaultClientId;
    public bool UseTls { get; set; }
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);
}

