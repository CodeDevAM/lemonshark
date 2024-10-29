"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark


class Encoding:

    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Encoding.__liblemonshark_initialized:
            liblemonshark.ls_field_encoding_na.argtypes = []
            liblemonshark.ls_field_encoding_na.restype = c_int32

            liblemonshark.ls_field_encoding_big_endian.argtypes = []
            liblemonshark.ls_field_encoding_big_endian.restype = c_int32

            liblemonshark.ls_field_encoding_little_endian.argtypes = []
            liblemonshark.ls_field_encoding_little_endian.restype = c_int32

            Encoding.__liblemonshark_initialized = True

        return liblemonshark
    
    def na() -> int:
        liblemonshark: CDLL = Encoding.get_liblemonshark()
        return liblemonshark.ls_field_encoding_na()

    def big_endian() -> int:
        liblemonshark: CDLL = Encoding.get_liblemonshark()
        return liblemonshark.ls_field_encoding_big_endian()

    def little_endian() -> int:
        liblemonshark: CDLL = Encoding.get_liblemonshark()
        return liblemonshark.ls_field_encoding_little_endian()
