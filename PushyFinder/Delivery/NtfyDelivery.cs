using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dalamud.Logging;
using Flurl.Http;

namespace PushyFinder.Delivery;

public abstract class NtfyDelivery : IDelivery
{
    public static string Name => "Ntfy.sh";

    public static void Deliver(string title, string text = "")
    {
        if (Plugin.Configuration.NtfyTopic.Length == 0 ||
            Plugin.Configuration.NtfyDomain.Length == 0) return;
        
        Task.Run(() => DeliverAsync(title, text));
    }

    private static async void DeliverAsync(string title, string text)
    {
        try
        {
            var url = Plugin.Configuration.NtfyDomain + Plugin.Configuration.NtfyTopic;

            await url.WithHeader("Title", title).PostStringAsync(text);
        }
        catch (FlurlHttpException e)
        {
            PluginLog.Error($"Failed to make ntfy req: '{e.Message}'");
            PluginLog.Error($"{e.StackTrace}");
        }
    }
}
