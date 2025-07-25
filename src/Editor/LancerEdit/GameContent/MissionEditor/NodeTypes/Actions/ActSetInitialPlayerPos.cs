﻿using ImGuiNET;
using LibreLancer.Data.Ini;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActSetInitialPlayerPos : NodeTriggerEntry
{
    public override string Name => "Set Initial Player Position";

    public readonly Act_SetInitialPlayerPos Data;
    public ActSetInitialPlayerPos(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SetInitialPlayerPos(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        ImGui.InputFloat3("Position", ref Data.Position);
        Controls.InputFlQuaternion("Orientation", ref Data.Orientation);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
