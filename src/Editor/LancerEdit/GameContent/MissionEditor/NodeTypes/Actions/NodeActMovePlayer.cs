﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActMovePlayer : BlueprintNode
{
    protected override string Name => "Move Player";

    private readonly Act_MovePlayer data;
    public NodeActMovePlayer(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_MovePlayer(action);
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.InputFloat3("Position", ref data.Position);
        ImGui.InputFloat("Unknown", ref data.Unknown);
    }
}
