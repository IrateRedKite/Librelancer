﻿using ImGuiNET;
using LibreLancer.Data.Ini;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActHostileClamp : NodeTriggerEntry
{
    public override string Name => "Toggle Hostile Clamp";

    public readonly Act_HostileClamp Data;
    public ActHostileClamp(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_HostileClamp(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        ImGui.Checkbox("Enable", ref Data.Enabled);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
