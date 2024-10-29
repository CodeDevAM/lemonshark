/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// glib includes
#include "glib.h"

// lemonshark icludes
#include "error_handler.h"

GSList *error_handlers = NULL;

void ls_error_handler_init(void)
{
    if (error_handlers == NULL)
    {
        return;
    }
}

void ls_error_handler_clear(void)
{
    if (error_handlers == NULL)
    {
        return;
    }

    g_slist_free(error_handlers);
}

void *ls_error_handler_add(error_handler_t error_handler)
{
    if (error_handler == NULL)
    {
        return NULL;
    }

    if (error_handlers == NULL)
    {
        error_handlers = g_slist_alloc();
        error_handlers->data = error_handler;
        return error_handlers;
    }

    GSList *new_list_item = g_slist_append(error_handlers, error_handler);

    return new_list_item;
}

void ls_error_handler_remove(void *error_handler_reference)
{
    if (error_handlers == NULL)
    {
        return;
    }

    gint index = g_slist_position(error_handlers, (GSList *)error_handler_reference);

    if (index < 0)
    {
        return;
    }

    g_slist_remove_link(error_handlers, (GSList *)error_handler_reference);
    g_slist_free_1((GSList *)error_handler_reference);
}

void ls_error_print(const char *msg_format, va_list ap)
{
    if (error_handlers == NULL)
    {
        return;
    }

    char *error_message = g_strdup_vprintf(msg_format, ap);

    GSList *list_item = error_handlers;

    while (list_item != NULL)
    {
        error_handler_t error_handler = (error_handler_t)list_item->data;

        error_handler(error_message);

        list_item = list_item->next;
    }

    g_free(error_message);
}