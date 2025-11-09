using GTA;
using WithLithum.NativeWrapper;

namespace DSS
{
    public class ManagedVehicle
    {
        public ManagedVehicle(Vehicle vehicle, bool lightsOn = false)
        {
            Vehicle = vehicle;
            LightsOn = lightsOn;
            SirenStage = SirenStage.Off;
            HasSiren = Natives.GetEntityBoneIndexByName(vehicle.Handle, "siren1") != -1;

            if (Entrypoint.SirenSets.TryGetValue(vehicle.Model.Hash, out var sirenSet))
            {
                SoundSet = sirenSet;
            }
            
            if (vehicle.Exists())
            {
                bool temp = vehicle.IsSirenActive;
                vehicle.IsSirenActive = false;
                vehicle.IsSirenActive = temp;
            }
        }

        // General
        public Vehicle Vehicle { get; set; }
        
        public bool HasSiren { get; }
        
        // Lights
        public bool LightsOn { get; set; } = false;
        public bool Blackout { get; set; } = false;
        public bool InteriorLight { get; set; } = false;
        public IndStatus IndStatus { get; set; } = IndStatus.Off;

        // Sirens
        public SoundSet SoundSet { get; set; }
        public SirenStage SirenStage { get; set; }
        public bool AuxOn { get; set; } = false;
        public int AuxID { get; set; } = 999;
        public int SoundId { get; set; } = 999;
        public int? AirManuState { get; set; } = null;
        public int? AirManuID { get; set; } = null;
    }

    public enum SirenStage
    {
        Horn = -1,
        Off,
        One,
        Two,
        Warning,
        Warning2        
    }

    public enum IndStatus
    {
        Left,
        Right,
        Both,
        Off
    }
}
