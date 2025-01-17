using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions.Conditions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;

public class NodeCndSpaceExit : TriggerEntryNode
{
    protected override string Name => "On Space Exit";

    public Cnd_SpaceExit Data = new();
    public NodeCndSpaceExit(ref int id, Entry entry) : base(ref id, NodeColours.Condition)
    {
        Inputs.Add(new NodePin(this, LinkType.Condition, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
