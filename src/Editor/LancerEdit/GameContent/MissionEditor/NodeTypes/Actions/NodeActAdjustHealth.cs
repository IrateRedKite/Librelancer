﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActAdjustHealth : TriggerEntryNode
{
    protected override string Name => "Adjust Health";

    public readonly Act_AdjHealth Data;
    public NodeActAdjustHealth(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new Act_AdjHealth() : new Act_AdjHealth(action);;

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        Controls.InputTextId("Target", ref Data.Target);
        ImGui.SliderFloat("Health", ref Data.Adjustment, -1f, 1f, "%.2f", ImGuiSliderFlags.AlwaysClamp);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
