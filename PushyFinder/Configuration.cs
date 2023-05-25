using Dalamud.Configuration;
using Dalamud.Plugin;
using PushyFinder.Delivery;
using System;

namespace PushyFinder
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public Deliveries DeliveryService { get; set; } = Deliveries.Pushover;

        public string PushoverAppKey { get; set; } = "";
        public string PushoverUserKey { get; set; } = "";
        public string PushoverDevice { get; set; } = "";

        public string NtfyTopic { get; set; } = "";
        public string NtfyDomain { get; set; } = "https://ntfy.sh/";

        public bool EnableForDutyPops { get; set; } = true;
        public bool IgnoreAfkStatus { get; set; } = false;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
