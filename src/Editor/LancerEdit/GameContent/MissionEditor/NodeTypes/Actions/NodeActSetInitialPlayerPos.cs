﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSetInitialPlayerPos : BlueprintNode
{
    protected override string Name => "Set Initial Player Position";

    private readonly Act_SetInitialPlayerPos data;
    public NodeActSetInitialPlayerPos(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_SetInitialPlayerPos(action);
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.InputFloat3("Position", ref data.Position);
        Controls.InputFlQuaternion("Orientation", ref data.Orientation);
    }
}
