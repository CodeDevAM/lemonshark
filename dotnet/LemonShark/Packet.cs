/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class Packet
{
    internal readonly IntPtr PacketReference;

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_packet_external_ref_count_add(IntPtr packet, long ref_count);

    internal Packet(IntPtr packetReference)
    {
        if (packetReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(packetReference));
        }
        PacketReference = packetReference;
        ls_packet_external_ref_count_add(PacketReference, 1L);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_free(IntPtr packet);

    ~Packet()
    {
        ls_packet_external_ref_count_add(PacketReference, -1L);
        ls_packet_free(PacketReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_size();

    public static int Size => ls_packet_size();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_id_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_id_set(IntPtr packet, int id);

    public int Id
    {
        get => ls_packet_id_get(PacketReference);
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            ls_packet_id_set(PacketReference, value);
        }

    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_packet_timestamp_seconds_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_timestamp_seconds_set(IntPtr packet, long timestamp_seconds);

    public long TimestampSeconds
    {
        get => ls_packet_timestamp_seconds_get(PacketReference);
        set => ls_packet_timestamp_seconds_set(PacketReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_timestamp_nanoseconds_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_timestamp_nanoseconds_set(IntPtr packet, int timestamp_nanoseconds);

    public int TimestampNanoSeconds
    {
        get => ls_packet_timestamp_nanoseconds_get(PacketReference);
        set
        {
            if (value < 0 || value > 999_999_999)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            ls_packet_timestamp_nanoseconds_set(PacketReference, value);
        }
    }

    public double Timestamp => (double)TimestampSeconds + (double)TimestampNanoSeconds / 1000000000.0;

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_length_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_length_set(IntPtr packet, int id);

    public int Length
    {
        get => ls_packet_length_get(PacketReference);
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            ls_packet_length_set(PacketReference, value);
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_interface_id_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_interface_id_set(IntPtr packet, int id);

    public int InterfaceId
    {
        get => ls_packet_interface_id_get(PacketReference);
        set => ls_packet_interface_id_set(PacketReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_packet_root_field_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_root_field_set(IntPtr packet, IntPtr root_field);

    public Field RootField
    {
        get
        {
            IntPtr fieldReference = ls_packet_root_field_get(PacketReference);

            if (fieldReference == IntPtr.Zero)
            {
                return null;
            }

            Field field = new(fieldReference);
            return field;
        }
        set
        {
            ls_packet_root_field_set(PacketReference, value.FieldReference);
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_packet_protocol_column_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_protocol_column_set(IntPtr packet, [MarshalAs(UnmanagedType.LPStr)] string protocolColumn);

    public string ProtocolColumn
    {
        get
        {
            IntPtr protocolColumnReference = ls_packet_protocol_column_get(PacketReference);
            if (protocolColumnReference == IntPtr.Zero)
            {
                return null;
            }
            string result = Util.NativeUtf8ToString(protocolColumnReference);
            return result;
        }
        set => ls_packet_protocol_column_set(PacketReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_packet_info_column_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_info_column_set(IntPtr packet, [MarshalAs(UnmanagedType.LPStr)] string infoColumn);

    public string InfoColumn
    {
        get
        {
            IntPtr infoColumnReference = ls_packet_info_column_get(PacketReference);
            if (infoColumnReference == IntPtr.Zero)
            {
                return null;
            }
            string result = Util.NativeUtf8ToString(infoColumnReference);
            return result;
        }
        set => ls_packet_info_column_set(PacketReference, value);
    }


    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_visited_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_visited_set(IntPtr packet, int visited);

    public bool Visited
    {
        get => ls_packet_visited_get(PacketReference) != 0;
        set => ls_packet_visited_set(PacketReference, value ? 1 : 0);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_visible_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_visible_set(IntPtr packet, int visible);

    public bool Visible
    {
        get => ls_packet_visible_get(PacketReference) != 0;
        set => ls_packet_visible_set(PacketReference, value ? 1 : 0);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_ignored_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_ignored_set(IntPtr packet, int ignored);

    public bool Ignored
    {
        get => ls_packet_ignored_get(PacketReference) != 0;
        set => ls_packet_ignored_set(PacketReference, value ? 1 : 0);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_packet_buffer_id_get(IntPtr packet);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_packet_buffer_id_set(IntPtr packet, int packet_buffer_id);

    public int PacketBufferId
    {
        get => ls_packet_packet_buffer_id_get(PacketReference);
        set => ls_packet_packet_buffer_id_set(PacketReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_packet_buffers_count(IntPtr packet);

    public int BuffersCount
    {
        get => ls_packet_buffers_count(PacketReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_packet_buffers_get(IntPtr packet, int id);

    public Buffer GetBuffer(int bufferId)
    {
        int buffersCount = BuffersCount;
        if (bufferId < 0 || bufferId >= buffersCount)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferId));
        }

        IntPtr bufferReference = ls_packet_buffers_get(PacketReference, bufferId);

        if (bufferReference == IntPtr.Zero)
        {
            return null;
        }

        Buffer buffer = new(bufferReference);
        return buffer;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_buffers_set(IntPtr packet, IntPtr buffer, int id);

    public void SetBuffer(Buffer buffer, int bufferId)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (buffer.BufferReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        int buffersCount = BuffersCount;
        if (bufferId < 0 || bufferId >= buffersCount)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferId));
        }

        ls_packet_buffers_set(PacketReference, buffer.BufferReference, bufferId);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_buffers_add(IntPtr packet, IntPtr buffer);

    public void AddBuffer(Buffer buffer)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (buffer.BufferReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        ls_packet_buffers_add(PacketReference, buffer.BufferReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_packet_buffers_remove(IntPtr packet, int bufferId);

    public void RemoveBuffer(int bufferId)
    {
        int buffersCount = BuffersCount;
        if (bufferId < 0 || bufferId >= buffersCount)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferId));
        }

        ls_packet_buffers_remove(PacketReference, bufferId);
    }

}
