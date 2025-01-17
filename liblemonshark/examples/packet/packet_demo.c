/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#include "lemonshark.h"
#include "wsutil/array.h"

static void handle_error(const char *error_message)
{
    g_print("%s", error_message);
}

static void print_fields(field_t *field, gint32 indentation_count)
{
    const char *representation = ls_field_representation_get(field);
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

    gint32 field_type = ls_field_type_get(field);
    gint32 field_id = ls_field_id_get(field);
    const char *name = ls_field_get_name(field_id);
    const char *display_name = ls_field_get_display_name(field_id);
    const char *type_name = ls_field_type_get_name(field_type);

    g_print("%s (%s, %s, %i)", display_name, name, type_name, field_id);

    gint32 generated = ls_field_generated_get(field);
    gint32 hidden = ls_field_hidden_get(field);
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

    gint32 buffer_id = ls_field_buffer_id_get(field);
    if (buffer_id >= 0)
    {
        gint32 offset = ls_field_offset_get(field);
        gint32 length = ls_field_length_get(field);
        g_print(" {Buffer %i:%i:%i}", buffer_id, offset, length);
    }

    g_print(": ");

    const char* value_representation = ls_field_value_representation_get(field);
    if (value_representation != NULL)
    {
        g_print("%s, ", value_representation);
    }

    if (ls_field_type_is_int64(field_type))
    {
        gint64 value = ls_field_value_get_int64(field);
        g_print("%li (0x%lX)", value, value);
    }
    else if (ls_field_type_is_uint64(field_type))
    {
        guint64 value = ls_field_value_get_uint64(field);
        g_print("%lu (0x%lX)", value, value);
    }
    else if (ls_field_type_is_double(field_type))
    {
        double value = ls_field_value_get_double(field);
        g_print("%g", value);
    }
    else if (ls_field_type_is_string(field_type))
    {
        const char *value = ls_field_value_get_string(field);

        if (value != NULL)
        {
            g_print("%s", value);
        }
    }
    else if (ls_field_type_is_bytes(field_type))
    {
        if (field_type != ls_field_type_protocol())
        {
            gint32 length = ls_field_length_get(field);
            const guint8 *value = ls_field_value_get_bytes(field);

            if (value != NULL)
            {
                for (gint32 i = 0; i < length; i++)
                {
                    g_print("%02X ", value[i]);
                }
            }
        }
    }
    else
    {
        g_print("unknown type");
    }

    g_print("\n");

    gint32 child_count = ls_field_children_count(field);
    for (gint32 i = 0; i < child_count; i++)
    {
        field_t *child = ls_field_children_get(field, i);
        print_fields(child, indentation_count + 1);
    }
}

static void print_packet(packet_t *packet)
{
    gint32 packet_id = ls_packet_id_get(packet);
    gint32 length = ls_packet_length_get(packet);
    gint64 timestamp_seconds = ls_packet_timestamp_seconds_get(packet);
    gint64 timestamp_nanoseconds = ls_packet_timestamp_nanoseconds_get(packet);
    const char *info_column = ls_packet_info_column_get(packet);

    double timestamp = (double)(timestamp_seconds) + (double)(timestamp_nanoseconds) / 1000000000.0;

    gint32 buffers_count = ls_packet_buffers_count(packet);
    g_print("Id: %i, Timestamp: %f, Length: %i, Buffers: %i, Info: %s\n", packet_id, timestamp, length, buffers_count, info_column);

    for (gint32 i = 0; i < buffers_count; i++)
    {
        g_print("Buffer %i: ", i);

        buffer_t *buffer = ls_packet_buffers_get(packet, i);
        gint32 buffer_length = ls_buffer_length_get(buffer);
        guint8 *data = ls_buffer_data_get(buffer);

        for (gint32 j = 0; j < buffer_length; j++)
        {
            g_print("%02X ", data[j]);
        }
        g_print("\n");
    }

    field_t *root_field = ls_packet_root_field_get(packet);

    gint32 child_count = ls_field_children_count(root_field);
    for (gint32 i = 0; i < child_count; i++)
    {
        field_t *child = ls_field_children_get(root_field, i);
        print_fields(child, 1);
    }

    g_print("\n");
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
        packet_t *packet = ls_session_get_packet(packet_id, TRUE, TRUE, TRUE, TRUE, TRUE, NULL, 0, &error_message);

        if (packet == NULL)
        {
            if (error_message != NULL)
            {
                g_print("%s", error_message);
                g_free(error_message);
            }

            break;
        }

        print_packet(packet);

        ls_packet_free(packet);
    }

    ls_session_close();

    ls_error_handler_remove(error_handler);

    return 0;
}