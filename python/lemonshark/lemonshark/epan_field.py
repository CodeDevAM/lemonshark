"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark
from .field_type import FieldType


class EpanField:
    __liblemonshark_initialized: bool = False

    EPAN_FIELD_HANDLER = CFUNCTYPE(None, c_void_p, c_void_p)

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not EpanField.__liblemonshark_initialized:
            liblemonshark.ls_epan_field_free.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_free.restype = None

            liblemonshark.ls_epan_field_size.argtypes = []
            liblemonshark.ls_epan_field_size.restype = c_int32

            liblemonshark.ls_epan_field_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_epan_field_external_ref_count_add.restype = c_int64

            liblemonshark.ls_epan_field_valid_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_valid_get.restype = c_int32

            liblemonshark.ls_epan_field_representation_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_representation_get.restype = c_char_p

            liblemonshark.ls_epan_field_id_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_id_get.restype = c_int32

            liblemonshark.ls_epan_field_type_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_type_get.restype = c_int32

            liblemonshark.ls_epan_field_name_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_name_get.restype = c_char_p

            liblemonshark.ls_epan_field_display_name_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_display_name_get.restype = c_char_p

            liblemonshark.ls_epan_field_type_name_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_type_name_get.restype = c_char_p

            liblemonshark.ls_epan_field_buffer_length_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_buffer_length_get.restype = c_int32

            liblemonshark.ls_epan_field_buffer_get.argtypes = [c_void_p, c_void_p, c_int32]
            liblemonshark.ls_epan_field_buffer_get.restype = c_int32

            liblemonshark.ls_epan_field_buffer_slice_get.argtypes = [c_void_p, c_void_p, c_int32]
            liblemonshark.ls_epan_field_buffer_slice_get.restype = c_int32

            liblemonshark.ls_epan_field_offset_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_offset_get.restype = c_int32

            liblemonshark.ls_epan_field_length_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_length_get.restype = c_int32

            liblemonshark.ls_epan_field_hidden_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_hidden_get.restype = c_int32

            liblemonshark.ls_epan_field_generated_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_generated_get.restype = c_int32

            liblemonshark.ls_epan_field_encoding_get.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_encoding_get.restype = c_int32

            liblemonshark.ls_epan_field_value_get_int64.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_value_get_int64.restype = c_int64

            liblemonshark.ls_epan_field_value_get_uint64.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_value_get_uint64.restype = c_uint64

            liblemonshark.ls_epan_field_value_get_double.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_value_get_double.restype = c_double

            liblemonshark.ls_epan_field_value_get_string.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_value_get_string.restype = c_char_p

            liblemonshark.ls_epan_field_value_get_bytes.argtypes = [c_void_p, c_void_p, c_int32]
            liblemonshark.ls_epan_field_value_get_bytes.restype = c_int32

            liblemonshark.ls_epan_field_children_count.argtypes = [c_void_p]
            liblemonshark.ls_epan_field_children_count.restype = c_int32

            liblemonshark.ls_epan_field_children_get.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_epan_field_children_get.restype = c_void_p

            liblemonshark.ls_epan_field_children_do_for_each.argtypes = [c_void_p, EpanField.EPAN_FIELD_HANDLER, c_void_p, c_int32]

            EpanField.__liblemonshark_initialized = True

        return liblemonshark
    
    def __init__(self, c_epan_field: c_void_p) -> None:
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        if c_epan_field is None or c_epan_field.value is None or c_epan_field.value == 0:
            raise Exception("c_epan_field must not be null.")

        self.c_epan_field: c_void_p = c_epan_field
        external_ref_count: int = liblemonshark.ls_epan_field_external_ref_count_add(self.c_epan_field, 1)

    def __del__(self):
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_epan_field_external_ref_count_add(self.c_epan_field, -1)
        liblemonshark.ls_epan_field_free(self.c_epan_field)
        self.c_epan_field = None

    def size() -> int:
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        size: int = liblemonshark.ls_epan_field_size()
        return size

    def new() -> "EpanField":
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_epan_field: int = liblemonshark.ls_epan_field_new()
        field: EpanField = EpanField(c_void_p(c_epan_field))
        return field
    
    def is_valid(self) -> bool:
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        if self.c_epan_field is None or self.c_epan_field.value is None or self.c_epan_field.value == 0:
            return False
        valid: bool = liblemonshark.ls_epan_field_valid_get(self.c_epan_field) != 0
        return valid

    def throw_if_not_valid(self) -> None:
        valid: bool = self.is_valid()
        if not valid:
            raise Exception("EpanPacket is expired.")
        
    def get_representation(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_representation: bytes = liblemonshark.ls_epan_field_representation_get(self.c_epan_field)

        if c_representation is None:
            return None

        representation: str = c_representation.decode("utf-8")

        return representation
    
    def get_field_id(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        field_id: int = liblemonshark.ls_epan_field_id_get(self.c_epan_field)
        return field_id
    
    def get_type(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        type: int = liblemonshark.ls_epan_field_type_get(self.c_epan_field)
        return type

    def get_name(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_name: bytes = liblemonshark.ls_epan_field_name_get(self.c_epan_field)

        if c_name is None:
            return None

        name: str = c_name.decode("utf-8")

        return name
    
    def get_display_name(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_display_name: bytes = liblemonshark.ls_epan_field_display_name_get(self.c_epan_field)

        if c_display_name is None:
            return None

        display_name: str = c_display_name.decode("utf-8")

        return display_name
    
    def get_type_name(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_type_name: bytes = liblemonshark.ls_epan_field_type_name_get(self.c_epan_field)

        if c_type_name is None:
            return None

        type_name: str = c_type_name.decode("utf-8")

        return type_name
    
    def get_buffer_length(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        buffer_length: int = liblemonshark.ls_epan_field_buffer_length_get(self.c_epan_field)
        return buffer_length
    
    def get_buffer(self) -> bytes:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        buffer_length: int = self.get_buffer_length()
        buffer: bytes = bytes(buffer_length)
        c_buffer: c_char_p = c_char_p(buffer)
        liblemonshark.ls_epan_field_buffer_get(self.c_epan_field, c_buffer, buffer_length)
        return buffer
    
    def get_buffer_slice(self) -> bytes:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        buffer_length: int = self.get_length()
        buffer: bytes = bytes(buffer_length)
        c_buffer: c_char_p = c_char_p(buffer)
        liblemonshark.ls_epan_field_buffer_slice_get(self.c_epan_field, c_buffer, buffer_length)
        return buffer
    
    def get_offset(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        offset: int = liblemonshark.ls_epan_field_offset_get(self.c_epan_field)
        return offset
    
    def get_length(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        length: int = liblemonshark.ls_epan_field_length_get(self.c_epan_field)
        return length
    
    def get_hidden(self) -> bool:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        hidden: bool = liblemonshark.ls_epan_field_hidden_get(self.c_epan_field) != 0
        return hidden
    
    def get_generated(self) -> bool:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        generated: bool = liblemonshark.ls_epan_field_generated_get(self.c_epan_field) != 0
        return generated
    
    def get_encoding(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        encoding: int = liblemonshark.ls_epan_field_encoding_get(self.c_epan_field)
        return encoding
    
    def get_value_int64(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        value: int = liblemonshark.ls_epan_field_value_get_int64(self.c_epan_field)
        return value
    
    def is_int64(self) -> bool:
        self.throw_if_not_valid()
        type: int = self.get_type()
        is_int64: bool = FieldType.is_int64(type)
        return is_int64
    
    def is_uint64(self) -> bool:
        self.throw_if_not_valid()
        type: int = self.get_type()
        is_uint64: bool = FieldType.is_uint64(type)
        return is_uint64
    
    def is_double(self) -> bool:
        self.throw_if_not_valid()
        type: int = self.get_type()
        is_double: bool = FieldType.is_double(type)
        return is_double
    
    def is_string(self) -> bool:
        self.throw_if_not_valid()
        type: int = self.get_type()
        is_string: bool = FieldType.is_string(type)
        return is_string
    
    def is_bytes(self) -> bool:
        self.throw_if_not_valid()
        type: int = self.get_type()
        is_bytes: bool = FieldType.is_bytes(type)
        return is_bytes
    
    def get_int64_value(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        value: int = liblemonshark.ls_epan_field_value_get_int64(self.c_epan_field)
        return value
    
    def get_uint64_value(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        value: int = liblemonshark.ls_epan_field_value_get_uint64(self.c_epan_field)
        return value
    
    def get_double_value(self) -> float:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        value: float = liblemonshark.ls_epan_field_value_get_double(self.c_epan_field)
        return value
    
    def get_string_value(self) -> str:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_value: bytes = liblemonshark.ls_epan_field_value_get_string(self.c_epan_field)

        if c_value is None:
            return None

        value: str = c_value.decode("utf-8")

        return value
    
    def get_bytes_value(self) -> bytes:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        length: int = self.get_length()
        if length <= 0:
            return None
        buffer: bytes = bytes(length)
        c_buffer: c_char_p = c_char_p(buffer)
        liblemonshark.ls_epan_field_value_get_bytes(self.c_epan_field, c_buffer, length)
        return buffer
    
    def get_children_count(self) -> int:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        children_count: int = liblemonshark.ls_epan_field_children_count(self.c_epan_field)
        return children_count
    
    def get_child(self, index: int) -> "EpanField":
        self.throw_if_not_valid()

        children_count: int = self.get_children_count()
        if index < 0 or index >= children_count:
            raise Exception("index < 0 or index >= children_count")
        
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        c_child: int = liblemonshark.ls_epan_field_children_get(self.c_epan_field, index)

        if c_child is None or c_child == 0:
            return None

        child: EpanField = EpanField(c_void_p(c_child))
        return child
    
    def get_children(self) -> List["EpanField"]:
        self.throw_if_not_valid()
        children_count: int = self.get_children_count()
        children: List["EpanField"] = []
        for i in range(children_count):
            child: EpanField = self.get_child(i)
            children.append(child)
        return children
    
    def do_for_each_child(self, epan_field_handler: Callable[["EpanField"], None], recursively: bool) -> None:
        self.throw_if_not_valid()
        liblemonshark: CDLL = EpanField.get_liblemonshark()
        
        def epan_field_handler_wrapper(c_epan_field: int, parameter: int) -> None:
            field: EpanField = EpanField(c_void_p(c_epan_field))
            epan_field_handler(field)

        liblemonshark.ls_epan_field_children_do_for_each(self.c_epan_field, EpanField.EPAN_FIELD_HANDLER(epan_field_handler_wrapper), None, 1 if recursively else 0)