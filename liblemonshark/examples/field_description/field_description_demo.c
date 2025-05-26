/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#include "lemonshark.h"

static void handle_error(const char *error_message)
{
    g_print("%s", error_message);
}

static void print_field_description(field_description_t* field_description)
{
    gint32 field_type = ls_field_description_type_get(field_description);
    gint32 field_id = ls_field_description_id_get(field_description);
    const char* name = ls_field_description_name_get(field_description);
    const char* display_name = ls_field_description_display_name_get(field_description);
    const char* type_name = ls_field_type_get_name(field_type);
    gint32 parent_id = ls_field_description_parent_id_get(field_description);

    g_print("%s (Name: %s, Type: %s, Id: %i", display_name, name, type_name, field_id);
    if (parent_id >= 0)
    {
        g_print(", Parent: %i", parent_id);
    }
    g_print(")\n");
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

    // Get all field descriptions
    gint32 field_descriptions_count = 0;
    field_description_t** field_descriptions = ls_field_description_get_all(&field_descriptions_count);

    for (gint32 i = 0; i < field_descriptions_count; i++)
    {
        field_description_t* field_description = field_descriptions[i];

        print_field_description(field_description);

        ls_field_description_free(field_description);
    }

    ls_memory_free(field_descriptions);

    g_print("\n");

    // Get a field description by name
    field_description_t* frame_length_field_description = ls_field_description_get_by_name("frame.len");
    print_field_description(frame_length_field_description);
    ls_field_description_free(frame_length_field_description);

    ls_session_close();

    ls_error_handler_remove(error_handler);

    return 0;
}