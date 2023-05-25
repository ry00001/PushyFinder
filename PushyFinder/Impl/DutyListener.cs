using Dalamud.Logging;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using PushyFinder.Delivery;
using PushyFinder.Util;

namespace PushyFinder.Impl;

public class DutyListener
{
    public static void On()
    {
        PluginLog.Debug("DutyListener On");
        Service.ClientState.CfPop += OnDutyPop;
    }

    public static void Off()
    {
        PluginLog.Debug("DutyListener Off");
        Service.ClientState.CfPop -= OnDutyPop;
    }

    private static void OnDutyPop(object? _, ContentFinderCondition e)
    {
        if (!Plugin.Configuration.EnableForDutyPops)
            return;

        if (!CharacterUtil.IsClientAfk())
            return;
        
        var dutyName = e.RowId == 0 ? "Duty Roulette" : e.Name.ToDalamudString().TextValue;
        DeliveryManager.Deliver().Invoke($"Duty pop", $"Duty registered: '{dutyName}'.");
    }
}
