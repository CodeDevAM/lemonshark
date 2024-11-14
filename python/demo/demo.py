"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

import sys
import os
import platform
from typing import *

system: str = platform.system().lower()
architecture: str = platform.machine().lower()

lemonshark_directory: str = os.path.dirname(os.path.realpath(__file__)) + f"/../lemonshark"
lemonshark_directory = os.path.realpath(lemonshark_directory)
sys.path.append(lemonshark_directory)

from lemonshark import LemonShark, Session, Packet, Field, Buffer, Filter


def print_fields(field: Field, indentation_count: int) -> None:
    representation: str = field.get_representation()
    if representation is not None:
        for i in range(indentation_count):
            print("    ", end="")

        print(representation)

    for i in range(indentation_count):
        print("    ", end="")

    display_name: str = field.get_display_name()
    name: str = field.get_name()
    type_name: str = field.get_type_name()
    field_id: int = field.get_field_id()

    print(f"""{display_name} ({name}, {type_name}, {field_id})""", end="")

    hidden: bool = field.get_hidden()
    generated: bool = field.get_generated()

    if hidden or generated:
        print(" [", end="")
        if hidden:
            print("H", end="")
        if generated:
            print("G", end="")
        print("]", end="")

    buffer_id: int = field.get_buffer_id()
    if buffer_id >= 0:
        buffer_offset: int = field.get_buffer_offset()
        buffer_length: int = field.get_buffer_length()

        print(f""" {{Buffer {buffer_id}:{buffer_offset}:{buffer_length}}}""", end="")

    print(": ", end="")

    if field.is_int64():
        value: int = field.get_int64_value()
        print(f"""{value} (0x{value:X})""", end="")
    elif field.is_uint64():
        value: int = field.get_uint64_value()
        print(f"""{value} (0x{value:X})""", end="")
    elif field.is_double():
        value: float = field.get_double_value()
        print(f"""{value}""", end="")
    elif field.is_string():
        value: str = field.get_string_value()
        if value is not None:
            print(value, end="")
    elif field.is_bytes():
        value: bytes = field.get_bytes_value()
        if value is not None:
            for j in range(len(value)):
                print(f"""{value[j]:02X} """, end="")
    else:
        print("unknown type", end="")

    print("\n", end="")

    children_count: int = field.children_count()

    for i in range(children_count):
        child: Field = field.get_child(i)
        print_fields(child, indentation_count + 1)


def print_packet(packet: Packet) -> None:
    packet_id: int = packet.get_packet_id()
    timestamp: float = packet.get_timestamp()
    length: int = packet.get_length()
    buffers_count: int = packet.buffers_count()
    info_column: str = packet.get_info_column()

    print(f"""Id: {packet_id}, Timestamp: {timestamp}, Length: {length}, Buffers: {buffers_count}, Info: {info_column}""")

    for i in range(buffers_count):
        print(f"""Buffer {i}: """, end="")
        buffer: Buffer = packet.get_buffer(i)

        data: bytes = buffer.get_data()
        if data is not None:
            for j in range(len(data)):
                print(f"""{data[j]:02X} """, end="")

        print("")

    root_field: Field = packet.get_root_field()
    children_count: int = root_field.children_count()

    for i in range(children_count):
        child: Field = root_field.get_child(i)
        print_fields(child, 0)


def main():
    wireshark_directory: str = None
    if system == "windows":
        if architecture == "amd64":
            wireshark_directory = os.path.dirname(os.path.realpath(__file__)) + f"/../../build/wireshark/windows/amd64/run/RelWithDebInfo"
            wireshark_directory = os.path.realpath(wireshark_directory)

    LemonShark.init([wireshark_directory])

    trace_file_path: str = os.environ["LS_EXAMPLE_FILE"]

    session: Session = Session.create_from_file(trace_file_path, "frame.len < 150")

    # Test filter
    (is_valid, error_message) = Filter.is_valid("frame.len < 150")
    (is_valid, error_message) = Filter.is_valid("frame.len ! 150")

    packets: List[Packet] = []
    try:
        packet_id: int = 0

        while True:
            packet_id = session.get_next_packet_id()
            if packet_id < 0:
                break
            if packet_id == 0:
                continue

            packet: Packet = session.get_packet(packet_id, True, True, True, True, True)

            print_packet(packet)
            packets.append(packet)

    except Exception as ex:
        print(ex)
    finally:
        session.close()


if __name__ == "__main__":
    main()
