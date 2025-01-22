﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActCanTradeLane : NodeTriggerEntry
{
    public override string Name => "Toggle Player Docking (Tradelane) Ability";

    public readonly Act_PlayerCanTradelane Data;
    public ActCanTradeLane(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_PlayerCanTradelane(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        ImGui.Checkbox("Can Dock", ref Data.CanDock);
        Controls.InputStringList("Exceptions", Data.Exceptions);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
