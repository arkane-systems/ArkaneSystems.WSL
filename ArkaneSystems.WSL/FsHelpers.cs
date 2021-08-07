#region header

// ArkaneSystems.WSL - FsHelpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/08/07 12:08 PM.

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
    ///     Helper functions for filesystem operations.
    /// </summary>
    [PublicAPI]
    public static class FsHelpers
    {
        /// <summary>
        ///     Create a symbolic link.
        /// </summary>
        /// <param name="at">The filesystem location of the symbolic link to be created.</param>
        /// <param name="to">The filesystem location to which the symbolic link is to point. This need not exist.</param>
        public static void CreateSymbolicLink (string at, string to)
        {
            if (string.IsNullOrEmpty (value: at))
                throw new ArgumentNullException (paramName: nameof (at),
                                                 message: "Symbolic link location cannot be null or empty.");

            if (string.IsNullOrEmpty (value: to))
                throw new ArgumentNullException (paramName: nameof (to),
                                                 message: "Symbolic link destination cannot be null or empty.");

            unsafe
            {
                fixed (byte* bAt = Encoding.UTF8.GetBytes (s: at))
                fixed (byte* bTo = Encoding.UTF8.GetBytes (s: to))
                {
                    if (LibC.symlink (target: bTo, linkpath: bAt) == 0) return;

                    // Error handling.
                    Helpers.ConvertErrnoToException (errno: LibC.errno);
                }
            }
        }
    }
}
