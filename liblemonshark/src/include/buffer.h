/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_BUFFER__
#define __LS_BUFFER__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct buffer
    {
        gint64 externel_ref_count;
        guint8 *data;
        gint32 length;
    } buffer_t;

    WS_DLL_PUBLIC buffer_t *ls_buffer_new(void);

    WS_DLL_PUBLIC void ls_buffer_free(buffer_t *buffer);

    WS_DLL_PUBLIC gint32 ls_buffer_size(void);

    WS_DLL_PUBLIC gint64 ls_buffer_external_ref_count_add(buffer_t *buffer, gint64 ref_count);

    WS_DLL_PUBLIC guint8 *ls_buffer_data_get(buffer_t *buffer);

    WS_DLL_PUBLIC void ls_buffer_data_set(buffer_t *buffer, guint8 *data, gint32 length);

    WS_DLL_PUBLIC void ls_buffer_data_take(buffer_t *buffer, guint8 *data, gint32 length);

    WS_DLL_PUBLIC gint32 ls_buffer_length_get(buffer_t *buffer);

#ifdef __cplusplus
}
#endif

#endif // __LS_BUFFER__