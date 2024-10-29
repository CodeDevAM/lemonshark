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

            int length = 0;
            while (Marshal.ReadByte(nativeStringReference, length) != 0) length++;

            if (length == 1)
            {
                return "";
            }

            byte[] buffer = new byte[length];
            Marshal.Copy(nativeStringReference, buffer, 0, buffer.Length);
            string result = System.Text.Encoding.UTF8.GetString(buffer);

            return result;
        }
    }
}
