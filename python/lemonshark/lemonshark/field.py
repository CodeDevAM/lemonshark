"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark
from .field_type import FieldType

class Field:
    __liblemonshark_initialized: bool = False

    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not Field.__liblemonshark_initialized:
            liblemonshark.ls_field_new.argtypes = []
            liblemonshark.ls_field_new.restype = c_void_p

            liblemonshark.ls_field_free.argtypes = [c_void_p]
            liblemonshark.ls_field_free.restype = None

            liblemonshark.ls_field_size.argtypes = []
            liblemonshark.ls_field_size.restype = c_int32

            liblemonshark.ls_field_external_ref_count_add.argtypes = [c_void_p, c_int64]
            liblemonshark.ls_field_external_ref_count_add.restype = c_int64

            liblemonshark.ls_field_representation_get.argtypes = [c_void_p]
            liblemonshark.ls_field_representation_get.restype = c_char_p

            liblemonshark.ls_field_representation_set.argtypes = [c_void_p, c_char_p]
            liblemonshark.ls_field_representation_set.restype = None

            liblemonshark.ls_field_id_get.argtypes = [c_void_p]
            liblemonshark.ls_field_id_get.restype = c_int32

            liblemonshark.ls_field_id_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_id_set.restype = None

            liblemonshark.ls_field_type_get.argtypes = [c_void_p]
            liblemonshark.ls_field_type_get.restype = c_int32

            liblemonshark.ls_field_name_get.argtypes = [c_void_p]
            liblemonshark.ls_field_name_get.restype = c_char_p

            liblemonshark.ls_field_display_name_get.argtypes = [c_void_p]
            liblemonshark.ls_field_display_name_get.restype = c_char_p

            liblemonshark.ls_field_type_name_get.argtypes = [c_void_p]
            liblemonshark.ls_field_type_name_get.restype = c_char_p

            liblemonshark.ls_field_buffer_id_get.argtypes = [c_void_p]
            liblemonshark.ls_field_buffer_id_get.restype = c_int32

            liblemonshark.ls_field_buffer_id_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_buffer_id_set.restype = None

            liblemonshark.ls_field_offset_get.argtypes = [c_void_p]
            liblemonshark.ls_field_offset_get.restype = c_int32

            liblemonshark.ls_field_offset_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_offset_set.restype = None

            liblemonshark.ls_field_length_get.argtypes = [c_void_p]
            liblemonshark.ls_field_length_get.restype = c_int32

            liblemonshark.ls_field_hidden_get.argtypes = [c_void_p]
            liblemonshark.ls_field_hidden_get.restype = c_int32

            liblemonshark.ls_field_hidden_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_hidden_set.restype = None

            liblemonshark.ls_field_generated_get.argtypes = [c_void_p]
            liblemonshark.ls_field_generated_get.restype = c_int32

            liblemonshark.ls_field_generated_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_generated_set.restype = None

            liblemonshark.ls_field_encoding_get.argtypes = [c_void_p]
            liblemonshark.ls_field_encoding_get.restype = c_int32

            liblemonshark.ls_field_encoding_set.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_encoding_set.restype = None

            liblemonshark.ls_field_value_get_int64.argtypes = [c_void_p]
            liblemonshark.ls_field_value_get_int64.restype = c_int64

            liblemonshark.ls_field_value_set_int64.argtypes = [c_void_p, c_int64, c_int32]
            liblemonshark.ls_field_value_set_int64.restype = c_int32

            liblemonshark.ls_field_value_get_uint64.argtypes = [c_void_p]
            liblemonshark.ls_field_value_get_uint64.restype = c_uint64

            liblemonshark.ls_field_value_set_uint64.argtypes = [c_void_p, c_uint64, c_int32]
            liblemonshark.ls_field_value_set_uint64.restype = c_int32

            liblemonshark.ls_field_value_get_double.argtypes = [c_void_p]
            liblemonshark.ls_field_value_get_double.restype = c_double

            liblemonshark.ls_field_value_set_double.argtypes = [c_void_p, c_double, c_int32]
            liblemonshark.ls_field_value_set_double.restype = c_int32

            liblemonshark.ls_field_value_get_string.argtypes = [c_void_p]
            liblemonshark.ls_field_value_get_string.restype = c_char_p

            liblemonshark.ls_field_value_set_string.argtypes = [c_void_p, c_char_p, c_int32]
            liblemonshark.ls_field_value_set_string.restype = c_int32

            liblemonshark.ls_field_value_get_bytes.argtypes = [c_void_p]
            liblemonshark.ls_field_value_get_bytes.restype = c_void_p

            liblemonshark.ls_field_value_set_bytes.argtypes = [c_void_p, c_char_p, c_int32, c_int32]
            liblemonshark.ls_field_value_set_bytes.restype = c_int32

            liblemonshark.ls_field_children_count.argtypes = [c_void_p]
            liblemonshark.ls_field_children_count.restype = c_int32

            liblemonshark.ls_field_children_get.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_children_get.restype = c_void_p

            liblemonshark.ls_field_children_set.argtypes = [c_void_p, c_void_p, c_int32]
            liblemonshark.ls_field_children_set.restype = None

            liblemonshark.ls_field_children_add.argtypes = [c_void_p, c_void_p]
            liblemonshark.ls_field_children_add.restype = None

            liblemonshark.ls_field_children_remove.argtypes = [c_void_p, c_int32]
            liblemonshark.ls_field_children_remove.restype = None

            liblemonshark.ls_field_get_name.argtypes = [c_int32]
            liblemonshark.ls_field_get_name.restype = c_char_p

            liblemonshark.ls_field_get_display_name.argtypes = [c_int32]
            liblemonshark.ls_field_get_display_name.restype = c_char_p

            Field.__liblemonshark_initialized = True

        return liblemonshark
    
    def __init__(self, c_field: c_void_p) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        if c_field is None or c_field.value is None or c_field.value == 0:
            raise Exception("c_field must not be null.")

        self.c_field: c_void_p = c_field
        external_ref_count: int = liblemonshark.ls_field_external_ref_count_add(self.c_field, 1)

    def __del__(self):
        liblemonshark: CDLL = Field.get_liblemonshark()
        external_ref_count: int = liblemonshark.ls_field_external_ref_count_add(self.c_field, -1)
        liblemonshark.ls_field_free(self.c_field)
        self.c_field = None

    def size() -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        size: int = liblemonshark.ls_field_size()
        return size

    def new() -> "Field":
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_field: int = liblemonshark.ls_field_new()
        field: Field = Field(c_void_p(c_field))
        return field

    def get_representation(self) -> str:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_representation: bytes = liblemonshark.ls_field_representation_get(self.c_field)

        if c_representation is None:
            return None

        representation: str = c_representation.decode("utf-8")

        return representation

    def set_representation(self, representation: str) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_representation: c_char_p = c_char_p(representation.encode("utf-8"))
        liblemonshark.ls_field_representation_set(self.c_field, c_representation)

    def get_field_id(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        field_id: int = liblemonshark.ls_field_id_get(self.c_field)
        return field_id

    def set_field_id(self, field_id: int) -> None:
        if field_id < 0:
            raise Exception("field_id < 0")

        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_id_set(self.c_field, field_id)

    def get_type(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        type: int = liblemonshark.ls_field_type_get(self.c_field)
        return type

    def get_name(self) -> str:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_name: bytes = liblemonshark.ls_field_name_get(self.c_field)

        if c_name is None:
            return None

        name: str = c_name.decode("utf-8")

        return name
    
    def get_display_name(self) -> str:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_display_name: bytes = liblemonshark.ls_field_display_name_get(self.c_field)

        if c_display_name is None:
            return None

        display_name: str = c_display_name.decode("utf-8")

        return display_name
    
    def get_type_name(self) -> str:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_type_name: bytes = liblemonshark.ls_field_type_name_get(self.c_field)

        if c_type_name is None:
            return None

        type_name: str = c_type_name.decode("utf-8")

        return type_name

    def get_buffer_id(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        buffer_id: int = liblemonshark.ls_field_buffer_id_get(self.c_field)
        return buffer_id

    def set_buffer_id(self, buffer_id: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_buffer_id_set(self.c_field, buffer_id)

    def get_offset(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        offset: int = liblemonshark.ls_field_offset_get(self.c_field)
        return offset

    def set_offset(self, offset: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_offset_set(self.c_field, offset)

    def get_length(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        length: int = liblemonshark.ls_field_length_get(self.c_field)
        return length

    def get_hidden(self) -> bool:
        liblemonshark: CDLL = Field.get_liblemonshark()
        hidden: int = liblemonshark.ls_field_hidden_get(self.c_field)
        return hidden != 0

    def set_hidden(self, hidden: bool) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_hidden_set(self.c_field, 1 if hidden else 0)

    def get_generated(self) -> bool:
        liblemonshark: CDLL = Field.get_liblemonshark()
        generated: int = liblemonshark.ls_field_generated_get(self.c_field)
        return generated != 0

    def set_generated(self, generated: bool) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_generated_set(self.c_field, 1 if generated else 0)

    def get_encoding(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        encoding: int = liblemonshark.ls_field_encoding_get(self.c_field)
        return encoding

    def set_encoding(self, encoding: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_encoding_set(self.c_field, encoding)

    def get_int64_value(self) -> int:
        is_int64: bool = self.is_int64()
        if not is_int64:
            raise Exception("Value is not of type int64.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        value: int = liblemonshark.ls_field_value_get_int64(self.c_field)
        return value

    def set_int64_value(self, value: int, type: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        set_result: int = liblemonshark.ls_field_value_set_int64(self.c_field, value, type)

        if set_result == LemonShark.error():
            raise Exception("Invalid type")

    def get_uint64_value(self) -> int:
        is_uint64: bool = self.is_uint64()
        if not is_uint64:
            raise Exception("Value is not of type uint64.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        value: int = liblemonshark.ls_field_value_get_uint64(self.c_field)
        return value

    def set_uint64_value(self, value: int, type: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        set_result: int = liblemonshark.ls_field_value_set_uint64(self.c_field, value, type)

        if set_result == LemonShark.error():
            raise Exception("Invalid type")

    def get_double_value(self) -> float:
        is_double: bool = self.is_double()
        if not is_double:
            raise Exception("Value is not of type double.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        value: float = liblemonshark.ls_field_value_get_double(self.c_field)
        return value

    def set_double_value(self, value: float, type: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        set_result: int = liblemonshark.ls_field_value_set_double(self.c_field, value, type)

        if set_result == LemonShark.error():
            raise Exception("Invalid type")

    def get_string_value(self) -> str:
        is_string: bool = self.is_string()
        if not is_string:
            raise Exception("Value is not of type string.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        c_value: bytes = liblemonshark.ls_field_value_get_string(self.c_field)

        if c_value is None:
            return None

        value: str = c_value.decode("utf-8")

        return value

    def set_string_value(self, value: str, type: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_value: c_char_p = c_char_p(value.encode("utf-8"))
        set_result: int = liblemonshark.ls_field_value_set_string(self.c_field, c_value, type)

        if set_result == LemonShark.error():
            raise Exception("Invalid type")

    def get_bytes_value(self) -> float:
        is_bytes: bool = self.is_bytes()
        if not is_bytes:
            raise Exception("Value is not of type bytes.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        c_value: int = liblemonshark.ls_field_value_get_bytes(self.c_field)

        if c_value is None or c_value == 0:
            return None

        length: int = self.get_length()

        value: bytes = string_at(c_void_p(c_value), length)

        return value

    def set_bytes_value(self, value: bytes, type: int) -> None:
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_value: c_char_p = c_char_p(value)

        length: int = len(value) if value is not None else 0
        set_result: int = liblemonshark.ls_field_value_set_bytes(self.c_field, c_value, length, type)

        if set_result == LemonShark.error():
            raise Exception("Invalid type")

    def is_int64(self) -> bool:
        field_type: int = self.get_type()
        result: bool = FieldType.is_int64(field_type)
        return result

    def is_uint64(self) -> bool:
        field_type: int = self.get_type()
        result: bool = FieldType.is_uint64(field_type)
        return result

    def is_double(self) -> bool:
        field_type: int = self.get_type()
        result: bool = FieldType.is_double(field_type)
        return result

    def is_string(self) -> bool:
        field_type: int = self.get_type()
        result: bool = FieldType.is_string(field_type)
        return result

    def is_bytes(self) -> bool:
        field_type: int = self.get_type()
        result: bool = FieldType.is_bytes(field_type)
        return result

    def children_count(self) -> int:
        liblemonshark: CDLL = Field.get_liblemonshark()
        children_count: int = liblemonshark.ls_field_children_count(self.c_field)
        return children_count

    def get_child(self, index: int) -> "Field":
        children_count: int = self.children_count()
        if index < 0 or index >= children_count:
            raise Exception("index < 0 or index >= children_count")

        liblemonshark: CDLL = Field.get_liblemonshark()
        c_child: int = liblemonshark.ls_field_children_get(self.c_field, index)

        if c_child is None or c_child == 0:
            return None
        field: Field = Field(c_void_p(c_child))
        return field

    def set_child(self, child: "Field", index: int) -> None:
        if (
            child is None
            or child.c_field is None
            or child.c_field.value is None
            or child.c_field.value == 0
        ):
            raise Exception("child must not be null.")

        children_count: int = self.children_count()
        if index < 0 or index >= children_count:
            raise Exception("index < 0 or index >= children_count")

        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_children_set(self.c_field, child.c_field, index)

    def add_child(self, child: "Field") -> None:
        if (
            child is None
            or child.c_field is None
            or child.c_field.value is None
            or child.c_field.value == 0
        ):
            raise Exception("child must not be null.")

        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_children_add(self.c_field, child.c_field)

    def remove_child(self, index: int) -> None:
        children_count: int = self.children_count()
        if index < 0 or index >= children_count:
            raise Exception("index < 0 or index >= children_count")

        liblemonshark: CDLL = Field.get_liblemonshark()
        liblemonshark.ls_field_children_remove(self.c_field, index)

    def get_children(self) -> List["Field"]:
        children: List["Field"] = []
        children_count: int = self.children_count()
        for i in range(children_count):
            child: "Field" = self.get_child(i)
            children.append(child)
        return children

    def get_name_from_id(field_id: int) -> str:
        if field_id <= 0:
            return None
        
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_name: bytes = liblemonshark.ls_field_get_name(field_id)

        if c_name is None:
            return None

        name: str = c_name.decode("utf-8")
        return name

    def get_display_name_from_id(field_id: int) -> str:
        if field_id <= 0:
            return None
        
        liblemonshark: CDLL = Field.get_liblemonshark()
        c_display_name: bytes = liblemonshark.ls_field_get_display_name(field_id)

        if c_display_name is None:
            return None

        display_name: str = c_display_name.decode("utf-8")
        return display_name
