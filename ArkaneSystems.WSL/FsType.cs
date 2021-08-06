#region header

// ArkaneSystems.WSL - FsType.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/08/06 10:54 AM.

#endregion

#region using

using JetBrains.Annotations;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Constant strings representing file system types.
    /// </summary>
    [PublicAPI]
    public static class FsType
    {
        /// <summary>
        ///     A pseudo-filesystem representing the kernel feature which directs executables to the appropriate
        ///     interpreter.
        /// </summary>
        public const string BinaryFormats = "binfmt_misc";

        /// <summary>
        ///     The Common Internet File System, a network filesystem protocol developed by Microsoft as an updated
        ///     version of the original SMB (Server Message Block) protocol developed by IBM. The standard protocol
        ///     for Windows file and printer sharing.
        /// </summary>
        public const string Cifs = "cifs";

        /// <summary>
        ///     The fourth extended file system; a general-purpose file system intended for use as the default
        ///     file system for Linux.
        /// </summary>
        public const string Extended4 = "ext4";

        /// <summary>
        ///     The standard filesystem for optical disk media, such as CD-ROMs, and disk images (.ISOs) taken from same.
        /// </summary>
        public const string Iso9660 = "iso9660";

        /// <summary>
        ///     The Network File System, a LAN file-sharing protocol developed by Sun Microsystems. Use this type for
        ///     NFS versions 1, 2, and 3. For NFS version 4, use <see cref="FsType.Nfs4" />.
        /// </summary>
        public const string Nfs = "nfs";

        /// <summary>
        ///     The Network File System, a LAN file-sharing protocol developed by Sun Microsystems. Use this type for
        ///     NFS version 4. For NFS versions 1, 2, and 3, use <see cref="FsType.Nfs" />.
        /// </summary>
        public const string Nfs4 = "nfs4";

        /// <summary>
        ///     A network filesystem protocol developed for Plan 9 by Bell Labs. Under WSL, it is principally used as
        ///     the mechanism to communicate across the Windows-Linux boundary.
        /// </summary>
        public const string Plan9 = "9p";

        /// <summary>
        ///     A compressed, read-only file system for Linux, used to mount squashed filesystem images created by
        ///     mksquashfs.
        /// </summary>
        public const string Squash = "squashfs";

        /// <summary>
        ///     A temporary storage file system in which data is stored in volatile memory instead of a persistent
        ///     backing store. Data stored in tmpfs is lost on reboot or dismount.
        /// </summary>
        public const string Temporary = "tmpfs";

        /// <summary>
        ///     The File Allocation Table file system, developed by Microsoft et. al. for use on floppy disks; also
        ///     used commonly on flash and other solid-state memory cards and modules.
        /// </summary>
        public const string VFat = "vfat";
    }
}
