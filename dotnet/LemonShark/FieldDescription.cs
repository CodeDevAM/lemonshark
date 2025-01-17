/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LemonShark;

[DebuggerDisplay("{Name}: {DisplayName}")]
public class FieldDescription
{
    internal readonly IntPtr FieldDescriptionReference;

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern long ls_field_description_external_ref_count_add(IntPtr field_description, long ref_count);

    internal FieldDescription(IntPtr fieldDescriptionReference)
    {
        if (fieldDescriptionReference == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(fieldDescriptionReference));
        }
        FieldDescriptionReference = fieldDescriptionReference;
        ls_field_description_external_ref_count_add(FieldDescriptionReference, 1L);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_field_description_free(IntPtr field_description);

    ~FieldDescription()
    {
        if (FieldDescriptionReference == IntPtr.Zero)
        {
            return;
        }
        ls_field_description_external_ref_count_add(FieldDescriptionReference, -1L);
        ls_field_description_free(FieldDescriptionReference);
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_description_size();

    public static int Size => ls_field_description_size();

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_description_id_get(IntPtr field_description);

    public int Id
    {
        get
        {
            int id = ls_field_description_id_get(FieldDescriptionReference);
            return id;
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_description_type_get(IntPtr field_description);
    public int Type
    {
        get
        {
            int type = ls_field_description_type_get(FieldDescriptionReference);
            return type;
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_description_name_get(IntPtr field_description);

    public string Name
    {
        get
        {
            IntPtr nameReference = ls_field_description_name_get(FieldDescriptionReference);
            string result = Util.NativeUtf8ToString(nameReference);
            return result;
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_description_display_name_get(IntPtr field_description);
    public string DisplayName
    {
        get
        {
            IntPtr displayNameReference = ls_field_description_display_name_get(FieldDescriptionReference);
            string result = Util.NativeUtf8ToString(displayNameReference);
            return result;
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_field_description_parent_id_get(IntPtr field_description);

    public int ParentId
    {
        get
        {
            int parent_id = ls_field_description_parent_id_get(FieldDescriptionReference);
            return parent_id;
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_description_get_by_id(int id);

    public static FieldDescription GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        IntPtr fieldDescriptionReference = ls_field_description_get_by_id(id);

        if (fieldDescriptionReference == IntPtr.Zero)
        {
            return null;
        }

        FieldDescription fieldDescription = new(fieldDescriptionReference);
        return fieldDescription;
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_description_get_by_name(IntPtr name);

    public static FieldDescription GetByName(string name)
    {
        if (name is null)
        {
            return null;
        }

        IntPtr utf8Value = Util.StringToNativeUtf8(name);
        try
        {
            IntPtr fieldDescriptionReference = ls_field_description_get_by_name(utf8Value);

            if (fieldDescriptionReference == IntPtr.Zero)
            {
                return null;
            }

            FieldDescription fieldDescription = new(fieldDescriptionReference);
            return fieldDescription;
        }
        finally
        {
            Marshal.FreeHGlobal(utf8Value);
        }
    }

    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr ls_field_description_get_all(ref int count);

    public static FieldDescription[] GetAll()
    {
        IntPtr fieldDescriptionsReference = IntPtr.Zero;
        try
        {
            int count = 0;
            fieldDescriptionsReference = ls_field_description_get_all(ref count);
            if (fieldDescriptionsReference == IntPtr.Zero)
            {
                return [];
            }
            if (count <= 0)
            {
                return [];
            }

            FieldDescription[] fieldDescriptions = new FieldDescription[count];


            for (int i = 0; i < count; i++)
            {
                IntPtr currentFieldDescriptionReference = Marshal.ReadIntPtr(fieldDescriptionsReference + i * IntPtr.Size);
                if (currentFieldDescriptionReference == IntPtr.Zero)
                {
                    continue;
                }
                FieldDescription fieldDescription = new(currentFieldDescriptionReference);
                fieldDescriptions[i] = fieldDescription;
            }

            return fieldDescriptions;

        }
        finally
        {
            if (fieldDescriptionsReference != IntPtr.Zero)
            {
                LemonShark.FreeMemory(fieldDescriptionsReference);
            }
        }
    }
}