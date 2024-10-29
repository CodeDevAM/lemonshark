/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// lemonshark includes
#include "buffer.h"

buffer_t *ls_buffer_new(void)
{
    buffer_t *buffer = g_malloc0(sizeof(buffer_t));

    return buffer;
}

void ls_buffer_free(buffer_t *buffer)
{
    if (buffer == NULL)
    {
        return;
    }

    if (buffer->externel_ref_count > 0)
    {
        return;
    }

    buffer->length = 0;

    if (buffer->data != NULL)
    {
        g_free(buffer->data);

        buffer->data = NULL;
    }

    g_free(buffer);
}

gint32 ls_buffer_size(void)
{
    return sizeof(buffer_t);
}

gint64 ls_buffer_external_ref_count_add(buffer_t *buffer, gint64 ref_count)
{
    buffer->externel_ref_count += ref_count;
    return buffer->externel_ref_count;
}

guint8 *ls_buffer_data_get(buffer_t *buffer)
{
    return buffer->data;
}

void ls_buffer_data_set(buffer_t *buffer, guint8 *data, gint32 length)
{
    if (buffer->data != NULL)
    {
        g_free(buffer->data);
        length = 0;
    }

    if (data == NULL)
    {
        buffer->data = NULL;
        buffer->length = 0;
    }
    else
    {
        buffer->data = g_memdup2(data, length);
        buffer->length = length;
    }
}

void ls_buffer_data_take(buffer_t *buffer, guint8 *data, gint32 length)
{
    if (buffer->data != NULL)
    {
        g_free(buffer->data);
        length = 0;
    }

    if (data == NULL)
    {
        buffer->data = NULL;
        buffer->length = 0;
    }
    else
    {
        buffer->data = data;
        buffer->length = length;
    }
}

gint32 ls_buffer_length_get(buffer_t *buffer)
{
    return buffer->length;
}