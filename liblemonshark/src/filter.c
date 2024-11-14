/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/dfilter/dfilter.h"

// lemonshark includes
#include "filter.h"
#include "common.h"

gint32 ls_filter_is_valid(const char* filter, char** error_message)
{
    df_error_t *df_error = NULL;
    dfilter_t *dfilter = NULL;
    gboolean filter_compilation_result = dfilter_compile(filter, &dfilter, &df_error);

    gint32 result = LS_OK;
    
    if (filter_compilation_result == FALSE)
    {
        result = LS_ERROR;
        *error_message = g_strdup(df_error->msg);
    }

    if (dfilter != NULL)
    {
        dfilter_free(dfilter);
    }
    if (df_error != NULL)
    {
        df_error_free(&df_error);
    }

    return result;
}