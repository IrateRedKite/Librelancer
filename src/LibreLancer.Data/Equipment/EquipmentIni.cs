﻿// MIT License - Copyright (c) Malte Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using LibreLancer.Data.Ini;

namespace LibreLancer.Data.Equipment
{
    [ParsedIni]
	public partial class EquipmentIni
	{
        [Section("light", typeof(Light))]
        [Section("power", typeof(PowerCore))]
        [Section("scanner", typeof(Scanner))]
        [Section("tractor", typeof(Tractor))]
        [Section("lootcrate", typeof(LootCrate))]
        [Section("repairkit", typeof(RepairKit))]
        [Section("countermeasure", typeof(Countermeasure))]
        [Section("countermeasuredropper", typeof(CountermeasureDropper))]
        [Section("shieldbattery", typeof(ShieldBattery))]
        [Section("armor", typeof(Armor))]
        [Section("cargopod", typeof(CargoPod))]
        [Section("commodity", typeof(Commodity))]
        [Section("tradelane", typeof(Tradelane))]
        [Section("internalfx", typeof(InternalFx))]
        [Section("attachedfx", typeof(AttachedFx))]
        [Section("shieldgenerator", typeof(ShieldGenerator))]
        [Section("shield", typeof(Shield))]
        [Section("engine", typeof(Engine))]
        [Section("thruster", typeof(Thruster))]
        [Section("cloakingdevice", typeof(CloakingDevice))]
        [Section("gun", typeof(Gun))]
        [Section("minedropper", typeof(MineDropper))]
        [Section("lod", typeof(Lod), Child = true)] //Attaches to parents
		public List<AbstractEquipment> Equip = new List<AbstractEquipment>();
        [Section("munition")]
        public List<Munition> Munitions = new List<Munition>();
        [Section("motor")]
        public List<Motor> Motors = new List<Motor>();
        [Section("explosion")]
        public List<Explosion> Explosions = new List<Explosion>();
        [Section("mine")]
        public List<Mine> Mines = new List<Mine>();

        public void ParseAllInis(IEnumerable<string> paths, FreelancerData fldata)
        {
            ParseInis(paths, fldata.VFS);
        }
    }
}
