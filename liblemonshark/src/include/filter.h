/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_FILTER__
#define __LS_FILTER__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

#ifdef __cplusplus
extern "C"
{
#endif

    WS_DLL_PUBLIC gint32 ls_filter_is_valid(const char *filter, char** error_message);

#ifdef __cplusplus
}
#endif

#endif // __LS_FILTER__