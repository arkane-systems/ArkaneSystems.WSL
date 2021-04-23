#region header

// ArkaneSystems.WSL - PlatformChecks.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/04/23 8:20 AM.

#endregion

#region using

using System;
using System.IO;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Tests related to the WSL platform itself.
    /// </summary>
    [PublicAPI]
    public static class PlatformChecks
    {
        /// <summary>
        ///     Are we running on the Linux platform? True if we are, false otherwise.
        /// </summary>
        public static bool IsLinux => RuntimeInformation.IsOSPlatform (osPlatform: OSPlatform.Linux);

        /// <summary>
        ///     Are we running under WSL 1? True if we are, false otherwise.
        /// </summary>
        public static bool IsWsl1
        {
            get
            {
                // We check for WSL 1 by examining the type of the root filesystem. If the root
                // filesystem is lxfs or wslfs, then we're running under WSL 1.
                var mounts = File.ReadAllLines (path: "/proc/self/mounts");

                foreach (var mnt in mounts)
                {
                    var details = mnt.Split (separator: ' ');

                    if (details.Length < 6)
                        throw new InvalidOperationException (message: "mounts format error; cannot determine filesystem type");

                    if (details[1] == "/")

                        // Root filesystem.
                        return details[2] == @"lxfs" || details[2] == @"wslfs";
                }

                throw new InvalidOperationException (message: "mounts format error; cannot find root filesystem mount");
            }
        }

        /// <summary>
        ///     Are we running under WSL 2? True if we are, false otherwise.
        /// </summary>
        public static bool IsWsl2
        {
            get
            {
                if (Directory.Exists (path: @"/run/WSL")) return true;

                var osRelease = File.ReadAllText (path: @"/proc/sys/kernel/osrelease");

                return osRelease.Contains (value: "microsoft", comparisonType: StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
