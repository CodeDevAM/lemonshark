/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using LemonShark.Structs;
using System.Runtime.InteropServices;

namespace LemonShark
{
    namespace Structs
    {
        public readonly struct FieldStruct : IDisposable
        {
            internal readonly IntPtr FieldReference;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_field_external_ref_count_add(IntPtr field, long ref_count);

            internal FieldStruct(IntPtr fieldReference)
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
            public FieldStruct()
            {
                FieldReference = ls_field_new();
                ls_field_external_ref_count_add(FieldReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_free(IntPtr field);

            public void Dispose()
            {
                if (FieldReference == IntPtr.Zero)
                {
                    return;
                }
                ls_field_external_ref_count_add(FieldReference, -1L);
                ls_field_free(FieldReference);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_size();

            public static int Size => ls_field_size();

            public readonly bool Valid => FieldReference != IntPtr.Zero;

            private void ThrowIfNotValid()
            {
                if (!Valid)
                {
                    throw new InvalidOperationException("Field is not valid.");
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_representation_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_representation_set(IntPtr field, IntPtr representation);

            public string Representation
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr representationReference = ls_field_representation_get(FieldReference);
                    if (representationReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(representationReference);
                    return result;
                }
            }

            public void SetRepresentation(string representation)
            {
                ThrowIfNotValid();
                IntPtr utf8Value = Util.StringToNativeUtf8(representation);
                try
                {
                    ls_field_representation_set(FieldReference, utf8Value);
                }
                finally
                {
                    Marshal.FreeHGlobal(utf8Value);
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_id_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_id_set(IntPtr field, int id);

            public int Id
            {
                get
                {
                    ThrowIfNotValid();
                    int id = ls_field_id_get(FieldReference);
                    return id;
                }
            }

            public void SetId(int id)
            {
                ThrowIfNotValid();
                if (id < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(id));
                }
                ls_field_id_set(FieldReference, id);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_type_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_type_set(IntPtr field, int type);

            public int Type
            {
                get
                {
                    ThrowIfNotValid();
                    int type = ls_field_type_get(FieldReference);
                    return type;
                }
            }


            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_name_get(IntPtr field);
            public string Name
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr nameReference = ls_field_name_get(FieldReference);
                    if (nameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(nameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_display_name_get(IntPtr field);
            public string DisplayName
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr displayNameReference = ls_field_display_name_get(FieldReference);
                    if (displayNameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(displayNameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_type_name_get(IntPtr field);
            public string TypeName
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr typeNameReference = ls_field_type_name_get(FieldReference);
                    if (typeNameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(typeNameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_buffer_id_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_buffer_id_set(IntPtr field, int buffer_id);

            public int BufferId
            {
                get
                {
                    ThrowIfNotValid();
                    int bufferId = ls_field_buffer_id_get(FieldReference);
                    return bufferId;
                }
            }

            public void SetBufferId(int bufferId)
            {
                ThrowIfNotValid();
                ls_field_buffer_id_set(FieldReference, bufferId);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_offset_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_offset_set(IntPtr field, int offset);

            public int Offset
            {
                get
                {
                    ThrowIfNotValid();
                    int offset = ls_field_offset_get(FieldReference);
                    return offset;
                }
            }

            public void SetOffset(int offset)
            {
                ThrowIfNotValid();
                ls_field_offset_set(FieldReference, offset);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_length_get(IntPtr field);

            public int Length
            {
                get
                {
                    ThrowIfNotValid();
                    int length = ls_field_length_get(FieldReference);
                    return length;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_hidden_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_hidden_set(IntPtr field, int hidden);

            public bool Hidden
            {
                get
                {
                    ThrowIfNotValid();
                    bool hidden = ls_field_hidden_get(FieldReference) != 0;
                    return hidden;
                }
            }

            public void SetHidden(bool hidden)
            {
                ThrowIfNotValid();
                ls_field_hidden_set(FieldReference, hidden ? 1 : 0);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_generated_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_generated_set(IntPtr field, int generated);

            public bool Generated
            {
                get
                {
                    ThrowIfNotValid();
                    bool generated = ls_field_generated_get(FieldReference) != 0;
                    return generated;
                }
            }

            public void SetGenerated(bool generated)
            {
                ThrowIfNotValid();
                ls_field_generated_set(FieldReference, generated ? 1 : 0);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_encoding_get(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_encoding_set(IntPtr field, int encoding);

            public int Encoding
            {
                get
                {
                    ThrowIfNotValid();
                    int encoding = ls_field_encoding_get(FieldReference);
                    return encoding;
                }
            }

            public void SetEncoding(int encoding)
            {
                ThrowIfNotValid();
                ls_field_encoding_set(FieldReference, encoding);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_field_value_get_int64(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_value_set_int64(IntPtr field, long value, int type);

            public long Int64Value
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsInt64)
                    {
                        throw new InvalidOperationException("Value is not of type int64.");
                    }

                    long value = ls_field_value_get_int64(FieldReference);
                    return value;
                }
            }

            public void SetInt64Value(long value, int type)
            {
                ThrowIfNotValid();
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

            public ulong UInt64Value
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsUInt64)
                    {
                        throw new InvalidOperationException("Value is not of type uint64.");
                    }

                    ulong value = ls_field_value_get_uint64(FieldReference);
                    return value;
                }
            }

            public void SetUInt64Value(ulong value, int type)
            {
                ThrowIfNotValid();
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

            public double DoubleValue
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsDouble)
                    {
                        throw new InvalidOperationException("Value is not of type double.");
                    }

                    double value = ls_field_value_get_double(FieldReference);
                    return value;
                }
            }

            public void SetDoubleValue(double value, int type)
            {
                ThrowIfNotValid();
                int setResult = ls_field_value_set_double(FieldReference, value, type);
                if (setResult == LemonShark.Error)
                {
                    throw new InvalidOperationException("Invalid type");
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_value_get_string(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_value_set_string(IntPtr field, IntPtr value, int type);

            public string StringValue
            {
                get
                {
                    ThrowIfNotValid();
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
            }

            public void SetStringValue(string value, int type)
            {
                ThrowIfNotValid();
                IntPtr utf8Value = Util.StringToNativeUtf8(value);

                int setResult = 0;
                try
                {
                    setResult = ls_field_value_set_string(FieldReference, utf8Value, type);

                }
                finally
                {
                    Marshal.FreeHGlobal(utf8Value);
                }

                if (setResult == LemonShark.Error)
                {
                    throw new InvalidOperationException("Invalid type");
                }

            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_value_get_bytes(IntPtr field);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_value_set_bytes(IntPtr field, byte[] value, int length, int type);

            public byte[] BytesValue
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsBytes)
                    {
                        throw new InvalidOperationException("Value is not of type bytes.");
                    }

                    IntPtr valueReference = ls_field_value_get_bytes(FieldReference);

                    if (valueReference == IntPtr.Zero)
                    {
                        return null;
                    }

                    byte[] array = new byte[Length];

                    Marshal.Copy(valueReference, array, 0, array.Length);

                    return array;
                }
            }

            public void SetBytesValue(byte[] value, int type)
            {
                ThrowIfNotValid();
                int setResult = ls_field_value_set_bytes(FieldReference, value, value is not null ? value.Length : 0, type);
                if (setResult == LemonShark.Error)
                {
                    throw new InvalidOperationException("Invalid type");
                }
            }

            public bool IsInt64 => FieldType.IsInt64(Type);

            public bool IsUInt64 => FieldType.IsUInt64(Type);

            public bool IsDouble => FieldType.IsDouble(Type);

            public bool IsString => FieldType.IsString(Type);

            public bool IsBytes => FieldType.IsBytes(Type);


            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_field_children_count(IntPtr field);

            public int ChildrenCount
            {
                get
                {
                    ThrowIfNotValid();
                    int childrenCount = ls_field_children_count(FieldReference);
                    return childrenCount;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_children_get(IntPtr field, int index);

            public FieldStruct GetChildStruct(int index)
            {
                ThrowIfNotValid();
                int childrenCount = ChildrenCount;
                if (index < 0 || index >= childrenCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                IntPtr childReference = ls_field_children_get(FieldReference, index);

                if (childReference == IntPtr.Zero)
                {
                    return default;
                }

                FieldStruct child = new(childReference);
                return child;
            }

            public Field GetChild(int index)
            {
                FieldStruct childStruct = GetChildStruct(index);
                Field child = new(childStruct);
                return child;
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_children_set(IntPtr field, IntPtr child, int index);

            public void SetChild(FieldStruct child, int index)
            {
                ThrowIfNotValid();

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

            public void SetChild(Field child, int index)
            {
                if (child is null)
                {
                    throw new ArgumentNullException(nameof(child));
                }

                SetChild(child.FieldStruct, index);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_children_add(IntPtr field, IntPtr child);

            public void AddChild(FieldStruct child)
            {
                ThrowIfNotValid();

                if (child.FieldReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(child));
                }

                ls_field_children_add(FieldReference, child.FieldReference);
            }

            public void AddChild(Field child)
            {
                ThrowIfNotValid();
                if (child is null)
                {
                    throw new ArgumentNullException(nameof(child));
                }

                AddChild(child.FieldStruct);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_field_children_remove(IntPtr field, int index);

            public void RemoveChild(int index)
            {
                ThrowIfNotValid();
                int childrenCount = ChildrenCount;
                if (index < 0 || index >= childrenCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                ls_field_children_remove(FieldReference, index);
            }

            public List<FieldStruct> ChildrenStructs
            {
                get
                {
                    ThrowIfNotValid();

                    int childrenCount = ChildrenCount;
                    if (childrenCount <= 0)
                    {
                        return null;
                    }
                    List<FieldStruct> result = new(childrenCount);

                    for (int i = 0; i < childrenCount; i++)
                    {
                        FieldStruct child = GetChildStruct(i);
                        result.Add(child);
                    }
                    return result;
                }
            }

            public List<Field> Children
            {
                get
                {
                    ThrowIfNotValid();

                    int childrenCount = ChildrenCount;

                    if (childrenCount <= 0)
                    {
                        return null;
                    }

                    List<Field> result = new(childrenCount);

                    for (int i = 0; i < childrenCount; i++)
                    {
                        Field child = GetChild(i);
                        result.Add(child);
                    }
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_get_name(int field_id);

            public static string GetName(int fieldId)
            {
                if (fieldId <= 0)
                {
                    return null;
                }

                IntPtr nameReference = ls_field_get_name(fieldId);
                if (nameReference == IntPtr.Zero)
                {
                    return null;
                }
                string result = Util.NativeUtf8ToString(nameReference);
                return result;
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_field_get_display_name(int field_id);

            public static string GetDisplayName(int fieldId)
            {
                if (fieldId <= 0)
                {
                    return null;
                }

                IntPtr displayNameReference = ls_field_get_display_name(fieldId);
                if (displayNameReference == IntPtr.Zero)
                {
                    return null;
                }
                string result = Util.NativeUtf8ToString(displayNameReference);

                return result;
            }
        }
    }

    public class Field
    {
        internal readonly FieldStruct FieldStruct;

        internal Field(FieldStruct fieldStruct)
        {
            FieldStruct = fieldStruct;
        }

        internal Field(IntPtr fieldReference)
        {
            if (fieldReference == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(fieldReference));
            }
            FieldStruct = new(fieldReference);
        }

        public Field()
        {
            FieldStruct = new();
        }

        ~Field()
        {
            FieldStruct.Dispose();
        }


        public static int Size => FieldStruct.Size;

        public bool Valid => FieldStruct.Valid;

        public string Representation
        {
            get => FieldStruct.Representation;
            set => FieldStruct.SetRepresentation(value);
        }

        public int Id
        {
            get => FieldStruct.Id;
            set => FieldStruct.SetId(value);
        }

        public int Type
        {
            get => FieldStruct.Type;
        }
        public int BufferId
        {
            get => FieldStruct.BufferId;
            set => FieldStruct.SetBufferId(value);
        }

        public int Offset
        {
            get => FieldStruct.Offset;
            set => FieldStruct.SetOffset(value);
        }

        public int Length
        {
            get => FieldStruct.Length;
        }

        public bool Hidden
        {
            get => FieldStruct.Hidden;
            set => FieldStruct.SetHidden(value);
        }

        public bool Generated
        {
            get => FieldStruct.Generated;
            set => FieldStruct.SetGenerated(value);
        }

        public int Encoding
        {
            get => FieldStruct.Encoding;
            set => FieldStruct.SetEncoding(value);
        }

        public long Int64Value => FieldStruct.Int64Value;

        public void SetInt64Value(long value, int type) => FieldStruct.SetInt64Value(value, type);

        public ulong UInt64Value => FieldStruct.UInt64Value;

        public void SetUInt64Value(ulong value, int type) => FieldStruct.SetUInt64Value(value, type);

        public double DoubleValue => FieldStruct.DoubleValue;

        public void SetDoubleValue(double value, int type) => FieldStruct.SetDoubleValue(value, type);

        public string StringValue => FieldStruct.StringValue;

        public void SetStringValue(string value, int type) => FieldStruct.SetStringValue(value, type);

        public byte[] BytesValue => FieldStruct.BytesValue;

        public void SetBytesValue(byte[] value, int type) => FieldStruct.SetBytesValue(value, type);

        public bool IsInt64 => FieldType.IsInt64(Type);

        public bool IsUInt64 => FieldType.IsUInt64(Type);

        public bool IsDouble => FieldType.IsDouble(Type);

        public bool IsString => FieldType.IsString(Type);

        public bool IsBytes => FieldType.IsBytes(Type);

        public int ChildrenCount => FieldStruct.ChildrenCount;

        public FieldStruct GetChildStruct(int index) => FieldStruct.GetChildStruct(index);

        public Field GetChild(int index) => FieldStruct.GetChild(index);

        public void SetChildStruct(FieldStruct child, int index) => FieldStruct.SetChild(child, index);
        public void SetChild(Field child, int index) => FieldStruct.SetChild(child, index);

        public void AddChild(FieldStruct child) => FieldStruct.AddChild(child);
        public void AddChild(Field child) => FieldStruct.AddChild(child);

        public void RemoveChild(int index) => FieldStruct.RemoveChild(index);

        public List<FieldStruct> ChildrenStructs => FieldStruct.ChildrenStructs;

        public List<Field> Children => FieldStruct.Children;

        public static string GetName(int fieldId) => FieldStruct.GetName(fieldId);

        public static string GetDisplayName(int fieldId) => FieldStruct.GetDisplayName(fieldId);

        public string Name => FieldStruct.Name;

        public string DisplayName => FieldStruct.DisplayName;

        public string TypeName => FieldStruct.TypeName;
    }

}

