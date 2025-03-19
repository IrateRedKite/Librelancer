﻿using LibreLancer.Data.Ini;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class ActDisableFriendlyFire : NodeTriggerEntry
{
    public override string Name => "Disable Friendly Fire";

    public readonly Act_DisableFriendlyFire Data;
    public ActDisableFriendlyFire(MissionAction action): base( NodeColours.Action)
    {
        Data = action is null ? new() : new Act_DisableFriendlyFire(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        // TODO: Comboify some how?
        Controls.InputStringList("Objects & Labels", Data.ObjectsAndLabels);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
