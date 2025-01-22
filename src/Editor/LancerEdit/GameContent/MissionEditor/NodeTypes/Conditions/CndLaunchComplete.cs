using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions.Conditions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;

public class CndLaunchComplete : NodeTriggerEntry
{
    public override string Name => "On Ship Launch Complete";

    public Cnd_LaunchComplete Data;
    public CndLaunchComplete(Entry entry): base(NodeColours.Condition)
    {
        Data = entry is null ? new() : new(entry);

    }

    public override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        Controls.InputTextId("Ship", ref Data.ship);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
