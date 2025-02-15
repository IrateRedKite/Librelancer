﻿using System.Linq;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActInvulnerable : TriggerEntryNode
{
    protected override string Name => "Set Invulnerable";

    public readonly Act_Invulnerable Data;
    public NodeActInvulnerable(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_Invulnerable(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var objects = missionIni.Ships.Select(x => x.Nickname).Concat(missionIni.Solars.Select(x => x.Nickname)).Order().ToArray();
        nodePopups.StringCombo("Objective", Data.Object, s => Data.Object = s, objects);

        ImGui.Checkbox("Is Invulnerable", ref Data.Invulnerable);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
