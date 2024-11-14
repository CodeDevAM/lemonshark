"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark


class Filter:
    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Filter.__liblemonshark_initialized:
            liblemonshark.ls_filter_is_valid.argtypes = [c_char_p, POINTER(c_void_p)]
            liblemonshark.ls_filter_is_valid.restype = c_int32

            Filter.__liblemonshark_initialized = True

        return liblemonshark
    
    def is_valid(filter: str) -> Tuple[bool, str]:
        liblemonshark: CDLL = Filter.get_liblemonshark()

        c_filter: c_char_p = c_char_p(filter.encode("utf-8"))
        c_error_message = c_void_p()
        is_valid: int = liblemonshark.ls_filter_is_valid(c_filter, byref(c_error_message))

        result : bool = True
        error_message: str = None
        if is_valid == LemonShark.error():
            result = False
            if c_error_message.value is not None and c_error_message.value != 0:
                error_message: str = string_at(c_error_message.value).decode("utf-8")
        
        if c_error_message.value is not None and c_error_message.value != 0:
            LemonShark.free_memory(c_error_message)

        return (result, error_message)