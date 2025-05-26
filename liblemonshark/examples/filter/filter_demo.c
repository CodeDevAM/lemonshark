/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#include "lemonshark.h"

static void handle_error(const char *error_message)
{
    g_print("%s", error_message);
}

int main(void)
{
    void *error_handler = ls_error_handler_add(handle_error);

	// Set the environment variable LS_EXAMPLE_FILE to the path of a valid trace file before running this example.
    const char *trace_file_path = g_getenv("LS_EXAMPLE_FILE");

    char *error_message = NULL;
    gint32 init_result = ls_session_create_from_file(trace_file_path, "", NULL, &error_message);

    if (init_result == LS_ERROR)
    {
        g_print("%s", error_message);
        g_free(error_message);
        return -1;
    }

    // Test a valid filter
    const char *filter = "frame.len < 150";
    error_message = NULL;
    gint32 is_valid_filter = ls_filter_is_valid(filter, &error_message);
    if (is_valid_filter)
    {
        g_print("'%s' is valid\n", filter);
    }
    else
    {
        g_print("'%s' is invalid: %s\n", filter, error_message);
    }
    if (error_message != NULL)
    {
        g_free(error_message);
    }

    // Test an invalid filter
    filter = "frame.len ! 150";
    error_message = NULL;
    is_valid_filter = ls_filter_is_valid(filter, &error_message);

    if (is_valid_filter)
    {
        g_print("'%s' is valid\n", filter);
    }
    else
    {
        g_print("'%s' is invalid: %s\n", filter, error_message);
    }

    if (error_message != NULL)
    {
        g_free(error_message);
    }

    ls_session_close();

    ls_error_handler_remove(error_handler);

    return 0;
}