#region header

// ArkaneSystems.WSL - MountOptions.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/08/06 10:55 AM.

#endregion

#region using

using System;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Standard Linux (i.e., not filesystem-specific) mount options.
    /// </summary>
    [Flags]
    public enum MountOptions
    {
        /// <summary>
        ///     No options specified.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Make directory changes on this filesystem synchronous. This property can be obtained for individual directories
        ///     or subtrees using chattr(1).
        /// </summary>
        DirectoryChangesSynchronous = 0x0001,

        /* LazyTimestampUpdate         = 0x0002, */

        /// <summary>
        ///     Permit mandatory locking on files in this filesystem. Mandatory locking must still be enabled on a per-file basis,
        ///     as described in fcntl(2).
        /// </summary>
        MandatoryLocking = 0x0004,

        /// <summary>
        ///     Do not update access times for (all types of) files on this filesystem.
        /// </summary>
        NoAccessTimes = 0x0008,

        /// <summary>
        ///     Do not allow access to devices (special files) on this filesystem.
        /// </summary>
        NoDeviceAccess = 0x0010,

        /// <summary>
        ///     Do not update access times for directories on this filesystem. This flag provides a subset of the functionality
        ///     provided by <see cref="NoAccessTimes" />; that is <see cref="NoAccessTimes" /> implies
        ///     <see cref="NoDirectoryAccessTimes" />.
        /// </summary>
        NoDirectoryAccessTimes = 0x0020,

        /// <summary>
        ///     Do not allow programs to be executed from this filesystem.
        /// </summary>
        NoExecutables = 0x0040,

        /// <summary>
        ///     Do not honor set-user-ID and set-group-ID bits or file capabilities when executing programs from this filesystem.
        /// </summary>
        NoSetUserId = 0x0080,

        /// <summary>
        ///     Mount filesystem read-only.
        /// </summary>
        ReadOnly = 0x0100,

        /// <summary>
        ///     When a file on this filesystem is accessed, update the file's last access time (atime) only if the current value
        ///     of atime is less than or equal to the file's last modification time (mtime) or last status change time (ctime).
        ///     This
        ///     option is useful for programs that need to know when a file has been read since it was last modified. In addition,
        ///     since Linux 2.6.30, the file's last access time is always updated if it is more than one day old.
        /// </summary>
        RelativeAccessTime = 0x0400,

        /// <summary>
        ///     Suppress the display of certain printk() warning messages in the kernel log.
        /// </summary>
        SilenceWarnings = 0x0800,

        /// <summary>
        ///     Always update the last access time (atime) when files on this filesystem are accessed.
        /// </summary>
        StrictAccessTime = 0x1000,

        /// <summary>
        ///     Make writes on this filesystem synchronous (as if the O_SYNC flag to open(2) was specified for all file opens to
        ///     this filesystem.
        /// </summary>
        SynchronousWrites = 0x2000,

        /* DoNotFollowSymbolicLinks    = 0x4000, */
    }

    /// <summary>
    ///     Options defining the manner in which mount events propagate.
    /// </summary>
    public enum MountPropagation
    {
        /// <summary>
        ///     None specified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        ///     Make this mount point shared. Mount and unmount events immediately under this mount point will propagate to the
        ///     other mount points that are members of this mount's peer group. Propagation here means that the same mount or
        ///     unmount will automatically occur under all of the other mount points in the peer group. Conversely, mount and
        ///     unmount
        ///     events that take place under peer mount points will propagate to this mount point.
        /// </summary>
        Shared,

        /// <summary>
        ///     Make this mount point private. Mount and unmount events do not propagate into or out of this mount point.
        /// </summary>
        Private,

        /// <summary>
        ///     <para>
        ///         If this is a shared mount point that is a member of a peer group that contains other members, convert it to a
        ///         slave mount. If this is a shared mount point that is a member of a peer group that contains no other members,
        ///         convert it to a private mount. Otherwise, the propagation type of the mount point is left unchanged.
        ///     </para>
        ///     <para>
        ///         When a mount point is a slave, mount and unmount events propagate into this mount point from the (master)
        ///         shared
        ///         peer group of which it was formerly a member. Mount and unmount events under this mount point do not propagate
        ///         to any peer.
        ///     </para>
        ///     <para>
        ///         A mount point can be the slave of another peer group while at the same time sharing mount and unmount events
        ///         with a
        ///         peer group of which it is a member.
        ///     </para>
        /// </summary>
        Slave,

        /// <summary>
        ///     Make this mount unbindable. This is like a private mount, and in addition, this mount can't be bind mounted. When a
        ///     recursive bind mount is performed on a directory subtree, any unbindable mounts within the subtree are
        ///     automatically
        ///     pruned (i.e., not replicated) when replicating that subtree to produce the target subtree.
        /// </summary>
        Unbindable,
    }
}
