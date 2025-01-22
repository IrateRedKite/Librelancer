﻿using System.Linq;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActRemoveCargo : NodeTriggerEntry
{
    public override string Name => "Remove Cargo";

    public readonly Act_RemoveCargo Data;
    public ActRemoveCargo(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_RemoveCargo(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        nodePopups.StringCombo("Cargo", Data.Cargo, s => Data.Cargo = s, gameData.GoodsByName);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
