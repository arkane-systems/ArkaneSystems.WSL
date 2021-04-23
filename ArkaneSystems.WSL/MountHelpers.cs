#region header

// ArkaneSystems.WSL - MountHelpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/04/23 9:14 AM.

#endregion

#region using

using System.Text;

using JetBrains.Annotations;

using Tmds.Linux;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Helper functions for file system mounts.
    /// </summary>
    [PublicAPI]
    public static class MountHelpers
    {
        /// <summary>
        ///     Bind mount the folder at <paramref name="source" /> to the mount point at <paramref name="mountPoint" />.
        /// </summary>
        /// <param name="source">The folder to bind mount.</param>
        /// <param name="mountPoint">The mount point at which to bind mount the folder.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool BindMount (string source, string mountPoint)
        {
            unsafe
            {
                fixed (byte* bSource = Encoding.UTF8.GetBytes (s: source))
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                {
                    return LibC.mount (source: bSource,
                                       target: bMountPoint,
                                       filesystemtype: null,
                                       mountflags: LibC.MS_BIND,
                                       data: null) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Unmount a mounted filesystem from a given mount point.
        /// </summary>
        /// <param name="mountPoint">The mount point to unmount.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool UnMount (string mountPoint)
        {
            unsafe
            {
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                {
                    return LibC.umount2 (target: bMountPoint, flags: LibC.MNT_DETACH) == 0;
                }
            }
        }
    }
}
