using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WithLithum.NativeWrapper;

namespace DSS.Utils
{
    internal static class Sirens
    {
        private static readonly List<SirenStage> defaultSirenStages = new List<SirenStage> { SirenStage.One, SirenStage.Two, SirenStage.Warning };
        public static void Update(ManagedVehicle activeVeh)
        {
            switch (activeVeh.SirenStage)
            {
                case SirenStage.Off:
                    Sound.NewSoundID(activeVeh);
                    break;
                case SirenStage.One:
                    if (activeVeh.SoundSet == null)
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), "VEHICLES_HORNS_SIREN_1", activeVeh.Vehicle.Handle, null, false, 0);
                    else
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), activeVeh.SoundSet.SirenTones[0], activeVeh.Vehicle.Handle, null, false, 0);
                    break;
                case SirenStage.Two:
                    if (activeVeh.SoundSet == null)
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), "VEHICLES_HORNS_SIREN_2", activeVeh.Vehicle.Handle, null, false, 0);
                    else
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), activeVeh.SoundSet.SirenTones[1], activeVeh.Vehicle.Handle, null, false, 0);
                    break;
                case SirenStage.Warning:
                    if (activeVeh.SoundSet == null)
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), "VEHICLES_HORNS_POLICE_WARNING", activeVeh.Vehicle.Handle, null, false, 0);
                    else
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), activeVeh.SoundSet.SirenTones[2], activeVeh.Vehicle.Handle, null, false, 0);
                    break;
                case SirenStage.Warning2:
                    Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), activeVeh.SoundSet.SirenTones[3], activeVeh.Vehicle.Handle, null, false, 0);
                    break;
                case SirenStage.Horn:
                    if (activeVeh.SoundSet == null)
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), "SIRENS_AIRHORN", activeVeh.Vehicle.Handle, null, false, 0);
                    else
                        Natives.PlaySoundFromEntity(Sound.NewSoundID(activeVeh), activeVeh.SoundSet.Horn, activeVeh.Vehicle.Handle, null, false, 0);
                    break;
                default:
                    break;
            }
        }
        public static void MoveUpStage(ManagedVehicle activeVeh)
        {
            Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
            if (activeVeh.SoundSet == null)
                activeVeh.SirenStage = defaultSirenStages.Next(activeVeh.SirenStage);
            else
                activeVeh.SirenStage = activeVeh.SoundSet.AvailableSirenStages.Next(activeVeh.SirenStage);
            Update(activeVeh);
        }
        public static void MoveDownStage(ManagedVehicle activeVeh)
        {
            Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
            if (activeVeh.SoundSet == null)
                activeVeh.SirenStage = defaultSirenStages.Previous(activeVeh.SirenStage);
            else
                activeVeh.SirenStage = activeVeh.SoundSet.AvailableSirenStages.Previous(activeVeh.SirenStage);
            Update(activeVeh);
        }
        public static void SetAirManuState(ManagedVehicle activeVeh, int? newState)
        {
            if(newState != activeVeh.AirManuState)
            {
                if(activeVeh.AirManuID != null)
                {
                    Sound.ClearTempSoundID((int)activeVeh.AirManuID);
                    activeVeh.AirManuID = null;
                }

                switch (newState)
                {
                    case 1:
                        activeVeh.AirManuID = Sound.TempSoundID();
                        if (activeVeh.SoundSet == null)
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, "SIRENS_AIRHORN", activeVeh.Vehicle.Handle, null, false, 0);
                        else
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, activeVeh.SoundSet.Horn, activeVeh.Vehicle.Handle, null, false, 0);
                        break;
                    case 2:
                        activeVeh.AirManuID = Sound.TempSoundID();
                        if (activeVeh.SoundSet == null)
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, "VEHICLES_HORNS_SIREN_1", activeVeh.Vehicle.Handle, null, false, 0);
                        else
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, activeVeh.SoundSet.SirenTones[0], activeVeh.Vehicle.Handle, null, false, 0);
                        break;
                    case 3:
                        activeVeh.AirManuID = Sound.TempSoundID();
                        if (activeVeh.SoundSet == null)
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, "VEHICLES_HORNS_SIREN_2", activeVeh.Vehicle.Handle, null, false, 0);
                        else
                            Natives.PlaySoundFromEntity(activeVeh.AirManuID.Value, activeVeh.SoundSet.SirenTones[1], activeVeh.Vehicle.Handle, null, false, 0);
                        break;
                }

                activeVeh.AirManuState = newState;
            }
        }

        public static Dictionary<int, SoundSet> GetSirenSets()
        {
            string path = @"Plugins\DSS\";
            _ = new SirenSet();
            var dictSirenSets = new Dictionary<int, SoundSet>();
            foreach (string file in Directory.EnumerateFiles(path, "*.xml"))
            {
                try
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(SirenSets));
                    StreamReader streamReader = new StreamReader(file);

                    SirenSets sirenSets = (SirenSets)mySerializer.Deserialize(streamReader);
                    streamReader.Close();

                    string name = Path.GetFileNameWithoutExtension(file);
                    ("Added SirenSets: " + name).ToLog();

                    foreach (SirenSet sirenSet in sirenSets.SirenSet)
                    {
                        List<string> vehicles = sirenSet.Vehicles.Replace(" ", "").Split(',').ToList();

                        List<string> SirenTones = sirenSet.SoundSet.Tones.Replace(" ", "").Split(',').ToList();
                        for (int i = 0; i < SirenTones.Count; i++)
                            sirenSet.SoundSet.SirenTones[i] = SirenTones[i];

                        if (sirenSet.SoundSet.SirenTones[2] != "")
                            sirenSet.SoundSet.AvailableSirenStages.Add(SirenStage.Warning);
                        if (sirenSet.SoundSet.SirenTones[3] != "")
                            sirenSet.SoundSet.AvailableSirenStages.Add(SirenStage.Warning2);

                        foreach (string vehicle in vehicles)
                        {
                            dictSirenSets.Add(Game.GenerateHash(vehicle.ToLowerInvariant()),
                                sirenSet.SoundSet);
                        }
                    }
                }
                catch (Exception e)
                {
                    ("SIRENSET IMPORT ERROR (" + Path.GetFileNameWithoutExtension(file) + "): " + e.Message).ToLog();
                }
            }

            return dictSirenSets;
        }
    }
}