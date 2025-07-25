﻿using System.Linq;
using LibreLancer.Data.Ini;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActSpawnLoot : NodeTriggerEntry
{
    public override string Name => "Spawn Loot";

    public readonly Act_SpawnLoot Data;
    public ActSpawnLoot(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SpawnLoot(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        nodePopups.StringCombo("Loot", Data.Loot, s => Data.Loot = s, lookups.Loots);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
