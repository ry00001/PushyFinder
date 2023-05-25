using System.Linq;
using Dalamud.Game.ClientState.Party;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using PushyFinder.Delivery;
using PushyFinder.Util;

namespace PushyFinder.Impl;

public static class PartyListener
{
    public static void On()
    {
        PluginLog.Debug("PartyListener On");
        CrossWorldPartyListSystem.OnJoin += OnJoin;
        CrossWorldPartyListSystem.OnLeave += OnLeave;
    }
    
    public static void Off()
    {
        PluginLog.Debug("PartyListener Off");
        CrossWorldPartyListSystem.OnJoin -= OnJoin;
        CrossWorldPartyListSystem.OnLeave -= OnLeave;
    }

    private static void OnJoin(CrossWorldPartyListSystem.CrossWorldMember m)
    {
        if (!CharacterUtil.IsClientAfk()) return;

        var jobAbbr = LuminaDataUtil.GetJobAbbreviation(m.JobId);

        if (m.PartyCount == 8)
        {
            DeliveryManager.Deliver().Invoke("Party full",
                                     $"{m.Name} (Lv{m.Level} {jobAbbr}) joins the party.\nParty recruitment ended. All spots have been filled.");
        }
        else
        {
            DeliveryManager.Deliver().Invoke($"{m.PartyCount}/8: Party join",
                                     $"{m.Name} (Lv{m.Level} {jobAbbr}) joins the party.");
        }
    }
    
    private static void OnLeave(CrossWorldPartyListSystem.CrossWorldMember m)
    {
        if (!CharacterUtil.IsClientAfk()) return;
        
        var jobAbbr = LuminaDataUtil.GetJobAbbreviation(m.JobId);

        DeliveryManager.Deliver().Invoke($"{m.PartyCount-1}/8: Party leave",
                                 $"{m.Name} (Lv{m.Level} {jobAbbr}) has left the party.");
    }
}
