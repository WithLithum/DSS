using GTA;
using WithLithum.NativeWrapper;

namespace DSS.Utils
{
    internal static class Controls
    {
        public static bool IsDisabledControlJustReleased(int padIndex, Control control)
        {
            return Natives.IsDisabledControlJustReleased(padIndex, (int)control);
        }

        public static bool IsDisabledControlPressed(int padIndex, Control control)
        {
            return Natives.IsDisabledControlPressed(padIndex, (int)control);
        }
    }
}