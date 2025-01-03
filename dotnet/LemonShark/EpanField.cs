using LemonShark.Structs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LemonShark
{

    public delegate void EpanFieldHandler(EpanField epanfield);

    namespace Structs
    {
        public delegate void EpanFieldStructHandler(EpanFieldStruct epanfield);

        [DebuggerDisplay("{Name}: {DisplayName}")]
        public readonly struct EpanFieldStruct : IDisposable
        {
            internal readonly IntPtr EpanFieldReference;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_field_external_ref_count_add(IntPtr epan_field, long ref_count);

            internal EpanFieldStruct(IntPtr epanFieldReference)
            {
                if (epanFieldReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(epanFieldReference));
                }
                EpanFieldReference = epanFieldReference;
                ls_epan_field_external_ref_count_add(EpanFieldReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_epan_field_free(IntPtr epan_field);

            public void Dispose()
            {
                if (EpanFieldReference == IntPtr.Zero)
                {
                    return;
                }
                ls_epan_field_external_ref_count_add(EpanFieldReference, -1L);
                ls_epan_field_free(EpanFieldReference);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_size();

            public static int Size => ls_epan_field_size();


            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_field_valid_get(IntPtr epan_field);
            public bool Valid
            {
                get
                {
                    if (EpanFieldReference == IntPtr.Zero)
                    {
                        return false;
                    }

                    return ls_epan_field_valid_get(EpanFieldReference) != 0;
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
            private static extern IntPtr ls_epan_field_representation_get(IntPtr epan_field);
            public string Representation
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr representationReference = ls_epan_field_representation_get(EpanFieldReference);
                    if (representationReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(representationReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_id_get(IntPtr epan_field);
            public int Id
            {
                get
                {
                    ThrowIfNotValid();
                    int id = ls_epan_field_id_get(EpanFieldReference);
                    return id;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_type_get(IntPtr epan_field);
            public int Type
            {
                get
                {
                    ThrowIfNotValid();
                    int type = ls_epan_field_type_get(EpanFieldReference);
                    return type;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_field_name_get(IntPtr epan_field);
            public string Name
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr nameReference = ls_epan_field_name_get(EpanFieldReference);
                    if (nameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(nameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_field_display_name_get(IntPtr epan_field);
            public string DisplayName
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr displayNameReference = ls_epan_field_display_name_get(EpanFieldReference);
                    if (displayNameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(displayNameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_field_type_name_get(IntPtr epan_field);
            public string TypeName
            {
                get
                {
                    ThrowIfNotValid();
                    IntPtr typeNameReference = ls_epan_field_type_name_get(EpanFieldReference);
                    if (typeNameReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(typeNameReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_buffer_length_get(IntPtr epan_field);
            public int BufferLength
            {
                get
                {
                    ThrowIfNotValid();

                    int bufferLength = ls_epan_field_buffer_length_get(EpanFieldReference);
                    return bufferLength;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_buffer_get(IntPtr epan_field, byte[] target, int max_length);
            public byte[] Buffer
            {
                get
                {
                    ThrowIfNotValid();

                    byte[] result = new byte[BufferLength];

                    _ = ls_epan_field_buffer_get(EpanFieldReference, result, result.Length);

                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_buffer_slice_get(IntPtr epan_field, byte[] target, int max_length);
            public byte[] BufferSlice
            {
                get
                {
                    ThrowIfNotValid();

                    byte[] result = new byte[Length];

                    _ = ls_epan_field_buffer_slice_get(EpanFieldReference, result, result.Length);

                    return result;
                }
            }


            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_offset_get(IntPtr epan_field);
            public int Offset
            {
                get
                {
                    ThrowIfNotValid();
                    int offset = ls_epan_field_offset_get(EpanFieldReference);
                    return offset;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_length_get(IntPtr epan_field);
            public int Length
            {
                get
                {
                    ThrowIfNotValid();
                    int length = ls_epan_field_length_get(EpanFieldReference);
                    return length;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_hidden_get(IntPtr epan_field);
            public bool Hidden
            {
                get
                {
                    ThrowIfNotValid();
                    bool hidden = ls_epan_field_hidden_get(EpanFieldReference) != 0;
                    return hidden;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_generated_get(IntPtr epan_field);
            public bool Generated
            {
                get
                {
                    ThrowIfNotValid();
                    bool generated = ls_epan_field_generated_get(EpanFieldReference) != 0;
                    return generated;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_encoding_get(IntPtr epan_field);
            public int Encoding
            {
                get
                {
                    ThrowIfNotValid();
                    int encoding = ls_epan_field_encoding_get(EpanFieldReference);
                    return encoding;
                }
            }

            public bool IsInt64 => FieldType.IsInt64(Type);
            public bool IsUInt64 => FieldType.IsUInt64(Type);
            public bool IsDouble => FieldType.IsDouble(Type);
            public bool IsString => FieldType.IsString(Type);
            public bool IsBytes => FieldType.IsBytes(Type);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_field_value_get_int64(IntPtr epan_field);

            public long Int64Value
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsInt64)
                    {
                        throw new InvalidOperationException("Value is not of type int64.");
                    }
                    long result = ls_epan_field_value_get_int64(EpanFieldReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern ulong ls_epan_field_value_get_uint64(IntPtr epan_field);

            public ulong UInt64Value
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsUInt64)
                    {
                        throw new InvalidOperationException("Value is not of type uint64.");
                    }
                    ulong result = ls_epan_field_value_get_uint64(EpanFieldReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_epan_field_value_get_double(IntPtr epan_field);

            public double DoubleValue
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsDouble)
                    {
                        throw new InvalidOperationException("Value is not of type double.");
                    }
                    double result = ls_epan_field_value_get_double(EpanFieldReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_field_value_get_string(IntPtr epan_field);

            public string StringValue
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsString)
                    {
                        throw new InvalidOperationException("Value is not of type string.");
                    }

                    IntPtr valueReference = ls_epan_field_value_get_string(EpanFieldReference);
                    if (valueReference == IntPtr.Zero)
                    {
                        return null;
                    }
                    string result = Util.NativeUtf8ToString(valueReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_value_get_bytes(IntPtr epan_field, byte[] target, int max_length);

            public byte[] BytesValue
            {
                get
                {
                    ThrowIfNotValid();
                    if (!IsBytes)
                    {
                        throw new InvalidOperationException("Value is not of type string.");
                    }

                    int length = ls_epan_field_length_get(EpanFieldReference);
                    byte[] result = new byte[length];

                    _ = ls_epan_field_value_get_bytes(EpanFieldReference, result, length);

                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_epan_field_children_count(IntPtr epan_field);

            public int ChildrenCount
            {
                get
                {
                    ThrowIfNotValid();
                    int result = ls_epan_field_children_count(EpanFieldReference);
                    return result;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_epan_field_children_get(IntPtr epan_field, int index);

            public EpanFieldStruct GetChildStruct(int index)
            {
                ThrowIfNotValid();
                int childrenCount = ChildrenCount;
                if (index < 0 || index >= childrenCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                IntPtr epanFieldRefenrence = ls_epan_field_children_get(EpanFieldReference, index);

                if (epanFieldRefenrence == IntPtr.Zero)
                {
                    return default;
                }

                EpanFieldStruct child = new(epanFieldRefenrence);
                return child;
            }

            public EpanField GetChild(int index)
            {
                EpanFieldStruct childStruct = GetChildStruct(index);

                EpanField child = new(childStruct);
                return child;
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_epan_field_children_do_for_each(IntPtr epan_field, EpanFieldHandlerInternal epan_field_handler, IntPtr parameter, int recursively);

            internal delegate void EpanFieldStructHandlerInternal(IntPtr epanFieldReference);

            public void DoForEachChild(EpanFieldStructHandler epanFieldHandler, bool recursively)
            {
                ThrowIfNotValid();
                if (epanFieldHandler is null)
                {
                    return;
                }

                void EpanFieldHandler(IntPtr epanFieldReference, IntPtr parameter)
                {
                    using EpanFieldStruct epanField = new(epanFieldReference);
                    epanFieldHandler.Invoke(epanField);
                }

                ls_epan_field_children_do_for_each(EpanFieldReference, EpanFieldHandler, IntPtr.Zero, recursively ? 1 : 0);
            }

            internal delegate void EpanFieldHandlerInternal(IntPtr epanFieldReference, IntPtr parameter);

            public void DoForEachChild(EpanFieldHandler epanFieldHandler, bool recursively)
            {
                ThrowIfNotValid();
                if (epanFieldHandler is null)
                {
                    return;
                }

                void EpanFieldHandler(IntPtr epanFieldReference, IntPtr parameter)
                {
                    EpanField epanField = new(epanFieldReference);
                    epanFieldHandler.Invoke(epanField);
                }

                ls_epan_field_children_do_for_each(EpanFieldReference, EpanFieldHandler, IntPtr.Zero, recursively ? 1 : 0);
            }

            public List<EpanFieldStruct> ChildrenStruct
            {
                get
                {
                    ThrowIfNotValid();

                    int childrenCount = ChildrenCount;

                    if (childrenCount <= 0)
                    {
                        return null;
                    }

                    List<EpanFieldStruct> result = new(childrenCount);

                    for (int i = 0; i < childrenCount; i++)
                    {
                        EpanFieldStruct child = GetChildStruct(i);
                        result.Add(child);
                    }
                    DoForEachChild(result.Add, false);
                    return result;
                }
            }

            public List<EpanField> Children
            {
                get
                {
                    ThrowIfNotValid();

                    int childrenCount = ChildrenCount;

                    if (childrenCount <= 0)
                    {
                        return null;
                    }

                    List<EpanField> result = new(childrenCount);

                    for (int i = 0; i < childrenCount; i++)
                    {
                        EpanField child = GetChild(i);
                        result.Add(child);
                    }
                    DoForEachChild(result.Add, false);
                    return result;
                }
            }

        }
    }

    [DebuggerDisplay("{Name}: {DisplayName}")]
    public class EpanField
    {
        internal readonly EpanFieldStruct EpanFieldStruct;

        internal EpanField(EpanFieldStruct epanFieldStruct)
        {
            EpanFieldStruct = epanFieldStruct;
        }

        internal EpanField(IntPtr epanFieldReference)
        {
            EpanFieldStruct = new(epanFieldReference);
        }

        ~EpanField()
        {
            EpanFieldStruct.Dispose();
        }

        public static int Size => EpanFieldStruct.Size;

        public bool Valid => EpanFieldStruct.Valid;
        public string Representation => EpanFieldStruct.Representation;
        public int Id => EpanFieldStruct.Id;
        public int Type => EpanFieldStruct.Type;
        public int BufferLength => EpanFieldStruct.BufferLength;
        public byte[] Buffer => EpanFieldStruct.Buffer;
        public byte[] BufferSlice => EpanFieldStruct.BufferSlice;
        public int Offset => EpanFieldStruct.Offset;
        public int Length => EpanFieldStruct.Length;
        public bool Hidden => EpanFieldStruct.Hidden;
        public bool Generated => EpanFieldStruct.Generated;
        public int Encoding => EpanFieldStruct.Encoding;
        public bool IsInt64 => EpanFieldStruct.IsInt64;
        public bool IsUInt64 => EpanFieldStruct.IsUInt64;
        public bool IsDouble => EpanFieldStruct.IsDouble;
        public bool IsString => EpanFieldStruct.IsString;
        public bool IsBytes => EpanFieldStruct.IsBytes;

        public long Int64Value => EpanFieldStruct.Int64Value;

        public ulong UInt64Value => EpanFieldStruct.UInt64Value;

        public double DoubleValue => EpanFieldStruct.DoubleValue;

        public string StringValue => EpanFieldStruct.StringValue;

        public byte[] BytesValue => EpanFieldStruct.BytesValue;
        public int ChildrenCount => EpanFieldStruct.ChildrenCount;

        public EpanFieldStruct GetChildStruct(int index) => EpanFieldStruct.GetChildStruct(index);

        public EpanField GetChild(int index) => EpanFieldStruct.GetChild(index);

        public void DoForEachChild(EpanFieldStructHandler epanFieldHandler, bool recursively) => EpanFieldStruct.DoForEachChild(epanFieldHandler, recursively);

        public void DoForEachChild(EpanFieldHandler epanFieldHandler, bool recursively) => EpanFieldStruct.DoForEachChild(epanFieldHandler, recursively);

        public List<EpanFieldStruct> ChildrenStruct => EpanFieldStruct.ChildrenStruct;

        public List<EpanField> Children => EpanFieldStruct.Children;

        public string Name => EpanFieldStruct.Name;
        public string DisplayName => EpanFieldStruct.DisplayName;

        public string TypeName => EpanFieldStruct.TypeName;

    }

}

