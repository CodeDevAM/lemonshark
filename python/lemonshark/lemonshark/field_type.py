"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from ctypes import *
from typing import *

from .lemonshark import LemonShark

class FieldType:

    __liblemonshark_initialized: bool = False
    
    def get_liblemonshark() -> CDLL:
        liblemonshark: CDLL = LemonShark.get_liblemonshark()

        if not FieldType.__liblemonshark_initialized:
            liblemonshark.ls_field_type_int8.argtypes = []
            liblemonshark.ls_field_type_int8.restype = c_int32
            liblemonshark.ls_field_type_int16.argtypes = []
            liblemonshark.ls_field_type_int16.restype = c_int32
            liblemonshark.ls_field_type_int24.argtypes = []
            liblemonshark.ls_field_type_int24.restype = c_int32
            liblemonshark.ls_field_type_int32.argtypes = []
            liblemonshark.ls_field_type_int32.restype = c_int32
            liblemonshark.ls_field_type_int40.argtypes = []
            liblemonshark.ls_field_type_int40.restype = c_int32
            liblemonshark.ls_field_type_int48.argtypes = []
            liblemonshark.ls_field_type_int48.restype = c_int32
            liblemonshark.ls_field_type_int56.argtypes = []
            liblemonshark.ls_field_type_int56.restype = c_int32
            liblemonshark.ls_field_type_int64.argtypes = []
            liblemonshark.ls_field_type_int64.restype = c_int32
            liblemonshark.ls_field_type_uint8.argtypes = []
            liblemonshark.ls_field_type_uint8.restype = c_int32
            liblemonshark.ls_field_type_uint16.argtypes = []
            liblemonshark.ls_field_type_uint16.restype = c_int32
            liblemonshark.ls_field_type_uint24.argtypes = []
            liblemonshark.ls_field_type_uint24.restype = c_int32
            liblemonshark.ls_field_type_uint32.argtypes = []
            liblemonshark.ls_field_type_uint32.restype = c_int32
            liblemonshark.ls_field_type_uint40.argtypes = []
            liblemonshark.ls_field_type_uint40.restype = c_int32
            liblemonshark.ls_field_type_uint48.argtypes = []
            liblemonshark.ls_field_type_uint48.restype = c_int32
            liblemonshark.ls_field_type_uint56.argtypes = []
            liblemonshark.ls_field_type_uint56.restype = c_int32
            liblemonshark.ls_field_type_uint64.argtypes = []
            liblemonshark.ls_field_type_uint64.restype = c_int32
            liblemonshark.ls_field_type_none.argtypes = []
            liblemonshark.ls_field_type_none.restype = c_int32
            liblemonshark.ls_field_type_protocol.argtypes = []
            liblemonshark.ls_field_type_protocol.restype = c_int32
            liblemonshark.ls_field_type_boolean.argtypes = []
            liblemonshark.ls_field_type_boolean.restype = c_int32
            liblemonshark.ls_field_type_char.argtypes = []
            liblemonshark.ls_field_type_char.restype = c_int32
            liblemonshark.ls_field_type_ieee_11073_float16.argtypes = []
            liblemonshark.ls_field_type_ieee_11073_float16.restype = c_int32
            liblemonshark.ls_field_type_ieee_11073_float32.argtypes = []
            liblemonshark.ls_field_type_ieee_11073_float32.restype = c_int32
            liblemonshark.ls_field_type_float.argtypes = []
            liblemonshark.ls_field_type_float.restype = c_int32
            liblemonshark.ls_field_type_double.argtypes = []
            liblemonshark.ls_field_type_double.restype = c_int32
            liblemonshark.ls_field_type_absolute_time.argtypes = []
            liblemonshark.ls_field_type_absolute_time.restype = c_int32
            liblemonshark.ls_field_type_relative_time.argtypes = []
            liblemonshark.ls_field_type_relative_time.restype = c_int32
            liblemonshark.ls_field_type_string.argtypes = []
            liblemonshark.ls_field_type_string.restype = c_int32
            liblemonshark.ls_field_type_stringz.argtypes = []
            liblemonshark.ls_field_type_stringz.restype = c_int32
            liblemonshark.ls_field_type_uint_string.argtypes = []
            liblemonshark.ls_field_type_uint_string.restype = c_int32
            liblemonshark.ls_field_type_ether.argtypes = []
            liblemonshark.ls_field_type_ether.restype = c_int32
            liblemonshark.ls_field_type_bytes.argtypes = []
            liblemonshark.ls_field_type_bytes.restype = c_int32
            liblemonshark.ls_field_type_uint_bytes.argtypes = []
            liblemonshark.ls_field_type_uint_bytes.restype = c_int32
            liblemonshark.ls_field_type_ipv4.argtypes = []
            liblemonshark.ls_field_type_ipv4.restype = c_int32
            liblemonshark.ls_field_type_ipv6.argtypes = []
            liblemonshark.ls_field_type_ipv6.restype = c_int32
            liblemonshark.ls_field_type_ipxnet.argtypes = []
            liblemonshark.ls_field_type_ipxnet.restype = c_int32
            liblemonshark.ls_field_type_framenum.argtypes = []
            liblemonshark.ls_field_type_framenum.restype = c_int32
            liblemonshark.ls_field_type_guid.argtypes = []
            liblemonshark.ls_field_type_guid.restype = c_int32
            liblemonshark.ls_field_type_oid.argtypes = []
            liblemonshark.ls_field_type_oid.restype = c_int32
            liblemonshark.ls_field_type_eui64.argtypes = []
            liblemonshark.ls_field_type_eui64.restype = c_int32
            liblemonshark.ls_field_type_ax25.argtypes = []
            liblemonshark.ls_field_type_ax25.restype = c_int32
            liblemonshark.ls_field_type_vines.argtypes = []
            liblemonshark.ls_field_type_vines.restype = c_int32
            liblemonshark.ls_field_type_rel_oid.argtypes = []
            liblemonshark.ls_field_type_rel_oid.restype = c_int32
            liblemonshark.ls_field_type_system_id.argtypes = []
            liblemonshark.ls_field_type_system_id.restype = c_int32
            liblemonshark.ls_field_type_stringzpad.argtypes = []
            liblemonshark.ls_field_type_stringzpad.restype = c_int32
            liblemonshark.ls_field_type_fcwwn.argtypes = []
            liblemonshark.ls_field_type_fcwwn.restype = c_int32
            liblemonshark.ls_field_type_stringztrunc.argtypes = []
            liblemonshark.ls_field_type_stringztrunc.restype = c_int32
            liblemonshark.ls_field_type_num_types.argtypes = []
            liblemonshark.ls_field_type_num_types.restype = c_int32
            liblemonshark.ls_field_type_scalar.argtypes = []
            liblemonshark.ls_field_type_scalar.restype = c_int32

            liblemonshark.ls_field_type_is_int64.argtypes = [c_int32]
            liblemonshark.ls_field_type_is_int64.restype = c_int32

            liblemonshark.ls_field_type_is_uint64.argtypes = [c_int32]
            liblemonshark.ls_field_type_is_uint64.restype = c_int32

            liblemonshark.ls_field_type_is_double.argtypes = [c_int32]
            liblemonshark.ls_field_type_is_double.restype = c_int32

            liblemonshark.ls_field_type_is_string.argtypes = [c_int32]
            liblemonshark.ls_field_type_is_string.restype = c_int32

            liblemonshark.ls_field_type_is_bytes.argtypes = [c_int32]
            liblemonshark.ls_field_type_is_bytes.restype = c_int32

            liblemonshark.ls_field_type_get_name.argtypes = [c_int32]
            liblemonshark.ls_field_type_get_name.restype = c_char_p

            FieldType.__liblemonshark_initialized = True

        return liblemonshark
    
    def int8() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int8()

    def int16() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int16()

    def int24() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int24()

    def int32() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int32()

    def int40() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int40()

    def int48() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int48()

    def int56() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int56()

    def int64() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_int64()

    def uint8() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint8()

    def uint16() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint16()

    def uint24() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint24()

    def uint32() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint32()

    def uint40() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint40()

    def uint48() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint48()

    def uint56() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint56()

    def uint64() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint64()

    def none() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_none()

    def protocol() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_protocol()

    def boolean() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_boolean()

    def char() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_char()

    def ieee_11073_float16() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ieee_11073_float16()

    def ieee_11073_float32() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ieee_11073_float32()

    def float() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_float()

    def double() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_double()

    def absolute_time() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_absolute_time()

    def relative_time() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_relative_time()

    def string() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_string()

    def stringz() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_stringz()

    def uint_string() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint_string()

    def ether() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ether()

    def bytes() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_bytes()

    def uint_bytes() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_uint_bytes()

    def ipv4() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ipv4()

    def ipv6() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ipv6()

    def ipxnet() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ipxnet()

    def framenum() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_framenum()

    def guid() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_guid()

    def oid() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_oid()

    def eui64() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_eui64()

    def ax25() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_ax25()

    def vines() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_vines()

    def rel_oid() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_rel_oid()

    def system_id() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_system_id()

    def stringzpad() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_stringzpad()

    def fcwwn() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_fcwwn()

    def stringztrunc() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_stringztrunc()

    def num_types() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_num_types()

    def scalar() -> int:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_scalar()

    def is_int64(field_type: int) -> bool:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_is_int64(field_type) != 0

    def is_uint64(field_type: int) -> bool:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_is_uint64(field_type) != 0

    def is_double(field_type: int) -> bool:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_is_double(field_type) != 0

    def is_string(field_type: int) -> bool:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_is_string(field_type) != 0

    def is_bytes(field_type: int) -> bool:
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        return liblemonshark.ls_field_type_is_bytes(field_type) != 0

    def get_name(field_type: int) -> str:
        if field_type < 0 or field_type >= FieldType.num_types():
            return None
        
        liblemonshark: CDLL = FieldType.get_liblemonshark()
        c_name: bytes = liblemonshark.ls_field_type_get_name(field_type)

        if c_name is None:
            return None

        name: str = c_name.decode("utf-8")

        return name
