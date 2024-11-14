"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark
from .packet import Packet


class Session:
    __current_session: "Session" = None

    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Session.__liblemonshark_initialized:
            liblemonshark.ls_session_create_from_file.argtypes = [c_char_p, c_char_p, POINTER(c_void_p)]
            liblemonshark.ls_session_create_from_file.restype = c_int32

            liblemonshark.ls_session_get_next_packet_id.argtypes = [POINTER(c_void_p)]
            liblemonshark.ls_session_get_next_packet_id.restype = c_int32

            liblemonshark.ls_session_get_packet.argtypes = [c_int32, c_int32, c_int32, c_int32, c_int32, c_int32, POINTER(c_void_p)]
            liblemonshark.ls_session_get_packet.restype = c_void_p

            liblemonshark.ls_session_close.argtypes = []
            liblemonshark.ls_session_close.restype = None

            Session.__liblemonshark_initialized = True

        return liblemonshark

    def __init__(self) -> None:
        self.closed = False

    def create_from_file(file_path: str, read_filter: str) -> "Session":
        liblemonshark: CDLL = Session.get_liblemonshark()

        if Session.__current_session is not None:
            raise Exception("There can only be one session at a time.")

        c_file_path: c_char_p = c_char_p(file_path.encode("utf-8"))
        c_read_filter: c_char_p = c_char_p(read_filter.encode("utf-8"))
        c_error_message = c_void_p()
        creation_result: int = liblemonshark.ls_session_create_from_file(c_file_path, c_read_filter, byref(c_error_message))

        if creation_result == LemonShark.error():
            error_message: str = ""
            if c_error_message.value is not None and c_error_message.value != 0:
                error_message: str = string_at(c_error_message.value).decode("utf-8")
                LemonShark.free_memory(c_error_message)
            raise Exception(error_message)

        if c_error_message.value is not None and c_error_message.value != 0:
            LemonShark.free_memory(c_error_message)

        session: Session = Session()
        Session.__current_session = session

        return session

    def close(self) -> None:
        if self.closed:
            return

        liblemonshark: CDLL = Session.get_liblemonshark()

        Session.__current_session = None
        self.closed = True

        liblemonshark.ls_session_close()

    def get_next_packet_id(self) -> int:
        if self.closed:
            raise Exception("Session is closed")

        liblemonshark: CDLL = Session.get_liblemonshark()

        c_error_message = c_void_p()
        packet_id: int = liblemonshark.ls_session_get_next_packet_id(byref(c_error_message))

        if packet_id < 0:
            error_message: str = ""
            # if no error message is given we assume as regular finish without a failure
            if c_error_message.value is not None and c_error_message.value != 0:
                error_message: str = string_at(c_error_message.value).decode("utf-8")
                LemonShark.free_memory(c_error_message)
                raise Exception(error_message)

        if c_error_message.value is not None and c_error_message.value != 0:
            LemonShark.free_memory(c_error_message)

        return packet_id

    def get_packet(
        self,
        packet_id: int,
        include_buffers: bool,
        include_columns: bool,
        include_representations: bool,
        include_strings: bool,
        include_bytes: bool,
    ) -> Packet:
        if self.closed:
            raise Exception("Session is closed")

        liblemonshark: CDLL = Session.get_liblemonshark()

        c_error_message = c_void_p()
        c_packet: int = liblemonshark.ls_session_get_packet(
            packet_id,
            1 if include_buffers else 0,
            1 if include_columns else 0,
            1 if include_representations else 0,
            1 if include_strings else 0,
            1 if include_bytes else 0,
            byref(c_error_message),
        )

        if c_packet == 0:
            error_message: str = ""
            if c_error_message.value is not None and c_error_message.value != 0:
                error_message: str = string_at(c_error_message.value).decode("utf-8")
                LemonShark.free_memory(c_error_message)
            raise Exception(error_message)

        if c_error_message.value is not None and c_error_message.value != 0:
            LemonShark.free_memory(c_error_message)

        packet: Packet = Packet(c_void_p(c_packet))
        return packet
