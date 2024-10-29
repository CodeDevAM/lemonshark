/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#ifndef __LS_ERROR_HANDLER__
#define __LS_ERROR_HANDLER__

// glib includes
#include "glib.h"

// wireshark includes
#include "ws_symbol_export.h"

#ifdef __cplusplus
extern "C"
{
#endif

    typedef void (*error_handler_t)(const char *);

    void ls_error_handler_init(void);

    void ls_error_handler_clear(void);

    WS_DLL_PUBLIC void *ls_error_handler_add(error_handler_t error_handler);

    WS_DLL_PUBLIC void ls_error_handler_remove(void *error_handler_reference);

    void ls_error_print(const char *msg_format, va_list ap);

#ifdef __cplusplus
}
#endif

#endif // __LS_ERROR_HANDLER__