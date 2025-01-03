/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Diagnostics;
using System.Runtime.InteropServices;
using LemonShark.Structs;

namespace LemonShark
{
    namespace Structs
    {
        [DebuggerDisplay("{Id}")]
        public readonly struct EpanPacketStruct : IDisposable
        {
            internal readonly IntPtr EpanPacketReference;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_packet_external_ref_count_add(IntPtr epan_packet, long ref_count);

            internal EpanPacketStruct(IntPtr epanPacketReference)
            {
                if (epanPacketReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(epanPacketReference));
                }
                EpanPacketReference = epanPacketReference;
                ls_epan_packet_external_ref_count_add(EpanPacketReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_epan_packet_free(IntPtr epan_packet);

            public void Dispose()
            {
                if (EpanPacketReference == IntPtr.Zero)
                {
                    return;
                }
                ls_epan_packet_external_ref_count_add(EpanPacketReference, -1L);
                ls_epan_packet_free(EpanPacketReference);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_size();

            public static int Size => ls_epan_packet_size();

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_packet_valid_get(IntPtr epan_packet);
            public bool Valid
            {
                get
                {
                    if (EpanPacketReference == IntPtr.Zero)
                    {
                        return false;
                    }

                    return ls_epan_packet_valid_get(EpanPacketReference) != 0;
                }
            }

            private void ThrowIfNotValid()
            {
                if (!Valid)
                {
                    throw new InvalidOperationException("EpanPacket is expired.");
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_id_get(IntPtr epan_packet);
            public int Id
            {
                get
                {
                    ThrowIfNotValid();
                    int id = ls_epan_packet_id_get(EpanPacketReference);
                    return id;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_packet_timestamp_seconds_get(IntPtr epan_packet);
            public long TimestampSeconds
            {
                get
                {
                    ThrowIfNotValid();
                    long timestampSeconds = ls_epan_packet_timestamp_seconds_get(EpanPacketReference);
                    return timestampSeconds;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_timestamp_nanoseconds_get(IntPtr epan_packet);
            public int TimestampNanoSeconds
            {
                get
                {
                    ThrowIfNotValid();
                    int timestampNanoSeconds = ls_epan_packet_timestamp_nanoseconds_get(EpanPacketReference);
                    return timestampNanoSeconds;
                }
            }

            public double Timestamp => (double)TimestampSeconds + (double)TimestampNanoSeconds / 1000000000.0;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_length_get(IntPtr epan_packet);
            public int Length
            {
                get
                {
                    ThrowIfNotValid();
                    int length = ls_epan_packet_length_get(EpanPacketReference);
                    return length;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_interface_id_get(IntPtr epan_packet);
            public int InterfaceId
            {
                get
                {
                    ThrowIfNotValid();
                    int interfaceId = ls_epan_packet_interface_id_get(EpanPacketReference);
                    return interfaceId;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_packet_root_field_get(IntPtr epan_packet);

            public EpanFieldStruct RootFieldStruct
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr rootFieldRefenrence = ls_epan_packet_root_field_get(EpanPacketReference);

                    if (rootFieldRefenrence == IntPtr.Zero)
                    {
                        return default;
                    }

                    EpanFieldStruct rootField = new(rootFieldRefenrence);
                    return rootField;
                }
            }

            public EpanField RootField
            {
                get
                {
                    EpanField rootField = new(RootFieldStruct);
                    return rootField;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_packet_protocol_column_get(IntPtr epan_packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_epan_packet_protocol_column_set(IntPtr epan_packet, IntPtr protocolColumn);

            public string ProtocolColumn
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr protocolColumnReference = ls_epan_packet_protocol_column_get(EpanPacketReference);
                    if (protocolColumnReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(protocolColumnReference);
                    return result;
                }
                set
                {
                    ThrowIfNotValid();
                    IntPtr utf8Value = Util.StringToNativeUtf8(value);

                    try
                    {
                        ls_epan_packet_protocol_column_set(EpanPacketReference, utf8Value);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(utf8Value);
                    }
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_packet_info_column_get(IntPtr epan_packet);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_epan_packet_info_column_set(IntPtr epan_packet, IntPtr infoColumn);

            public string InfoColumn
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr infoColumnReference = ls_epan_packet_info_column_get(EpanPacketReference);
                    if (infoColumnReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(infoColumnReference);
                    return result;
                }
                set
                {
                    ThrowIfNotValid();
                    IntPtr utf8Value = Util.StringToNativeUtf8(value);

                    try
                    {
                        ls_epan_packet_info_column_set(EpanPacketReference, utf8Value);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(utf8Value);
                    }
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_visited_get(IntPtr epan_packet);
            public bool Visited
            {
                get
                {
                    ThrowIfNotValid();
                    bool visited = ls_epan_packet_visited_get(EpanPacketReference) != 0;
                    return visited;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_visible_get(IntPtr epan_packet);

            public bool Visible
            {
                get
                {
                    ThrowIfNotValid();
                    bool visible = ls_epan_packet_visible_get(EpanPacketReference) != 0;
                    return visible;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_ignored_get(IntPtr epan_packet);

            public bool Ignored
            {
                get
                {
                    ThrowIfNotValid();
                    bool ignored = ls_epan_packet_ignored_get(EpanPacketReference) != 0;
                    return ignored;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_packet_buffer_get(IntPtr packet, byte[] target, int max_length);
            public byte[] Buffer
            {
                get
                {
                    ThrowIfNotValid();

                    byte[] result = new byte[Length];

                    _ = ls_epan_packet_buffer_get(EpanPacketReference, result, result.Length);

                    return result;
                }
            }
        }
    }

    [DebuggerDisplay("{Id}")]
    public class EpanPacket
    {
        internal readonly EpanPacketStruct EpanPacketStruct;

        internal EpanPacket(EpanPacketStruct epanPacketStruct)
        {
            EpanPacketStruct = epanPacketStruct;
        }

        internal EpanPacket(IntPtr epanPacketReference)
        {
            EpanPacketStruct = new(epanPacketReference);
        }

        ~EpanPacket()
        {
            EpanPacketStruct.Dispose();
        }

        public static int Size => EpanPacketStruct.Size;

        public bool Valid => EpanPacketStruct.Valid;

        public int Id => EpanPacketStruct.Id;

        public long TimestampSeconds => EpanPacketStruct.TimestampSeconds;

        public int TimestampNanoSeconds => EpanPacketStruct.TimestampNanoSeconds;

        public int Length => EpanPacketStruct.Length;

        public int InterfaceId => EpanPacketStruct.InterfaceId;

        public EpanFieldStruct RootFieldStruct => EpanPacketStruct.RootFieldStruct;

        public EpanField RootField => EpanPacketStruct.RootField;

        public string ProtocolColumn
        {
            get
            {
                return EpanPacketStruct.ProtocolColumn;
            }
            set
            {
                EpanPacketStruct.ProtocolColumn = value;
            }
        }

        public string InfoColumn
        {
            get
            {
                return EpanPacketStruct.InfoColumn;
            }
            set
            {
                EpanPacketStruct.InfoColumn = value;
            }
        }

        public bool Visited => EpanPacketStruct.Visited;

        public bool Visible => EpanPacketStruct.Visible;

        public bool Ignored => EpanPacketStruct.Ignored;

        public byte[] Buffer => EpanPacketStruct.Buffer;
    }
}

