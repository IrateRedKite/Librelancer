﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActInvulnerable : BlueprintNode
{
    protected override string Name => "Set Invulnerable";

    private readonly Act_Invulnerable data;
    public NodeActInvulnerable(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_Invulnerable(action);
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.Checkbox("Is Invulnerable", ref data.Invulnerable);
    }
}
