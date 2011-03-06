﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MemoryIO;
using HookApplication;

namespace PvPTracker
{
	// PvPTracker by Chameleon
	// Reuses some code from WRadar
	public partial class MainForm : Form
	{
		int procId = 0;
		int SelfFaction = -1;
		int FriendlyPlayers = 0;
		int EnemyPlayers = 0;

		public MainForm()
		{
			InitializeComponent();
			KeyboardHook.SetHook( new Action(StartStop),
                 KeyboardHook.BeginKeys.Alt_Control, KeyboardHook.EndKey.T );
		}

		private void StartStop()
		{
			updateTracking.Enabled = !updateTracking.Enabled;
			TopMost = updateTracking.Enabled;
		}

		/// <summary>
		/// Gets the descriptor value of a WowObject
		/// </summary>
		/// <typeparam name="T">Descriptor struct</typeparam>
		/// <param name="wowObjectPtr">Base address of any WowObject</param>
		/// <param name="field">Descriptor field</param>
		/// <returns>Value of the descriptor field in memory.</returns>
		static T GetStorageField<T>(IntPtr wowObjectPtr, uint field) where T : struct
		{
			var descriptors = Memory.ReadAtOffset<IntPtr>(wowObjectPtr, 0x8);

			return Memory.ReadAtOffset<T>(descriptors, field * 4);
		}

		static void SetStorageField<T>(IntPtr wowObjectPtr, uint field, T val) where T : struct
		{
			IntPtr descriptors = Memory.ReadAtOffset<IntPtr>(wowObjectPtr, 0x8);
			IntPtr loc = new IntPtr(descriptors.ToInt32() + field * 4);
			Memory.Write<T>(loc, val);
		}

		// Based on code from WRadar
		private IntPtr UpdateObjects()
		{
			IntPtr CurrentManager = Memory.Read<IntPtr>(
				new IntPtr((uint)ObjectManager.ClientConnection),
				new IntPtr((uint)ObjectManager.CurrentManager));
			var localPlayerGuid =
				Memory.ReadAtOffset<ulong>(CurrentManager, (uint)ObjectManager.LocalGUID);
			var current =
				Memory.ReadAtOffset<IntPtr>(CurrentManager, (uint)ObjectManager.FirstObject);

			if (current == IntPtr.Zero || ((uint)current & 1) == 1)
			{
				FriendlyPlayers = -1;
				EnemyPlayers = -1;
				return IntPtr.Zero;
			}
			FriendlyPlayers = 0;
			EnemyPlayers = 0;
			IntPtr player = IntPtr.Zero;
			for (;current != IntPtr.Zero && ((uint)current & 1) != 1;
				current = Memory.ReadAtOffset<IntPtr>(current, (uint)ObjectManager.NextObject))
			{
				var guid = GetStorageField<ulong>(current, (uint)ObjectFields.GUID);
				bool self = localPlayerGuid == guid;
				if (self)
                {
                    player = current;
					if (SelfFaction < 0)
					{
						SelfFaction = GetFaction(player);
					}
                }
				ObjectType type = (ObjectType)Memory.ReadAtOffset<uint>(
					current, (uint)ObjectManager.ObjectType);


				bool tracked = false;
				if (type == ObjectType.Player && !self)
				{
					//only want to track players
					tracked = true;
					if (GetFaction(current) != SelfFaction)
					{
						EnemyPlayers++;
					}
					else
					{
						FriendlyPlayers++;
					}
				}
				if (type == ObjectType.Unit || type == ObjectType.Player)
				{
					if(tracked)
					{
						// clean untrackable flag
						uint untrackable  = 0x04;
						uint bytes = GetStorageField<uint>(current, (uint)UnitFields.BYTES_1);
						if((bytes & untrackable) != 0)
						{
							bytes &= ~untrackable;
							SetStorageField<uint>(current, (uint)UnitFields.BYTES_1, untrackable);
						}
					}
					uint dynFlagTracked = 0x0002;
					uint flags = GetStorageField<uint>(current, (uint)UnitFields.DYNAMIC_FLAGS);
					uint newflags;
					if(tracked)
					{
						newflags = flags | dynFlagTracked;
					}
					else
					{
						newflags = flags & ~dynFlagTracked;
					}
					if (newflags != flags)
					{
						SetStorageField<uint>(current, (uint)UnitFields.DYNAMIC_FLAGS, newflags);
					}
				}
			}

			return player;
		}

		void SetTracking(IntPtr player, TrackCreatureFlags flags)
		{
			SetStorageField<uint>(player, (uint)PlayerFields.TRACK_CREATURES, (uint)flags);
		}

		private static int[] AllianceIDs = { 1, 3, 4, 115, 1629 };
		private static int[] HordeIDs = { 2, 5, 6, 116, 1610 };
		public int GetFaction(IntPtr unit)
		{
			uint factionTemp = GetStorageField<uint>(unit, (uint)UnitFields.FACTIONTEMPLATE);
			for (int i = 0; i <= AllianceIDs.Length - 1; i++)
			{
				if (factionTemp == AllianceIDs[i])
				{
					return 0;
				}
			}

			for (int i = 0; i <= HordeIDs.Length - 1; i++)
			{
				if (factionTemp == HordeIDs[i])
				{
					return 1;
				}
			}
			return -1;
		}


		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			KeyboardHook.UnhookWindowsHook();
			Properties.Settings.Default.Save();
		}

		private void updateTracking_Tick(object sender, EventArgs e)
		{
			try
			{
				// check if client was closed and process needs to be reopened
				if(procId != 0)
				{
					try 
					{	        
						Process.GetProcessById(procId);
					}
					catch (Exception)
					{
						procId = 0;
					}
				}
				if (procId == 0)
				{
					Process[] procs = Process.GetProcessesByName("wow");
					if (procs.Length == 0)
					{
						SetStatus("wow.exe process not running");
						return;
					}
					Process wowProc = procs[0];
					Memory.OpenProcess(wowProc.Id, AccessRights.ReadWrite);
					procId = wowProc.Id;
				}
				IntPtr player = UpdateObjects();
				//SetTracking(player, TrackCreatureFlags.Humanoids | TrackCreatureFlags.Beasts);
				
				OutLabel.Text = EnemyPlayers + " | " + FriendlyPlayers;
				//Memory.CloseProcess();
			}
			catch (Exception ex)
			{
				SetStatus(ex.ToString());
			}
		}

		string status;
		void SetStatus(string str)
		{
			status = str;
		}

		#region Window moving
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd,
						 int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		private void MainForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}
		#endregion

		private void MainForm_Load(object sender, EventArgs e)
		{
			StartStop();
		}
	}
}
