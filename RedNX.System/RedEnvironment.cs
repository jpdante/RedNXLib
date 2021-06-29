using System;

namespace RedNX.System {
    public class RedEnvironment {

        public static SystemOS OperatingSystem {
            get {
                switch (Environment.OSVersion.Platform) {
                    case PlatformID.Unix:
                        return SystemOS.Unix;
                    case PlatformID.MacOSX:
                        return SystemOS.MacOSX;
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return SystemOS.Windows;
                    default:
                        return SystemOS.Unknown;
                }
            }
        }

    }
}
