/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_PACKET__
#define __LS_PACKET__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

// lemonshark includes
#include "field.h"
#include "buffer.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct packet
    {
        gint64 externel_ref_count;
        gint32 id;
        gint64 timestamp_seconds;
        gint32 timestamp_nanoseconds;
        gint32 length;
        gint32 interface_id;
        field_t *root_field;
        char *protocol_column;
        char *info_column;
        gint32 visited;
        gint32 visible;
        gint32 ignored;
        gint32 packet_buffer_id;
        GArray *buffers;
    } packet_t;

    WS_DLL_PUBLIC packet_t *ls_packet_new(void);

    WS_DLL_PUBLIC void ls_packet_free(packet_t *packet);

    WS_DLL_PUBLIC gint32 ls_packet_size(void);

    WS_DLL_PUBLIC gint64 ls_packet_external_ref_count_add(packet_t *packet, gint64 ref_count);

    WS_DLL_PUBLIC gint32 ls_packet_id_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_id_set(packet_t *packet, gint32 id);

    WS_DLL_PUBLIC gint64 ls_packet_timestamp_seconds_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_timestamp_seconds_set(packet_t *packet, gint64 timestamp_seconds);

    WS_DLL_PUBLIC gint32 ls_packet_timestamp_nanoseconds_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_timestamp_nanoseconds_set(packet_t *packet, gint32 timestamp_nanoseconds);

    WS_DLL_PUBLIC gint32 ls_packet_length_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_length_set(packet_t *packet, gint32 length);

    WS_DLL_PUBLIC gint32 ls_packet_interface_id_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_interface_id_set(packet_t *packet, gint32 interface_id);

    WS_DLL_PUBLIC field_t *ls_packet_root_field_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_root_field_set(packet_t *packet, field_t *root_field);

    WS_DLL_PUBLIC const char *ls_packet_protocol_column_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_protocol_column_set(packet_t *packet, const char *protocol_column);

    WS_DLL_PUBLIC void ls_packet_protocol_column_take(packet_t *packet, char *protocol_column);

    WS_DLL_PUBLIC const char *ls_packet_info_column_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_info_column_set(packet_t *packet, const char *info_column);

    WS_DLL_PUBLIC void ls_packet_info_column_take(packet_t *packet, char *info_column);

    WS_DLL_PUBLIC gint32 ls_packet_visited_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_visited_set(packet_t *packet, gint32 visited);

    WS_DLL_PUBLIC gint32 ls_packet_visible_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_visible_set(packet_t *packet, gint32 visible);

    WS_DLL_PUBLIC gint32 ls_packet_ignored_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_ignored_set(packet_t *packet, gint32 ignored);

    WS_DLL_PUBLIC gint32 ls_packet_packet_buffer_id_get(packet_t *packet);

    WS_DLL_PUBLIC void ls_packet_packet_buffer_id_set(packet_t *packet, gint32 packet_buffer_id);

    WS_DLL_PUBLIC gint32 ls_packet_buffers_count(packet_t *packet);

    WS_DLL_PUBLIC buffer_t *ls_packet_buffers_get(packet_t *packet, gint32 id);

    WS_DLL_PUBLIC void ls_packet_buffers_set(packet_t *packet, buffer_t *buffer, gint32 id);

    WS_DLL_PUBLIC void ls_packet_buffers_add(packet_t *packet, buffer_t *buffer);

    WS_DLL_PUBLIC buffer_t *ls_packet_buffers_remove(packet_t *packet, gint32 id);

#ifdef __cplusplus
}
#endif

#endif // __LS_PACKET__