/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class Field
{
    internal readonly IntPtr FieldReference;

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_field_external_ref_count_add(IntPtr field, long ref_count);

    internal Field(IntPtr fieldReference)
    {
        if (fieldReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(fieldReference));
        }
        FieldReference = fieldReference;
        ls_field_external_ref_count_add(FieldReference, 1L);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_new();
    public Field()
    {
        FieldReference = ls_field_new();
        ls_field_external_ref_count_add(FieldReference, 1L);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_free(IntPtr field);

    ~Field()
    {
        ls_field_external_ref_count_add(FieldReference, -1L);
        ls_field_free(FieldReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_size();

    public static int Size => ls_field_size();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_representation_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_representation_set(IntPtr field, [MarshalAs(UnmanagedType.LPStr)] string representation);

    public string Representation
    {
        get
        {
            IntPtr representationReference = ls_field_representation_get(FieldReference);
            if (representationReference == IntPtr.Zero)
            {
                return null;
            }
            string result = Util.NativeUtf8ToString(representationReference);
            return result;
        }
        set => ls_field_representation_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_id_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_id_set(IntPtr field, int id);

    public int Id
    {
        get => ls_field_id_get(FieldReference);
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            ls_field_id_set(FieldReference, value);
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_type_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_type_set(IntPtr field, int type);

    public int Type
    {
        get => ls_field_type_get(FieldReference);
        set => ls_field_type_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_buffer_id_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_buffer_id_set(IntPtr field, int buffer_id);

    public int BufferId
    {
        get => ls_field_buffer_id_get(FieldReference);
        set => ls_field_buffer_id_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_buffer_offset_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_buffer_offset_set(IntPtr field, int buffer_offset);

    public int BufferOffset
    {
        get => ls_field_buffer_offset_get(FieldReference);
        set => ls_field_buffer_offset_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_buffer_length_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_buffer_length_set(IntPtr field, int buffer_offset);

    public int BufferLength
    {
        get => ls_field_buffer_length_get(FieldReference);
        set => ls_field_buffer_length_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_hidden_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_hidden_set(IntPtr field, int hidden);

    public bool Hidden
    {
        get => ls_field_hidden_get(FieldReference) != 0;
        set => ls_field_hidden_set(FieldReference, value ? 1 : 0);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_generated_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_generated_set(IntPtr field, int generated);

    public bool Generated
    {
        get => ls_field_generated_get(FieldReference) != 0;
        set => ls_field_generated_set(FieldReference, value ? 1 : 0);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_encoding_get(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_encoding_set(IntPtr field, int encoding);

    public int Encoding
    {
        get => ls_field_encoding_get(FieldReference);
        set => ls_field_encoding_set(FieldReference, value);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_field_value_get_int64(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_set_int64(IntPtr field, long value, int type);

    public long GetInt64Value()
    {
        if (!IsInt64)
        {
            throw new InvalidOperationException("Value is not of type int64.");
        }

        long value = ls_field_value_get_int64(FieldReference);
        return value;
    }

    public void SetInt64Value(long value, int type)
    {
        int setResult = ls_field_value_set_int64(FieldReference, value, type);
        if (setResult == LemonShark.Error)
        {
            throw new InvalidOperationException("Invalid type");
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern ulong ls_field_value_get_uint64(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_set_uint64(IntPtr field, ulong value, int type);

    public ulong GetUInt64Value()
    {
        if (!IsUInt64)
        {
            throw new InvalidOperationException("Value is not of type uint64.");
        }

        ulong value = ls_field_value_get_uint64(FieldReference);
        return value;
    }

    public void SetUInt64Value(ulong value, int type)
    {
        int setResult = ls_field_value_set_uint64(FieldReference, value, type);
        if (setResult == LemonShark.Error)
        {
            throw new InvalidOperationException("Invalid type");
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern double ls_field_value_get_double(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_set_double(IntPtr field, double value, int type);

    public double GetDoubleValue()
    {
        if (!IsDouble)
        {
            throw new InvalidOperationException("Value is not of type double.");
        }

        double value = ls_field_value_get_double(FieldReference);
        return value;
    }

    public void SetDoubleValue(double value, int type)
    {
        int setResult = ls_field_value_set_double(FieldReference, value, type);
        if (setResult == LemonShark.Error)
        {
            throw new InvalidOperationException("Invalid type");
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_value_get_string(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_set_string(IntPtr field, [MarshalAs(UnmanagedType.LPStr)] string value, int type);

    public string GetStringValue()
    {
        if (!IsString)
        {
            throw new InvalidOperationException("Value is not of type string.");
        }

        IntPtr valueReference = ls_field_value_get_string(FieldReference);
        if (valueReference == IntPtr.Zero)
        {
            return null;
        }
        string result = Util.NativeUtf8ToString(valueReference);
        return result;
    }

    public void SetStringValue(string value, int type)
    {
        int setResult = ls_field_value_set_string(FieldReference, value, type);
        if (setResult == LemonShark.Error)
        {
            throw new InvalidOperationException("Invalid type");
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_value_get_bytes(IntPtr field);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_set_bytes(IntPtr field, byte[] value, int length, int type);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_value_length_get(IntPtr field);

    public byte[] GetBytesValue()
    {
        if (!IsBytes)
        {
            throw new InvalidOperationException("Value is not of type bytes.");
        }

        IntPtr valueReference = ls_field_value_get_bytes(FieldReference);

        if (valueReference == IntPtr.Zero)
        {
            return null;
        }

        int length = ValueLength;

        byte[] array = new byte[length];

        Marshal.Copy(valueReference, array, 0, length);

        return array;
    }

    public void SetBytesValue(byte[] value, int type)
    {
        int setResult = ls_field_value_set_bytes(FieldReference, value, value is not null ? value.Length : 0, type);
        if (setResult == LemonShark.Error)
        {
            throw new InvalidOperationException("Invalid type");
        }
    }

    public int ValueLength => ls_field_value_length_get(FieldReference);

    public bool IsInt64 => FieldType.IsInt64(Type);

    public bool IsUInt64 => FieldType.IsUInt64(Type);

    public bool IsDouble => FieldType.IsDouble(Type);

    public bool IsString => FieldType.IsString(Type);

    public bool IsBytes => FieldType.IsBytes(Type);


    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_children_count(IntPtr field);

    public int ChildrenCount
    {
        get => ls_field_children_count(FieldReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_children_get(IntPtr field, int index);

    public Field GetChild(int index)
    {
        int childrenCount = ChildrenCount;
        if (index < 0 || index >= childrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        IntPtr childReference = ls_field_children_get(FieldReference, index);

        if (childReference == IntPtr.Zero)
        {
            return null;
        }

        Field child = new(childReference);
        return child;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_children_set(IntPtr field, IntPtr child, int index);

    public void SetChild(Field child, int index)
    {
        if (child is null)
        {
            throw new ArgumentNullException(nameof(child));
        }
        if (child.FieldReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(child));
        }

        int childrenCount = ChildrenCount;
        if (index < 0 || index >= childrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        ls_field_children_set(FieldReference, child.FieldReference, index);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_children_add(IntPtr field, IntPtr child);

    public void AddChild(Field child)
    {
        if (child is null)
        {
            throw new ArgumentNullException(nameof(child));
        }
        if (child.FieldReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(child));
        }

        ls_field_children_add(FieldReference, child.FieldReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_children_remove(IntPtr field, int index);

    public Field RemoveChild(int index)
    {
        int childrenCount = ChildrenCount;
        if (index < 0 || index >= childrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        IntPtr childReference = ls_field_children_remove(FieldReference, index);

        if (childReference == IntPtr.Zero)
        {
            return null;
        }

        Field removedChild = new(childReference);
        return removedChild;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_get_name(int field_id);

    public static string GetName(int fieldId)
    {
        IntPtr nameReference = ls_field_get_name(fieldId);
        if (nameReference == IntPtr.Zero)
        {
            return null;
        }
        string result = Util.NativeUtf8ToString(nameReference);
        return result;
    }

    public string Name => GetName(Id);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_get_display_name(int field_id);

    public static string GetDisplayName(int fieldId)
    {
        IntPtr displayNameReference = ls_field_get_display_name(fieldId);
        if (displayNameReference == IntPtr.Zero)
        {
            return null;
        }
        string result = Util.NativeUtf8ToString(displayNameReference);

        return result;
    }

    public string DisplayName => GetDisplayName(Id);

    public string TypeName => FieldType.GetName(Type);
}
