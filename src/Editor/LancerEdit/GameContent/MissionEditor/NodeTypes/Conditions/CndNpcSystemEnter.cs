using System;
using System.Collections.Generic;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions.Conditions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;
public class CndNpcSystemEnter : NodeTriggerEntry
{
    public override string Name => "On NPC System Enter";

    public Cnd_NPCSystemEnter Data;
    public CndNpcSystemEnter(Entry entry): base(NodeColours.Condition)
    {
        Data = entry is null ? new() : new(entry);

    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        Controls.InputTextId("System", ref Data.system);
        Controls.InputStringList("Ships", Data.ships);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
