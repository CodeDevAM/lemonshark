/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_SESSION__
#define __LS_SESSION__

#ifdef __cplusplus
extern "C"
{
#endif

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

// lemonshark includes
#include "ls_common.h"
#include "packet.h"
#include "epan_packet.h"

    WS_DLL_PUBLIC gint32 ls_session_create_from_file(const char *file_path, const char *read_filter, const char *profile, char **error_message);

    WS_DLL_PUBLIC gint32 ls_session_get_next_packet_id(char **error_message);

    WS_DLL_PUBLIC packet_t *ls_session_get_packet(gint32 packet_id, const gint32 include_buffers, const gint32 include_columns, const gint32 include_representations, const gint32 include_strings, const gint32 include_bytes, const gint32 *requested_field_ids, const gint32 requested_field_id_count, char **error_message);

    WS_DLL_PUBLIC epan_packet_t *ls_session_get_epan_packet(gint32 packet_id, const gint32 include_columns, const gint32 *requested_field_ids, const gint32 requested_field_id_count, char **error_message);

    WS_DLL_PUBLIC void ls_session_close(void);

#ifdef __cplusplus
}
#endif

#endif // __LS_SESSION__