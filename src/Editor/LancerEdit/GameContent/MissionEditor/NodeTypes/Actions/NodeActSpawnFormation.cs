﻿using System.Linq;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSpawnFormation : TriggerEntryNode
{
    protected override string Name => "Spawn Formation";

    public readonly Act_SpawnFormation Data;
    public NodeActSpawnFormation(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SpawnFormation(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var formations = missionIni.Formations.Select(x => x.Nickname).Order().ToArray();
        nodePopups.StringCombo("Formation", Data.Formation, s => Data.Formation = s, formations);
        Controls.InputVec3Nullable("Position", ref Data.Position);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
