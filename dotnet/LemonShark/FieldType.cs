/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class FieldType
{
    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int8();
    public static int Int8 => ls_field_type_int8();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int16();
    public static int Int16 => ls_field_type_int16();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int24();
    public static int Int24 => ls_field_type_int24();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int32();
    public static int Int32 => ls_field_type_int32();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int40();
    public static int Int40 => ls_field_type_int40();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int48();
    public static int Int48 => ls_field_type_int48();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int56();
    public static int Int56 => ls_field_type_int56();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_int64();
    public static int Int64 => ls_field_type_int64();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint8();
    public static int UInt8 => ls_field_type_uint8();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint16();
    public static int UInt16 => ls_field_type_uint16();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint24();
    public static int UInt24 => ls_field_type_uint24();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint32();
    public static int UInt32 => ls_field_type_uint32();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint40();
    public static int UInt40 => ls_field_type_uint40();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint48();
    public static int UInt48 => ls_field_type_uint48();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint56();
    public static int UInt56 => ls_field_type_uint56();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint64();
    public static int UInt64 => ls_field_type_uint64();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_none();
    public static int None => ls_field_type_none();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_protocol();
    public static int Protocol => ls_field_type_protocol();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_boolean();
    public static int Boolean => ls_field_type_boolean();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_char();
    public static int Char => ls_field_type_char();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ieee_11073_float16();
    public static int IEEE11073Float16 => ls_field_type_ieee_11073_float16();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ieee_11073_float32();
    public static int IEEE11073Float32 => ls_field_type_ieee_11073_float32();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_float();
    public static int Float => ls_field_type_float();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_double();
    public static int Double => ls_field_type_double();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_absolute_time();
    public static int AbsoluteTime => ls_field_type_absolute_time();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_relative_time();
    public static int RelativeTime => ls_field_type_relative_time();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_string();
    public static int String => ls_field_type_string();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_stringz();
    public static int StringZ => ls_field_type_stringz();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint_string();
    public static int UIntString => ls_field_type_uint_string();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ether();
    public static int Ether => ls_field_type_ether();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_bytes();
    public static int Bytes => ls_field_type_bytes();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_uint_bytes();
    public static int UIntBytes => ls_field_type_uint_bytes();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ipv4();
    public static int IPv4 => ls_field_type_ipv4();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ipv6();
    public static int IPv6 => ls_field_type_ipv6();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ipxnet();
    public static int IPXNet => ls_field_type_ipxnet();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_framenum();
    public static int frameNum => ls_field_type_framenum();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_guid();
    public static int GUID => ls_field_type_guid();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_oid();
    public static int OID => ls_field_type_oid();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_eui64();
    public static int EUI64 => ls_field_type_eui64();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_ax25();
    public static int AX25 => ls_field_type_ax25();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_vines();
    public static int Vines => ls_field_type_vines();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_rel_oid();
    public static int RelOID => ls_field_type_rel_oid();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_system_id();
    public static int SystemId => ls_field_type_system_id();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_stringzpad();
    public static int StringZPad => ls_field_type_stringzpad();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_fcwwn();
    public static int FCWWN => ls_field_type_fcwwn();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_stringztrunc();
    public static int StringZTrunc => ls_field_type_stringztrunc();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_num_types();
    public static int NumTypes => ls_field_type_num_types();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_scalar();
    public static int Scalar => ls_field_type_scalar();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_is_int64(int field_type);

    public static bool IsInt64(int fieldType)
    {
        bool result = ls_field_type_is_int64(fieldType) != 0;
        return result;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_is_uint64(int field_type);

    public static bool IsUInt64(int fieldType)
    {
        bool result = ls_field_type_is_uint64(fieldType) != 0;
        return result;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_is_double(int field_type);

    public static bool IsDouble(int fieldType)
    {
        bool result = ls_field_type_is_double(fieldType) != 0;
        return result;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_is_string(int field_type);

    public static bool IsString(int fieldType)
    {
        bool result = ls_field_type_is_string(fieldType) != 0;
        return result;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_is_bytes(int field_type);

    public static bool IsBytes(int fieldType)
    {
        bool result = ls_field_type_is_bytes(fieldType) != 0;
        return result;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_type_get_name(int field_type);

    public static string GetName(int fieldType)
    {
        if (fieldType < 0 || fieldType >= NumTypes)
        {
            return null;
        }
        IntPtr nameReference = ls_field_type_get_name(fieldType);
        if (nameReference == IntPtr.Zero)
        {
            return null;
        }
        string result = Util.NativeUtf8ToString(nameReference);
        return result;
    }

}
