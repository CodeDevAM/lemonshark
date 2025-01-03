/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/column.h"

// lemonshark includes
#include "epan_packet.h"
#include "epan_field.h"

epan_packet_t *ls_epan_packet_new(void)
{
    epan_packet_t *epan_packet = g_malloc0(sizeof(epan_packet_t));
    return epan_packet;
}

void ls_epan_packet_free(epan_packet_t *epan_packet)
{
    if (epan_packet == NULL)
    {
        return;
    }

    if (epan_packet->externel_ref_count > 0)
    {
        return;
    }

    g_free(epan_packet);
}

gint32 ls_epan_packet_size(void)
{
    return sizeof(epan_packet_t);
}

gint64 ls_epan_packet_external_ref_count_add(epan_packet_t *epan_packet, gint64 ref_count)
{
    if (epan_packet == NULL)
    {
        return 0;
    }
    epan_packet->externel_ref_count += ref_count;
    return epan_packet->externel_ref_count;
}

gint32 ls_epan_packet_valid_get(epan_packet_t *epan_packet)
{
    gint32 valid = (epan_packet->epan_dissect != NULL && epan_packet->externel_ref_count > 0) ? 1 : 0;
    return valid;
}

void ls_epan_packet_invalidate(epan_packet_t *epan_packet)
{
    epan_packet->epan_dissect = NULL;
}

void ls_epan_packet_init(epan_packet_t *epan_packet, epan_dissect_t *epan_dissect)
{
    epan_packet->epan_dissect = epan_dissect;
}

gint32 ls_epan_packet_id_get(epan_packet_t *epan_packet)
{
    gint32 packet_id = epan_packet->epan_dissect->pi.num;
    return packet_id;
}

gint64 ls_epan_packet_timestamp_seconds_get(epan_packet_t *epan_packet)
{
    gint64 timestamp_seconds = epan_packet->epan_dissect->pi.abs_ts.secs;
    return timestamp_seconds;
}

gint32 ls_epan_packet_timestamp_nanoseconds_get(epan_packet_t *epan_packet)
{
    gint32 timestamp_nanoseconds = epan_packet->epan_dissect->pi.abs_ts.nsecs;
    return timestamp_nanoseconds;
}

gint32 ls_epan_packet_length_get(epan_packet_t *epan_packet)
{
    gint32 length = (gint32)(tvb_captured_length(epan_packet->epan_dissect->tvb) & 0x7FFFFFFF);
    return length;
}

gint32 ls_epan_packet_interface_id_get(epan_packet_t *epan_packet)
{
    gint32 interface_id = epan_packet->epan_dissect->pi.link_number;
    return interface_id;
}

epan_field_t *ls_epan_packet_root_field_get(epan_packet_t *epan_packet)
{
    epan_field_t *root_field = ls_epan_field_new(epan_packet, epan_packet->epan_dissect->tree);

    return root_field;
}

const char *ls_epan_packet_protocol_column_get(epan_packet_t *epan_packet)
{
    if (epan_packet->epan_dissect->pi.cinfo == NULL)
    {
        return NULL;
    }

    const char *protocol_column = col_get_text(epan_packet->epan_dissect->pi.cinfo, COL_PROTOCOL);
    return protocol_column;
}

void ls_epan_packet_protocol_column_set(epan_packet_t *epan_packet, const char *protocol_column)
{
    if (epan_packet->epan_dissect->pi.cinfo == NULL)
    {
        return;
    }

    col_clear(epan_packet->epan_dissect->pi.cinfo, COL_PROTOCOL);
    col_add_str(epan_packet->epan_dissect->pi.cinfo, COL_PROTOCOL, protocol_column);
}

const char *ls_epan_packet_info_column_get(epan_packet_t *epan_packet)
{
    if (epan_packet->epan_dissect->pi.cinfo == NULL)
    {
        return NULL;
    }

    const char *info_column = col_get_text(epan_packet->epan_dissect->pi.cinfo, COL_INFO);
    return info_column;
}

void ls_epan_packet_info_column_set(epan_packet_t *epan_packet, const char *info_column)
{
    if (epan_packet->epan_dissect->pi.cinfo == NULL)
    {
        return;
    }

    col_clear(epan_packet->epan_dissect->pi.cinfo, COL_INFO);
    col_add_str(epan_packet->epan_dissect->pi.cinfo, COL_INFO, info_column);
}

gint32 ls_epan_packet_visited_get(epan_packet_t *epan_packet)
{
    gint32 visited = epan_packet->epan_dissect->pi.fd->visited;
    return visited;
}

gint32 ls_epan_packet_visible_get(epan_packet_t *epan_packet)
{
    gint32 visible = epan_packet->epan_dissect->tree != NULL && epan_packet->epan_dissect->tree->tree_data->visible != FALSE;
    return visible;
}

gint32 ls_epan_packet_ignored_get(epan_packet_t *epan_packet)
{
    gint32 ignored = epan_packet->epan_dissect->pi.fd->ignored;
    return ignored;
}

gint32 ls_epan_packet_buffer_get(epan_packet_t *epan_packet, guint8 *target, gint32 max_length)
{
    gint32 length = (gint32)(tvb_captured_length(epan_packet->epan_dissect->tvb) & 0x7FFFFFFF);
    length = max_length < length ? max_length : length;
    tvb_memcpy(epan_packet->epan_dissect->tvb, target, 0, length);

    return length;
}