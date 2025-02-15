﻿using System.Linq;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActCloak : TriggerEntryNode
{
    protected override string Name => "Act Cloak";

    public readonly Act_Cloak Data;
    public NodeActCloak(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_Cloak(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var ships = missionIni.Ships.Select(x => x.Nickname).Order().ToArray();
        nodePopups.StringCombo("Ship", Data.Target, s => Data.Target = s, ships);
        ImGui.Checkbox("Cloak", ref Data.Cloaked);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
