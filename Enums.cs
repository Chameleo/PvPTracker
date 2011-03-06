using System;
using System.Collections.Generic;
using System.Text;

namespace PvPTracker
{
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
	public enum ObjectManager : uint
	{
		ClientConnection = 0x00C79CE0,
		CurrentManager = 0x2ED0,
		LocalGUID = 0xC0,
		FirstObject = 0xAC,
		NextObject = 0x3C,
		ObjectType = 0x14
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
