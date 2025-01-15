"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark
from .epan_field import EpanField


class EpanPacket:
    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not EpanPacket.__liblemonshark_initialized:
            liblemonshark.ls_epan_packet_new.argtypes = []
            liblemonshark.ls_epan_packet_new.restype = c_void_p

            liblemonshark.ls_epan_packet_free.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_free.restype = None

            liblemonshark.ls_epan_packet_new.argtypes = []
            liblemonshark.ls_epan_packet_size.restype = c_int32

            liblemonshark.ls_epan_packet_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_epan_packet_external_ref_count_add.restype = c_int64

            liblemonshark.ls_epan_packet_valid_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_valid_get.restype = c_int32

            liblemonshark.ls_epan_packet_id_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_id_get.restype = c_int32

            liblemonshark.ls_epan_packet_timestamp_seconds_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_timestamp_seconds_get.restype = c_int64

            liblemonshark.ls_epan_packet_timestamp_nanoseconds_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_timestamp_nanoseconds_get.restype = c_int32

            liblemonshark.ls_epan_packet_length_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_length_get.restype = c_int32

            liblemonshark.ls_epan_packet_interface_id_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_interface_id_get.restype = c_int32

            liblemonshark.ls_epan_packet_root_field_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_root_field_get.restype = c_void_p

            liblemonshark.ls_epan_packet_protocol_column_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_protocol_column_get.restype = c_char_p

            liblemonshark.ls_epan_packet_protocol_column_set.argtypes = [c_void_p, c_char_p]
            liblemonshark.ls_epan_packet_protocol_column_set.restype = None

            liblemonshark.ls_epan_packet_info_column_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_info_column_get.restype = c_char_p

            liblemonshark.ls_epan_packet_info_column_set.argtypes = [c_void_p, c_char_p]
            liblemonshark.ls_epan_packet_info_column_set.restype = None

            liblemonshark.ls_epan_packet_visited_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_visited_get.restype = c_int32

            liblemonshark.ls_epan_packet_visible_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_visible_get.restype = None

            liblemonshark.ls_epan_packet_ignored_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_packet_ignored_get.restype = c_int32

            liblemonshark.ls_epan_packet_buffer_get.argtypes = [c_void_p, c_void_p, c_int32]
            liblemonshark.ls_epan_packet_buffer_get.restype = c_int32

            liblemonshark.ls_epan_packet_field_count_get.argtypes = [c_void_p, POINTER(c_int32), POINTER(c_int32), POINTER(c_int32), POINTER(c_int32), POINTER(c_int32), POINTER(c_int32), POINTER(c_int32)]
            liblemonshark.ls_epan_packet_field_count_get.restype = None

            EpanPacket.__liblemonshark_initialized = True

        return liblemonshark
    
    def __init__(self, c_epan_packet: c_void_p) -> None:
        if c_epan_packet is None or c_epan_packet.value is None or c_epan_packet.value == 0:
            raise Exception("c_epan_packet must not be null.")

        self.c_epan_packet: c_void_p = c_epan_packet
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_epan_packet_external_ref_count_add(self.c_epan_packet, 1)

    def __del__(self):
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_epan_packet_external_ref_count_add(self.c_epan_packet, -1)
        liblemonshark.ls_epan_packet_free(self.c_epan_packet)
        self.c_epan_packet = None

    def size() -> int:
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        size: int = liblemonshark.ls_epan_packet_size()
        return size

    def new() -> "EpanPacket":
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_epan_packet: int = liblemonshark.ls_epan_packet_new()
        epan_packet: EpanPacket = EpanPacket(c_void_p(c_epan_packet))
        return epan_packet
    
    def is_valid(self) -> bool:
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        if self.c_epan_packet is None or self.c_epan_packet.value is None or self.c_epan_packet.value == 0:
            return False
        valid: bool = liblemonshark.ls_epan_packet_valid_get(self.c_epan_packet) != 0
        return valid

    def throw_if_not_valid(self) -> None:
        valid: bool = self.is_valid()
        if not valid:
            raise Exception("EpanPacket is expired.")
        
    def get_packet_id(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        packet_id: int = liblemonshark.ls_epan_packet_id_get(self.c_epan_packet)
        return packet_id
    
    def get_timestamp_seconds(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        timestamp_seconds: int = liblemonshark.ls_epan_packet_timestamp_seconds_get(self.c_epan_packet)
        return timestamp_seconds
    
    def get_timestamp_nanoseconds(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        timestamp_nanoseconds: int = liblemonshark.ls_epan_packet_timestamp_nanoseconds_get(self.c_epan_packet)
        return timestamp_nanoseconds
        
    def get_timestamp(self) -> float:
        timestamp_seconds: int = self.get_timestamp_seconds()
        timestamp_nanoseconds: int = self.get_timestamp_nanoseconds()
        timestamp: float = float(timestamp_seconds) + float(timestamp_nanoseconds) / 1000000000.0
        return timestamp
    
    def get_length(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        length: int = liblemonshark.ls_epan_packet_length_get(self.c_epan_packet)
        return length
    
    def get_interface_id(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        interface_id: int = liblemonshark.ls_epan_packet_interface_id_get(self.c_epan_packet)
        return interface_id
    
    def get_root_field(self) -> EpanField:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_root_field: int = liblemonshark.ls_epan_packet_root_field_get(self.c_epan_packet)
        if c_root_field is None or c_root_field == 0:
            return None
        root_field: EpanField = EpanField(c_void_p(c_root_field))
        return root_field
    
    def get_protocol_column(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_protocol_column: bytes = liblemonshark.ls_epan_packet_protocol_column_get(self.c_epan_packet)

        if c_protocol_column is None:
            return None

        protocol_column: str = c_protocol_column.decode("utf-8")
        return protocol_column
    
    def set_protocol_column(self, protocol_column: str) -> None:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_protocol_column: bytes = protocol_column.encode("utf-8")
        liblemonshark.ls_epan_packet_protocol_column_set(self.c_epan_packet, c_protocol_column)

    def get_info_column(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_info_column: bytes = liblemonshark.ls_epan_packet_info_column_get(self.c_epan_packet)

        if c_info_column is None:
            return None

        info_column: str = c_info_column.decode("utf-8")
        return info_column
    
    def set_info_column(self, info_column: str) -> None:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        c_info_column: bytes = info_column.encode("utf-8")
        liblemonshark.ls_epan_packet_info_column_set(self.c_epan_packet, c_info_column)

    def is_visited(self) -> bool:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        visited: bool = liblemonshark.ls_epan_packet_visited_get(self.c_epan_packet) != 0
        return visited
    
    def is_visible(self) -> bool:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        visible: bool = liblemonshark.ls_epan_packet_visible_get(self.c_epan_packet) != 0
        return visible
    
    def is_ignored(self) -> bool:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        ignored: bool = liblemonshark.ls_epan_packet_ignored_get(self.c_epan_packet) != 0
        return ignored
    
    def get_buffer(self) -> bytes:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()
        length: int = self.get_length()
        if length <= 0:
            return None
        buffer: bytes = bytes(length)
        c_buffer: c_char_p = c_char_p(buffer)
        liblemonshark.ls_epan_packet_buffer_get(self.c_epan_packet, c_buffer, length)
        return buffer
    
    def get_field_count(self) -> Tuple[int, int, int, int, int, int, int]:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanPacket.get_liblemonshark()

        c_field_count: c_int32 = c_int32(0)
        c_int64_count: c_int32 = c_int32(0)
        c_uint64_count: c_int32 = c_int32(0)
        c_double_count: c_int32 = c_int32(0)
        c_string_count: c_int32 = c_int32(0)
        c_bytes_count: c_int32 = c_int32(0)
        c_representation_count: c_int32 = c_int32(0)

        liblemonshark.ls_epan_packet_field_count_get(self.c_epan_packet, byref(c_field_count), byref(c_int64_count), byref(c_uint64_count), byref(c_double_count), byref(c_string_count), byref(c_bytes_count), byref(c_representation_count))

        result: Tuple[int, int, int, int, int, int, int] = (c_field_count.value, c_int64_count.value, c_uint64_count.value, c_double_count.value, c_string_count.value, c_bytes_count.value, c_representation_count.value)
        return result
    
