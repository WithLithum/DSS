using WithLithum.NativeWrapper;

namespace DSS.Utils
{
    class Lights
    {
        public static void UpdateIndicator(ManagedVehicle activeVeh)
        {
            switch (activeVeh.IndStatus)
            {
                case IndStatus.Off:
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 0, false);
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 1, false);
                    break;
                case IndStatus.Left:
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 0, false);
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 1, true);
                    break;
                case IndStatus.Right:
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 0, true);
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 1, false);
                    break;
                case IndStatus.Both:
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 0, true);
                    Natives.SetVehicleIndicatorLights(activeVeh.Vehicle.Handle, 1, true);
                    break;
            }
        }
    }
}
