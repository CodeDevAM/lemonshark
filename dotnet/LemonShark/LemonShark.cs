﻿/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Reflection;
using System.Runtime.InteropServices;

namespace LemonShark;

public class LemonShark
{
    internal const string LemonSharkLibName = "liblemonshark";
    private static bool isInitialized = false;

    public static void Init(IEnumerable<string> wiresharkDirectories)
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("Init can be called only once.");
        }

        string pathEnvironmentVariableName = "PATH";
        string path = Environment.GetEnvironmentVariable(pathEnvironmentVariableName, EnvironmentVariableTarget.Process);

        Assembly assembly = Assembly.GetAssembly(typeof(Session));
        string assemblyLocation = assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                string nativeAssemblyDirectory = $"{assemblyDirectory}\\native\\windows\\amd64";
                path = nativeAssemblyDirectory + (string.IsNullOrEmpty(path) ? "" : $";{path}");
            }

            if (wiresharkDirectories is not null)
            {
                foreach (string wiresharkDirectory in wiresharkDirectories)
                {
                    if (!string.IsNullOrEmpty(wiresharkDirectory))
                    {
                        path = $"{wiresharkDirectory}" + (string.IsNullOrEmpty(path) ? "" : $";{path}");
                    }
                }
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                string nativeAssemblyDirectory = $"{assemblyDirectory}/native/linux/x86_64";
                path = nativeAssemblyDirectory + (string.IsNullOrEmpty(path) ? "" : $":{path}");
            }

            if (wiresharkDirectories is not null)
            {
                foreach (string wiresharkDirectory in wiresharkDirectories)
                {
                    if (!string.IsNullOrEmpty(wiresharkDirectory))
                    {
                        path = $"{wiresharkDirectory}" + (string.IsNullOrEmpty(path) ? "" : $":{path}");
                    }
                }
            }

            // TODO Find a way to load native dependencies like libwireshark.so that are located in a non-standard location
#if NET6_0_OR_GREATER
            IntPtr libReference = NativeLibrary.Load($"{assemblyDirectory}/native/linux/x86_64/{LemonSharkLibName}.so");
#endif
        }

        Environment.SetEnvironmentVariable(pathEnvironmentVariableName, path, EnvironmentVariableTarget.Process);

        isInitialized = true;
    }

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_major();

    public static int MajorVersion => ls_version_get_major();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_minor();

    public static int MinorVersion => ls_version_get_minor();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_patch();

    public static int PatchVersion => ls_version_get_patch();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_wireshark_major();

    public static int WiresharkMajorVersion => ls_version_get_wireshark_major();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_wireshark_minor();

    public static int WiresharkMinorVersion => ls_version_get_wireshark_minor();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_version_get_wireshark_patch();

    public static int WiresharkPatchVersion => ls_version_get_wireshark_patch();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void ls_memory_free(IntPtr memory);

    internal static void FreeMemory(IntPtr memory)
    {
        if (memory == IntPtr.Zero)
        {
            return;
        }
        ls_memory_free(memory);
    }

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_ok();

    public static int Ok => ls_ok();

    [DllImport(LemonSharkLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern int ls_error();

    public static int Error => ls_error();

}