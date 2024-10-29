/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class Session : IDisposable
{
    private static Session _CurrentSession { get; set; } = null;

    private bool _Disposed { get; set; }


    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_session_create_from_file([MarshalAs(UnmanagedType.LPStr)] string filePath, [MarshalAs(UnmanagedType.LPStr)] string readFilter, ref IntPtr errorMessage);

    public static Session CreateFromFile(string filePath, string readFilter)
    {
        if (_CurrentSession is not null)
        {
            throw new InvalidOperationException("There can only be one session at a time.");
        }

        IntPtr errorMessage = default;
        int creationResult = ls_session_create_from_file(filePath, readFilter, ref errorMessage);

        if (creationResult == LemonShark.Error)
        {
            string message = "";
            if (errorMessage != IntPtr.Zero)
            {
                message = Util.NativeUtf8ToString(errorMessage);
                LemonShark.FreeMemory(errorMessage);
            }
            throw new Exception(message);
        }

        Session session = new();

        _CurrentSession = session;
        return session;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_session_close();

    public void Close()
    {
        if (_Disposed)
        {
            return;
        }

        _CurrentSession = null;
        _Disposed = true;

        ls_session_close();
    }

    public void Dispose()
    {
        Close();
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_session_get_next_packet_id(ref IntPtr errorMessage);

    public bool GetNextPacketId(out int packetId)
    {
        if (_Disposed)
        {
            throw new ObjectDisposedException(nameof(Session));
        }

        IntPtr errorMessage = default;
        packetId = ls_session_get_next_packet_id(ref errorMessage);
        if (packetId < 0)
        {
            //if not error message is given we assume as regular finish without a failure
            if (errorMessage != IntPtr.Zero)
            {
                string message = Util.NativeUtf8ToString(errorMessage);
                LemonShark.FreeMemory(errorMessage);
                throw new Exception(message);
            }

            return false;
        }

        return true;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_session_get_packet(int packet_id,
                                                        int include_buffers,
                                                        int include_columns,
                                                        int include_representations,
                                                        int include_strings,
                                                        int include_bytes,
                                                        ref IntPtr errorMessage);

    public Packet GetPacket(int packetId,
                            bool includeBuffers,
                            bool includeColumns,
                            bool includeRepresentations,
                            bool includeStrings,
                            bool includeBytes)
    {
        if (_Disposed)
        {
            throw new ObjectDisposedException(nameof(Session));
        }

        IntPtr errorMessage = default;
        IntPtr packetReference = ls_session_get_packet(packetId, includeBuffers ? 1 : 0, includeColumns ? 1 : 0, includeRepresentations ? 1 : 0, includeStrings ? 1 : 0, includeBytes ? 1 : 0, ref errorMessage);

        if (packetReference == IntPtr.Zero)
        {
            string message = "";
            if (errorMessage != IntPtr.Zero)
            {
                message = Util.NativeUtf8ToString(errorMessage);
                LemonShark.FreeMemory(errorMessage);
            }
            throw new Exception(message);
        }

        Packet packet = new(packetReference);
        return packet;
    }
}
