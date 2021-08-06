#region header

// ArkaneSystems.WSL - MountHelpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/04/23 9:14 AM.

#endregion

#region using

using System;
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
        /// <param name="recursive">
        ///     Recursively mount submounts of the source subtree at the corresponding location in
        ///     the target subtree.
        /// </param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool BindMount ([NotNull] string source, [NotNull] string mountPoint, bool recursive = false)
        {
            if (string.IsNullOrEmpty (value: source))
                throw new ArgumentNullException (paramName: nameof (source), message: "Mount source cannot be null or empty.");

            if (string.IsNullOrEmpty (value: mountPoint))
                throw new ArgumentNullException (paramName: nameof (mountPoint), message: "Mount point cannot be null or empty.");

            ulong_t flags = recursive ? LibC.MS_BIND | LibC.MS_REC : LibC.MS_BIND;

            unsafe
            {
                fixed (byte* bSource = Encoding.UTF8.GetBytes (s: source))
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                {
                    return LibC.mount (source: bSource,
                                       target: bMountPoint,
                                       filesystemtype: null,
                                       mountflags: flags,
                                       data: null) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Change the mount propagation type of a specific mount point.
        /// </summary>
        /// <param name="mountPoint">The mount point to modify.</param>
        /// <param name="propagation">The desired mount propagation type. See <see cref="MountPropagation" />.</param>
        /// <param name="recursive">Also modifies all mount points in the subtree of the specified mount point.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool ChangeMountPropagationType ([NotNull] string mountPoint,
                                                       MountPropagation propagation,
                                                       bool             recursive = false)
        {
            if (string.IsNullOrEmpty (value: mountPoint))
                throw new ArgumentNullException (paramName: nameof (mountPoint), message: "Mount point cannot be null or empty.");

            if (propagation == MountPropagation.Unspecified)

                // this is a no-op
                return true;

            ulong_t options = 0;

            MountHelpers.UpdateWithPropagation (options: ref options, propagation: propagation);

            if (recursive)
                options |= LibC.MS_REC;

            unsafe
            {
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                {
                    return LibC.mount (source: null,
                                       target: bMountPoint,
                                       filesystemtype: null,
                                       mountflags: options,
                                       data: null) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Mount the device or other source specified on the specified mount point, as the specified filesystem type.
        /// </summary>
        /// <param name="source">The block device, or other source, to mount.</param>
        /// <param name="mountPoint">The mount point at which to mount the source.</param>
        /// <param name="fsType">The filesystem type to mount.See <see cref="FsType" />.</param>
        /// <param name="options">Standard options to mount. See <see cref="MountOptions" />.</param>
        /// <param name="propagation">The desired mount propagation type. See <see cref="MountPropagation" />.</param>
        /// <param name="extendedOptions">A string of extended options parsed by the specified filesystem, or null if none.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool Mount ([NotNull] string source,
                                  [NotNull] string mountPoint,
                                  [NotNull] string fsType,
                                  MountOptions     options         = MountOptions.None,
                                  MountPropagation propagation     = MountPropagation.Unspecified,
                                  string?          extendedOptions = null)
        {
            if (string.IsNullOrEmpty (value: source))
                throw new ArgumentNullException (paramName: nameof (source), message: "Mount source cannot be null or empty.");

            if (string.IsNullOrEmpty (value: mountPoint))
                throw new ArgumentNullException (paramName: nameof (mountPoint), message: "Mount point cannot be null or empty.");

            if (string.IsNullOrEmpty (value: fsType))
                throw new ArgumentNullException (paramName: nameof (fsType), message: "Filesystem type cannot be null or empty.");

            // Create and validate mount options.
            ulong_t realOptions = MountHelpers.TranslateMountOptions (options: options);
            MountHelpers.UpdateWithPropagation (options: ref realOptions, propagation: propagation);

            // Do the mounting.
            unsafe
            {
                fixed (byte* bSource = Encoding.UTF8.GetBytes (s: source))
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                fixed (byte* bFsType = Encoding.UTF8.GetBytes (s: fsType))
                fixed (byte* bExtendedOptions = extendedOptions != null ? Encoding.UTF8.GetBytes (s: extendedOptions) : null)
                {
                    return LibC.mount (source: bSource,
                                       target: bMountPoint,
                                       filesystemtype: bFsType,
                                       mountflags: realOptions,
                                       data: bExtendedOptions) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Moves the mount at a given mount point to a different mount point. The move is atomic; at no point is
        ///     the subtree unmounted.
        /// </summary>
        /// <param name="oldMountPoint">The mount point to move the mount from.</param>
        /// <param name="newMountPoint">The mount point to move the mount to.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool MoveMount ([NotNull] string oldMountPoint, [NotNull] string newMountPoint)
        {
            if (string.IsNullOrEmpty (value: oldMountPoint))
                throw new ArgumentNullException (paramName: nameof (oldMountPoint),
                                                 message: "Old mount point cannot be null or empty.");

            if (string.IsNullOrEmpty (value: newMountPoint))
                throw new ArgumentNullException (paramName: nameof (newMountPoint),
                                                 message: "New mount point cannot be null or empty.");

            ulong_t options = LibC.MS_MOVE;

            unsafe
            {
                fixed (byte* bOld = Encoding.UTF8.GetBytes (s: oldMountPoint))
                fixed (byte* bNew = Encoding.UTF8.GetBytes (s: newMountPoint))
                {
                    return LibC.mount (source: bOld,
                                       target: bNew,
                                       filesystemtype: null,
                                       mountflags: options,
                                       data: null) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Remounts an existing mount. This enables you to change the mount options and extended options of an existing
        ///     mount without having to unmount and remount the filesystem.
        /// </summary>
        /// <param name="mountPoint">The mount point to remount.</param>
        /// <param name="options">The new standard mount options. See <see cref="MountOptions" />.</param>
        /// <param name="extendedOptions">The new extended mount options, if any.</param>
        /// <param name="perMountPoint">If true, modify only the per-mount-point options.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public static bool Remount ([NotNull] string mountPoint,
                                    MountOptions     options         = MountOptions.None,
                                    string?          extendedOptions = null,
                                    bool             perMountPoint   = false)
        {
            if (string.IsNullOrEmpty (value: mountPoint))
                throw new ArgumentNullException (paramName: nameof (mountPoint), message: "Mount point cannot be null or empty.");

            ulong_t realOptions = MountHelpers.TranslateMountOptions (options: options);

            if (perMountPoint)
                realOptions |= LibC.MS_BIND;

            // Do the remounting.
            realOptions |= LibC.MS_REMOUNT;

            unsafe
            {
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                fixed (byte* bExtendedOptions = extendedOptions != null ? Encoding.UTF8.GetBytes (s: extendedOptions) : null)
                {
                    return LibC.mount (source: null,
                                       target: bMountPoint,
                                       filesystemtype: null,
                                       mountflags: realOptions,
                                       data: bExtendedOptions) ==
                           0;
                }
            }
        }

        /// <summary>
        ///     Unmount a mounted filesystem from a given mount point.
        /// </summary>
        /// <param name="mountPoint">The mount point to unmount.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool UnMount ([NotNull] string mountPoint)
        {
            if (string.IsNullOrEmpty (value: mountPoint))
                throw new ArgumentNullException (paramName: nameof (mountPoint), message: "Mount point cannot be null or empty.");

            unsafe
            {
                fixed (byte* bMountPoint = Encoding.UTF8.GetBytes (s: mountPoint))
                {
                    return LibC.umount2 (target: bMountPoint, flags: LibC.MNT_DETACH) == 0;
                }
            }
        }

        private static ulong_t TranslateMountOptions (MountOptions options)
        {
            var     accessTimeCompatibility = 0;
            ulong_t realOptions;

            if (options == MountOptions.None)
            {
                realOptions = 0;
            }
            else
            {
                realOptions = 0;
                if ((options & MountOptions.DirectoryChangesSynchronous) != 0) realOptions |= LibC.MS_DIRSYNC;
                /* if ((options & MountOptions.LazyTimestampUpdate)         != 0) realOptions |= LibC.MS_LAZYTIME; */
                if ((options & MountOptions.MandatoryLocking) != 0) realOptions |= LibC.MS_MANDLOCK;

                if ((options & MountOptions.NoAccessTimes) != 0)
                {
                    realOptions |= LibC.MS_NOATIME;
                    accessTimeCompatibility++;
                }

                if ((options & MountOptions.NoDeviceAccess) != 0) realOptions |= LibC.MS_NODEV;

                if ((options & MountOptions.NoDirectoryAccessTimes) != 0) realOptions |= LibC.MS_NODIRATIME;

                if ((options & MountOptions.NoExecutables) != 0) realOptions |= LibC.MS_NOEXEC;
                if ((options & MountOptions.NoSetUserId)   != 0) realOptions |= LibC.MS_NOSUID;
                if ((options & MountOptions.ReadOnly)      != 0) realOptions |= LibC.MS_RDONLY;

                if ((options & MountOptions.RelativeAccessTime) != 0)
                {
                    realOptions |= LibC.MS_RELATIME;
                    accessTimeCompatibility++;
                }

                if ((options & MountOptions.SilenceWarnings) != 0) realOptions |= LibC.MS_SILENT;

                if ((options & MountOptions.StrictAccessTime) != 0)
                {
                    realOptions |= LibC.MS_STRICTATIME;
                    accessTimeCompatibility++;
                }

                if ((options & MountOptions.SynchronousWrites) != 0) realOptions |= LibC.MS_SYNCHRONOUS;
                /* if ((options & MountOptions.DoNotFollowSymbolicLinks) != 0) realOptions |= LibC.MS_NOSYMFOLLOW; */
            }

            if (accessTimeCompatibility > 1)
                throw new ArgumentException (message:
                                             "Only one of [NoAccessTimes|RelativeAccessTime|StrictAccessTime] can be specified.",
                                             paramName: nameof (options));

            return realOptions;
        }

        private static void UpdateWithPropagation (ref ulong_t options, MountPropagation propagation)
        {
            switch (propagation)
            {
                case MountPropagation.Shared:
                    options |= LibC.MS_SHARED;

                    break;

                case MountPropagation.Private:
                    options |= LibC.MS_PRIVATE;

                    break;

                case MountPropagation.Slave:
                    options |= LibC.MS_SLAVE;

                    break;

                case MountPropagation.Unbindable:
                    options |= LibC.MS_UNBINDABLE;

                    break;
            }
        }
    }
}
