﻿/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using LemonShark;
using LemonShark.Structs;
using System.Reflection;
using System.Runtime.InteropServices;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Set the environment variable LS_EXAMPLE_FILE to the path of a valid trace file before running this example.
string traceFilePath = Environment.GetEnvironmentVariable("LS_EXAMPLE_FILE", EnvironmentVariableTarget.Process);

string assemblyDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
string wiresharkDirectory = null;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
    {
        wiresharkDirectory = Path.GetFullPath($"{assemblyDirectory}/../../../../../../build/wireshark/windows/amd64/run/RelWithDebInfo");
    }
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
    {
        wiresharkDirectory = Path.GetFullPath($"{assemblyDirectory}/../../../../../../build/wireshark/linux/x86_64/run");
    }
}

LemonShark.LemonShark.Init([wiresharkDirectory]);

using Session session = Session.CreateFromFile(traceFilePath, "", null);

FieldDescription frameLengthFieldDescription = FieldDescription.GetByName("frame.len");
int[] requestedFieldIds = [frameLengthFieldDescription.Id];

try
{
    while (session.GetNextPacketId(out int packetId))
    {
        if (packetId == 0)
        {
            continue;
        }

        // Using EpanPacketStruct and EpanFieldStruct is the recommended approach when performance is critical
        using EpanPacketStruct epanPacket = session.GetEpanPacketStruct(packetId, true, requestedFieldIds, requestedFieldIds.Length);
        PrintPacketStats(epanPacket);
        PrintPacket(epanPacket);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static void PrintPacketStats(EpanPacketStruct epanPacket)
{
    epanPacket.GetFieldCount(out int fieldCount, out int int64Count, out int uint64Count, out int doubleCount, out int stringCount, out int bytesCount, out int representationCount);

    Console.WriteLine($"Field count: {fieldCount}, Int64 count: {int64Count}, UInt64 count: {uint64Count}, Double count: {doubleCount}, String count: {stringCount}, Bytes count: {bytesCount}, Representation Count: {representationCount}");
}

static void PrintPacket(EpanPacketStruct epanPacket)
{
    Console.WriteLine($"Id: {epanPacket.Id}, Timestamp: {epanPacket.Timestamp}, Length: {epanPacket.Length}, Info: {epanPacket.InfoColumn}");

    byte[] buffer = epanPacket.Buffer;
    if (buffer is not null)
    {
        Console.Write($"Buffer: ");
        for (int j = 0; j < buffer.Length; j++)
        {
            Console.Write($"{buffer[j]:X2} ");
        }

        Console.Write("\n");
    }

    using EpanFieldStruct rootField = epanPacket.RootFieldStruct;

    rootField.DoForEachChild(static epanField => PrintFields(epanField, 1), false);

    Console.Write("\n");
}

static void PrintFields(EpanFieldStruct epanField, int indentationCount)
{
    string representation = epanField.Representation;
    if (representation != null)
    {
        for (int i = 0; i < indentationCount; i++)
        {
            Console.Write("    ");
        }
        Console.WriteLine(representation);
    }

    for (int i = 0; i < indentationCount; i++)
    {
        Console.Write("    ");
    }

    Console.Write($"{epanField.DisplayName} ({epanField.Name}, {epanField.TypeName}, {epanField.Id})");

    if (epanField.Hidden || epanField.Generated)
    {
        Console.Write(" [");
        if (epanField.Hidden)
        {
            Console.Write("H");
        }
        if (epanField.Generated)
        {
            Console.Write("G");
        }
        Console.Write("]");
    }

    int offset = epanField.Offset;
    int length = epanField.Length;
    if (offset >= 0 && length > 0)
    {
        Console.Write($" {{Buffer {offset}:{length}}}");
    }

    Console.Write(": ");

    string valueRepresentation = epanField.ValueRepresentation;
    if (valueRepresentation is not null)
    {
        Console.Write($"{valueRepresentation}, ");
    }

    if (epanField.IsInt64)
    {
        long value = epanField.Int64Value;
        Console.Write($"{value} (0x{value:X})");
    }
    else if (epanField.IsUInt64)
    {
        ulong value = epanField.UInt64Value;
        Console.Write($"{value} (0x{value:X})");
    }
    else if (epanField.IsDouble)
    {
        double value = epanField.DoubleValue;
        Console.Write($"{value}");
    }
    else if (epanField.IsString)
    {
        string value = epanField.StringValue;
        if (value is not null)
        {
            Console.Write($"{value}");
        }
    }
    else if (epanField.IsBytes)
    {
        if (epanField.Type != FieldType.Protocol)
        {
            byte[] value = epanField.BytesValue;
            if (value is not null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    Console.Write($"{value[i]:X2} ");
                }
            }
        }
    }
    else
    {
        Console.Write("unknown type");
    }

    Console.Write("\n");

    int childrenCount = epanField.ChildrenCount;
    for (int i = 0; i < childrenCount; i++)
    {
        using EpanFieldStruct child = epanField.GetChildStruct(i);
        PrintFields(child, indentationCount + 1);
    }
}
