/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using LemonShark.Structs;
using System.Runtime.InteropServices;

namespace LemonShark;

public class Session : IDisposable
{
    private static Session _CurrentSession { get; set; } = null;

    private bool _Disposed { get; set; }


    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_session_create_from_file(IntPtr filePath, IntPtr readFilter, IntPtr profile, ref IntPtr errorMessage);

    public static Session CreateFromFile(string filePath, string readFilter, string profile)
    {
        if (_CurrentSession is not null)
        {
            throw new InvalidOperationException("There can only be one session at a time.");
        }

        LemonShark.CheckWiresharkVersion();

        IntPtr utf8FilePath = Util.StringToNativeUtf8(filePath);

        IntPtr utf8ReadFilter = Util.StringToNativeUtf8(readFilter);

        IntPtr utf8Profile = Util.StringToNativeUtf8(profile);

        IntPtr errorMessage = default;
        int creationResult = 0;

        try
        {
            creationResult = ls_session_create_from_file(utf8FilePath, utf8ReadFilter, utf8Profile, ref errorMessage);
        }
        finally
        {
            Marshal.FreeHGlobal(utf8FilePath);
            Marshal.FreeHGlobal(utf8ReadFilter);
        }

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

        if (errorMessage != IntPtr.Zero)
        {
            LemonShark.FreeMemory(errorMessage);
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

        if (errorMessage != IntPtr.Zero)
        {
            LemonShark.FreeMemory(errorMessage);
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
        IntPtr requested_field_ids,
        int requested_field_id_count,
        ref IntPtr errorMessage);

    public PacketStruct GetPacketStruct(int packetId, bool includeBuffers, bool includeColumns, bool includeRepresentations, bool includeStrings, bool includeBytes, int[] requestedFieldIds, int requestedFieldIdCount)
    {
        if (_Disposed)
        {
            throw new ObjectDisposedException(nameof(Session));
        }

        IntPtr errorMessage = default;
        IntPtr packetReference = IntPtr.Zero;

        if (requestedFieldIds is null || requestedFieldIds.Length == 0 || requestedFieldIdCount <= 0)
        {
            packetReference = ls_session_get_packet(packetId, includeBuffers ? 1 : 0, includeColumns ? 1 : 0, includeRepresentations ? 1 : 0, includeStrings ? 1 : 0, includeBytes ? 1 : 0, IntPtr.Zero, 0, ref errorMessage);
        }
        else
        {
            if (requestedFieldIdCount > requestedFieldIds.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedFieldIdCount), requestedFieldIdCount, "requestedFieldIdCount > requestedFieldIds.Length");
            }

            GCHandle requestedFieldsHandle = GCHandle.Alloc(requestedFieldIds, GCHandleType.Pinned);

            try
            {
                IntPtr requestedFieldsReference = requestedFieldsHandle.AddrOfPinnedObject();
                packetReference = ls_session_get_packet(
                    packetId,
                    includeBuffers ? 1 : 0,
                    includeColumns ? 1 : 0,
                    includeRepresentations ? 1 : 0,
                    includeStrings ? 1 : 0,
                    includeBytes ? 1 : 0,
                    requestedFieldsReference,
                    requestedFieldIdCount,
                    ref errorMessage);
            }
            finally
            {
                requestedFieldsHandle.Free();
            }
        }

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

        if (errorMessage != IntPtr.Zero)
        {
            LemonShark.FreeMemory(errorMessage);
        }

        PacketStruct packet = new(packetReference);
        return packet;
    }

    public Packet GetPacket(int packetId, bool includeBuffers, bool includeColumns, bool includeRepresentations, bool includeStrings, bool includeBytes, int[] requestedFieldIds, int requestedFieldIdCount)
    {
        PacketStruct packetStruct = GetPacketStruct(packetId, includeBuffers, includeColumns, includeRepresentations, includeStrings, includeBytes, requestedFieldIds, requestedFieldIdCount);
        Packet packet = new(packetStruct);
        return packet;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_session_get_epan_packet(int packet_id, int include_columns, IntPtr requested_field_ids, int requested_field_id_count, ref IntPtr errorMessage);

    public EpanPacketStruct GetEpanPacketStruct(int packetId, bool includeColumns, int[] requestedFieldIds, int requestedFieldIdCount)
    {
        if (_Disposed)
        {
            throw new ObjectDisposedException(nameof(Session));
        }

        IntPtr errorMessage = default;
        IntPtr epanPacketReference = IntPtr.Zero;

        if (requestedFieldIds is null || requestedFieldIds.Length == 0 || requestedFieldIdCount <= 0)
        {
            epanPacketReference = ls_session_get_epan_packet(packetId, includeColumns ? 1 : 0, IntPtr.Zero, 0, ref errorMessage);
        }
        else
        {
            if (requestedFieldIdCount > requestedFieldIds.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedFieldIdCount), requestedFieldIdCount, "requestedFieldIdCount > requestedFieldIds.Length");
            }

            GCHandle requestedFieldsHandle = GCHandle.Alloc(requestedFieldIds, GCHandleType.Pinned);

            try
            {
                IntPtr requestedFieldsReference = requestedFieldsHandle.AddrOfPinnedObject();
                epanPacketReference = ls_session_get_epan_packet(packetId, includeColumns ? 1 : 0, requestedFieldsReference, requestedFieldIdCount, ref errorMessage);
            }
            finally
            {
                requestedFieldsHandle.Free();
            }
        }

        if (epanPacketReference == IntPtr.Zero)
        {
            string message = "";
            if (errorMessage != IntPtr.Zero)
            {
                message = Util.NativeUtf8ToString(errorMessage);
                LemonShark.FreeMemory(errorMessage);
            }
            throw new Exception(message);
        }

        if (errorMessage != IntPtr.Zero)
        {
            LemonShark.FreeMemory(errorMessage);
        }

        EpanPacketStruct epanPacket = new(epanPacketReference);
        return epanPacket;
    }

    public EpanPacket GetEpanPacket(int packetId, bool includeColumns, int[] requestedFieldIds, int requestedFieldIdCount)
    {
        EpanPacketStruct epanPacketStruct = GetEpanPacketStruct(packetId, includeColumns, requestedFieldIds, requestedFieldIdCount);
        EpanPacket epanPacket = new(epanPacketStruct);
        return epanPacket;
    }
}
