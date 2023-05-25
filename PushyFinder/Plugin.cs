using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using PushyFinder.Impl;
using PushyFinder.Util;
using PushyFinder.Windows;

namespace PushyFinder
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "PushyFinder";
        private const string CommandName = "/pushyfinder";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        
        // This *is* used.
#pragma warning disable CS8618
        public static Configuration Configuration { get; private set; }
#pragma warning restore
        
        public WindowSystem WindowSystem = new("PushyFinder");

        private ConfigWindow ConfigWindow { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            pluginInterface.Create<Service>();
            
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(this.PluginInterface);
            
            ConfigWindow = new ConfigWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the configuration window."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            CrossWorldPartyListSystem.Start();
            PartyListener.On();
            DutyListener.On();
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            ConfigWindow.Dispose();

            CrossWorldPartyListSystem.Stop();
            PartyListener.Off();
            DutyListener.Off();

            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            if (args == "debugOnlineStatus")
            {
                Service.ChatGui.Print($"OnlineStatus ID = {Service.ClientState.LocalPlayer.OnlineStatus.Id}");
                return;
            }
            
            ConfigWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
