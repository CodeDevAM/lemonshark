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
        public readonly struct BufferStruct : IDisposable
        {
            internal readonly IntPtr BufferReference;

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern long ls_buffer_external_ref_count_add(IntPtr packet, long ref_count);

            internal BufferStruct(IntPtr bufferReference)
            {
                if (bufferReference == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(bufferReference));
                }
                BufferReference = bufferReference;
                ls_buffer_external_ref_count_add(BufferReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_buffer_new();
            public BufferStruct()
            {
                BufferReference = ls_buffer_new();
                ls_buffer_external_ref_count_add(BufferReference, 1L);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_buffer_free(IntPtr buffer);

            public void Dispose()
            {
                if (BufferReference == IntPtr.Zero)
                {
                    return;
                }
                ls_buffer_external_ref_count_add(BufferReference, -1L);
                ls_buffer_free(BufferReference);
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_buffer_size();

            public static int Size => ls_buffer_size();

            public readonly bool Valid => BufferReference != IntPtr.Zero;

            private void ThrowIfNotValid()
            {
                if (!Valid)
                {
                    throw new InvalidOperationException("Buffer is not valid.");
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern int ls_buffer_length_get(IntPtr buffer);

            public int Length
            {
                get
                {
                    ThrowIfNotValid();
                    int length = ls_buffer_length_get(BufferReference);
                    return length;
                }
            }

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern IntPtr ls_buffer_data_get(IntPtr buffer);

            [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void ls_buffer_data_set(IntPtr buffer, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U8)] byte[] data, int length);

            public byte[] Data
            {
                get
                {
                    ThrowIfNotValid();

                    IntPtr dataReference = ls_buffer_data_get(BufferReference);

                    if (dataReference == IntPtr.Zero)
                    {
                        return null;
                    }

                    int length = ls_buffer_length_get(BufferReference);

                    byte[] array = new byte[length];

                    Marshal.Copy(dataReference, array, 0, length);

                    return array;
                }
            }

            public void SetData(byte[] data)
            {
                ThrowIfNotValid();
                int length = data is not null ? data.Length : 0;
                ls_buffer_data_set(BufferReference, data, length);
            }
        }
    }

    public class Buffer
    {
        internal readonly BufferStruct BufferStruct;

        internal Buffer(BufferStruct bufferStruct)
        {
            BufferStruct = bufferStruct;
        }

        internal Buffer(IntPtr bufferReference)
        {
            if (bufferReference == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(bufferReference));
            }
            BufferStruct = new(bufferReference);
        }

        public Buffer()
        {
            BufferStruct = new();
        }

        ~Buffer()
        {
            BufferStruct.Dispose();
        }

        public static int Size => BufferStruct.Size;

        public bool Valid => BufferStruct.Valid;

        public int Length => BufferStruct.Length;

        public byte[] Data => BufferStruct.Data;

        public void SetData(byte[] data) => BufferStruct.SetData(data);
    }
}


