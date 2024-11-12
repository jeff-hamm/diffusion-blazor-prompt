using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Shared;
using ButtsBlazor.Printer;
using DataUtils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Polly;
using Polly.Extensions.Http;

namespace ButtsBlazor.Shared.Services;
public delegate Task<IMqttClient> MqttConnectedClient();
public delegate Task<IManagedMqttClient> MqttManagedConnectedClient();

public static class ClientConfigExtensions
{

    public static TObject LogObject<TLog, TObject>(this ILogger<TLog> @this, TObject value,
        CancellationToken token = default, LogLevel level = LogLevel.Debug) =>
        @this.LogObject(null, value, token, level);
    public static TObject LogObject<TLog,TObject>(this ILogger<TLog> @this, string? message, TObject value, CancellationToken token=default,LogLevel level=LogLevel.Debug)
    {
        var output = "NULL";
        if (value != null)
            output = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = true
            });
                
        
        @this.Log(logLevel:level, $"[{message}: {value?.GetType().Name}]:\r\n{output}");
        return value;
    }

    private static readonly SemaphoreSlim synclock = new(1);
    private static readonly SemaphoreSlim managedsynclock = new(1);
    public static IHostApplicationBuilder AddMqtt(this IHostApplicationBuilder @this, ClusterConfig config)
    {
        AddMqttClient(@this, config);
        AddManagedMqttClient(@this,config);
        return @this;
    }
    private static void AddManagedMqttClient(IHostApplicationBuilder @this, ClusterConfig config)
    {
        @this.Services.AddSingleton(sp => new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(config.ReconnectDelay)
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(config.ClientId)
                .WithTcpServer(config.MqttServer)
                .WithTlsOptions(o =>
                {
                    o.UseTls(config.UseTls);
                }).Build())
        );
        @this.Services.AddTransient(sp => 
            sp.GetRequiredService<ManagedMqttClientOptionsBuilder>().Build());
        @this.Services.AddSingleton<MqttManagedClientAccessor>();
        MqttManagedConnectedClient ConnectedClient(IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILogger<MqttConnectedClient>>();
            var optionsBuilder = sp.GetRequiredService<ManagedMqttClientOptionsBuilder>();
            var accessor = sp.GetRequiredService<MqttManagedClientAccessor>();
            return async () =>
            {
                var client = //accessor.MqttClient ??= 
                    sp.GetRequiredService<MqttFactory>().CreateManagedMqttClient();;
                if (client.IsConnected) return client;
                await managedsynclock.WaitAsync();
                try
                {
                    if (client.IsConnected) return client;
                    logger.LogInformation($"Connecting to MQTT: {config.MqttServer} as {config.ClientId} {config.UseTls}");
                    await client.StartAsync(optionsBuilder.Build());
                    logger.LogObject("Connected!");
                    return client;
                }
                finally
                {
                    managedsynclock.Release();
                }
            };
        }
//        @this.Services.AddTransient(ConnectedClient);
        @this.Services.AddScoped(ConnectedClient);
        @this.Services.AddSingleton(ConnectedClient);
    }

    private static void AddMqttClient(IHostApplicationBuilder @this, ClusterConfig config)
    {
        @this.Services.AddSingleton<MqttFactory>();
        @this.Services.AddSingleton<MqttClientAccessor>();
        MqttConnectedClient ConnectedClient(IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILogger<MqttConnectedClient>>();
            var optionsBuilder = new MqttClientOptionsBuilder().WithClientId(config.ClientId)
                .WithTcpServer(config.MqttServer)
                .WithTlsOptions(o => { o.UseTls(config.UseTls); });
            var accessor = sp.GetRequiredService<MqttClientAccessor>();
            return async () =>
            {
                var client = //accessor.MqttClient ??=
                     sp.GetRequiredService<MqttFactory>().CreateMqttClient();;
                if (client.IsConnected) return client;
                await synclock.WaitAsync();
                try
                {
                    if (client.IsConnected) return client;
                    logger.LogInformation($"Connecting to MQTT: {config.MqttServer} as {config.ClientId} {config.UseTls}");
                    var connectResult = await client.ConnectAsync(optionsBuilder.Build());
                    logger.LogObject("Connection Result: ", connectResult);
                    return client;
                }
                finally
                {
                    synclock.Release();
                }
            };
        }
//        @this.Services.AddTransient(ConnectedClient);
        @this.Services.AddScoped(ConnectedClient);
        @this.Services.AddSingleton(ConnectedClient);
    }

    public static void AddRetryPolicy(this Microsoft.Extensions.DependencyInjection.IHttpClientBuilder builder) =>
        builder.AddPolicyHandler(GetRetryPolicy());

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }


    public static string ConvertToBase64(this Stream stream)
    {
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        return Convert.ToBase64String(bytes);
    }

    public static string ToDataUrlStream(
        this Stream content,
        string contentType,
        IEnumerable<KeyValuePair<string, string>>? parameters = null)
    {

        StringBuilder stringBuilder = new StringBuilder("data:" + contentType);
        stringBuilder.Append(";base64");
        foreach (KeyValuePair<string, string> parameter in parameters ?? [])
            stringBuilder.Append(
                ";" + WebUtility.UrlEncode(parameter.Key) + "=" + WebUtility.UrlEncode(parameter.Value));
        stringBuilder.Append(",");
        stringBuilder.Append(content.ConvertToBase64());
//        using var cryptoStream = new CryptoStream(content, new ToBase64Transform(), CryptoStreamMode.Read);
        return stringBuilder.ToString();
    }
}


class ConcatenatedStream(params Stream[] streams) : Stream
{
    private readonly Queue<Stream> _streams = new(streams);

    public override bool CanRead
    {
        get { return true; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (count > 0 && _streams.Count > 0)
        {
            int bytesRead = _streams.Peek().Read(buffer, offset, count);
            if (bytesRead == 0)
            {
                _streams.Dequeue().Dispose();
                continue;
            }

            totalBytesRead += bytesRead;
            offset += bytesRead;
            count -= bytesRead;
        }

        return totalBytesRead;
    }

    public override bool CanSeek
    {
        get { return false; }
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override long Length
    {
        get { throw new NotImplementedException(); }
    }

    public override long Position
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}