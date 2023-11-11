using Microsoft.JSInterop;

namespace ButtsBlazor.Client.Components
{
    public record GradioProgressData(double? Progress, int? Index, int? Length, string? Unit, string? Desc);

    public record GradioStatus(bool Queue, string? Code, bool? Success, string Stage, bool? Broken,
        int? Size, int? position, double? Eta, string? Message, GradioProgressData? progress_data, DateTime? Timetime);
public class CallbackHelper<TArg>
    {
        private Func<TArg?,Task> action;

        public CallbackHelper(Func<TArg?,Task> action)
        {
            this.action = action;
        }

        [JSInvokable]
        public async Task Callback(TArg? arg)
        {
            await action.Invoke(arg);
        }
    }
}
