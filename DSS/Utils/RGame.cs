// MineJason.Serialization - serialization library
// Copyright (C) 2025 WithLithum & contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using GTA;
using JetBrains.Annotations;
using WithLithum.NativeWrapper;

namespace DSS.Utils
{
    public static class RGame
    {
        public static bool Ok([CanBeNull] this Entity entity)
        {
            return entity != null && entity.Exists();
        }
        
        public static void DisableControlAction(int controller, Control control, bool disable)
        {
            Natives.DisableControlAction(controller, (int)control, disable);
        }

        public static string ReadString(this IniFile ini, string section, string name, string defaultValue)
        {
            if (!ini.TryGetSection(section, out var iniSection))
            {
                return defaultValue;
            }

            if (!iniSection.TryGetValue(name, out var iniValue))
            {
                return defaultValue;
            }

            return iniValue.GetString();
        }
        
        public static int ReadInt32(this IniFile ini, string section, string name, int defaultValue)
        {
            if (!ini.TryGetSection(section, out var iniSection))
            {
                return defaultValue;
            }

            if (!iniSection.TryGetValue(name, out var iniValue))
            {
                return defaultValue;
            }

            if (!iniValue.TryConvertInt(out var result))
            {
                return defaultValue;
            }

            return result;
        }
    }
}