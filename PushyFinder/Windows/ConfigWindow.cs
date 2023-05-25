using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PushyFinder.Delivery;
using PushyFinder.Util;

namespace PushyFinder.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    
    public ConfigWindow(Plugin plugin) : base(
        "PushyFinder Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        Configuration = Plugin.Configuration;
    }

    public void Dispose() { }

    private TimedBool notifSentMessageTimer = new(3.0f);

    public override void Draw()
    {
        var service = Configuration.DeliveryService;
        if (ImGui.BeginCombo("Service", service.ToString()))
        {
            foreach (var item in Enum.GetValues<Deliveries>())
            {
                if (ImGui.Selectable(item.ToString(), Configuration.DeliveryService == item)) Configuration.DeliveryService = item;
            }
        }
        if (service == Deliveries.Pushover)
        {
            {
                var cfg = Configuration.PushoverAppKey;
                if (ImGui.InputText("Application key", ref cfg, 2048u))
                {
                    Configuration.PushoverAppKey = cfg;
                }
            }
            {
                var cfg = Configuration.PushoverUserKey;
                if (ImGui.InputText("User key", ref cfg, 2048u))
                {
                    Configuration.PushoverUserKey = cfg;
                }
            }
            {
                var cfg = Configuration.PushoverDevice;
                if (ImGui.InputText("Device name", ref cfg, 2048u))
                {
                    Configuration.PushoverDevice = cfg;
                }
            }
        }
        else if (service == Deliveries.Ntfy)
        {
            {
                var cfg = Configuration.NtfyTopic;
                if (ImGui.InputText("Topic", ref cfg, 2048u))
                {
                    Configuration.NtfyTopic = cfg;
                }
            }
            {
                var cfg = Configuration.NtfyDomain;
                if (ImGui.InputText("Domain", ref cfg, 2048u))
                {
                    Configuration.NtfyDomain = cfg;
                }
            }
        }
        {
            var cfg = Configuration.EnableForDutyPops;
            if (ImGui.Checkbox("Send message for duty pop?", ref cfg))
            {
                Configuration.EnableForDutyPops = cfg;
            }
        }

        if (ImGui.Button("Send test notification"))
        {
            notifSentMessageTimer.Start();
            DeliveryManager.Deliver().Invoke("Test notification", 
                                     "If you received this, PushyFinder is configured correctly.");
        }

        if (notifSentMessageTimer.Value)
        {
            ImGui.SameLine();
            ImGui.Text("Notification sent!");
        }

        {
            var cfg = Configuration.IgnoreAfkStatus;
            if (ImGui.Checkbox("Ignore AFK status and always notify", ref cfg))
            {
                Configuration.IgnoreAfkStatus = cfg;
            }
        }

        if (!Configuration.IgnoreAfkStatus)
        {
            if (!CharacterUtil.IsClientAfk())
            {
                var red = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                ImGui.TextColored(red, "This plugin will only function while your client is AFK (/afk, red icon)!");

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("The reasoning for this is that if you are not AFK, you are assumed to");
                    ImGui.Text("be at your computer, and ready to respond to a join or a duty pop.");
                    ImGui.Text("Notifications would be bothersome, so they are disabled.");
                    ImGui.EndTooltip();
                }
            }
            else
            {
                var green = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                ImGui.TextColored(green, "You are AFK. The plugin is active and notifications will be served.");
            }
        }

        if (ImGui.Button("Save and close"))
        {
            Configuration.Save();
            IsOpen = false;
        }
    }
}
