/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;
using LemonShark.Structs;

namespace LemonShark
{
    namespace Structs
    {
        public readonly struct PacketStruct : IDisposable
        {
            internal readonly IntPtr PacketReference;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_packet_external_ref_count_add(IntPtr packet, long ref_count);

            internal PacketStruct(IntPtr packetReference)
            {
                if (packetReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(packetReference));
                }
                PacketReference = packetReference;
                ls_packet_external_ref_count_add(PacketReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_packet_new();

            public PacketStruct()
            {
                PacketReference = ls_packet_new();
                ls_packet_external_ref_count_add(PacketReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_free(IntPtr packet);

            public void Dispose()
            {
                if (PacketReference == IntPtr.Zero)
                {
                    return;
                }
                ls_packet_external_ref_count_add(PacketReference, -1L);
                ls_packet_free(PacketReference);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_size();

            public static int Size => ls_packet_size();

            public readonly bool Valid => PacketReference != IntPtr.Zero;

            private void ThrowIfNotValid()
            {
                if (!Valid)
                {
                    throw new InvalidOperationException("Packet is not valid.");
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_id_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_id_set(IntPtr packet, int id);

            public int Id
            {
                get
                {
                    ThrowIfNotValid();
                    int id = ls_packet_id_get(PacketReference);
                    return id;
                }
            }

            public void SetId(int id)
            {
                ThrowIfNotValid();
                if (id < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(id));
                }
                ls_packet_id_set(PacketReference, id);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_packet_timestamp_seconds_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_timestamp_seconds_set(IntPtr packet, long timestamp_seconds);

            public long TimestampSeconds
            {
                get
                {
                    ThrowIfNotValid();
                    long timestampSeconds = ls_packet_timestamp_seconds_get(PacketReference);
                    return timestampSeconds;
                }
            }

            public void SetTimestampSeconds(long timestampSeconds)
            {
                ThrowIfNotValid();
                ls_packet_timestamp_seconds_set(PacketReference, timestampSeconds);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_timestamp_nanoseconds_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_timestamp_nanoseconds_set(IntPtr packet, int timestamp_nanoseconds);

            public int TimestampNanoSeconds
            {
                get
                {
                    ThrowIfNotValid();
                    int timestampNanoSeconds = ls_packet_timestamp_nanoseconds_get(PacketReference);
                    return timestampNanoSeconds;
                }
            }

            public void SetTimestampNanoSeconds(int timestampNanoSeconds)
            {
                ThrowIfNotValid();
                if (timestampNanoSeconds < 0 || timestampNanoSeconds > 999_999_999)
                {
                    throw new ArgumentOutOfRangeException(nameof(timestampNanoSeconds));
                }
                ls_packet_timestamp_nanoseconds_set(PacketReference, timestampNanoSeconds);
            }
            public double Timestamp => (double)TimestampSeconds + (double)TimestampNanoSeconds / 1000000000.0;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_length_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_length_set(IntPtr packet, int id);

            public int Length
            {
                get
                {
                    ThrowIfNotValid();
                    int length = ls_packet_length_get(PacketReference); ;
                    return length;
                }
            }

            public void SetLength(int length)
            {
                ThrowIfNotValid();
                if (length < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }
                ls_packet_length_set(PacketReference, length);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_interface_id_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_interface_id_set(IntPtr packet, int id);

            public int InterfaceId
            {
                get
                {
                    ThrowIfNotValid();
                    int interfaceId = ls_packet_interface_id_get(PacketReference);
                    return interfaceId;
                }
            }

            public void SetInterfaceId(int interfaceId)
            {
                ThrowIfNotValid();
                ls_packet_interface_id_set(PacketReference, interfaceId);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_packet_root_field_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_root_field_set(IntPtr packet, IntPtr root_field);

            public FieldStruct RootFieldStruct
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr rootFieldRefenrence = ls_packet_root_field_get(PacketReference);

                    if (rootFieldRefenrence == IntPtr.Zero)
                    {
                        return default;
                    }

                    FieldStruct rootField = new(rootFieldRefenrence);
                    return rootField;
                }
                set
                {
                    ThrowIfNotValid();
                    ls_packet_root_field_set(PacketReference, value.FieldReference);
                }
            }

            public Field RootField
            {
                get
                {
                    FieldStruct rootFieldStrucht = RootFieldStruct;
                    Field rootField = new(rootFieldStrucht);
                    return rootField;
                }
            }

            public void SetRootField(FieldStruct rootField)
            {
                ThrowIfNotValid();
                ls_packet_root_field_set(PacketReference, rootField.FieldReference);
            }

            public void SetRootField(Field rootField) => SetRootField(rootField.FieldStruct);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_packet_protocol_column_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_protocol_column_set(IntPtr packet, IntPtr protocolColumn);

            public string ProtocolColumn
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr protocolColumnReference = ls_packet_protocol_column_get(PacketReference);
                    if (protocolColumnReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(protocolColumnReference);
                    return result;
                }
            }

            public void SetProtocolColumn(string protocolColumn)
            {
                ThrowIfNotValid();
                IntPtr utf8Value = Util.StringToNativeUtf8(protocolColumn);

                try
                {
                    ls_packet_protocol_column_set(PacketReference, utf8Value);
                }
                finally
                {
                    Marshal.FreeHGlobal(utf8Value);
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_packet_info_column_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_info_column_set(IntPtr packet, IntPtr infoColumn);

            public string InfoColumn
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr infoColumnReference = ls_packet_info_column_get(PacketReference);
                    if (infoColumnReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(infoColumnReference);
                    return result;
                }
            }

            public void SetInfoColumn(string infoColumn)
            {
                ThrowIfNotValid();
                IntPtr utf8Value = Util.StringToNativeUtf8(infoColumn);

                try
                {
                    ls_packet_info_column_set(PacketReference, utf8Value);
                }
                finally
                {
                    Marshal.FreeHGlobal(utf8Value);
                }
            }


            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_visited_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_visited_set(IntPtr packet, int visited);

            public bool Visited
            {
                get
                {
                    ThrowIfNotValid();
                    bool visited = ls_packet_visited_get(PacketReference) != 0; ;
                    return visited;
                }
            }

            public void SetVisited(bool visited)
            {
                ThrowIfNotValid();
                ls_packet_visited_set(PacketReference, visited ? 1 : 0);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_visible_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_visible_set(IntPtr packet, int visible);

            public bool Visible
            {
                get
                {
                    ThrowIfNotValid();
                    bool visible = ls_packet_visible_get(PacketReference) != 0;
                    return visible;
                }
            }

            public void SetVisible(bool visible)
            {
                ThrowIfNotValid();
                ls_packet_visible_set(PacketReference, visible ? 1 : 0);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_ignored_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_ignored_set(IntPtr packet, int ignored);

            public bool Ignored
            {
                get
                {
                    ThrowIfNotValid();
                    bool ignored = ls_packet_ignored_get(PacketReference) != 0;
                    return ignored;
                }
            }

            public void SetIgnored(bool ignored)
            {
                ThrowIfNotValid();
                ls_packet_ignored_set(PacketReference, ignored ? 1 : 0);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_packet_buffer_id_get(IntPtr packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_packet_buffer_id_set(IntPtr packet, int packet_buffer_id);

            public int PacketBufferId
            {
                get
                {
                    ThrowIfNotValid();
                    int packetBufferId = ls_packet_packet_buffer_id_get(PacketReference);
                    return packetBufferId;
                }
            }

            public void SetPacketBufferId(int packetBufferId)
            {
                ThrowIfNotValid();
                ls_packet_packet_buffer_id_set(PacketReference, packetBufferId);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_packet_buffers_count(IntPtr packet);

            public int BuffersCount
            {

                get
                {
                    ThrowIfNotValid();
                    int buffersCount = ls_packet_buffers_count(PacketReference);
                    return buffersCount;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_packet_buffers_get(IntPtr packet, int id);

            public BufferStruct GetBufferStruct(int bufferId)
            {
                ThrowIfNotValid();
                int buffersCount = BuffersCount;
                if (bufferId < 0 || bufferId >= buffersCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(bufferId));
                }

                IntPtr bufferReference = ls_packet_buffers_get(PacketReference, bufferId);

                if (bufferReference == IntPtr.Zero)
                {
                    return default;
                }

                BufferStruct buffer = new(bufferReference);
                return buffer;
            }

            public Buffer GetBuffer(int bufferId)
            {
                ThrowIfNotValid();
                BufferStruct bufferStruct = GetBufferStruct(bufferId);
                Buffer buffer = new(bufferStruct);
                return buffer;
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_buffers_set(IntPtr packet, IntPtr buffer, int id);

            public void SetBuffer(BufferStruct buffer, int bufferId)
            {
                ThrowIfNotValid();
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

            public void SetBuffer(Buffer buffer, int bufferId)
            {
                if (buffer is null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                SetBuffer(buffer.BufferStruct, bufferId);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_buffers_add(IntPtr packet, IntPtr buffer);

            public void AddBuffer(BufferStruct buffer)
            {
                ThrowIfNotValid();
                if (buffer.BufferReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                ls_packet_buffers_add(PacketReference, buffer.BufferReference);
            }

            public void AddBuffer(Buffer buffer)
            {
                if (buffer is null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                AddBuffer(buffer.BufferStruct);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_packet_buffers_remove(IntPtr packet, int bufferId);

            public void RemoveBuffer(int bufferId)
            {
                ThrowIfNotValid();
                int buffersCount = BuffersCount;
                if (bufferId < 0 || bufferId >= buffersCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(bufferId));
                }

                ls_packet_buffers_remove(PacketReference, bufferId);
            }

            public List<BufferStruct> BufferStructs
            {
                get
                {
                    ThrowIfNotValid();
                    List<BufferStruct> result = [];
                    int buffersCount = BuffersCount;
                    for (int i = 0; i < buffersCount; i++)
                    {
                        BufferStruct buffer = GetBufferStruct(i);
                        result.Add(buffer);
                    }
                    return result;
                }
            }

            public List<Buffer> Buffers
            {
                get
                {
                    ThrowIfNotValid();
                    List<Buffer> result = [];
                    int buffersCount = BuffersCount;
                    for (int i = 0; i < buffersCount; i++)
                    {
                        Buffer buffer = GetBuffer(i);
                        result.Add(buffer);
                    }
                    return result;
                }
            }
        }

    }

    public class Packet
    {
        internal readonly PacketStruct PacketStruct;

        internal Packet(PacketStruct packetStruct)
        {
            PacketStruct = packetStruct;
        }

        internal Packet(IntPtr packetReference)
        {
            if (packetReference == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(packetReference));
            }
            PacketStruct = new(packetReference);
        }

        public Packet()
        {
            PacketStruct = new();
        }

        ~Packet()
        {
            PacketStruct.Dispose();
        }

        public static int Size => PacketStruct.Size;

        public int Id => PacketStruct.Id;

        public void SetId(int id) => PacketStruct.SetId(id);

        public long TimestampSeconds => PacketStruct.TimestampSeconds;

        public void SetTimestampSeconds(long timestampSeconds) => PacketStruct.SetTimestampSeconds(timestampSeconds);


        public int TimestampNanoSeconds => PacketStruct.TimestampNanoSeconds;

        public void SetTimestampNanoSeconds(int timestampNanoSeconds) => PacketStruct.SetTimestampNanoSeconds(timestampNanoSeconds);
        public double Timestamp => PacketStruct.Timestamp;

        public int Length => PacketStruct.Length;

        public void SetLength(int length) => PacketStruct.SetLength(length);

        public int InterfaceId => PacketStruct.InterfaceId;

        public void SetInterfaceId(int interfaceId) => PacketStruct.SetInterfaceId(interfaceId);

        public FieldStruct RootFieldStruct => PacketStruct.RootFieldStruct;

        public Field RootField => PacketStruct.RootField;

        public void SetRootField(FieldStruct rootField) => PacketStruct.SetRootField(rootField);

        public void SetRootField(Field rootField) => SetRootField(rootField.FieldStruct);

        public string ProtocolColumn => PacketStruct.ProtocolColumn;

        public void SetProtocolColumn(string protocolColumn) => PacketStruct.SetProtocolColumn(protocolColumn);

        public string InfoColumn => PacketStruct.InfoColumn;

        public void SetInfoColumn(string infoColumn) => PacketStruct.SetInfoColumn(infoColumn);

        public bool Visited => PacketStruct.Visited;

        public void SetVisited(bool visited) => PacketStruct.SetVisited(visited);
        public bool Visible => PacketStruct.Visible;

        public void SetVisible(bool visible) => PacketStruct.SetVisible(visible);

        public bool Ignored => PacketStruct.Ignored;

        public void SetIgnored(bool ignored) => PacketStruct.SetIgnored(ignored);

        public int PacketBufferId => PacketStruct.PacketBufferId;

        public void SetPacketBufferId(int packetBufferId) => PacketStruct.SetPacketBufferId(packetBufferId);

        public int BuffersCount => PacketStruct.BuffersCount;

        public BufferStruct GetBufferStruct(int bufferId) => PacketStruct.GetBufferStruct(bufferId);

        public Buffer GetBuffer(int bufferId) => PacketStruct.GetBuffer(bufferId);

        public void SetBuffer(BufferStruct buffer, int bufferId) => PacketStruct.SetBuffer(buffer, bufferId);

        public void SetBuffer(Buffer buffer, int bufferId) => PacketStruct.SetBuffer(buffer, bufferId);

        public void AddBuffer(BufferStruct buffer) => PacketStruct.AddBuffer(buffer);

        public void AddBuffer(Buffer buffer) => PacketStruct.AddBuffer(buffer);

        public void RemoveBuffer(int bufferId) => PacketStruct.RemoveBuffer(bufferId);

        public List<BufferStruct> BufferStructs => PacketStruct.BufferStructs;

        public List<Buffer> Buffers => PacketStruct.Buffers;
    }
}

