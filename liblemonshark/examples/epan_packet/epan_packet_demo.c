/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#include "lemonshark.h"

static void handle_error(const char *error_message)
{
    g_print("%s", error_message);
}

static void print_fields(epan_field_t *epan_field, void *parameter)
{
    gint32 indentation_count = *((gint32 *)parameter);
    const char *representation = ls_epan_field_representation_get(epan_field);
    if (representation != NULL)
    {
        for (gint32 i = 0; i < indentation_count; i++)
        {
            g_print("    ");
        }

        g_print("%s\n", representation);
    }

    for (gint32 i = 0; i < indentation_count; i++)
    {
        g_print("    ");
    }

    gint32 field_type = ls_epan_field_type_get(epan_field);
    gint32 field_id = ls_epan_field_id_get(epan_field);
    const char *name = ls_epan_field_name_get(epan_field);
    const char *display_name = ls_epan_field_display_name_get(epan_field);
    const char *type_name = ls_epan_field_type_name_get(epan_field);

    g_print("%s (%s, %s, %i)", display_name, name, type_name, field_id);

    gint32 generated = ls_epan_field_generated_get(epan_field);
    gint32 hidden = ls_epan_field_hidden_get(epan_field);
    if (hidden || generated)
    {
        g_print(" [");

        if (hidden)
        {
            g_print("H");
        }

        if (generated)
        {
            g_print("G");
        }
        g_print("]");
    }

    gint32 offset = ls_epan_field_offset_get(epan_field);
    gint32 length = ls_epan_field_length_get(epan_field);
    if (offset >= 0 && length > 0)
    {
        g_print(" {Buffer %i:%i}", offset, length);
    }

    g_print(": ");

    const char *value_representation = ls_epan_field_value_representation_get(epan_field);
    if (value_representation != NULL)
    {
        g_print("%s, ", value_representation);
    }

    if (ls_field_type_is_int64(field_type))
    {
        gint64 value = ls_epan_field_value_get_int64(epan_field);
        g_print("%li (0x%lX)", value, value);
    }
    else if (ls_field_type_is_uint64(field_type))
    {
        guint64 value = ls_epan_field_value_get_uint64(epan_field);
        g_print("%lu (0x%lX)", value, value);
    }
    else if (ls_field_type_is_double(field_type))
    {
        double value = ls_epan_field_value_get_double(epan_field);
        g_print("%g", value);
    }
    else if (ls_field_type_is_string(field_type))
    {
        const char *value = ls_epan_field_value_get_string(epan_field);

        if (value != NULL)
        {
            g_print("%s", value);
        }
    }
    else if (ls_field_type_is_bytes(field_type))
    {
        if (field_type != ls_field_type_protocol())
        {
            length = ls_epan_field_length_get(epan_field);
            guint8 *value = g_malloc(length);
            ls_epan_field_value_get_bytes(epan_field, value, length);

            if (value != NULL)
            {
                for (gint32 i = 0; i < length; i++)
                {
                    g_print("%02X ", value[i]);
                }

                g_free(value);
            }
        }
    }
    else
    {
        g_print("unknown type");
    }

    g_print("\n");

    gint32 next_indentation_count = indentation_count + 1;
    ls_epan_field_children_do_for_each(epan_field, print_fields, &next_indentation_count, FALSE);
}

static void print_packet(epan_packet_t *epan_packet)
{
    gint32 packet_id = ls_epan_packet_id_get(epan_packet);
    gint32 length = ls_epan_packet_length_get(epan_packet);
    gint64 timestamp_seconds = ls_epan_packet_timestamp_seconds_get(epan_packet);
    gint64 timestamp_nanoseconds = ls_epan_packet_timestamp_nanoseconds_get(epan_packet);
    const char *info_column = ls_epan_packet_info_column_get(epan_packet);

    double timestamp = (double)(timestamp_seconds) + (double)(timestamp_nanoseconds) / 1000000000.0;

    g_print("Id: %i, Timestamp: %f, Length: %i, Info: %s\n", packet_id, timestamp, length, info_column);

    gint32 buffer_length = ls_epan_packet_length_get(epan_packet);
    if (buffer_length > 0)
    {
        guint8 *buffer = g_malloc(buffer_length);
        ls_epan_packet_buffer_get(epan_packet, buffer, buffer_length);
        g_print("Buffer: ");
        for (gint32 i = 0; i < buffer_length; i++)
        {
            g_print("%02X ", buffer[i]);
        }
        g_print("\n");
        g_free(buffer);
    }

    epan_field_t *root_field = ls_epan_packet_root_field_get(epan_packet);

    gint32 indentation_count = 1;
    ls_epan_field_children_do_for_each(root_field, print_fields, &indentation_count, FALSE);
}

int main(void)
{
    void *error_handler = ls_error_handler_add(handle_error);

    const char *trace_file_path = g_getenv("LS_EXAMPLE_FILE");

    char *error_message = NULL;
    gint32 init_result = ls_session_create_from_file(trace_file_path, "", &error_message);

    if (init_result == LS_ERROR)
    {
        g_print("%s", error_message);
        g_free(error_message);
        return -1;
    }

    field_description_t *frame_length_field_description = ls_field_description_get_by_name("frame.time");

    gint32 packet_id = 0;
    while (TRUE)
    {
        error_message = NULL;
        packet_id = ls_session_get_next_packet_id(&error_message);

        if (packet_id < 0)
        {
            break;
        }
        if (packet_id == 0)
        {
            continue;
        }

        error_message = NULL;
        // Using epan_packet_t and epan_field_t is the recommended approach when performance is critical
        epan_packet_t *epan_packet = ls_session_get_epan_packet(packet_id, TRUE, &frame_length_field_description->id, 1, &error_message);

        if (epan_packet == NULL)
        {
            if (error_message != NULL)
            {
                g_print("%s", error_message);
                g_free(error_message);
            }

            break;
        }

        print_packet(epan_packet);

        ls_epan_packet_free(epan_packet);
    }

    ls_field_description_free(frame_length_field_description);

    ls_session_close();

    ls_error_handler_remove(error_handler);

    return 0;
}