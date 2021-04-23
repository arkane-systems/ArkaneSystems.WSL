#region header

// ArkaneSystems.WSL - HostHelpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/04/23 9:02 AM.

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
    ///     Helper functions for the Linux host.
    /// </summary>
    [PublicAPI]
    public static class HostHelpers
    {
        /// <summary>
        ///     The hostname of the Linux host (current UTS namespace).
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         This property is a wrapper around gethostname()/sethostname(). Reading it returns the system hostname; writing
        ///         it sets the system hostname immediately.
        ///     </p>
        /// </remarks>
        public static string Hostname
        {
            get
            {
                unsafe
                {
                    int success;

                    var bytes = new byte[64];

                    fixed (byte* buffer = bytes)
                    {
                        success = LibC.gethostname (name: buffer, len: 64);
                    }

                    if (success != 0)
                        throw new InvalidOperationException (message: $"Error retrieving hostname: {success}");

                    return Encoding.UTF8.GetString (bytes: bytes).TrimEnd (trimChar: '\0');
                }
            }
            set
            {
                unsafe
                {
                    var bytes = Encoding.UTF8.GetBytes (s: value);

                    fixed (byte* bHostname = bytes)
                    {
                        var success = LibC.sethostname (name: bHostname, len: bytes.Length);

                        if (success != 0)
                            throw new InvalidOperationException (message: $"Error setting hostname: {success}");
                    }
                }
            }
        }
    }
}
