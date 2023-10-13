using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using CliWrap;
using CliWrap.EventStream;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Invokable;

public class ButtPromptProcess(ILogger<ButtPromptProcess> logger, IOptions<ButtOptions> options,
        ButtRenderTracker tracker) : IInvocable, IInvocableWithPayload<ButtPromptArgs?>
{
    private readonly ButtOptions options = options.Value;
    public ButtPromptArgs? Payload { get; set; }
    public CancellationToken Token { get; set; }
    public Guid? Id { get; set; }
    public async Task Invoke()
    {
        Payload ??= new ButtPromptArgs();
        Id = tracker.ProcessStarted(this);
        var def = options.DefaultArgs;
        var prompt = (Payload.Prompt ?? def.Prompt!)+ ", Nikon Z9, Canon 5d, masterpiece";
        var filename = (Payload.OutputFileName ?? def.OutputFileName!) + $"-{DateTime.UtcNow.Ticks}.png";
        var cmd = Cli.Wrap(options.ButtPromptExe)
            .WithWorkingDirectory(options.WorkingDirectory)
            .WithArguments(ab =>
            {
                ab.Add(new []
                {
                    options.PythonScript,
                    "-p", prompt,
                    "-n", Payload.NegativePrompt ?? def.NegativePrompt!,
                    "-i", Path.Combine(options.ControlImagePath, Payload.ControlImageName ?? def.ControlImageName!),
                    "-s", (Payload.ControlImageSize ?? def.ControlImageSize).ToString()!,
                    "-c", (Payload.ConditioningScale ?? def.ConditioningScale!).Value.ToString("F2"),
                    "-l", (Payload.CannyLowThreshold ?? def.CannyLowThreshold).ToString()!,
                    "-u", (Payload.CannyHighThreshold ?? def.CannyHighThreshold).ToString()!,
                    "-o", Path.Combine(options.PromptOutputPath, filename)
                }, true);
            });
        logger.LogInformation($"Executing Cmd: {cmd.ToString()}");
        await foreach (var cmdEvent in cmd.ListenAsync(Token))
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent started:
                    logger.LogInformation($"Process started; ID: {started.ProcessId}");
                    tracker.StatusMessage(Id.Value, "Processing Started...");
                    break;
                case StandardOutputCommandEvent outputCmd:
                    logger.LogInformation(outputCmd.Text);
                    var output = outputCmd.Text.Split(':');
                    if (outputCmd.Text.StartsWith("output:"))
                        await tracker.OutputFile(Id.Value, "/buttoutput/" +  Path.GetFileName(outputCmd.Text.Substring("output:".Length).Trim()));
                    break;
                case StandardErrorCommandEvent errorCmd:
                    logger.LogInformation(errorCmd.Text);
                    tracker.StatusMessage(Id.Value, errorCmd.Text);
                    break;
                case ExitedCommandEvent exited:
                    tracker.StatusMessage(Id.Value, "Processing Complete.");
                    logger.LogInformation($"Process exited; Code: {exited.ExitCode}");
                    break;
            }
        }
    }
}