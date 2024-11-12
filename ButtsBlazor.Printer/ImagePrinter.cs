using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Shared.Services;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ButtsBlazor.Printer;

public class ImageUploadMonitor(MqttFactory clientFactory, 
    PrintConfig config, 
    ManagedMqttClientOptionsBuilder optionsBuilder,
    PrintQueue printQueue,
    ILogger<Logger<ImageUploadMonitor>> logger) : BackgroundService
{
    private MqttClientSubscribeResult? _subscription;
    protected string TopicFilter { get; } = config.SiteConfig.MqttTopic(ImageType.Photo);

    private CancellationTokenSource? _internalSource;
    private CancellationToken _internalToken;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(_internalSource != null)
            await _internalSource.CancelAsync();
        _internalSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _internalToken = _internalSource.Token;
        using var mqttClient = clientFactory.CreateManagedMqttClient();
        await mqttClient.SubscribeAsync(TopicFilter, MqttQualityOfServiceLevel.AtMostOnce);
        mqttClient.ApplicationMessageReceivedAsync += MessageReceived;
        try
        {
            await mqttClient.StartAsync(optionsBuilder.Build());
//            var pending = mqttClient.
            while (!stoppingToken.IsCancellationRequested)
            {
                var printJob = await _printQueue.Reader.ReadAsync(stoppingToken);
                await printQueue.Print(printJob,stoppingToken);
            }
        }finally {
            mqttClient.ApplicationMessageReceivedAsync -= MessageReceived;
            if(!stoppingToken.IsCancellationRequested)
                await mqttClient.StopAsync();
        }
    }

    private readonly Channel<PrintJob> _printQueue = Channel.CreateBounded<PrintJob>(config.PrintQueueLength);
    private async Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        var message = args.ApplicationMessage;
        try
        {
            logger.LogInformation($"Received message on {message.Topic}: {args.TryToJson()}");
            var payload = message.ConvertPayloadToString();
            var newImageResult = JsonSerializer.Deserialize<UploadResult>(payload) ??
                                 throw new InvalidOperationException($"Could not serialize {payload} to {typeof(UploadResult)}");
            await _printQueue.Writer.WriteAsync(new PrintJob(newImageResult));
        }
        catch(Exception e)
        {
            logger.LogError(e, "Error processing message on {Topic}", message.Topic);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _internalSource?.Cancel(true);
        _internalSource?.Dispose();
        _internalSource = null;
    }
}

public record PrintJob(UploadResult UploadResult);