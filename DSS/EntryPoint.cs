using System;
using DSS.Threads;
using DSS.Utils;
using System.Collections.Generic;
using GTA;
using WithLithum.NativeWrapper;

namespace DSS
{
    public class Entrypoint : Script
    {
        // Vehicles currently being managed by DSS
        public static List<ManagedVehicle> activeVehicles = new List<ManagedVehicle>();
        // List of used Sound IDs
        public static List<int> UsedSoundIDs = new List<int>();
        // List of Siren Sets
        public static Dictionary<int, SoundSet> SirenSets = new Dictionary<int, SoundSet>();

        // If DSS is on Key Lock method
        public static bool keysLocked = false;

        public Entrypoint()
        {
            //Initiates Log File
            Log Log = new Log();

            // Checks if .ini file is created.
            Utils.Settings.IniCheck();

            //Loads SirenSets
            SirenSets = Sirens.GetSirenSets();

            //Creates player controller
            "Loading: DSS - Player Controller".ToLog();
            
            Tick += OnTick;
            Aborted += OnAborted;
            
            "Loaded: DSS - Player Controller".ToLog();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            OnUnload();
        }

        private void OnTick(object sender, EventArgs e)
        {
            PlayerController.Tick();
        }

        private static void OnUnload()
        {
            "Unloading DSS".ToLog();
            if (UsedSoundIDs.Count > 0)
            {
                "Unloading used SoundIDs".ToLog();
                foreach (int id in UsedSoundIDs)
                {
                    Natives.StopSound(id);
                    Natives.ReleaseSoundId(id);
                    ("Unloaded SoundID " + id).ToLog();
                }
                "Unloaded all used SoundIDs".ToLog();
            }
            if (activeVehicles.Count > 0)
            {
                "Refreshing vehicle's default EL".ToLog();
                foreach (ManagedVehicle aVeh in activeVehicles)
                {
                    if (aVeh.Vehicle.Ok())
                    {
                        aVeh.Vehicle.IsSirenActive = false;
                        aVeh.Vehicle.IsSirenSilent = false;
                        aVeh.Vehicle.IsLeftIndicatorLightOn = false;
                        aVeh.Vehicle.IsRightIndicatorLightOn = false;
                        ("Refreshed " + aVeh.Vehicle.Handle).ToLog();
                    }
                    else
                        ("Vehicle does not exist anymore!").ToLog();
                }
                "Refreshed vehicle's default EL".ToLog();
            }
        }
    }
}