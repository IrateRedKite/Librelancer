﻿using LibreLancer.Data.Ini;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActDisableEncounter : NodeTriggerEntry
{
    public override string Name => "Disable Encounter";

    public readonly Act_DisableEnc Data;
    public ActDisableEncounter(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_DisableEnc(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        Controls.InputTextId("Encounter", ref Data.Encounter);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
