﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActChangeState : BlueprintNode
{
    protected override string Name => "Change State";

    private readonly Act_ChangeState data;
    public NodeActChangeState(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_ChangeState(action);
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.Checkbox("Success", ref data.Succeed);
    }
}
