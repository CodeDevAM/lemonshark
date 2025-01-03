"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark
from .field import Field
from .buffer import Buffer


class Packet:
    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Packet.__liblemonshark_initialized:
            liblemonshark.ls_packet_new.argtypes = []
            liblemonshark.ls_packet_new.restype = c_void_p

            liblemonshark.ls_packet_free.argtypes = [c_void_p]
            liblemonshark.ls_packet_free.restype = None

            liblemonshark.ls_packet_new.argtypes = []
            liblemonshark.ls_packet_size.restype = c_int32

            liblemonshark.ls_packet_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_packet_external_ref_count_add.restype = c_int64

            liblemonshark.ls_packet_id_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_id_get.restype = c_int32

            liblemonshark.ls_packet_id_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_id_get.restype = None

            liblemonshark.ls_packet_timestamp_seconds_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_timestamp_seconds_get.restype = c_int64

            liblemonshark.ls_packet_timestamp_seconds_set.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_packet_timestamp_seconds_set.restype = None

            liblemonshark.ls_packet_timestamp_nanoseconds_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_timestamp_nanoseconds_get.restype = c_int32

            liblemonshark.ls_packet_timestamp_nanoseconds_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_timestamp_nanoseconds_set.restype = None

            liblemonshark.ls_packet_length_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_length_get.restype = c_int32

            liblemonshark.ls_packet_length_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_length_set.restype = None

            liblemonshark.ls_packet_interface_id_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_interface_id_get.restype = c_int32

            liblemonshark.ls_packet_interface_id_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_interface_id_set.restype = None

            liblemonshark.ls_packet_root_field_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_root_field_get.restype = c_void_p

            liblemonshark.ls_packet_root_field_set.argtypes = [c_void_p, c_void_p]
            liblemonshark.ls_packet_root_field_set.restype = None

            liblemonshark.ls_packet_protocol_column_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_protocol_column_get.restype = c_char_p

            liblemonshark.ls_packet_protocol_column_set.argtypes = [c_void_p, c_char_p]
            liblemonshark.ls_packet_protocol_column_set.restype = None

            liblemonshark.ls_packet_info_column_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_info_column_get.restype = c_char_p

            liblemonshark.ls_packet_info_column_set.argtypes = [c_void_p, c_char_p]
            liblemonshark.ls_packet_info_column_set.restype = None

            liblemonshark.ls_packet_visited_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_visited_get.restype = c_int32

            liblemonshark.ls_packet_visited_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_visited_set.restype = None

            liblemonshark.ls_packet_visible_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_visible_get.restype = None

            liblemonshark.ls_packet_visible_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_visible_set.restype = None

            liblemonshark.ls_packet_ignored_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_ignored_get.restype = c_int32

            liblemonshark.ls_packet_ignored_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_ignored_set.restype = None

            liblemonshark.ls_packet_packet_buffer_id_get.argtypes = [c_void_p]
            liblemonshark.ls_packet_packet_buffer_id_get.restype = c_int32

            liblemonshark.ls_packet_packet_buffer_id_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_packet_buffer_id_set.restype = None

            liblemonshark.ls_packet_buffers_count.argtypes = [c_void_p]
            liblemonshark.ls_packet_buffers_count.restype = c_int32

            liblemonshark.ls_packet_buffers_get.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_buffers_get.restype = c_void_p

            liblemonshark.ls_packet_buffers_add.argtypes = [c_void_p, c_void_p]
            liblemonshark.ls_packet_buffers_add.restype = None

            liblemonshark.ls_packet_buffers_remove.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_packet_buffers_remove.restype = None

            Packet.__liblemonshark_initialized = True

        return liblemonshark
    
    def __init__(self, c_packet: c_void_p) -> None:
        if c_packet is None or c_packet.value is None or c_packet.value == 0:
            raise Exception("c_packet must not be null.")

        self.c_packet: c_void_p = c_packet
        liblemonshark: CDLL = Packet.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_packet_external_ref_count_add(self.c_packet, 1)

    def __del__(self):
        liblemonshark: CDLL = Packet.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_packet_external_ref_count_add(self.c_packet, -1)
        liblemonshark.ls_packet_free(self.c_packet)
        self.c_packet = None

    def size() -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        size: int = liblemonshark.ls_packet_size()
        return size

    def new() -> "Packet":
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_packet: int = liblemonshark.ls_packet_new()
        packet: Packet = Packet(c_void_p(c_packet))
        return packet

    def get_packet_id(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        packet_id: int = liblemonshark.ls_packet_id_get(self.c_packet)
        return packet_id

    def set_packet_id(self, packet_id: int) -> None:
        if packet_id < 1:
            raise Exception("id < 1")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_id_set(self.c_packet, packet_id)

    def get_timestamp_seconds(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        timestamp_seconds: int = liblemonshark.ls_packet_timestamp_seconds_get(self.c_packet)
        return timestamp_seconds

    def set_timestamp_seconds(self, timestamp_seconds: int) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_timestamp_seconds_set(self.c_packet, timestamp_seconds)

    def get_timestamp_nanoseconds(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        timestamp_nanoseconds: int = liblemonshark.ls_packet_timestamp_nanoseconds_get(self.c_packet)
        return timestamp_nanoseconds

    def set_timestamp_nanoseconds(self, timestamp_nanoseconds: int) -> None:
        if timestamp_nanoseconds < 0 or timestamp_nanoseconds > 999_999_999:
            raise Exception("timestamp_nanoseconds < 0 or timestamp_nanoseconds > 999_999_999")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_timestamp_nanoseconds_set(self.c_packet, timestamp_nanoseconds)

    def get_timestamp(self) -> float:
        timestamp_seconds: int = self.get_timestamp_seconds()
        timestamp_nanoseconds: int = self.get_timestamp_nanoseconds()
        timestamp: float = float(timestamp_seconds) + float(timestamp_nanoseconds) / 1000000000.0
        return timestamp

    def get_length(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        length: int = liblemonshark.ls_packet_length_get(self.c_packet)
        return length

    def set_length(self, length: int) -> None:
        if length < 0:
            raise Exception("length < 0")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_interface_id_set(self.c_packet, length)

    def get_interface_id(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        length: int = liblemonshark.ls_packet_interface_id_get(self.c_packet)
        return length

    def set_interface_id(self, interface_id: int) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_interface_id_set(self.c_packet, interface_id)

    def get_root_field(self) -> Field:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_root_field: int = liblemonshark.ls_packet_root_field_get(self.c_packet)
        if c_root_field is None or c_root_field == 0:
            return None
        root_field: Field = Field(c_void_p(c_root_field))
        return root_field

    def set_root_field(self, root_field: Field) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        if (
            root_field is None
            or root_field.c_field is None
            or root_field.c_field.value is None
            or root_field.c_field.value == 0
        ):
            raise Exception("root_field must not be null.")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_root_field_set(self.c_packet, root_field.c_field)

    def get_protocol_column(self) -> str:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_protocol_column: bytes = liblemonshark.ls_packet_protocol_column_get(self.c_packet)

        if c_protocol_column is None:
            return None

        protocol_column: str = c_protocol_column.decode("utf-8")
        return protocol_column

    def set_protocol_column(self, protocol_column: str) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_protocol_column: c_char_p = c_char_p(protocol_column.encode("utf-8"))
        liblemonshark.ls_packet_protocol_column_set(self.c_packet, c_protocol_column)

    def get_info_column(self) -> str:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_info_column: bytes = liblemonshark.ls_packet_info_column_get(self.c_packet)

        if c_info_column is None:
            return None

        info_column: str = c_info_column.decode("utf-8")
        return info_column

    def set_info_column(self, info_column: str) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_info_column: c_char_p = c_char_p(info_column.encode("utf-8"))
        liblemonshark.ls_packet_info_column_set(self.c_packet, c_info_column)

    def get_visited(self) -> bool:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        visited: bool = liblemonshark.ls_packet_visited_get(self.c_packet) != 0
        return visited

    def set_visited(self, visited: bool) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_visited_set(self.c_packet, 1 if visited else 0)

    def get_visible(self) -> bool:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        visible: bool = liblemonshark.ls_packet_visible_get(self.c_packet) != 0
        return visible

    def set_visible(self, visible: bool) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_visible_set(self.c_packet, 1 if visible else 0)

    def get_ignored(self) -> bool:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        ignored: bool = liblemonshark.ls_packet_ignored_get(self.c_packet) != 0
        return ignored

    def set_ignored(self, ignored: bool) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_ignored_set(self.c_packet, 1 if ignored else 0)

    def get_packet_buffer_id(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        packet_buffer_id: int = liblemonshark.ls_packet_buffer_id_get(self.c_packet)
        return packet_buffer_id

    def set_packet_buffer_id(self, packet_buffer_id: int) -> None:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_buffer_id_set(self.c_packet, packet_buffer_id)

    def buffers_count(self) -> int:
        liblemonshark: CDLL = Packet.get_liblemonshark()
        buffer_count: int = liblemonshark.ls_packet_buffers_count(self.c_packet)
        return buffer_count

    def get_buffer(self, buffer_id: int) -> Buffer:
        buffers_count: int = self.buffers_count()
        if buffer_id < 0 or buffer_id >= buffers_count:
            raise Exception("buffer_id < 0 or buffer_id >= buffers_count")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        c_buffer: int = liblemonshark.ls_packet_buffers_get(self.c_packet, buffer_id)

        if c_buffer is None or c_buffer == 0:
            return None
        buffer: Buffer = Buffer(c_void_p(c_buffer))
        return buffer

    def set_buffer(self, buffer: Buffer, buffer_id: int) -> None:
        if (
            buffer is None
            or buffer.c_buffer is None
            or buffer.c_buffer.value is None
            or buffer.c_buffer.value == 0
        ):
            raise Exception("buffer must not be null.")

        buffers_count: int = self.buffers_count()
        if buffer_id < 0 or buffer_id >= buffers_count:
            raise Exception("buffer_id < 0 or buffer_id >= buffers_count")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_buffers_set.argtypes = [c_void_p, c_void_p, c_int32]
        liblemonshark.ls_packet_buffers_set(self.c_packet, buffer.c_buffer, buffer_id)

    def add_buffer(self, buffer: Buffer) -> None:
        if (
            buffer is None
            or buffer.c_buffer is None
            or buffer.c_buffer.value is None
            or buffer.c_buffer.value == 0
        ):
            raise Exception("buffer must not be null.")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_buffers_add(self.c_packet, buffer.c_buffer)

    def remove_buffer(self, buffer_id: int) -> None:
        buffers_count: int = self.buffers_count()
        if buffer_id < 0 or buffer_id >= buffers_count:
            raise Exception("buffer_id < 0 or buffer_id >= buffers_count")

        liblemonshark: CDLL = Packet.get_liblemonshark()
        liblemonshark.ls_packet_buffers_remove(self.c_packet, buffer_id)

    def get_children(self) -> List["Buffer"]:
        result: List["Buffer"] = []
        buffers_count: int = self.buffers_count()
        for i in range(buffers_count):
            buffer: "Buffer" = self.get_buffer(i)
            result.append(buffer)
        return result
