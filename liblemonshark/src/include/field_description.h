/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_FIELD_DESCRIPTION__
#define __LS_FIELD_DESCRIPTION__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct field_description
    {
        gint64 externel_ref_count;
        gint32 id;
        gint32 type;
        const char *name;
        const char *display_name;
        gint32 parent_id;
    } field_description_t;

    WS_DLL_PUBLIC field_description_t *ls_field_description_new(void);

    void ls_field_description_init(field_description_t *field_description, gint32 id, gint32 type, const char *name, const char *display_name, gint32 parent_id);

    void ls_field_description_init_from_header_field_info(field_description_t *field_description, header_field_info *current_header_field_info);

    WS_DLL_PUBLIC void ls_field_description_free(field_description_t *field_description);

    WS_DLL_PUBLIC gint32 ls_field_description_size(void);

    WS_DLL_PUBLIC gint64 ls_field_description_external_ref_count_add(field_description_t *field_description, gint64 ref_count);

    WS_DLL_PUBLIC gint32 ls_field_description_id_get(field_description_t *field_description);

    WS_DLL_PUBLIC gint32 ls_field_description_type_get(field_description_t *field_description);

    WS_DLL_PUBLIC const char *ls_field_description_name_get(field_description_t *field_description);

    WS_DLL_PUBLIC const char *ls_field_description_display_name_get(field_description_t *field_description);

    WS_DLL_PUBLIC gint32 ls_field_description_parent_id_get(field_description_t *field_description);

    WS_DLL_PUBLIC field_description_t *ls_field_description_get_by_id(gint32 id);

    WS_DLL_PUBLIC field_description_t *ls_field_description_get_by_name(const char *name);

    WS_DLL_PUBLIC field_description_t **ls_field_description_get_all(gint32 *count);

#ifdef __cplusplus
}
#endif

#endif // __LS_FIELD_DESCRIPTION__