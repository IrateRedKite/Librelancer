﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActPlaySound : TriggerEntryNode
{
    protected override string Name => "Play sound";

    public readonly Act_PlaySoundEffect Data;
    public NodeActPlaySound(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_PlaySoundEffect(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        Controls.InputTextId("Sound Id", ref Data.Effect);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
