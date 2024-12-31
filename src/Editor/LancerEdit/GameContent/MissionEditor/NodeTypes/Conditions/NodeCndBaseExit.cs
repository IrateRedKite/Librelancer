using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;

public class NodeCndBaseExit : BlueprintNode
{
    protected override string Name => "On Base Exit";

    private string @base = string.Empty;
    public NodeCndBaseExit(ref int id, Entry entry) : base(ref id, NodeColours.Condition)
    {
        if (entry.Count >= 1)
        {
            @base = entry[0].ToString();
        }

        Inputs.Add(new NodePin(id++, "Trigger", this, LinkType.Condition, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        Controls.InputTextId("Base", ref @base);
    }
}
