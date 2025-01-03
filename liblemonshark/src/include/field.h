/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_FIELD__
#define __LS_FIELD__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

// lemonshark includes
#include "ls_common.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef union field_value
    {
        guint64 value;
        double double_value;
        void* pointer;
    } field_value_t;

    typedef struct field
    {
        gint64 externel_ref_count;
        char *representation;
        gint32 id;
        gint32 type;
        gint32 buffer_id;
        gint32 offset;
        gint32 length;
        gint32 hidden;
        gint32 generated;
        gint32 encoding;
        field_value_t value;
        gboolean value_requires_free;
        GArray *children;
    } field_t;

    WS_DLL_PUBLIC field_t *ls_field_new(void);

    WS_DLL_PUBLIC void ls_field_free(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_size(void);

    WS_DLL_PUBLIC gint64 ls_field_external_ref_count_add(field_t *field, gint64 ref_count);

    WS_DLL_PUBLIC const char *ls_field_representation_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_representation_set(field_t *field, const char *representation);

    WS_DLL_PUBLIC gint32 ls_field_id_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_id_set(field_t *field, const gint32 id);

    WS_DLL_PUBLIC gint32 ls_field_type_get(field_t *field);

    WS_DLL_PUBLIC const char *ls_field_name_get(field_t *field);

    WS_DLL_PUBLIC const char *ls_field_display_name_get(field_t *field);

    WS_DLL_PUBLIC const char *ls_field_type_name_get(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_buffer_id_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_buffer_id_set(field_t *field, const gint32 buffer_id);

    WS_DLL_PUBLIC gint32 ls_field_offset_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_offset_set(field_t *field, const gint32 offset);

    WS_DLL_PUBLIC gint32 ls_field_length_get(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_hidden_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_hidden_set(field_t *field, const gint32 hidden);

    WS_DLL_PUBLIC gint32 ls_field_generated_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_generated_set(field_t *field, const gint32 generated);

    WS_DLL_PUBLIC gint32 ls_field_encoding_get(field_t *field);

    WS_DLL_PUBLIC void ls_field_encoding_set(field_t *field, const gint32 encoding);

    WS_DLL_PUBLIC gint64 ls_field_value_get_int64(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_value_set_int64(field_t *field, const gint64 value, const gint32 type);

    WS_DLL_PUBLIC guint64 ls_field_value_get_uint64(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_value_set_uint64(field_t *field, const guint64 value, const gint32 type);

    WS_DLL_PUBLIC double ls_field_value_get_double(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_value_set_double(field_t *field, const double value, const gint32 type);

    WS_DLL_PUBLIC const char *ls_field_value_get_string(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_value_set_string(field_t *field, const char *value, const gint32 type);

    WS_DLL_PUBLIC gint32 ls_field_value_take_string(field_t *field, const char *value, const gint32 type);

    WS_DLL_PUBLIC const guint8 *ls_field_value_get_bytes(field_t *field);

    WS_DLL_PUBLIC gint32 ls_field_value_set_bytes(field_t *field, const guint8 *value, gint32 length, const gint32 type);

    WS_DLL_PUBLIC gint32 ls_field_value_take_bytes(field_t *field, const guint8 *value, gint32 length, const gint32 type);

    WS_DLL_PUBLIC gint32 ls_field_children_count(field_t *field);

    WS_DLL_PUBLIC field_t *ls_field_children_get(field_t *field, gint32 index);

    WS_DLL_PUBLIC void ls_field_children_set(field_t *field, field_t *child, gint32 index);

    WS_DLL_PUBLIC void ls_field_children_add(field_t *field, field_t *child);

    WS_DLL_PUBLIC void ls_field_children_remove(field_t *field, gint32 index);

    WS_DLL_PUBLIC gint32 ls_field_type_int8(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int16(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int24(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int32(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int40(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int48(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int56(void);

    WS_DLL_PUBLIC gint32 ls_field_type_int64(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint8(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint16(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint24(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint32(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint40(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint48(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint56(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint64(void);

    WS_DLL_PUBLIC gint32 ls_field_type_none(void);

    WS_DLL_PUBLIC gint32 ls_field_type_protocol(void);

    WS_DLL_PUBLIC gint32 ls_field_type_boolean(void);

    WS_DLL_PUBLIC gint32 ls_field_type_char(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ieee_11073_float16(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ieee_11073_float32(void);

    WS_DLL_PUBLIC gint32 ls_field_type_float(void);

    WS_DLL_PUBLIC gint32 ls_field_type_double(void);

    WS_DLL_PUBLIC gint32 ls_field_type_absolute_time(void);

    WS_DLL_PUBLIC gint32 ls_field_type_relative_time(void);

    WS_DLL_PUBLIC gint32 ls_field_type_string(void);

    WS_DLL_PUBLIC gint32 ls_field_type_stringz(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint_string(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ether(void);

    WS_DLL_PUBLIC gint32 ls_field_type_bytes(void);

    WS_DLL_PUBLIC gint32 ls_field_type_uint_bytes(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ipv4(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ipv6(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ipxnet(void);

    WS_DLL_PUBLIC gint32 ls_field_type_framenum(void);

    WS_DLL_PUBLIC gint32 ls_field_type_guid(void);

    WS_DLL_PUBLIC gint32 ls_field_type_oid(void);

    WS_DLL_PUBLIC gint32 ls_field_type_eui64(void);

    WS_DLL_PUBLIC gint32 ls_field_type_ax25(void);

    WS_DLL_PUBLIC gint32 ls_field_type_vines(void);

    WS_DLL_PUBLIC gint32 ls_field_type_rel_oid(void);

    WS_DLL_PUBLIC gint32 ls_field_type_system_id(void);

    WS_DLL_PUBLIC gint32 ls_field_type_stringzpad(void);

    WS_DLL_PUBLIC gint32 ls_field_type_fcwwn(void);

    WS_DLL_PUBLIC gint32 ls_field_type_stringztrunc(void);

    WS_DLL_PUBLIC gint32 ls_field_type_num_types(void);

    WS_DLL_PUBLIC gint32 ls_field_type_scalar(void);

    WS_DLL_PUBLIC gint32 ls_field_encoding_na(void);

    WS_DLL_PUBLIC gint32 ls_field_encoding_big_endian(void);

    WS_DLL_PUBLIC gint32 ls_field_encoding_little_endian(void);

    WS_DLL_PUBLIC gint32 ls_field_type_is_int64(gint32 field_type);

    WS_DLL_PUBLIC gint32 ls_field_type_is_uint64(gint32 field_type);

    WS_DLL_PUBLIC gint32 ls_field_type_is_double(gint32 field_type);

    WS_DLL_PUBLIC gint32 ls_field_type_is_string(gint32 field_type);

    WS_DLL_PUBLIC gint32 ls_field_type_is_bytes(gint32 field_type);

    WS_DLL_PUBLIC const char *ls_field_type_get_name(gint32 field_type);

    WS_DLL_PUBLIC const char *ls_field_get_name(gint32 field_id);

    WS_DLL_PUBLIC const char *ls_field_get_display_name(gint32 field_id);

#ifdef __cplusplus
}
#endif

#endif // __LS_FIELD__