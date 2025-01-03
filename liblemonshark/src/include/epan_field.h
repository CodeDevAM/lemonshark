/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_EPAN_FIELD__
#define __LS_EPAN_FIELD__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

// lemonshark includes
#include "epan_types.h"

#ifdef __cplusplus
extern "C"
{
#endif

    epan_field_t *ls_epan_field_new(epan_packet_t *epan_packet, proto_node *tree_node);

    WS_DLL_PUBLIC void ls_epan_field_free(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_size(void);

    WS_DLL_PUBLIC gint64 ls_epan_field_external_ref_count_add(epan_field_t *epan_field, gint64 ref_count);

    WS_DLL_PUBLIC gint32 ls_epan_field_valid_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC const char *ls_epan_field_representation_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_id_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_type_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC const char *ls_epan_field_name_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC const char *ls_epan_field_display_name_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC const char *ls_epan_field_type_name_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_buffer_length_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_buffer_get(epan_field_t *epan_field, guint8 *target, gint32 max_length);

    WS_DLL_PUBLIC gint32 ls_epan_field_buffer_slice_get(epan_field_t *epan_field, guint8 *target, gint32 max_length);

    WS_DLL_PUBLIC gint32 ls_epan_field_offset_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_length_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_hidden_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_generated_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_encoding_get(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint64 ls_epan_field_value_get_int64(epan_field_t *epan_field);

    WS_DLL_PUBLIC guint64 ls_epan_field_value_get_uint64(epan_field_t *epan_field);

    WS_DLL_PUBLIC double ls_epan_field_value_get_double(epan_field_t *epan_field);

    WS_DLL_PUBLIC const char *ls_epan_field_value_get_string(epan_field_t *epan_field);

    WS_DLL_PUBLIC gint32 ls_epan_field_value_get_bytes(epan_field_t *epan_field, guint8 *target, gint32 max_length);

    WS_DLL_PUBLIC gint32 ls_epan_field_children_count(epan_field_t *epan_field);

    WS_DLL_PUBLIC epan_field_t *ls_epan_field_children_get(epan_field_t *epan_field, gint32 index);

    WS_DLL_PUBLIC void ls_epan_field_children_do_for_each(epan_field_t *epan_field, epan_field_handler_t epan_field_handler, void *parameter, gint32 recursively);

#ifdef __cplusplus
}
#endif

#endif // __LS_EPAN_FIELD__