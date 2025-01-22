using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using LancerEdit.GameContent.MissionEditor.Popups;
using LibreLancer;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes;

public class NodeMissionTrigger : Node
{
    public readonly MissionTrigger Data;
    public List<NodeTriggerEntry> Conditions = new();
    public List<NodeTriggerEntry> Actions = new();


    public NodeMissionTrigger(MissionTrigger data) : base(NodeColours.Trigger)
    {

        this.Data = data ?? new MissionTrigger();

        Inputs.Add(new NodePin(this, LinkType.Trigger, PinKind.Input));
        Outputs.Add(new NodePin(this, LinkType.Trigger, PinKind.Output));

        foreach (var c in data.Conditions)
        {
            Conditions.Add(NodeTriggerEntry.ConditionToNode(c.Type, c.Entry));
        }
        foreach (var a in data.Actions)
        {
            Actions.Add(NodeTriggerEntry.ActionToNode(a.Type, a));
        }
    }

    public override string Name => "Mission Trigger";

    static bool StartChild(NodeTriggerEntry e, out bool remove)
    {
        ImGui.BeginGroup();
        ImGui.PushStyleColor(ImGuiCol.Header, e.Color);
        ImGui.PushStyleColor(ImGuiCol.Button, e.Color);
        remove = ImGui.Button(ImGuiExt.IDWithExtra($"{Icons.TrashAlt}", e.Id));
        ImGui.SameLine();
        var render = ImGui.CollapsingHeader(ImGuiExt.IDWithExtra(e.Name, (long)e.Id));
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        return render;
    }

    static void EndChild(uint bordercol, float szContent, float contentPad)
    {
        ImGui.EndGroup();
        var min = ImGui.GetItemRectMin();
        var max = ImGui.GetItemRectMax();
        max.X = min.X + szContent + contentPad;
        var dl = ImGui.GetWindowDrawList();
        dl.AddRect(min, max, bordercol, 2, ImDrawFlags.RoundCornersAll, 1);
    }

    void RenderConditions(bool usePins, float szPin, float szContent, float pad,
        GameDataContext gameData, PopupManager popups, ref NodePopups nodePopups,
        ref NodeLookups nodeLookups)
    {
        if (usePins)
        {
            ImGui.BeginTable("##conditions", 2, ImGuiTableFlags.PreciseWidths, new Vector2(szPin + szContent + pad, 0));
            ImGui.TableSetupColumn("##pins", ImGuiTableColumnFlags.WidthFixed, szPin);
            ImGui.TableSetupColumn("##content", ImGuiTableColumnFlags.WidthFixed, szContent);
        }

        var bordercol = ImGui.GetColorU32(ImGuiCol.Border);
        float contentPad = usePins ? pad : pad * 2;

        for(int i = 0; i < Conditions.Count; i++)
        {
            var e = Conditions[i];
            if (usePins)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                foreach(var input in e.Inputs)
                {
                    var iconSize  = new Vector2(16 * ImGuiHelper.Scale);
                    NodeEditor.BeginPin(input.Id, PinKind.Input);
                    NodeEditor.PinPivotAlignment(new Vector2(0f, 0.5f));
                    NodeEditor.PinPivotSize(new Vector2(0, 0));
                    VectorIcons.Icon(iconSize, VectorIcon.Diamond, false, Color4.Green);
                    ImGui.SameLine();
                    ImGui.Text(input.LinkType.ToString());
                    NodeEditor.EndPin();
                }
                ImGui.TableNextColumn();
            }

            if (StartChild(e, out bool remove))
            {
                ImGui.Dummy(new Vector2(1, 4) * ImGuiHelper.Scale); //pad
                e.RenderContent(gameData, popups, ref nodePopups, ref nodeLookups);
                ImGui.Dummy(new Vector2(1, 4) * ImGuiHelper.Scale); //pad
            }
            EndChild(bordercol, szContent, contentPad);
            if (remove)
            {
                Conditions.RemoveAt(i);
                i--;
            }
        }

        if (usePins)
        {
            ImGui.EndTable();
        }
    }

    void RenderActions(bool usePins, float szPin, float szContent, float pad,
        GameDataContext gameData, PopupManager popups, ref NodePopups nodePopups,
        ref NodeLookups nodeLookups)
    {
        var bordercol = ImGui.GetColorU32(ImGuiCol.Border);
        float contentPad = usePins ? 0 : pad * 2;
        if (usePins)
        {
            ImGui.BeginTable("##actions", 2, ImGuiTableFlags.PreciseWidths, new Vector2(szPin + szContent + pad, 0));
            ImGui.TableSetupColumn("##content", ImGuiTableColumnFlags.WidthFixed, szContent);
            ImGui.TableSetupColumn("##pins", ImGuiTableColumnFlags.WidthFixed, szPin);
        }
        for (int i = 0; i < Actions.Count; i++)
        {
            var e = Actions[i];
            if (usePins)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
            }

            if (StartChild(e, out bool remove))
            {
                ImGui.Dummy(new Vector2(1, 4) * ImGuiHelper.Scale); //pad
                e.RenderContent(gameData, popups, ref nodePopups, ref nodeLookups);
                ImGui.Dummy(new Vector2(1, 4) * ImGuiHelper.Scale); //pad
            }
            EndChild(bordercol, szContent, contentPad);
            if (remove)
            {
                Actions.RemoveAt(i);
                i--;
            }

            if (usePins)
            {
                ImGui.TableNextColumn();
                foreach(var o in e.Outputs)
                {
                    var iconSize  = new Vector2(16 * ImGuiHelper.Scale);
                    NodeEditor.BeginPin(o.Id, PinKind.Output);
                    NodeEditor.PinPivotAlignment(new Vector2(1f, 0.5f));
                    NodeEditor.PinPivotSize(new Vector2(0, 0));
                    ImGui.Text(o.LinkType.ToString());
                    ImGui.SameLine();
                    VectorIcons.Icon(iconSize, VectorIcon.Diamond, false, Color4.Green);
                    NodeEditor.EndPin();
                }
            }
        }

        if (usePins)
        {
            ImGui.EndTable();
        }
    }

    public float EstimateHeight()
    {
        var iHeight = 24 * ImGuiHelper.Scale;

        var maxItems = Math.Max(Conditions.Count, Actions.Count);

        return (maxItems * iHeight * 1.75f) + iHeight * 7; //7 controls + 1.75x items
    }

    public override bool OnContextMenu(PopupManager popups)
    {
        if (ImGui.MenuItem("Add Action"))
        {
            popups.OpenPopup(new NewActionPopup(action =>
            {
                var node = NodeTriggerEntry.ActionToNode(action, null);
                Actions.Add(node);
            }));
        }

        if (ImGui.MenuItem("Add Condition"))
        {
            popups.OpenPopup(new NewConditionPopup(condition =>
            {
                var node = NodeTriggerEntry.ConditionToNode(condition, null);
                Conditions.Add(node);
            }));
        }

        return ImGui.MenuItem("Delete Trigger");
    }

    public sealed override void Render(GameDataContext gameData, PopupManager popup, ref NodeLookups lookups)
    {
        // Measurements
        // Do we need to use pins?
        bool conditionPin = false;
        bool actionPin = false;
        for (int i = 0; i < Conditions.Count; i++) {
            if (Conditions[i].Inputs.Count > 0)
            {
                conditionPin = true;
                break;
            }
        }
        for (int i = 0; i < Actions.Count; i++)
        {
            if (Actions[i].Outputs.Count > 0)
            {
                actionPin = true;
                break;
            }
        }

        float szPin = 70 * ImGuiHelper.Scale;
        float szContent = 300 * ImGuiHelper.Scale;
        float szRight = szContent + (actionPin ? szPin : 0);
        float szLeft = szContent + (conditionPin ? szPin : 0);
        var pad = ImGui.GetStyle().FramePadding.X;


        var iconSize  = new Vector2(24 * ImGuiHelper.Scale);
        var nb = NodeBuilder.Begin(Id);

        nb.Header(Color);
        ImGui.Text(Name);
        nb.EndHeader();

        NodeEditor.BeginPin(Inputs[0].Id, PinKind.Input);
        NodeEditor.PinPivotAlignment(new Vector2(0f, 0.5f));
        NodeEditor.PinPivotSize(new Vector2(0, 0));
        VectorIcons.Icon(iconSize, VectorIcon.Flow, false, Color4.Green);
        ImGui.SameLine();
        ImGui.Text(Inputs[0].LinkType.ToString());
        NodeEditor.EndPin();

        float szTriggerPin = 75 * ImGuiHelper.Scale;
        var tWidth = szLeft + szRight + 8 * pad;
        ImGui.SameLine();
        ImGui.Dummy(new Vector2(tWidth - 2 * szTriggerPin, 1));
        ImGui.SameLine();

        NodeEditor.BeginPin(Outputs[0].Id, PinKind.Output);
        NodeEditor.PinPivotAlignment(new Vector2(1f, 0.5f));
        NodeEditor.PinPivotSize(new Vector2(0, 0));
        ImGui.Text(Outputs[0].LinkType.ToString());
        ImGui.SameLine();
        VectorIcons.Icon(iconSize, VectorIcon.Flow, false, Color4.Green);
        NodeEditor.EndPin();


        ImGui.PushItemWidth(180 * ImGuiHelper.Scale);
        Controls.InputTextId("ID", ref Data.Nickname);
        nb.Popups.StringCombo("System", Data.System, s => Data.System = s, gameData.SystemsByName, true);
        ImGui.Checkbox("Repeatable", ref Data.Repeatable);
        nb.Popups.Combo("Initial State", Data.InitState, x => Data.InitState = x);
        ImGui.PopItemWidth();

        // Draw conditions/actions
        ImGui.BeginTable("##trigger", 2, ImGuiTableFlags.PreciseWidths, new Vector2(szLeft + szRight + 8 * pad, 0));
        ImGui.TableSetupColumn("Conditions", ImGuiTableColumnFlags.WidthFixed, szLeft + 2 * pad);
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, szRight + 2 * pad);
        ImGui.TableHeadersRow();
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        RenderConditions(conditionPin, szPin, szContent, pad, gameData, popup, ref nb.Popups, ref lookups);
        ImGui.TableNextColumn();
        RenderActions(actionPin, szPin, szContent, pad, gameData, popup, ref nb.Popups, ref lookups);
        ImGui.EndTable();
        nb.Dispose();

    }

    public void WriteNode(MissionScriptEditorTab missionEditor, IniBuilder builder)
    {
        var s = builder.Section("Trigger");


        if (string.IsNullOrWhiteSpace(Data.Nickname))
        {
            return;
        }

        s.Entry("nickname", Data.Nickname);
        s.Entry("InitState", Data.InitState.ToString());
        if (Data.System != string.Empty)
        {
            s.Entry("system", Data.System);
        }

        s.Entry("repeatable", Data.Repeatable);

        foreach (var condition in Conditions)
        {
            condition.WriteEntry(s);
        }

        foreach (var action in Actions)
        {
            action.WriteEntry(s);
        }
    }
}
