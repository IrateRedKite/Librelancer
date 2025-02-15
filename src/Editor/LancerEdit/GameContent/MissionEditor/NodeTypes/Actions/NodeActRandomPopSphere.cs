﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActRandomPopSphere : TriggerEntryNode
{
    protected override string Name => "Toggle Random Population Sphere";

    public readonly Act_RandomPopSphere Data;
    public NodeActRandomPopSphere(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_RandomPopSphere(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        ImGui.InputFloat3("Position", ref Data.Position);
        ImGui.InputFloat("Radius", ref Data.Radius);
        ImGui.Checkbox("Enable", ref Data.On);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
