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

lemonshark_directory: str = os.path.dirname(os.path.realpath(__file__)) + f"/../../lemonshark"
lemonshark_directory = os.path.realpath(lemonshark_directory)
sys.path.append(lemonshark_directory)

from lemonshark import LemonShark, Session, EpanPacket, EpanField, FieldDescription, FieldType


def print_fields(epan_field: EpanField, indentation_count: int) -> None:
    representation: str = epan_field.get_representation()
    if representation is not None:
        for i in range(indentation_count):
            print("    ", end="")

        print(representation)

    for i in range(indentation_count):
        print("    ", end="")

    display_name: str = epan_field.get_display_name()
    name: str = epan_field.get_name()
    type_name: str = epan_field.get_type_name()
    field_id: int = epan_field.get_field_id()

    print(f"""{display_name} ({name}, {type_name}, {field_id})""", end="")

    hidden: bool = epan_field.get_hidden()
    generated: bool = epan_field.get_generated()

    if hidden or generated:
        print(" [", end="")
        if hidden:
            print("H", end="")
        if generated:
            print("G", end="")
        print("]", end="")

    offset: int = epan_field.get_offset()
    length: int = epan_field.get_length()
    if offset >= 0 and length > 0:
        print(f""" {{Buffer {offset}:{length}}}""", end="")

    print(": ", end="")

    if epan_field.is_int64():
        value: int = epan_field.get_int64_value()
        print(f"""{value} (0x{value:X})""", end="")
    elif epan_field.is_uint64():
        value: int = epan_field.get_uint64_value()
        print(f"""{value} (0x{value:X})""", end="")
    elif epan_field.is_double():
        value: float = epan_field.get_double_value()
        print(f"""{value}""", end="")
    elif epan_field.is_string():
        value: str = epan_field.get_string_value()
        if value is not None:
            print(value, end="")
    elif epan_field.is_bytes():
        if epan_field.get_type() != FieldType.protocol():
            value: bytes = epan_field.get_bytes_value()
            if value is not None:
                for j in range(len(value)):
                    print(f"""{value[j]:02X} """, end="")
    else:
        print("unknown type", end="")

    print("\n", end="")

    children_count: int = epan_field.get_children_count()
    for i in range(children_count):
        child: EpanField = epan_field.get_child(i)
        print_fields(child, indentation_count + 1)



def print_packet(epan_packet: EpanPacket) -> None:
    packet_id: int = epan_packet.get_packet_id()
    timestamp: float = epan_packet.get_timestamp()
    length: int = epan_packet.get_length()
    info_column: str = epan_packet.get_info_column()

    print(f"""Id: {packet_id}, Timestamp: {timestamp}, Length: {length}, Info: {info_column}""")

    buffer: bytes = epan_packet.get_buffer()
    if buffer is not None:
        print(f"""Buffer: """, end="")
        for j in range(len(buffer)):
            print(f"""{buffer[j]:02X} """, end="")

        print("")

    root_field: EpanField = epan_packet.get_root_field()

    def root_field_print_fields(field: EpanField) -> None:
        print_fields(field, 1)

    root_field.do_for_each_child(root_field_print_fields, 0)

    print("")


def print_packet_stats(epan_packet: EpanPacket) -> None:
    (field_count, int64_count, uint64_count, double_count, string_count, bytes_count) = epan_packet.get_field_count()
    
    print(f"Field count: {field_count}, Int64 count: {int64_count}, UInt64 count: {uint64_count}, Double count: {double_count}, String count: {string_count}, Bytes count: {bytes_count}");

def main():
    wireshark_directory: str = None
    if system == "windows":
        if architecture == "amd64":
            wireshark_directory = os.path.dirname(os.path.realpath(__file__)) + f"/../../../build/wireshark/windows/amd64/run/RelWithDebInfo"
            wireshark_directory = os.path.realpath(wireshark_directory)

    LemonShark.init([wireshark_directory])

    trace_file_path: str = os.environ["LS_EXAMPLE_FILE"]

    session: Session = Session.create_from_file(trace_file_path, "")

    frame_length_field_description: FieldDescription = FieldDescription.get_by_name("frame.len")
    requested_field_ids: List[int] = [frame_length_field_description.get_id()]

    try:
        packet_id: int = 0

        while True:
            packet_id = session.get_next_packet_id()
            if packet_id < 0:
                break
            if packet_id == 0:
                continue

            epan_packet: EpanPacket = session.get_epan_packet(packet_id, True, requested_field_ids)
            print_packet_stats(epan_packet)
            print_packet(epan_packet)

    except Exception as ex:
        print(ex)
    finally:
        session.close()


if __name__ == "__main__":
    main()
