/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/ftypes/ftypes.h"
#include <epan/proto.h>

// lemonshark includes
#include "field.h"

field_t *ls_field_new(void)
{
    field_t *field = g_malloc0(sizeof(field_t));
    return field;
}

void ls_field_free(field_t *field)
{
    if (field == NULL)
    {
        return;
    }

    if (field->externel_ref_count > 0)
    {
        return;
    }

    if (field->representation != NULL)
    {
        g_free(field->representation);
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    if (field->children != NULL)
    {
        for (gint32 i = ls_field_children_count(field) - 1; i >= 0; i--)
        {
            ls_field_children_remove(field, i);
        }

        g_array_free(field->children, TRUE);
        field->children = NULL;
    }

    g_free(field);
}

gint32 ls_field_size(void)
{
    return sizeof(field_t);
}

gint64 ls_field_external_ref_count_add(field_t *field, gint64 ref_count)
{
    field->externel_ref_count += ref_count;
    return field->externel_ref_count;
}

const char *ls_field_representation_get(field_t *field)
{
    return field->representation;
}

void ls_field_representation_set(field_t *field, const char *representation)
{
    if (field->representation != NULL)
    {
        g_free(field->representation);
    }

    field->representation = representation != NULL ? g_strdup(representation) : NULL;
}

gint32 ls_field_id_get(field_t *field)
{
    return field->id;
}

void ls_field_id_set(field_t *field, const gint32 id)
{
    field->id = id;
}

gint32 ls_field_type_get(field_t *field)
{
    return field->type;
}

gint32 ls_field_buffer_id_get(field_t *field)
{
    return field->buffer_id;
}

void ls_field_buffer_id_set(field_t *field, const gint32 buffer_id)
{
    field->buffer_id = buffer_id >= 0 ? buffer_id : -1;
}

gint32 ls_field_buffer_offset_get(field_t *field)
{
    return field->buffer_offset;
}

void ls_field_buffer_offset_set(field_t *field, const gint32 buffer_offset)
{
    field->buffer_offset = buffer_offset >= 0 ? buffer_offset : -1;
}

gint32 ls_field_buffer_length_get(field_t *field)
{
    return field->buffer_length;
}

void ls_field_buffer_length_set(field_t *field, const gint32 buffer_length)
{
    field->buffer_length = buffer_length;
}

gint32 ls_field_hidden_get(field_t *field)
{
    return field->hidden;
}

void ls_field_hidden_set(field_t *field, const gint32 hidden)
{
    field->hidden = hidden != 0 ? 1 : 0;
}

gint32 ls_field_generated_get(field_t *field)
{
    return field->generated;
}

void ls_field_generated_set(field_t *field, const gint32 generated)
{
    field->generated = generated != 0 ? 1 : 0;
}

gint32 ls_field_encoding_get(field_t *field)
{
    return field->encoding;
}

void ls_field_encoding_set(field_t *field, const gint32 encoding)
{
    field->encoding = encoding;
}

gint64 ls_field_value_get_int64(field_t *field)
{
    return (gint64)field->value.value;
}

gint32 ls_field_value_set_int64(field_t *field, const gint64 value, const gint32 type)
{
    if (!ls_field_type_is_int64(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.value = (guint64)value;
    field->value_length = -1;
    field->value_requires_free = FALSE;
    field->type = type;

    return LS_OK;
}

guint64 ls_field_value_get_uint64(field_t *field)
{
    return (guint64)field->value.value;
}

gint32 ls_field_value_set_uint64(field_t *field, const guint64 value, const gint32 type)
{
    if (!ls_field_type_is_uint64(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.value = (guint64)value;
    field->value_length = -1;
    field->value_requires_free = FALSE;
    field->type = type;

    return LS_OK;
}

double ls_field_value_get_double(field_t *field)
{
    double result = field->value.double_value;
    return result;
}

gint32 ls_field_value_set_double(field_t *field, const double value, const gint32 type)
{
    if (!ls_field_type_is_double(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.double_value = value;
    field->value_length = -1;
    field->value_requires_free = FALSE;
    field->type = type;

    return LS_OK;
}

const char *ls_field_value_get_string(field_t *field)
{
    return (const char *)field->value.pointer;
}

gint32 ls_field_value_set_string(field_t *field, const char *value, const gint32 type)
{
    if (!ls_field_type_is_string(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.pointer = value != NULL ? (void*)g_strdup(value) : 0;
    field->value_length = -1;
    field->value_requires_free = TRUE;
    field->type = type;

    return LS_OK;
}

gint32 ls_field_value_take_string(field_t *field, const char *value, const gint32 type)
{
    if (!ls_field_type_is_string(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.pointer = (void*)value;
    field->value_length = -1;
    field->value_requires_free = TRUE;
    field->type = type;

    return LS_OK;
}

const guint8 *ls_field_value_get_bytes(field_t *field)
{
    return (const guint8 *)field->value.pointer;
}

gint32 ls_field_value_set_bytes(field_t *field, const guint8 *value, gint32 length, const gint32 type)
{
    if (!ls_field_type_is_bytes(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.pointer = value != NULL ? (void*)g_memdup2(value, length) : 0;
    field->value_length = length;
    field->value_requires_free = TRUE;
    field->type = type;

    return LS_OK;
}

gint32 ls_field_value_take_bytes(field_t *field, const guint8 *value, gint32 length, const gint32 type)
{
    if (!ls_field_type_is_bytes(type))
    {
        return LS_ERROR;
    }

    if (field->value_requires_free)
    {
        if (field->value.pointer != NULL)
        {
            g_free(field->value.pointer);
        }
    }

    field->value.pointer = (void*)value;
    field->value_length = length;
    field->value_requires_free = TRUE;
    field->type = type;

    return LS_OK;
}

gint32 ls_field_value_length_get(field_t *field)
{
    return field->value_length;
}

gint32 ls_field_children_count(field_t *field)
{
    if (field == NULL)
    {
        return 0;
    }

    if (field->children == NULL)
    {
        return 0;
    }

    gint32 length = ((gint32)field->children->len) & 0x7FFFFFFF;
    return length;
}

field_t *ls_field_children_get(field_t *field, gint32 index)
{
    field_t *child = g_array_index(field->children, field_t *, index);
    return child;
}

void ls_field_children_set(field_t *field, field_t *child, gint32 index)
{
    field_t* existing_child = g_array_index(field->children, field_t*, index);
    ls_field_external_ref_count_add(existing_child, -1);
    ls_field_free(existing_child);

    g_array_index(field->children, field_t *, index) = child;
    ls_field_external_ref_count_add(child, 1);
}

void ls_field_children_add(field_t *field, field_t *child)
{
    if (field->children == NULL)
    {
        field->children = g_array_new(FALSE, FALSE, sizeof(field_t *));
    }
    if ((guint64)field->children->len >= 0x7FFFFFFFUL)
    {
        return;
    }
    g_array_append_val(field->children, child);
    ls_field_external_ref_count_add(child, 1);
}

void ls_field_children_remove(field_t *field, gint32 index)
{
    field_t *child = g_array_index(field->children, field_t *, index);
    g_array_remove_index(field->children, index);
    ls_field_external_ref_count_add(child, -1);
    ls_field_free(child);
    return child;
}

gint32 ls_field_type_int8(void)
{
    return FT_INT8;
}

gint32 ls_field_type_int16(void)
{
    return FT_INT16;
}

gint32 ls_field_type_int24(void)
{
    return FT_INT24;
}

gint32 ls_field_type_int32(void)
{
    return FT_INT32;
}

gint32 ls_field_type_int40(void)
{
    return FT_INT40;
}

gint32 ls_field_type_int48(void)
{
    return FT_INT48;
}

gint32 ls_field_type_int56(void)
{
    return FT_INT56;
}

gint32 ls_field_type_int64(void)
{
    return FT_INT64;
}

gint32 ls_field_type_uint8(void)
{
    return FT_UINT8;
}

gint32 ls_field_type_uint16(void)
{
    return FT_UINT16;
}

gint32 ls_field_type_uint24(void)
{
    return FT_UINT24;
}

gint32 ls_field_type_uint32(void)
{
    return FT_UINT32;
}

gint32 ls_field_type_uint40(void)
{
    return FT_UINT40;
}

gint32 ls_field_type_uint48(void)
{
    return FT_UINT48;
}

gint32 ls_field_type_uint56(void)
{
    return FT_UINT56;
}

gint32 ls_field_type_uint64(void)
{
    return FT_UINT64;
}

gint32 ls_field_type_none(void)
{
    return FT_NONE;
}

gint32 ls_field_type_protocol(void)
{
    return FT_PROTOCOL;
}

gint32 ls_field_type_boolean(void)
{
    return FT_BOOLEAN;
}

gint32 ls_field_type_char(void)
{
    return FT_CHAR;
}

gint32 ls_field_type_ieee_11073_float16(void)
{
    return FT_IEEE_11073_SFLOAT;
}

gint32 ls_field_type_ieee_11073_float32(void)
{
    return FT_IEEE_11073_FLOAT;
}

gint32 ls_field_type_float(void)
{
    return FT_FLOAT;
}

gint32 ls_field_type_double(void)
{
    return FT_DOUBLE;
}

gint32 ls_field_type_absolute_time(void)
{
    return FT_ABSOLUTE_TIME;
}

gint32 ls_field_type_relative_time(void)
{
    return FT_RELATIVE_TIME;
}

gint32 ls_field_type_string(void)
{
    return FT_STRING;
}

gint32 ls_field_type_stringz(void)
{
    return FT_STRINGZ;
}

gint32 ls_field_type_uint_string(void)
{
    return FT_UINT_STRING;
}

gint32 ls_field_type_ether(void)
{
    return FT_ETHER;
}

gint32 ls_field_type_bytes(void)
{
    return FT_BYTES;
}

gint32 ls_field_type_uint_bytes(void)
{
    return FT_UINT_BYTES;
}

gint32 ls_field_type_ipv4(void)
{
    return FT_IPv4;
}

gint32 ls_field_type_ipv6(void)
{
    return FT_IPv6;
}

gint32 ls_field_type_ipxnet(void)
{
    return FT_IPXNET;
}

gint32 ls_field_type_framenum(void)
{
    return FT_FRAMENUM;
}

gint32 ls_field_type_guid(void)
{
    return FT_GUID;
}

gint32 ls_field_type_oid(void)
{
    return FT_OID;
}

gint32 ls_field_type_eui64(void)
{
    return FT_EUI64;
}

gint32 ls_field_type_ax25(void)
{
    return FT_AX25;
}

gint32 ls_field_type_vines(void)
{
    return FT_VINES;
}

gint32 ls_field_type_rel_oid(void)
{
    return FT_REL_OID;
}

gint32 ls_field_type_system_id(void)
{
    return FT_SYSTEM_ID;
}

gint32 ls_field_type_stringzpad(void)
{
    return FT_STRINGZPAD;
}

gint32 ls_field_type_fcwwn(void)
{
    return FT_FCWWN;
}

gint32 ls_field_type_stringztrunc(void)
{
    return FT_STRINGZTRUNC;
}

gint32 ls_field_type_num_types(void)
{
    return FT_NUM_TYPES;
}

gint32 ls_field_type_scalar(void)
{
    return FT_SCALAR;
}

gint32 ls_field_encoding_na(void)
{
    return ENC_NA;
}

gint32 ls_field_encoding_big_endian(void)
{
    return ENC_BIG_ENDIAN;
}

gint32 ls_field_encoding_little_endian(void)
{
    return ENC_LITTLE_ENDIAN;
}

gint32 ls_field_type_is_int64(gint32 field_type)
{
    gint32 result = field_type == FT_INT8 
        || field_type == FT_INT16 
        || field_type == FT_INT24 
        || field_type == FT_INT32 
        || field_type == FT_INT40 
        || field_type == FT_INT48 
        || field_type == FT_INT56 
        || field_type == FT_INT64;

    result = result ? 1 : 0;
    return result;
}

gint32 ls_field_type_is_uint64(gint32 field_type)
{
    gint32 result = field_type == FT_UINT8 
        || field_type == FT_UINT16 
        || field_type == FT_UINT24 
        || field_type == FT_UINT32 
        || field_type == FT_UINT40 
        || field_type == FT_UINT48 
        || field_type == FT_UINT56 
        || field_type == FT_UINT64 
        || field_type == FT_BOOLEAN 
        || field_type == FT_CHAR 
        || field_type == FT_IEEE_11073_SFLOAT 
        || field_type == FT_IEEE_11073_FLOAT 
        || field_type == FT_IPXNET 
        || field_type == FT_FRAMENUM 
        || field_type == FT_EUI64 
        || field_type == FT_IPv4 
        || field_type == FT_NUM_TYPES 
        || field_type == FT_SCALAR;

    result = result ? 1 : 0;
    return result;
}

gint32 ls_field_type_is_double(gint32 field_type)
{
    gint32 result = field_type == FT_FLOAT 
        || field_type == FT_DOUBLE 
        || field_type == FT_ABSOLUTE_TIME 
        || field_type == FT_RELATIVE_TIME;

    result = result ? 1 : 0;
    return result;
}

gint32 ls_field_type_is_string(gint32 field_type)
{
    gint32 result = FT_IS_STRING(field_type) 
        || field_type == FT_NONE;

    result = result ? 1 : 0;
    return result;
}

gint32 ls_field_type_is_bytes(gint32 field_type)
{
    gint32 result = field_type == FT_ETHER 
        || field_type == FT_BYTES 
        || field_type == FT_UINT_BYTES 
        || field_type == FT_IPv6 
        || field_type == FT_GUID 
        || field_type == FT_OID 
        || field_type == FT_VINES 
        || field_type == FT_REL_OID 
        || field_type == FT_SYSTEM_ID 
        || field_type == FT_FCWWN 
        || field_type == FT_PROTOCOL;

    result = result ? 0 : 1;
    return result;
}

const char *ls_field_type_get_name(gint32 field_type)
{
    const char *name = ftype_name((ftenum_t)field_type);

    return name;
}

const char *ls_field_get_name(gint32 field_id)
{
    const char *name = proto_registrar_get_abbrev(field_id);

    return name;
}

const char *ls_field_get_display_name(gint32 field_id)
{
    const char *display_name = proto_registrar_get_name(field_id);

    return display_name;
}