/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_EPAN_PACKET__
#define __LS_EPAN_PACKET__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"
#include "epan/epan_dissect.h"

// lemonshark includes
#include "epan_types.h"

#ifdef __cplusplus
extern "C"
{
#endif

    WS_DLL_PUBLIC epan_packet_t *ls_epan_packet_new(void);

    WS_DLL_PUBLIC void ls_epan_packet_free(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_size(void);

    WS_DLL_PUBLIC gint64 ls_epan_packet_external_ref_count_add(epan_packet_t *epan_packet, gint64 ref_count);

    WS_DLL_PUBLIC gint32 ls_epan_packet_valid_get(epan_packet_t *epan_packet);

    void ls_epan_packet_invalidate(epan_packet_t *epan_packet);

    void ls_epan_packet_init(epan_packet_t *epan_packet, epan_dissect_t *epan_dissect);

    WS_DLL_PUBLIC gint32 ls_epan_packet_id_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint64 ls_epan_packet_timestamp_seconds_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_timestamp_nanoseconds_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_length_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_interface_id_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC epan_field_t *ls_epan_packet_root_field_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC const char *ls_epan_packet_protocol_column_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC void ls_epan_packet_protocol_column_set(epan_packet_t *epan_packet, const char *protocol_column);

    WS_DLL_PUBLIC const char *ls_epan_packet_info_column_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC void ls_epan_packet_info_column_set(epan_packet_t *epan_packet, const char *info_column);

    WS_DLL_PUBLIC gint32 ls_epan_packet_visited_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_visible_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_ignored_get(epan_packet_t *epan_packet);

    WS_DLL_PUBLIC gint32 ls_epan_packet_buffer_get(epan_packet_t *epan_packet, guint8 *target, gint32 max_length);

#ifdef __cplusplus
}
#endif

#endif // __LS_EPAN_PACKET__