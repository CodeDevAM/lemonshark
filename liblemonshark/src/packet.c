/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// lemonshark includes
#include "packet.h"

packet_t *ls_packet_new(void)
{
	packet_t *packet = g_malloc0(sizeof(packet_t));
	packet->root_field = ls_field_new();
	ls_field_external_ref_count_add(packet->root_field, 1);
	return packet;
}

void ls_packet_free(packet_t *packet)
{
	if (packet == NULL)
	{
		return;
	}

	if (packet->externel_ref_count > 0)
	{
		return;
	}

	if (packet->protocol_column != NULL)
	{
		g_free(packet->protocol_column);
		packet->protocol_column = NULL;
	}

	if (packet->info_column != NULL)
	{
		g_free(packet->info_column);
		packet->info_column = NULL;
	}

	if (packet->root_field != NULL)
	{
		ls_field_external_ref_count_add(packet->root_field, -1);
		ls_field_free(packet->root_field);
		packet->root_field = NULL;
	}

	if (packet->buffers != NULL)
	{
		for (gint32 i = ls_packet_buffers_count(packet) - 1; i >= 0; i--)
		{
			ls_packet_buffers_remove(packet, i);
		}

		g_array_free(packet->buffers, TRUE);
		packet->buffers = NULL;
	}

	g_free(packet);
}

gint32 ls_packet_size(void)
{
	return sizeof(packet_t);
}

gint64 ls_packet_external_ref_count_add(packet_t *packet, gint64 ref_count)
{
	packet->externel_ref_count += ref_count;
	return packet->externel_ref_count;
}

gint32 ls_packet_id_get(packet_t *packet)
{
	return packet->id;
}

void ls_packet_id_set(packet_t *packet, gint32 id)
{
	packet->id = id;
}

gint64 ls_packet_timestamp_seconds_get(packet_t *packet)
{
	return packet->timestamp_seconds;
}

void ls_packet_timestamp_seconds_set(packet_t *packet, gint64 timestamp_seconds)
{
	packet->timestamp_seconds = timestamp_seconds;
}

gint32 ls_packet_timestamp_nanoseconds_get(packet_t *packet)
{
	return packet->timestamp_nanoseconds;
}

void ls_packet_timestamp_nanoseconds_set(packet_t *packet, gint32 timestamp_nanoseconds)
{
	packet->timestamp_nanoseconds = timestamp_nanoseconds;
}

gint32 ls_packet_length_get(packet_t *packet)
{
	return packet->length;
}

void ls_packet_length_set(packet_t *packet, gint32 length)
{
	packet->length = length;
}

gint32 ls_packet_interface_id_get(packet_t *packet)
{
	return packet->interface_id;
}

void ls_packet_interface_id_set(packet_t *packet, gint32 interface_id)
{
	packet->interface_id = interface_id;
}

field_t *ls_packet_root_field_get(packet_t *packet)
{
	return packet->root_field;
}

void ls_packet_root_field_set(packet_t *packet, field_t *root_field)
{
	ls_field_external_ref_count_add(packet->root_field, -1);
	ls_field_free(packet->root_field);
	packet->root_field = root_field;
}

const char *ls_packet_protocol_column_get(packet_t *packet)
{
	return packet->protocol_column;
}

void ls_packet_protocol_column_set(packet_t *packet, const char *protocol_column)
{
	if (packet->protocol_column != NULL)
	{
		g_free(packet->protocol_column);
	}

	packet->protocol_column = protocol_column != NULL ? g_strdup(protocol_column) : NULL;
}

void ls_packet_protocol_column_take(packet_t *packet, char *protocol_column)
{
	if (packet->protocol_column != NULL)
	{
		g_free(packet->protocol_column);
	}

	packet->protocol_column = protocol_column;
}

const char *ls_packet_info_column_get(packet_t *packet)
{
	return packet->info_column;
}

void ls_packet_info_column_set(packet_t *packet, const char *info_column)
{
	if (packet->info_column != NULL)
	{
		g_free(packet->info_column);
	}

	packet->info_column = info_column != NULL ? g_strdup(info_column) : NULL;
}

void ls_packet_info_column_take(packet_t *packet, char *info_column)
{
	if (packet->info_column != NULL)
	{
		g_free(packet->info_column);
	}

	packet->info_column = info_column;
}

gint32 ls_packet_visited_get(packet_t *packet)
{
	return packet->visited;
}

void ls_packet_visited_set(packet_t *packet, gint32 visited)
{
	packet->visited = visited != 0 ? 1 : 0;
}

gint32 ls_packet_visible_get(packet_t *packet)
{
	return packet->visible;
}

void ls_packet_visible_set(packet_t *packet, gint32 visible)
{
	packet->visible = visible != 0 ? 1 : 0;
}

gint32 ls_packet_ignored_get(packet_t *packet)
{
	return packet->ignored;
}

void ls_packet_ignored_set(packet_t *packet, gint32 ignored)
{
	packet->ignored = ignored != 0 ? 1 : 0;
}

gint32 ls_packet_packet_buffer_id_get(packet_t *packet)
{
	return packet->packet_buffer_id;
}

void ls_packet_packet_buffer_id_set(packet_t *packet, gint32 packet_buffer_id)
{
	packet->packet_buffer_id = packet_buffer_id < 0 ? -1 : packet_buffer_id;
}

gint32 ls_packet_buffers_count(packet_t *packet)
{
	if (packet == NULL)
	{
		return 0;
	}

	if (packet->buffers == NULL)
	{
		return 0;
	}

	gint32 length = ((gint32)packet->buffers->len) & 0x7FFFFFFF;
	return length;
}

buffer_t *ls_packet_buffers_get(packet_t *packet, gint32 id)
{
	buffer_t *buffer = g_array_index(packet->buffers, buffer_t *, id);
	return buffer;
}

void ls_packet_buffers_set(packet_t *packet, buffer_t *buffer, gint32 id)
{
	buffer_t* existing_buffer = g_array_index(packet->buffers, buffer_t*, id);
	ls_buffer_external_ref_count_add(buffer, -1);
	ls_buffer_free(existing_buffer);

	g_array_index(packet->buffers, buffer_t *, id) = buffer;
	ls_buffer_external_ref_count_add(buffer, 1);
}

void ls_packet_buffers_add(packet_t *packet, buffer_t *buffer)
{
	if (packet->buffers == NULL)
	{
		packet->buffers = g_array_new(FALSE, FALSE, sizeof(buffer_t *));
	}
	if ((guint64)packet->buffers->len >= 0x7FFFFFFFUL)
	{
		return;
	}
	g_array_append_val(packet->buffers, buffer);
	ls_buffer_external_ref_count_add(buffer, 1);
}

void ls_packet_buffers_remove(packet_t *packet, gint32 id)
{
	buffer_t *buffer = g_array_index(packet->buffers, buffer_t *, id);
	g_array_remove_index(packet->buffers, id);
	ls_buffer_external_ref_count_add(buffer, -1);
	ls_buffer_free(buffer);
	return buffer;
}