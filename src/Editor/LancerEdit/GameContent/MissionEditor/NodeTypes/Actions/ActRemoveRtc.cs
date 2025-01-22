﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActRemoveRtc : NodeTriggerEntry
{
    public override string Name => "Remove Real-Time Cutscene";

    public readonly Act_RemoveRTC Data;
    public ActRemoveRtc(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() :  new Act_RemoveRTC(action);;

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        Controls.InputTextId("RTC", ref Data.RTC);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
