using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using LancerEdit.GameContent.MissionEditor.NodeTypes;
using LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;
using LibreLancer;
using LibreLancer.ContentEdit;
using LibreLancer.Data.Missions;
using LibreLancer.Dialogs;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;
using ImGui = ImGuiNET.ImGui;
using Reg = LancerEdit.GameContent.MissionEditor.Registers.Registers;

namespace LancerEdit.GameContent.MissionEditor;
public sealed partial class MissionScriptEditorTab : GameContentTab
{
    private readonly GameDataContext gameData;
    private MainWindow win;
    private PopupManager popup;

    private readonly NodeEditorConfig config;
    private readonly NodeEditorContext context;

    private readonly List<Node> nodes;
    private readonly List<NodeLink> links;

    private int nextId;

    private NodePin newLinkPin = null;
    private NodePin newNodeLinkPin = null;

    private static bool _registeredNodeValueRenderers = false;

    private readonly MissionScript missionScript;

    public MissionScriptEditorTab(GameDataContext gameData, MainWindow win, string file)
    {
        Title = $"Mission Script Editor - {Path.GetFileName(file)}";
        this.gameData = gameData;
        this.win = win;
        popup = new PopupManager();

        config = new NodeEditorConfig();
        context = new NodeEditorContext(config);

        NodeBuilder.LoadTexture(win.RenderContext);

        RegisterNodeValues();

        nodes = [];
        links = [];
        missionScript = new MissionScript(new MissionIni(file, null));

        var npcPath = gameData.GameData.VFS.GetBackingFileName(gameData.GameData.DataPath(missionScript.Ini.Info.NpcShipFile));
        if (npcPath is not null)
        {
            missionScript.Ini.ShipIni = new NPCShipIni(npcPath, null);
        }

        foreach (var action in missionScript.AvailableTriggers.Values.SelectMany(trigger => trigger.Actions))
        {
            BlueprintNode node = action switch
            {
                Act_PlaySoundEffect act => new NodeActPlaySound(ref nextId, act),
                Act_Invulnerable act => new NodeActInvulnerable(ref nextId, act),
                Act_PlayMusic act => new NodeActPlayMusic(ref nextId, act),
                Act_SetShipAndLoadout act => new NodeActSetShipAndLoadout(ref nextId, act),
                Act_RemoveAmbient act => new NodeActRemoveAmbient(ref nextId, act),
                Act_AddAmbient act => new NodeActAddAmbient(ref nextId, act),
                Act_RemoveRTC act => new NodeActRemoveRtc(ref nextId, act),
                Act_AddRTC act => new NodeActAddRtc(ref nextId, act),
                Act_AdjAcct act => new NodeActAdjustAccount(ref nextId, act),
                Act_DeactTrig act => new NodeActDeactivateTrigger(ref nextId, act),
                Act_ActTrig act => new NodeActActivateTrigger(ref nextId, act),
                Act_SetNNObj act => new NodeActSetNNObject(ref nextId, act),
                Act_ForceLand act => new NodeActForceLand(ref nextId, act),
                Act_LightFuse act => new NodeActLightFuse(ref nextId, act),
                Act_PopupDialog act => new NodeActPopupDialog(ref nextId, act),
                Act_ChangeState act => new NodeActChangeState(ref nextId, act),
                Act_RevertCam act => new NodeActRevertCamera(ref nextId, act),
                Act_CallThorn act => new NodeActCallThorn(ref nextId, act),
                Act_MovePlayer act => new NodeActMovePlayer(ref nextId, act),
                Act_Cloak act => new NodeActCloak(ref nextId, act),
                Act_PobjIdle act => new NodeActPObjectIdle(ref nextId, act),
                Act_SetInitialPlayerPos act => new NodeActSetInitialPlayerPos(ref nextId, act),
                Act_RelocateShip act => new NodeActRelocateShip(ref nextId, act),
                Act_StartDialog act => new NodeActStartDialog(ref nextId, act),
                Act_SendComm act => new NodeActSendComm(ref nextId, act),
                Act_EtherComm act => new NodeActEtherComm(ref nextId, act),
                Act_SetVibe act => new NodeActSetVibe(ref nextId, act),
                Act_SetVibeLbl act => new NodeActSetVibeLabel(ref nextId, act),
                Act_SetVibeShipToLbl act => new NodeActSetVibeShipToLabel(ref nextId, act),
                Act_SetVibeLblToShip act => new NodeActSetVibeLabelToShip(ref nextId, act),
                Act_SpawnSolar act => new NodeActSpawnSolar(ref nextId, act),
                Act_SpawnShip act => new NodeActSpawnShip(ref nextId, act),
                Act_SpawnFormation act => new NodeActSpawnFormation(ref nextId, act),
                Act_MarkObj act => new NodeActMarkObject(ref nextId, act),
                Act_Destroy act => new NodeActDestroy(ref nextId, act),
                _ => null,
            };

            if (node is null)
            {
                FLLog.Warning("MissionScriptEditor", $"Unable to render node for action type: {action.GetType().FullName}");
                continue;
            }

            nodes.Add(node);
        }
    }

    public override void Draw(double elapsed)
    {
        ImGuiHelper.AnimatingElement();
        if (!ImGui.BeginTable("ME Table", 3, ImGuiTableFlags.None))
        {
            return;
        }

        CheckIndexes();

        ImGui.TableSetupColumn("ME Left Sidebar", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("ME Node Editor", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("ME Right Sidebar", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        RenderLeftSidebar();

        ImGui.TableNextColumn();
        RenderNodeEditor();

        ImGui.TableNextColumn();
        RenderRightSidebar();

        ImGui.EndTable();

        popup.Run();
    }

    private void CheckIndexes()
    {
        if (selectedShipIndex is -1 && missionScript.Ini.Ships.Count is not 0)
        {
            selectedShipIndex = 0;
        }

        if (selectedArchIndex is -1 && missionScript.Ini.ShipIni.ShipArches.Count is not 0)
        {
            selectedArchIndex = 0;
        }

        if (selectedNpcIndex is -1 && missionScript.Ini.NPCs.Count is not 0)
        {
            selectedNpcIndex = 0;
        }

        if (selectedSolarIndex is -1 && missionScript.Ini.Solars.Count is not 0)
        {
            selectedSolarIndex = 0;
        }

        if (selectedFormationIndex is -1 && missionScript.Ini.Formations.Count is not 0)
        {
            selectedFormationIndex = 0;
        }

        if (selectedLootIndex is -1 && missionScript.Ini.Loots.Count is not 0)
        {
            selectedLootIndex = 0;
        }
    }

    private void RenderNodeEditor()
    {
        NodeEditor.SetCurrentEditor(context);
        NodeEditor.Begin("Node Editor", Vector2.Zero);

        foreach (var node in nodes)
        {
            node.Render(gameData, popup, missionScript);
        }

        foreach (var link in links)
        {
            NodeEditor.Link(link.Id, link.StartPin.Id, link.EndPin.Id, link.Color, 2.0f);
        }

        TryCreateLink();

        NodeEditor.End();
        NodeEditor.SetCurrentEditor(null);
    }

    private void TryCreateLink()
    {
        if (NodeEditor.BeginCreate(Color4.White, 2.0f))
        {
            void ShowLabel(string label, Color4 color)
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - ImGui.GetTextLineHeight());
                var size = ImGui.CalcTextSize(label);

                var padding = ImGui.GetStyle().FramePadding;
                var spacing = ImGui.GetStyle().ItemSpacing;

                ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(spacing.X, -spacing.Y));

                var rectMin = ImGui.GetCursorScreenPos() - padding;
                var rectMax = ImGui.GetCursorScreenPos() + size + padding;

                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled(rectMin, rectMax, ImGui.ColorConvertFloat4ToU32(color), size.Y * 0.15f);
                ImGui.TextUnformatted(label);
            }

            if (NodeEditor.QueryNewLink(out var startPinId, out var endPinId))
            {
                var startPin = FindPin(startPinId);
                var endPin = FindPin(endPinId);

                newLinkPin = startPin ?? endPin;

                Debug.Assert(startPin != null, nameof(startPin) + " != null");

                if (startPin.PinKind == PinKind.Input)
                {
                    // Swap pins
                    (startPin, endPin) = (endPin, startPin);
                }

                // If we are dragging a pin and hovering a pin, check if we can connect
                if (startPin is not null && endPin is not null)
                {
                    if (endPin == startPin)
                    {
                        NodeEditor.RejectNewItem(Color4.Red, 2.0f);
                    }
                    else if (endPin.PinKind == startPin.PinKind)
                    {
                        ShowLabel("x Incompatible Pin Kind", new Color4(45, 32, 32, 180));
                        NodeEditor.RejectNewItem(Color4.Red, 2.0f);
                    }
                    else if (endPin.OwnerNode == startPin.OwnerNode)
                    {
                        ShowLabel("x Cannot connect to self", new Color4(45, 32, 32, 180));
                        NodeEditor.RejectNewItem(Color4.Red, 1.0f);
                    }
                    else if (endPin.LinkType != startPin.LinkType)
                    {
                        ShowLabel("x Incompatible Link Type", new Color4(45, 32, 32, 180));
                        NodeEditor.RejectNewItem(new Color4(255, 128, 128, 255));
                    }
                    else
                    {
                        ShowLabel("+ Create Link", new Color4(32, 45, 32, 180));
                        if (NodeEditor.AcceptNewItem(new Color4(128, 255, 128, 255), 4.0f))
                        {
                            var nodeLink = new NodeLink(nextId++, startPin, endPin)
                            {
                                Color = startPin.OwnerNode.Color
                            };
                            links.Add(nodeLink);
                        }
                    }
                }
            }

            if (NodeEditor.QueryNewNode(out var newPinId))
            {
                newLinkPin = FindPin(newPinId);
                if (newLinkPin is not null)
                {
                    ShowLabel("+ Create Node", new Color4(32, 45, 32, 180));
                }

                if (NodeEditor.AcceptNewItem())
                {
                    // TODO createNewNode = true;
                    newLinkPin = null;
                    NodeEditor.Suspend();
                    ImGui.OpenPopup("Create New Node");
                    NodeEditor.Resume();
                }
            }
        }
        else
        {
            newLinkPin = null;
        }

        NodeEditor.EndCreate();
    }

    private void RegisterNodeValues()
    {
        if (_registeredNodeValueRenderers)
        {
            return;
        }

        _registeredNodeValueRenderers = true;

        Node.RegisterNodeValueRenderer<MissionTrigger>(Reg.MissionTriggerContent);
    }

    private NodePin FindPin(PinId id)
    {
        if (id.Value.ToInt64() == 0)
        {
            return null;
        }

        foreach (var node in nodes)
        {
            var pin = node.Inputs.FirstOrDefault(x => x.Id == id);
            if (pin is not null)
            {
                return pin;
            }

            pin = node.Outputs.FirstOrDefault(x => x.Id == id);
            if (pin is not null)
            {
                return pin;
            }
        }

        return null;
    }

    public override void Dispose()
    {
        context.Dispose();
        config.Dispose();
        base.Dispose();
    }
}
