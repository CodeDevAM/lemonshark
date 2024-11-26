/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark;

public class Filter
{
    [DllImport(LemonShark.LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_filter_is_valid(IntPtr filter, ref IntPtr errorMessage);

    public static bool IsValid(string filter, out string errorMessage)
    {
        errorMessage = null;
        IntPtr errorMessageReference = default;

        IntPtr utf8Filter = Util.StringToNativeUtf8(filter);

        int creationResult = 0;
        try
        {
            creationResult = ls_filter_is_valid(utf8Filter, ref errorMessageReference);
        }
        finally
        {
            Marshal.FreeHGlobal(utf8Filter);
        }

        bool result = true;
        if (creationResult == LemonShark.Error)
        {
            result = false;
            if (errorMessageReference != IntPtr.Zero)
            {
                errorMessage = Util.NativeUtf8ToString(errorMessageReference);
            }
        }
        
        if (errorMessageReference != IntPtr.Zero)
        {
            LemonShark.FreeMemory(errorMessageReference);
        }

        return result;
    }
}