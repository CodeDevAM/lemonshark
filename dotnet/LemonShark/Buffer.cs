/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class Buffer
{
    internal readonly IntPtr BufferReference;

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_buffer_external_ref_count_add(IntPtr packet, long ref_count);

    internal Buffer(IntPtr bufferReference)
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
    public Buffer()
    {
        BufferReference = ls_buffer_new();
        ls_buffer_external_ref_count_add(BufferReference, 1L);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_buffer_free(IntPtr buffer);

    ~Buffer()
    {
        ls_buffer_external_ref_count_add(BufferReference, -1L);
        ls_buffer_free(BufferReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_buffer_size();

    public static int Size => ls_buffer_size();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_buffer_length_get(IntPtr buffer);

    public int Length
    {
        get => ls_buffer_length_get(BufferReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_buffer_data_get(IntPtr buffer);

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_buffer_data_set(IntPtr buffer, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U8)] byte[] data, int length);

    public byte[] Data
    {
        get
        {
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
        set
        {
            ls_buffer_data_set(BufferReference, value, value is not null ? value.Length : 0);
        }
    }
}
