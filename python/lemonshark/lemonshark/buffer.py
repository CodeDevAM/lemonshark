"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark


class Buffer:

    __liblemonshark_initialized: bool = False
    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Buffer.__liblemonshark_initialized:

            liblemonshark.ls_buffer_new.argtypes = []
            liblemonshark.ls_buffer_new.restype = c_void_p
            
            liblemonshark.ls_buffer_free.argtypes = [c_void_p]
            liblemonshark.ls_buffer_free.restype = None

            liblemonshark.ls_buffer_size.argtypes = []
            liblemonshark.ls_buffer_size.restype = c_int32 

            liblemonshark.ls_buffer_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_buffer_external_ref_count_add.restype = c_int64

            liblemonshark.ls_buffer_data_get.argtypes = [c_void_p]
            liblemonshark.ls_buffer_data_get.restype = c_void_p

            liblemonshark.ls_buffer_data_set.argtypes = [c_void_p, c_char_p, c_int32]
            liblemonshark.ls_buffer_data_set.restype = None

            liblemonshark.ls_buffer_length_get.argtypes = [c_void_p]
            liblemonshark.ls_buffer_length_get.restype = c_int32

            Buffer.__liblemonshark_initialized = True

        return liblemonshark


    def __init__(self, c_buffer: c_void_p) -> None:
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        if c_buffer is None or c_buffer.value is None or c_buffer.value == 0:
            raise Exception("c_buffer must not be null.")

        self.c_buffer: c_void_p = c_buffer
        external_ref_count_add: int = liblemonshark.ls_buffer_external_ref_count_add(self.c_buffer, 1)

    def __del__(self):
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        external_ref_count_add: int = liblemonshark.ls_buffer_external_ref_count_add(self.c_buffer, -1)
        liblemonshark.ls_buffer_free(self.c_buffer)
        self.c_buffer = None

    def size() -> int:
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        size: int = liblemonshark.ls_buffer_size()
        return size

    def new() -> "Buffer":
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        c_buffer: int = liblemonshark.ls_buffer_new()
        buffer: Buffer = Buffer(c_void_p(c_buffer))
        return buffer

    def get_length(self) -> int:
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        length: int = liblemonshark.ls_buffer_length_get(self.c_buffer)
        return length

    def get_data(self) -> bytes:
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        c_data: int = liblemonshark.ls_buffer_data_get(self.c_buffer)

        if c_data is None or c_data == 0:
            return None

        length: int = self.get_length()
        data: bytes = string_at(c_void_p(c_data), length)
        return data

    def set_data(self, data: bytes) -> None:
        liblemonshark: CDLL = Buffer.get_liblemonshark()
        c_data: c_char_p = c_char_p(data)
        length: int = len(data) if data is not None else 0
        liblemonshark.ls_buffer_data_set(self.c_buffer, c_data, length)
