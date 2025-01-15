/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

using System.Reflection;
using System.Runtime.InteropServices;

namespace LemonShark;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

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

        LemonShark.Init([wiresharkDirectory]);

        // Read a file with a read filter
        using Session session = Session.CreateFromFile(traceFilePath, "frame.len < 150");

        try
        {
            while (session.GetNextPacketId(out int packetId))
            {
                if (packetId == 0)
                {
                    continue;
                }

                Packet packet = session.GetPacket(packetId, true, true, true, true, true, null, 0);
                PrintPacket(packet);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static void PrintPacket(Packet packet)
    {
        Console.WriteLine($"Id: {packet.Id}, Timestamp: {packet.Timestamp}, Length: {packet.Length}, Buffers: {packet.BuffersCount}, Info: {packet.InfoColumn}");

        for (int i = 0; i < packet.BuffersCount; i++)
        {
            Console.Write($"Buffer {i}: ");
            Buffer buffer = packet.GetBuffer(i);

            byte[] data = buffer.Data;
            if (data is not null)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    Console.Write($"{data[j]:X2} ");
                }
            }

            Console.Write("\n");
        }

        int childrenCount = packet.RootField.ChildrenCount;
        for (int i = 0; i < childrenCount; i++)
        {
            Field child = packet.RootField.GetChild(i);
            PrintFields(child, 1);
        }

        Console.Write("\n");
    }

    private static void PrintFields(Field field, int indentationCount)
    {
        string representation = field.Representation;
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

        Console.Write($"{field.DisplayName} ({field.Name}, {field.TypeName}, {field.Id})");

        if (field.Hidden || field.Generated)
        {
            Console.Write(" [");
            if (field.Hidden)
            {
                Console.Write("H");
            }
            if (field.Generated)
            {
                Console.Write("G");
            }
            Console.Write("]");
        }

        if (field.BufferId >= 0)
        {
            Console.Write($" {{Buffer {field.BufferId}:{field.Offset}:{field.Length}}}");
        }

        Console.Write(": ");

        if (field.IsInt64)
        {
            long value = field.Int64Value;
            Console.Write($"{value} (0x{value:X})");
        }
        else if (field.IsUInt64)
        {
            ulong value = field.UInt64Value;
            Console.Write($"{value} (0x{value:X})");
        }
        else if (field.IsDouble)
        {
            double value = field.DoubleValue;
            Console.Write($"{value}");
        }
        else if (field.IsString)
        {
            string value = field.StringValue;
            if (value is not null)
            {
                Console.Write($"{value}");
            }
        }
        else if (field.IsBytes)
        {
            if (field.Type != FieldType.Protocol)
            {
                byte[] value = field.BytesValue;
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

        for (int i = 0; i < field.ChildrenCount; i++)
        {
            Field child = field.GetChild(i);
            PrintFields(child, indentationCount + 1);
        }
    }
}
