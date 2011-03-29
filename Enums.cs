using System;
using System.Collections.Generic;
using System.Text;

namespace PvPTracker
{
	internal enum WowVersions
	{
		wow335 = 12340,
		wow406 = 13623
	}

	// By Apoc of mmowned.com
	[Flags]
	internal enum TrackObjectFlags
	{
		All = -1,
		None = 0x0,
		Lockpicking = 0x1,
		Herbs = 0x2,
		Minerals = 0x4,
		DisarmTrap = 0x8,
		Open = 0x10,
		Treasure = 0x20,
		CalcifiedElvenGems = 0x40,
		Close = 0x80,
		ArmTrap = 0x100,
		QuickOpen = 0x200,
		QuickClose = 0x400,
		OpenTinkering = 0x800,
		OpenKneeling = 0x1000,
		OpenAttacking = 0x2000,
		Gahzridian = 0x4000,
		Blasting = 0x8000,
		PvPOpen = 0x10000,
		PvPClose = 0x20000,
		Fishing = 0x40000,
		Inscription = 0x80000,
		OpenFromVehicle = 0x100000,
	}

	// By Apoc of mmowned.com
	[Flags]
	internal enum TrackCreatureFlags
	{
		Beasts = 0x01,
		Dragons = 0x02,
		Demons = 0x04,
		Elementals = 0x08,
		Giants = 0x10,
		Undead = 0x20,
		Humanoids = 0x40,
		Critters = 0x80,
		Machines = 0x100,
		Slimes = 0x200,
		Totem = 0x400,
		NonCombatPet = 0x800,
		GasCloud = 0x1000,
		All = -1,
	}

	/// <summary>
	/// Memory locations specific to the ObjectManager.
	/// 3.3.5a
	/// </summary>
	public class ObjectManager335
	{
		public int ClientConnection = 0x879CE0;//0xC79CE0;
		public int CurrentManager = 0x2ED0;
		public int LocalGUID = 0xC0;
		public int FirstObject = 0xAC;
		public int NextObject = 0x3C;
        public int ObjectType = 0x14;
	}

    /// <summary>
    /// Memory locations specific to the ObjectManager.
    /// 4.0.6a
    /// </summary>
    public class ObjectManager406
    {
        public int ClientConnection = 0x8BF1A8;//0xCBF1A8;
        public int CurrentManager = 0x462C;
        public int LocalGUID = 0xB8;
        public int FirstObject = 0xB4;
        public int NextObject = 0x3C;
        public int ObjectType = 0x14;
    }

	/// <summary>
	/// Types available for a <see cref="WowObject"/>
	/// </summary>
	public enum ObjectType : uint
	{
		Object = 0,
		Item = 1,
		Container = 2,
		Unit = 3,
		Player = 4,
		GameObject = 5,
		DynamicObject = 6,
		Corpse = 7,
		AiGroup = 8,
		AreaTrigger = 9
	}
}
