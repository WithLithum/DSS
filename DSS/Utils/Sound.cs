using WithLithum.NativeWrapper;

namespace DSS.Utils
{
    class Sound
    {
        public static int NewSoundID(ManagedVehicle activeVeh)
        {
            if (activeVeh.SoundId != 999)
            {
                Natives.StopSound(activeVeh.SoundId);
                Natives.ReleaseSoundId(activeVeh.SoundId);
                Entrypoint.UsedSoundIDs.Remove(activeVeh.SoundId);
            }
            int newID = Natives.GetSoundId();
            activeVeh.SoundId = newID;
            Entrypoint.UsedSoundIDs.Add(newID);
            return newID;
        }

        public static int TempSoundID()
        {
            int newID = Natives.GetSoundId();
            Entrypoint.UsedSoundIDs.Add(newID);
            return newID;
        }

        public static void ClearTempSoundID(int id)
        {
            Natives.StopSound(id);
            Natives.ReleaseSoundId(id);
            Entrypoint.UsedSoundIDs.Remove(id);
        }
    }
}
