"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
import ctypes
from typing import *

from .lemonshark import LemonShark


class FieldDescription:

    __liblemonshark_initialized: bool = False
    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not FieldDescription.__liblemonshark_initialized:

            liblemonshark.ls_field_description_new.argtypes = []
            liblemonshark.ls_field_description_new.restype = c_void_p
            
            liblemonshark.ls_field_description_free.argtypes = [c_void_p]
            liblemonshark.ls_field_description_free.restype = None

            liblemonshark.ls_field_description_size.argtypes = []
            liblemonshark.ls_field_description_size.restype = c_int32 

            liblemonshark.ls_field_description_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_field_description_external_ref_count_add.restype = c_int64

            liblemonshark.ls_field_description_id_get.argtypes = [c_void_p]
            liblemonshark.ls_field_description_id_get.restype = c_int32

            liblemonshark.ls_field_description_type_get.argtypes = [c_void_p]
            liblemonshark.ls_field_description_type_get.restype = c_int32

            liblemonshark.ls_field_description_name_get.argtypes = [c_void_p]
            liblemonshark.ls_field_description_name_get.restype = c_char_p

            liblemonshark.ls_field_description_display_name_get.argtypes = [c_void_p]
            liblemonshark.ls_field_description_display_name_get.restype = c_char_p

            liblemonshark.ls_field_description_get_by_id.argtypes = [c_int32]
            liblemonshark.ls_field_description_get_by_id.restype = c_void_p

            liblemonshark.ls_field_description_get_by_name.argtypes = [c_char_p]
            liblemonshark.ls_field_description_get_by_name.restype = c_void_p

            liblemonshark.ls_field_description_parent_id_get.argtypes = [c_void_p]
            liblemonshark.ls_field_description_parent_id_get.restype = c_int32

            liblemonshark.ls_field_description_get_all.argtypes = [POINTER(c_int32)]
            liblemonshark.ls_field_description_get_all.restype = c_void_p

            FieldDescription.__liblemonshark_initialized = True

        return liblemonshark


    def __init__(self, c_field_description: c_void_p) -> None:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        if c_field_description is None or c_field_description.value is None or c_field_description.value == 0:
            raise Exception("c_field_description must not be null.")

        self.c_field_description: c_void_p = c_field_description
        external_ref_count_add: int = liblemonshark.ls_field_description_external_ref_count_add(self.c_field_description, 1)

    def __del__(self):
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        external_ref_count_add: int = liblemonshark.ls_field_description_external_ref_count_add(self.c_field_description, -1)
        liblemonshark.ls_field_description_free(self.c_field_description)
        self.c_field_description = None

    def size() -> int:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        size: int = liblemonshark.ls_field_description_size()
        return size

    def new() -> "FieldDescription":
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        c_field_description: int = liblemonshark.ls_field_description_new()
        field_description: FieldDescription = FieldDescription(c_void_p(c_field_description))
        return field_description

    def get_id(self) -> int:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        id: int = liblemonshark.ls_field_description_id_get(self.c_field_description)
        return id
    
    def get_type(self) -> int:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        type: int = liblemonshark.ls_field_description_type_get(self.c_field_description)
        return type

    def get_name(self) -> str:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        c_name: bytes = liblemonshark.ls_field_description_name_get(self.c_field_description)

        if c_name is None:
            return None

        name: str = c_name.decode("utf-8")

        return name
    
    def get_display_name(self) -> str:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        c_display_name: bytes = liblemonshark.ls_field_description_display_name_get(self.c_field_description)

        if c_display_name is None:
            return None

        name: str = c_display_name.decode("utf-8")

        return name

    def get_parent_id(self) -> int:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        parent_id: int = liblemonshark.ls_field_description_parent_id_get(self.c_field_description)
        return parent_id

    def get_by_id(id: int) -> "FieldDescription":
        if id < 0:
            raise Exception("id < 0")

        liblemonshark: CDLL = FieldDescription.get_liblemonshark()

        c_field_description: int = liblemonshark.ls_field_description_get_by_id(id)

        if c_field_description is None or c_field_description == 0:
            return None
        
        field_description: FieldDescription = FieldDescription(c_void_p(c_field_description))
        return field_description

    def get_by_name(name: str) -> "FieldDescription":
        if name is None:
            return None

        liblemonshark: CDLL = FieldDescription.get_liblemonshark()

        c_name: c_char_p = c_char_p(name.encode("utf-8"))
        c_field_description: int = liblemonshark.ls_field_description_get_by_name(c_name)

        if c_field_description is None or c_field_description == 0:
            return None
        
        field_description: FieldDescription = FieldDescription(c_void_p(c_field_description))
        return field_description

    def get_all() -> List["FieldDescription"]:
        liblemonshark: CDLL = FieldDescription.get_liblemonshark()
        count: c_int32 = c_int32()
        c_field_descriptions: int = liblemonshark.ls_field_description_get_all(byref(count))

        if c_field_descriptions is None or c_field_descriptions == 0:
            return []
        
        result: List["FieldDescription"] = []
        for i in range(count.value):
            c_field_description: c_void_p = ctypes.cast(c_field_descriptions + i * ctypes.sizeof(c_void_p), POINTER(c_void_p)).contents

            if c_field_description is None or c_field_description == 0:
                continue

            field_description: FieldDescription = FieldDescription(c_field_description)
            result.append(field_description)

        return result
