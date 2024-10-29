"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

import os
import platform
from ctypes import *
from typing import *


class LemonShark:
    __is_initialized: bool = False
    __liblemonshark: CDLL = None
    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.__liblemonshark

        if not LemonShark.__liblemonshark_initialized:

            liblemonshark.ls_version_get_major.argtypes = []
            liblemonshark.ls_version_get_major.restype = c_int32
            liblemonshark.ls_version_get_minor.argtypes = []
            liblemonshark.ls_version_get_minor.restype = c_int32
            liblemonshark.ls_version_get_patch.argtypes = []
            liblemonshark.ls_version_get_patch.restype = c_int32

            liblemonshark.ls_version_get_wireshark_major.argtypes = []
            liblemonshark.ls_version_get_wireshark_major.restype = c_int32
            liblemonshark.ls_version_get_wireshark_minor.argtypes = []
            liblemonshark.ls_version_get_wireshark_minor.restype = c_int32
            liblemonshark.ls_version_get_wireshark_patch.argtypes = []
            liblemonshark.ls_version_get_wireshark_patch.restype = c_int32

            liblemonshark.ls_memory_free.argtypes = [c_void_p]
            liblemonshark.ls_memory_free.restype = None

            liblemonshark.ls_ok.argtypes = []
            liblemonshark.ls_ok.restype = c_int32

            liblemonshark.ls_error.argtypes = []
            liblemonshark.ls_error.restype = c_int32


            LemonShark.__liblemonshark_initialized = True

        return liblemonshark

    def init(wireshark_directories: List[str]) -> None:
        if LemonShark.__is_initialized:
            raise Exception("Init can be called only once.")

        system: str = platform.system().lower()
        architecture: str = platform.machine().lower()

        path: str = os.environ.get("PATH", "")

        if system == "windows":
            for wireshark_directory in wireshark_directories:
                if wireshark_directory is not None and wireshark_directory != "":
                    path = wireshark_directory + ";" + path
        elif system == "linux":
            for wireshark_directory in wireshark_directories:
                if wireshark_directory is not None and wireshark_directory != "":
                    path = wireshark_directory + ":" + path

        os.environ["PATH"] = path

        current_file_directory: str = os.path.dirname(os.path.realpath(__file__))
        if system == "windows":
            if architecture == "amd64":
                liblemonshark_path: str = f"{current_file_directory}\\native\\{system}\\{architecture}\\liblemonshark.dll"
                LemonShark.__liblemonshark = CDLL(liblemonshark_path, winmode=0) # winmode=0 to take changes to PATH into account
        elif system == "linux":
            if architecture == "x86_64":
                liblemonshark_path: str = f"{current_file_directory}/native/{system}/{architecture}/liblemonshark.so"
                LemonShark.__liblemonshark = CDLL(liblemonshark_path, winmode=0)  # winmode=0 to take changes to PATH into account

        LemonShark.__is_initialized = True

    def get_major_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        major_version: int = liblemonshark.ls_version_get_major()
        return major_version

    def get_minor_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        minor_version: int = liblemonshark.ls_version_get_minor()
        return minor_version

    def get_patch_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        patch_version: int = liblemonshark.ls_version_get_patch()
        return patch_version

    def get_wireshark_major_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        wireshark_major_version: int = liblemonshark.ls_version_get_wireshark_major()
        return wireshark_major_version

    def get_minor_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        wireshark_minor_version: int = liblemonshark.ls_version_get_wireshark_minor()
        return wireshark_minor_version

    def get_patch_version() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        wireshark_patch_version: int = liblemonshark.ls_version_get_wireshark_patch()
        return wireshark_patch_version

    def free_memory(memory: c_void_p):
        if memory is None:
            return
        if memory.value is None:
            return
        if memory.value == 0:
            return
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        liblemonshark.ls_memory_free(memory)

    def free_memory_at_address(address: int):
        if address is None:
            return
        if address == 0:
            return
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        memory: c_void_p = c_void_p(address)
        liblemonshark.ls_memory_free(memory)

    def ok() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        ok: int = liblemonshark.ls_ok()
        return ok

    def error() -> int:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()
        error: int = liblemonshark.ls_error()
        return error


#def add_error_handler(error_handler: Callable[[str], NoReturn]) -> int:
#    liblemonshark: CDLL = LemonShark.get_liblemonshark()
#    c_error_handler = CFUNCTYPE(None, c_char_p)(error_handler)
#    liblemonshark.ls_error_add_error_handler.restype = c_void_p
#    error_handler_reference: int = int(
#        liblemonshark.ls_error_add_error_handler(c_error_handler)
#    )
#    return error_handler_reference
