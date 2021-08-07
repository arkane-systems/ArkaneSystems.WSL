#region header

// ArkaneSystems.WSL - Helpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/08/07 12:44 PM.

#endregion

#region using

using System;
using System.Text;

using Tmds.Linux;

#endregion

namespace ArkaneSystems.WindowsSubsystemForLinux
{
    /// <summary>
    ///     Internal helper functions for this library.
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        ///     Convert a Linux error code to a .NET exception.
        /// </summary>
        /// <param name="errno">The error code received.</param>
        internal static void ConvertErrnoToException (int errno)
        {
            byte[] messageBuf = new byte[1024];

            unsafe
            {
                fixed (byte* bMessage = messageBuf)
                {
                    if (LibC.strerror_r (errnum: errno, buf: bMessage, buflen: 1024) != 0)
                    {
                        var oops =
                            new InvalidOperationException (message: $"Unable to translate error message. Sorry. errno={errno}");

                        oops.Data[key: "errno"] = errno;

                        throw oops;
                    }
                }
            }

            string message = Encoding.UTF8.GetString (bytes: messageBuf);

            var ex = new InvalidOperationException (message: message);
            ex.Data[key: "errno"] = errno;

            throw ex;
        }
    }
}
