﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActDockRequest : TriggerEntryNode
{
    protected override string Name => "Start Dock Request with Object";

    public readonly Act_DockRequest Data;
    public NodeActDockRequest(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_DockRequest(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        Controls.InputTextId("Object", ref Data.Object);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
