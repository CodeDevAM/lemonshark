/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/proto.h"

// lemonshark includes
#include "field_description.h"

field_description_t *ls_field_description_new(void)
{
    field_description_t *field_description = g_malloc0(sizeof(field_description_t));
    return field_description;
}

void ls_field_description_init(field_description_t *field_description, gint32 id, gint32 type, const char *name, const char *display_name, gint32 parent_id)
{
    field_description->id = id;
    field_description->type = type;
    field_description->name = name;
    field_description->display_name = display_name;
    field_description->parent_id = parent_id;
}

void ls_field_description_init_from_header_field_info(field_description_t *field_description, header_field_info *current_header_field_info)
{
    ls_field_description_init(field_description, current_header_field_info->id, current_header_field_info->type, current_header_field_info->abbrev, current_header_field_info->name, current_header_field_info->parent);
}

void ls_field_description_free(field_description_t *field_description)
{
    if (field_description == NULL)
    {
        return;
    }

    if (field_description->externel_ref_count > 0)
    {
        return;
    }

    g_free(field_description);
}

gint32 ls_field_description_size(void)
{
    return sizeof(field_description_t);
}

gint64 ls_field_description_external_ref_count_add(field_description_t *field_description, gint64 ref_count)
{
    if (field_description == NULL)
    {
        return 0;
    }
    field_description->externel_ref_count += ref_count;
    return field_description->externel_ref_count;
}

gint32 ls_field_description_id_get(field_description_t *field_description)
{
    return field_description->id;
}

gint32 ls_field_description_type_get(field_description_t *field_description)
{
    return field_description->type;
}

const char *ls_field_description_name_get(field_description_t *field_description)
{
    return field_description->name;
}

const char *ls_field_description_display_name_get(field_description_t *field_description)
{
    return field_description->display_name;
}

gint32 ls_field_description_parent_id_get(field_description_t *field_description)
{
    return field_description->parent_id;
}

field_description_t *ls_field_description_get_by_id(gint32 id)
{
    if (id <= 0)
    {
        return NULL;
    }

    header_field_info *current_header_field_info = proto_registrar_get_nth(id);

    if (current_header_field_info == NULL)
    {
        return NULL;
    }

    field_description_t *field_description = ls_field_description_new();
    ls_field_description_init_from_header_field_info(field_description, current_header_field_info);

    return field_description;
}

field_description_t *ls_field_description_get_by_name(const char *name)
{
    if (name == NULL)
    {
        return NULL;
    }

    header_field_info *current_header_field_info = proto_registrar_get_byname(name);

    if (current_header_field_info == NULL)
    {
        return NULL;
    }

    field_description_t *field_description = ls_field_description_new();
    ls_field_description_init_from_header_field_info(field_description, current_header_field_info);

    return field_description;
}

field_description_t **ls_field_description_get_all(gint32 *count)
{
    void *proto_cookie = NULL;
    void *field_cookie = NULL;
    int protocol_id = -1;

    *count = 0;

    for (protocol_id = proto_get_first_protocol(&proto_cookie); protocol_id != -1; protocol_id = proto_get_next_protocol(&proto_cookie))
    {
        protocol_t* protocol = find_protocol_by_id(protocol_id);
        if (!proto_is_protocol_enabled(protocol))
        {
            continue;
        }

        *count += 1;
        header_field_info *current_header_field_info;
        for (current_header_field_info = proto_get_first_protocol_field(protocol_id, &field_cookie); current_header_field_info != NULL; current_header_field_info = proto_get_next_protocol_field(protocol_id, &field_cookie))
        {
            if (current_header_field_info->same_name_prev_id != -1)
            {
                continue;
            }
            *count += 1;
        }
    }

    field_description_t **field_descriptions = g_malloc(*count * sizeof(field_description_t *));
    gint32 index = 0;

    proto_cookie = NULL;
    field_cookie = NULL;
    protocol_id = -1;

    for (protocol_id = proto_get_first_protocol(&proto_cookie); protocol_id != -1; protocol_id = proto_get_next_protocol(&proto_cookie))
    {
        protocol_t* protocol = find_protocol_by_id(protocol_id);
        if (!proto_is_protocol_enabled(protocol))
        {
            continue;
        }

        header_field_info *current_header_field_info = proto_registrar_get_nth(proto_get_id(protocol));

        field_description_t *field_description = ls_field_description_new();
        field_descriptions[index] = field_description;
        ls_field_description_init_from_header_field_info(field_description, current_header_field_info);
        index++;

        for (current_header_field_info = proto_get_first_protocol_field(protocol_id, &field_cookie); current_header_field_info != NULL; current_header_field_info = proto_get_next_protocol_field(protocol_id, &field_cookie))
        {
            if (current_header_field_info->same_name_prev_id != -1)
            {
                continue;
            }

            field_description = ls_field_description_new();
            field_descriptions[index] = field_description;
            ls_field_description_init_from_header_field_info(field_description, current_header_field_info);
            index++;
        }
    }

    return field_descriptions;
}