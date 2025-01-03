/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_EPAN_TYPES__
#define __LS_EPAN_TYPES__
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"
#include "epan/epan_dissect.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct epan_packet
    {
        epan_dissect_t *epan_dissect;
        gint64 externel_ref_count;
    } epan_packet_t;

    typedef struct epan_field
    {
        epan_packet_t *epan_packet;
        proto_node *tree_node;
        gint64 externel_ref_count;
    } epan_field_t;

    typedef void (*epan_field_handler_t)(epan_field_t *epan_field, void *parameter);

#ifdef __cplusplus
}
#endif

#endif // __LS_EPAN_TYPES__