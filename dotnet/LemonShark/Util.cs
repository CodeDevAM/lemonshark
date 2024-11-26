/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Runtime.InteropServices;

namespace LemonShark
{
    static internal class Util
    {
        internal static string NativeUtf8ToString(IntPtr nativeStringReference)
        {
            if (nativeStringReference == IntPtr.Zero)
            {
                return null;
            }
            string result = Marshal.PtrToStringUTF8(nativeStringReference);

            return result;
        }

        internal static IntPtr StringToNativeUtf8(string value)
        {
            IntPtr result = Marshal.StringToCoTaskMemUTF8(value);
            return result;
        }
    }
}
