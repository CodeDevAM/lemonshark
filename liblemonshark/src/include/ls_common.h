/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_COMMON__
#define __LS_COMMON__

#ifdef __cplusplus
extern "C"
{
#endif

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

#define LS_APP_NAME "liblemonshark"

#define LS_OK 1
#define LS_ERROR 0

    WS_DLL_PUBLIC gint32 ls_version_get_major(void);

    WS_DLL_PUBLIC gint32 ls_version_get_minor(void);

    WS_DLL_PUBLIC gint32 ls_version_get_patch(void);

    WS_DLL_PUBLIC gint32 ls_version_get_wireshark_major(void);

    WS_DLL_PUBLIC gint32 ls_version_get_wireshark_minor(void);

    WS_DLL_PUBLIC gint32 ls_version_get_wireshark_patch(void);

    WS_DLL_PUBLIC gint32 ls_version_get_target_wireshark_major(void);

    WS_DLL_PUBLIC gint32 ls_version_get_target_wireshark_minor(void);

    WS_DLL_PUBLIC gint32 ls_version_get_target_wireshark_patch(void);

    WS_DLL_PUBLIC void ls_memory_free(void *memory);

    WS_DLL_PUBLIC gint64 ls_string_length_get(const char *string);

    WS_DLL_PUBLIC gint32 ls_ok(void);

    WS_DLL_PUBLIC gint32 ls_error(void);

#ifdef __cplusplus
}
#endif

#endif // __LS_COMMON__