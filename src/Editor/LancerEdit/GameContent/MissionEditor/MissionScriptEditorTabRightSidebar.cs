using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;

namespace LancerEdit.GameContent.MissionEditor;

public sealed partial class MissionScriptEditorTab
{
    private void RenderRightSidebar()
    {
        var padding = ImGui.GetStyle().FramePadding.Y + ImGui.GetStyle().FrameBorderSize;
        ImGui.BeginChild("NavbarRight", new Vector2(300f * ImGuiHelper.Scale, ImGui.GetContentRegionMax().Y - padding), ImGuiChildFlags.None,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoCollapse);

        ImGui.PushStyleColor(ImGuiCol.Header, ImGui.GetColorU32(ImGuiCol.FrameBg));

        if (ImGui.CollapsingHeader("Ship Manager", ImGuiTreeNodeFlags.DefaultOpen))
        {
            RenderMissionShipManager();
        }

        ImGui.NewLine();
        if (ImGui.CollapsingHeader("Solar Manager", ImGuiTreeNodeFlags.DefaultOpen))
        {
            RenderMissionSolarManager();
        }

        ImGui.NewLine();
        if (ImGui.CollapsingHeader("Loot Manager", ImGuiTreeNodeFlags.DefaultOpen))
        {
            RenderLootManager();
        }

        ImGui.NewLine();
        if (ImGui.CollapsingHeader("Dialog Manager", ImGuiTreeNodeFlags.DefaultOpen))
        {
            RenderDialogManager();
        }

        ImGui.PopStyleColor();

        ImGui.EndChild();
    }

    private int selectedDialogIndex = -1;

    private void RenderDialogManager()
    {
        if (ImGui.Button("Create New Dialog"))
        {
            selectedDialogIndex = missionIni.Dialogs.Count;
            missionIni.Dialogs.Add(new MissionDialog());
        }

        ImGui.BeginDisabled(selectedDialogIndex == -1);
        if (ImGui.Button("Delete Dialog"))
        {
            win.Confirm("Are you sure you want to delete this dialog?",
                () => { missionIni.Dialogs.RemoveAt(selectedDialogIndex--); });
        }

        ImGui.EndDisabled();

        var selectedDialog = selectedDialogIndex != -1 ? missionIni.Dialogs[selectedDialogIndex] : null;
        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("Dialogs", selectedDialog is not null ? selectedDialog.Nickname : ""))
        {
            for (var index = 0; index < missionIni.Dialogs.Count; index++)
            {
                var arch = missionIni.Dialogs[index];
                var selected = arch == selectedDialog;
                if (!ImGui.Selectable(arch?.Nickname, selected))
                {
                    continue;
                }

                selectedDialogIndex = index;
                selectedDialog = arch;
            }

            ImGui.EndCombo();
        }

        if (selectedDialog is null)
        {
            return;
        }

        ImGui.PushID(selectedDialogIndex);

        Controls.InputTextId("Nickname##Dialog", ref selectedDialog.Nickname, 150f);
        Controls.InputTextId("System##Dialog", ref selectedDialog.System, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedDialog.System.Length is 0 ||
                                                     gameData.GameData.Systems.Any(x =>
                                                         x.Nickname == selectedDialog.System));

        for (var index = 0; index < selectedDialog.Lines.Count; index++)
        {
            var line = selectedDialog.Lines[index];
            ImGui.PushID(line.GetHashCode());
            Controls.InputTextId("Source##ID", ref line.Source);
            Controls.InputTextId("Target##ID", ref line.Target);
            Controls.InputTextId("Line##ID", ref line.Line);
            ImGui.PopID();

            if (index + 1 != selectedDialog.Lines.Count)
            {
                ImGui.NewLine();
            }
        }

        MissionEditorHelpers.AddRemoveListButtons(selectedDialog.Lines);

        ImGui.PopID();
    }

    private int selectedLootIndex = -1;

    private void RenderLootManager()
    {
        if (ImGui.Button("Create New Loot"))
        {
            selectedLootIndex = missionIni.Loots.Count;
            missionIni.Loots.Add(new MissionLoot());
        }

        ImGui.BeginDisabled(selectedLootIndex == -1);
        if (ImGui.Button("Delete Loot"))
        {
            win.Confirm("Are you sure you want to delete this loot?",
                () => { missionIni.Loots.RemoveAt(selectedLootIndex--); });
        }

        ImGui.EndDisabled();

        var selectedLoot = selectedLootIndex != -1 ? missionIni.Loots[selectedLootIndex] : null;
        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("Loots", selectedLoot is not null ? selectedLoot.Nickname : ""))
        {
            for (var index = 0; index < missionIni.Loots.Count; index++)
            {
                var arch = missionIni.Loots[index];
                var selected = arch == selectedLoot;
                if (!ImGui.Selectable(arch?.Nickname, selected))
                {
                    continue;
                }

                selectedLootIndex = index;
                selectedLoot = arch;
            }

            ImGui.EndCombo();
        }

        if (selectedLoot is null)
        {
            return;
        }

        ImGui.PushID(selectedLootIndex);

        Controls.InputTextId("Nickname##Loot", ref selectedLoot.Nickname, 150f);
        Controls.InputTextId("Archetype##Loot", ref selectedLoot.Archetype, 150f);
        Controls.IdsInputString("Name##Loot", gameData, popup, ref selectedLoot.StringId,
            x => selectedLoot.StringId = x, inputWidth: 150f);

        ImGui.BeginDisabled(!string.IsNullOrEmpty(selectedLoot.RelPosObj) && selectedLoot.RelPosOffset != Vector3.Zero);
        ImGui.SetNextItemWidth(200f);
        ImGui.InputFloat3("Position##Loot", ref selectedLoot.Position);
        ImGui.EndDisabled();

        ImGui.Text("Relative Position");
        ImGui.BeginDisabled(selectedLoot.Position != Vector3.Zero);

        Controls.InputTextId("Object##LootRel", ref selectedLoot.RelPosObj, 150f);

        ImGui.SetNextItemWidth(200f);
        ImGui.InputFloat3("Position##LootRel", ref selectedLoot.RelPosOffset);

        ImGui.EndDisabled();

        ImGui.NewLine();
        ImGui.SetNextItemWidth(200f);
        ImGui.InputInt("Equip Amount##Loot", ref selectedLoot.EquipAmount);

        ImGui.SetNextItemWidth(200f);
        ImGui.SliderFloat("Health##Loot", ref selectedLoot.Health, 0f, 1f);

        ImGui.Checkbox("Can Jettison##Loot", ref selectedLoot.CanJettison);

        ImGui.PopID();
    }

    private int selectedSolarIndex = -1;

    private void RenderMissionSolarManager()
    {
        if (ImGui.Button("Create New Solar"))
        {
            selectedSolarIndex = missionIni.Solars.Count;
            missionIni.Solars.Add(new MissionSolar());
        }

        ImGui.BeginDisabled(selectedSolarIndex == -1);
        if (ImGui.Button("Delete Solar"))
        {
            win.Confirm("Are you sure you want to delete this solar?",
                () => { missionIni.Solars.RemoveAt(selectedSolarIndex--); });
        }

        ImGui.EndDisabled();

        var selectedSolar = selectedSolarIndex != -1 ? missionIni.Solars[selectedSolarIndex] : null;
        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("Solars", selectedSolar is not null ? selectedSolar.Nickname : ""))
        {
            for (var index = 0; index < missionIni.Solars.Count; index++)
            {
                var arch = missionIni.Solars[index];
                var selected = arch == selectedSolar;
                if (!ImGui.Selectable(arch?.Nickname, selected))
                {
                    continue;
                }

                selectedSolarIndex = index;
                selectedSolar = arch;
            }

            ImGui.EndCombo();
        }

        if (selectedSolar is null)
        {
            return;
        }

        ImGui.PushID(selectedSolarIndex);

        Controls.InputTextId("Nickname##Solar", ref selectedSolar.Nickname, 150f);
        Controls.InputTextId("System##Solar", ref selectedSolar.System, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedSolar.System.Length is 0 ||
                                                     gameData.GameData.Systems.Any(x =>
                                                         x.Nickname.Equals(selectedSolar.System,
                                                             StringComparison.InvariantCultureIgnoreCase)));

        Controls.InputTextId("Faction##Solar", ref selectedSolar.Faction, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedSolar.Faction.Length is 0 ||
                                                     gameData.GameData.Factions.Any(x =>
                                                         x.Nickname.Equals(selectedSolar.Faction,
                                                             StringComparison.InvariantCultureIgnoreCase)));

        Controls.InputTextId("Archetype##Solar", ref selectedSolar.Archetype, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedSolar.Archetype.Length is 0 ||
                                                     gameData.GameData.Archetypes.Any(x =>
                                                         x.Nickname.Equals(selectedSolar.Archetype,
                                                             StringComparison.InvariantCultureIgnoreCase)));

        Controls.InputTextId("Base##Solar", ref selectedSolar.Base, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedSolar.Base.Length is 0 ||
                                                     gameData.GameData.Bases.Any(x =>
                                                         x.Nickname.Equals(selectedSolar.Base,
                                                             StringComparison.InvariantCultureIgnoreCase)));

        Controls.InputTextId("Loadout##Solar", ref selectedSolar.Loadout, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedSolar.Loadout.Length is 0 ||
                                                     gameData.GameData.Loadouts.Any(x =>
                                                         x.Nickname.Equals(selectedSolar.Loadout,
                                                             StringComparison.InvariantCultureIgnoreCase)));

        Controls.InputTextId("Voice##Solar", ref selectedSolar.Voice, 150f);
        Controls.InputTextId("Pilot##Solar", ref selectedSolar.Pilot, 150f);
        Controls.InputTextId("Costume Head##Solar", ref selectedSolar.Costume[0], 150f);
        Controls.InputTextId("Costume Body##Solar", ref selectedSolar.Costume[1], 150f);
        Controls.InputTextId("Costume Accessory##Solar", ref selectedSolar.Costume[2], 150f);
        Controls.InputTextId("Visit##Solar", ref selectedSolar.Visit, 150f);

        ImGui.SetNextItemWidth(100f);
        ImGui.InputInt("String ID##Solar", ref selectedSolar.StringId);

        ImGui.SetNextItemWidth(100f);
        ImGui.InputFloat("Radius##Solar", ref selectedSolar.Radius);

        ImGui.NewLine();

        Controls.InputStringList("Labels", selectedSolar.Labels);

        ImGui.SetNextItemWidth(200f);
        ImGui.InputFloat3("Position##Solar", ref selectedSolar.Position);

        ImGui.SetNextItemWidth(200f);
        Controls.InputFlQuaternion("Orientation##Solar", ref selectedSolar.Orientation);

        ImGui.PopID();
    }

    private int selectedShipIndex = -1;

    private void RenderMissionShipManager()
    {
        if (ImGui.Button("Create New Ship"))
        {
            selectedShipIndex = missionIni.Ships.Count;
            missionIni.Ships.Add(new MissionShip());
        }

        ImGui.BeginDisabled(selectedShipIndex == -1);
        if (ImGui.Button("Delete Ship"))
        {
            win.Confirm("Are you sure you want to delete this ship?",
                () => { missionIni.NPCs.RemoveAt(selectedShipIndex--); });
        }

        ImGui.EndDisabled();

        var selectedShip = selectedShipIndex != -1 ? missionIni.Ships[selectedShipIndex] : null;
        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("Ships", selectedShip is not null ? selectedShip.Nickname : ""))
        {
            for (var index = 0; index < missionIni.Ships.Count; index++)
            {
                var arch = missionIni.Ships[index];
                var selected = arch == selectedShip;
                if (!ImGui.Selectable(arch?.Nickname, selected))
                {
                    continue;
                }

                selectedShipIndex = index;
                selectedShip = arch;
            }

            ImGui.EndCombo();
        }

        if (selectedShip is null)
        {
            return;
        }

        ImGui.PushID(selectedShipIndex);

        Controls.InputTextId("Nickname##Ship", ref selectedShip.Nickname, 150f);
        Controls.InputTextId("System##Ship", ref selectedShip.System, 150f);
        MissionEditorHelpers.AlertIfInvalidRef(() => selectedShip.System.Length is 0 ||
                                                     gameData.GameData.Systems.Any(x =>
                                                         x.Nickname == selectedShip.System));

        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("NPC##Ship", selectedShip.NPC ?? ""))
        {
            foreach (var npc in missionIni.NPCs
                         .Select(x => x.Nickname)
                         .Where(x => ImGui.Selectable(x ?? "", selectedShip.NPC == x)))
            {
                selectedShip.NPC = npc;
            }

            ImGui.EndCombo();
        }

        MissionEditorHelpers.AlertIfInvalidRef(() => missionIni.NPCs.Any(x => x.Nickname == selectedShip.NPC));

        ImGui.NewLine();

        Controls.InputStringList("Labels", selectedShip.Labels);

        ImGui.InputFloat3("Position##Ship", ref selectedShip.Position);

        ImGui.NewLine();

        ImGui.Text("Relative Position:");

        // Disable relative data if absolute data is provided
        ImGui.BeginDisabled(selectedShip.Position.Length() is not 0f);

        Controls.InputTextId("Obj##Ship", ref selectedShip.RelativePosition.ObjectName, 150f);
        // Don't think it's possible to validate this one, as it could refer to any solar object in any system

        ImGui.SetNextItemWidth(150f);
        ImGui.InputFloat("Min Range##Ship", ref selectedShip.RelativePosition.MinRange);

        ImGui.SetNextItemWidth(150f);
        ImGui.InputFloat("Max Range##Ship", ref selectedShip.RelativePosition.MaxRange);

        ImGui.EndDisabled();

        ImGui.NewLine();
        ImGui.SetNextItemWidth(200f);
        Controls.InputFlQuaternion("Orientation##Ship", ref selectedShip.Orientation);
        ImGui.Checkbox("Random Name##Ship", ref selectedShip.RandomName);
        ImGui.Checkbox("Jumper##Ship", ref selectedShip.Jumper);
        ImGui.SetNextItemWidth(100f);
        ImGui.InputFloat("Radius##Ship", ref selectedShip.Radius);
        Controls.InputTextId("Arrival Object##Ship", ref selectedShip.ArrivalObj, 150f);

        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo("Initial Objectives##Ship", selectedShip.InitObjectives ?? ""))
        {
            if (ImGui.Selectable("no_op", selectedShip.InitObjectives == "no_op"))
            {
                selectedShip.InitObjectives = "no_op";
            }

            foreach (var npc in missionIni.ObjLists
                         .Select(x => x.Nickname)
                         .Where(x => ImGui.Selectable(x ?? "", selectedShip.InitObjectives == x)))
            {
                selectedShip.InitObjectives = npc;
            }

            ImGui.EndCombo();
        }

        MissionEditorHelpers.AlertIfInvalidRef(() => selectedShip.InitObjectives is null ||
                                                     selectedShip.InitObjectives.Length is 0 ||
                                                     selectedShip.InitObjectives == "no_op" ||
                                                     missionIni.ObjLists.Any(x => x.Nickname == selectedShip.InitObjectives));

        ImGui.Text("Cargo");
        if (selectedShip.Cargo.Count is not 0)
        {
            for (var i = 0; i < selectedShip.Cargo.Count; i++)
            {
                var cargo = selectedShip.Cargo[i];
                ImGui.PushID(i);
                Controls.InputTextId("##Cargo", ref cargo.Cargo, 150f);
                MissionEditorHelpers.AlertIfInvalidRef(() =>
                    gameData.GameData.Equipment.Any(x =>
                        x.Nickname.Equals(cargo.Cargo, StringComparison.InvariantCultureIgnoreCase)));
                ImGui.SameLine();
                ImGui.PushItemWidth(75f);
                ImGui.InputInt("##Count", ref cargo.Count);
                if (cargo.Count < 0)
                {
                    cargo.Count = 0;
                }

                ImGui.PopID();
                selectedShip.Cargo[i] = cargo;
            }
        }

        MissionEditorHelpers.AddRemoveListButtons(selectedShip.Cargo);

        ImGui.PopID();
    }
}
