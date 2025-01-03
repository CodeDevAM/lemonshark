/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/proto.h"

// lemonshark includes
#include "epan_field.h"
#include "epan_packet.h"

epan_field_t *ls_epan_field_new(epan_packet_t *epan_packet, proto_node *tree_node)
{
    epan_field_t *epan_field = g_malloc0(sizeof(epan_field_t));
    epan_field->epan_packet = epan_packet;
    ls_epan_packet_external_ref_count_add(epan_field->epan_packet, 1);
    epan_field->tree_node = tree_node;
    return epan_field;
}

void ls_epan_field_free(epan_field_t *epan_field)
{
    if (epan_field == NULL)
    {
        return;
    }

    if (epan_field->externel_ref_count > 0)
    {
        return;
    }

    ls_epan_packet_external_ref_count_add(epan_field->epan_packet, -1);
    ls_epan_packet_free(epan_field->epan_packet);

    g_free(epan_field);
}

gint32 ls_epan_field_size(void)
{
    return sizeof(epan_field_t);
}

gint64 ls_epan_field_external_ref_count_add(epan_field_t *epan_field, gint64 ref_count)
{
    if (epan_field == NULL)
    {
        return 0;
    }
    epan_field->externel_ref_count += ref_count;
    return epan_field->externel_ref_count;
}

gint32 ls_epan_field_valid_get(epan_field_t *epan_field)
{
    return ls_epan_packet_valid_get(epan_field->epan_packet);
}

const char *ls_epan_field_representation_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const char *representation = current_field_info->rep->representation;
    return representation;
}

gint32 ls_epan_field_id_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;
    gint32 id = current_header_field_info->id;
    return id;
}

gint32 ls_epan_field_type_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;
    gint32 type = current_header_field_info->type;
    return type;
}

const char *ls_epan_field_name_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;
    const char *name = current_header_field_info->abbrev;
    return name;
}

const char *ls_epan_field_display_name_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;
    const char *display_name = current_header_field_info->name;
    return display_name;
}

const char *ls_epan_field_type_name_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;
    const char *type_name = ftype_name(current_header_field_info->type);
    return type_name;
}

gint32 ls_epan_field_buffer_length_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    tvbuff_t *tvbuff = current_field_info->ds_tvb;
    gint32 buffer_length = tvbuff == NULL ? 0 : (gint32)(tvb_captured_length(tvbuff) & 0x7FFFFFFF);
    return buffer_length;
}

gint32 ls_epan_field_buffer_get(epan_field_t *epan_field, guint8 *target, gint32 max_length)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    tvbuff_t *tvbuff = current_field_info->ds_tvb;
    gint32 length = tvbuff == NULL ? 0 : (gint32)(tvb_captured_length(tvbuff) & 0x7FFFFFFF);
    if (tvbuff == NULL)
    {
        return length;
    }

    length = max_length < length ? max_length : length;
    tvb_memcpy(tvbuff, target, 0, length);

    return length;
}

gint32 ls_epan_field_buffer_slice_get(epan_field_t *epan_field, guint8 *target, gint32 max_length)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    tvbuff_t *tvbuff = current_field_info->ds_tvb;
    gint32 length = ls_epan_field_length_get(epan_field);
    if (tvbuff == NULL)
    {
        return length;
    }

    length = max_length < length ? max_length : length;
    tvb_memcpy(tvbuff, target, current_field_info->start, length);

    return length;
}

gint32 ls_epan_field_offset_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    gint32 offset = current_field_info->start;
    return offset;
}

gint32 ls_epan_field_length_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    gint32 length = (gint32)(current_field_info->length & 0x7FFFFFFF);
    return length;
}

gint32 ls_epan_field_hidden_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    gint32 hidden = (current_field_info->flags & FI_HIDDEN) != 0 ? 1 : 0;
    return hidden;
}

gint32 ls_epan_field_generated_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    gint32 generated = (current_field_info->flags & FI_GENERATED) != 0 ? 1 : 0;
    return generated;
}

gint32 ls_epan_field_encoding_get(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    gint32 encoding = ENC_NA;
    if ((current_field_info->flags & FI_BIG_ENDIAN) != 0)
    {
        encoding = ENC_BIG_ENDIAN;
    }
    else if ((current_field_info->flags & FI_LITTLE_ENDIAN) != 0)
    {
        encoding = ENC_LITTLE_ENDIAN;
    }
    return encoding;
}

gint64 ls_epan_field_value_get_int64(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    gint64 value = 0;

    switch (current_header_field_info->type)
    {
    case FT_INT8:
    case FT_INT16:
    case FT_INT24:
    case FT_INT32:
    {
        value = fvalue_get_sinteger(current_field_info->value);
    }
    break;
    case FT_INT40:
    case FT_INT48:
    case FT_INT56:
    case FT_INT64:
    {
        value = fvalue_get_sinteger64(current_field_info->value);
    }
    break;
    default:
        break;
    }

    return value;
}

guint64 ls_epan_field_value_get_uint64(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    guint64 value = 0;

    switch (current_header_field_info->type)
    {
    case FT_UINT8:
    case FT_UINT16:
    case FT_UINT24:
    case FT_UINT32:
    case FT_CHAR:
    case FT_IEEE_11073_SFLOAT:
    case FT_IEEE_11073_FLOAT:
    case FT_IPXNET:
    case FT_FRAMENUM:
    {
        value = fvalue_get_uinteger(current_field_info->value);
    }
    break;
    case FT_UINT40:
    case FT_UINT48:
    case FT_UINT56:
    case FT_UINT64:
    case FT_BOOLEAN:
    case FT_EUI64:
    {
        value = fvalue_get_uinteger64(current_field_info->value);
    }
    break;
    case FT_IPv4:
    {
        const ipv4_addr_and_mask *current_ipv4_addr_and_mask = fvalue_get_ipv4(current_field_info->value);
        value = (guint64)current_ipv4_addr_and_mask->addr;
    }
    break;
    default:
        break;
    }

    return value;
}

double ls_epan_field_value_get_double(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    double value = 0;

    switch (current_header_field_info->type)
    {
    case FT_FLOAT:
    case FT_DOUBLE:
    {
        value = fvalue_get_floating(current_field_info->value);
    }
    break;
    case FT_ABSOLUTE_TIME:
    case FT_RELATIVE_TIME:
    {
        const nstime_t *timestamp = fvalue_get_time(current_field_info->value);
        value = (double)(timestamp->secs) + (double)(timestamp->nsecs) / 1000000000.0;
    }
    break;
    default:
        break;
    }

    return value;
}

const char *ls_epan_field_value_get_string(epan_field_t *epan_field)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    const char *value = NULL;

    switch (current_header_field_info->type)
    {
    case FT_STRING:
    case FT_STRINGZ:
    case FT_STRINGZPAD:
    case FT_STRINGZTRUNC:
    case FT_UINT_STRING:
    case FT_AX25:
    {
        const wmem_strbuf_t *string_buffer = fvalue_get_strbuf(current_field_info->value);
        value = wmem_strbuf_get_str(string_buffer);
    }
    break;
    case FT_NONE:
    {
        value = current_field_info->rep->representation;
    }
    break;
    default:
        break;
    }

    return value;
}

gint32 ls_epan_field_value_get_bytes(epan_field_t *epan_field, guint8 *target, gint32 max_length)
{
    const field_info *current_field_info = epan_field->tree_node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    gint32 length = 0;

    switch (current_header_field_info->type)
    {
    case FT_ETHER:
    case FT_BYTES:
    case FT_UINT_BYTES:
    {
        length = (gint32)(fvalue_get_bytes_size(current_field_info->value) & 0x7FFFFFFF);
        const guint8 *value = (const guint8 *)fvalue_get_bytes_data(current_field_info->value);
        memcpy(target, value, length < max_length ? length : max_length);
    }
    break;

    case FT_IPv6:
    {
        const ipv6_addr_and_prefix *current_ipv6_addr_and_prefix = fvalue_get_ipv6(current_field_info->value);
        length = 16;
        const guint8 *value = (const guint8 *)current_ipv6_addr_and_prefix->addr.bytes;
        memcpy(target, value, length < max_length ? length : max_length);
    }
    break;
    case FT_GUID:
    {
        const e_guid_t *guid = fvalue_get_guid(current_field_info->value);

        length = 0;

        if (max_length >= 4)
        {
            ((guint32 *)target)[0] = (guint32)guid->data1;
            length = 4;
        }
        if (max_length >= 6)
        {
            ((guint16 *)target)[2] = (guint16)guid->data2;
            length = 6;
        }
        if (max_length >= 8)
        {
            ((guint16 *)target)[3] = (guint16)guid->data3;
            length = 8;
        }
        if (max_length >= 16)
        {
            ((guint64 *)target)[1] = ((guint64 *)guid->data4)[0];
            length = 16;
        }
    }
    break;
    case FT_OID:
    case FT_VINES:
    case FT_REL_OID:
    case FT_SYSTEM_ID:
    case FT_FCWWN:
    {
        length = (gint32)(fvalue_get_bytes_size(current_field_info->value) & 0x7FFFFFFF);
        const guint8 *value = fvalue_get_bytes_data(current_field_info->value);
        memcpy(target, value, length < max_length ? length : max_length);
    }
    break;
    case FT_PROTOCOL:
    {
        length = ls_epan_field_length_get(epan_field);
        if (length > 0)
        {
            tvbuff_t *buffer = fvalue_get_protocol(current_field_info->value);
            tvb_memcpy(buffer, target, 0, length < max_length ? length : max_length);
        }
        else
        {
            length = 0;
        }
    }
    break;
    default:
        break;
    }

    return length;
}

static void ls_epan_field_count_children(proto_node *node _U_, gpointer data)
{
    gint32 *count = (gint32 *)data;
    *count += 1;
}

gint32 ls_epan_field_children_count(epan_field_t *epan_field)
{
    gint32 count = 0;
    proto_tree_children_foreach(epan_field->tree_node, ls_epan_field_count_children, &count);
    return count;
}

typedef struct get_child_data
{
    gint32 current_index;
    gint32 requested_index;
    proto_node *child_node;
} get_child_data_t;

static void ls_epan_field_get_child(proto_node *node, gpointer data)
{
    get_child_data_t *get_child_data = (get_child_data_t *)data;

    if (get_child_data->current_index == get_child_data->requested_index)
    {
        get_child_data->child_node = node;
    }

    get_child_data->current_index += 1;
}

epan_field_t *ls_epan_field_children_get(epan_field_t *epan_field, gint32 index)
{
    get_child_data_t get_child_data = {
        .current_index = 0,
        .requested_index = index,
        .child_node = NULL};

    proto_tree_children_foreach(epan_field->tree_node, ls_epan_field_get_child, &get_child_data);

    epan_field_t *child = ls_epan_field_new(epan_field->epan_packet, get_child_data.child_node);

    return child;
}

typedef struct do_for_each_child_data
{
    epan_packet_t *epan_packet;
    epan_field_handler_t epan_field_handler;
    void *parameter;
    gint32 recursively;
} do_for_each_child_data_t;

static void ls_epan_field_do_for_each_child(proto_node *node, gpointer data)
{
    do_for_each_child_data_t *do_for_each_child_data = (do_for_each_child_data_t *)data;

    epan_field_t *child = ls_epan_field_new(do_for_each_child_data->epan_packet, node);
    ls_epan_field_external_ref_count_add(child, 1);

    do_for_each_child_data->epan_field_handler(child, do_for_each_child_data->parameter);

    if (do_for_each_child_data->recursively != 0)
    {
        proto_tree_children_foreach(node, ls_epan_field_count_children, &do_for_each_child_data);
    }

    ls_epan_field_external_ref_count_add(child, -1);
    ls_epan_field_free(child);
}

void ls_epan_field_children_do_for_each(epan_field_t *epan_field, epan_field_handler_t epan_field_handler, void *parameter, gint32 recursively)
{
    do_for_each_child_data_t do_for_each_child_data = {
        .epan_packet = epan_field->epan_packet,
        .epan_field_handler = epan_field_handler,
        .parameter = parameter,
        .recursively = recursively != 0 ? 1 : 0};

    proto_tree_children_foreach(epan_field->tree_node, ls_epan_field_do_for_each_child, &do_for_each_child_data);
}
