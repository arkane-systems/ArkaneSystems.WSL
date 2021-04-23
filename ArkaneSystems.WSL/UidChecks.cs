#region header

// ArkaneSystems.WSL - UidChecks.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/04/23 8:48 AM.

#endregion

#region using

using JetBrains.Annotations;

using Tmds.Linux;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Tests related to the UID/GID under which we are running.
    /// </summary>
    [PublicAPI]
    public static class UidChecks
    {
        /// <summary>
        ///     Are we running as the actual root user; i.e., is our real UID root, whatever our effective UID is?
        ///     True if we are, false otherwise.
        /// </summary>
        public static bool IsReallyRoot => LibC.getuid () == 0;

        /// <summary>
        ///     Are we running as the effective root user; i.e., is our effective UID root, whatever our real UID is?
        ///     True if we are, false otherwise.
        /// </summary>
        public static bool IsEffectivelyRoot => LibC.geteuid () == 0;

        /// <summary>
        ///     Are we running setuid root; i.e., is our effective UID root and our real UID not root?
        ///     True if we are, false otherwise.
        /// </summary>
        public static bool IsSetuidRoot => !UidChecks.IsReallyRoot && UidChecks.IsEffectivelyRoot;

        /// <summary>
        ///     Are we running setuid; i.e., are our real UID and effective UID different?
        ///     True if we are, false otherwise.
        /// </summary>
        public static bool IsSetuid => LibC.getuid () != LibC.geteuid ();

        /// <summary>
        ///     Are we running setgid; i.e., are our real GID and effective GID different?
        ///     True if we are, false otherwise.
        /// </summary>
        public static bool IsSetgid => LibC.getgid () != LibC.getegid ();
    }
}
