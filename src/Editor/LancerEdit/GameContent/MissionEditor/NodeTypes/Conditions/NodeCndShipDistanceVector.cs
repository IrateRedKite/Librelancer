using System;
using System.Numerics;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions.Conditions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;

public class NodeCndShipDistanceVector : TriggerEntryNode
{
    protected override string Name => "On Ship Distance Change (Position)";

    public Cnd_DistVec Data;

    public NodeCndShipDistanceVector(ref int id, Entry entry) : base(ref id, NodeColours.Condition)
    {
        Data = entry is null ? new() : new(entry);

        Inputs.Add(new NodePin(this, LinkType.Condition, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        Controls.InputTextId("Source Ship", ref Data.sourceShip);

        ImGui.Checkbox("Inside", ref Data.inside);
        Controls.HelpMarker(
            "Whether the source ship should be within (true) the specified distance, or if the condition is " +
            "triggered when the source ship is at least the specified distance away from the destination object.",
            true);

        ImGui.InputFloat3("Position", ref Data.position, "%.0f");
        ImGui.SliderFloat("Distance", ref Data.distance, 0.0f, 100000.0f, "%.0f", ImGuiSliderFlags.AlwaysClamp);
        ImGui.Checkbox("Tick Away", ref Data.tickAway);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
