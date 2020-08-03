using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    public static class Globals
    {
        public static string BasePath
        {
            get => Process.GetCurrentProcess().MainModule.FileName;
        }

        public static string SettingsPath
        {
            get => Path.Combine(BasePath, "settings.json");
        }
    }
}
