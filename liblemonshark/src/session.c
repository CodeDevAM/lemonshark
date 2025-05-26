/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/epan.h"
#include "epan/epan_dissect.h"
#include "epan/packet.h"
#include "epan/proto.h"
#include "epan/tap.h"
#include "epan/tvbuff.h"
#include "epan/column.h"
#include "epan/timestamp.h"
#include "epan/addr_resolv.h"
#include "epan/secrets.h"
#include "epan/frame_data.h"
#include "epan/ftypes/ftypes.h"

#include "ui/failure_message.h"

#include "wsutil/report_message.h"
#include "wsutil/codecs.h"
#include "wsutil/filter_files.h"
#include "wsutil/cmdarg_err.h"
#include "wsutil/filesystem.h"
#include "wsutil/version_info.h"
#include "wsutil/privileges.h"

#include "wiretap/wtap.h"

#include "cfile.h"
#include "file.h"
#include "frame_tvbuff.h"

#include "locale.h"

// lemonshark includes
#include "session.h"
#include "ls_common.h"
#include "packet.h"
#include "field.h"
#include "buffer.h"
#include "error_handler.h"
#include "epan_packet.h"

typedef struct session_t
{
    capture_file *cap_file;
    char *read_filter;
    guint32 cum_bytes;
    frame_data ref_frame;
    frame_data prev_dis_frame;
    frame_data prev_cap_frame;
    epan_dissect_t current_epan_dissect;
    epan_packet_t *current_epan_packet;

} session_t;

session_t *session = NULL;
gboolean infrastructure_is_initialized = FALSE;
e_prefs *preferences = NULL;

packet_t *ls_packet_new_from_packet_info(packet_info *packet_info, proto_tree *tree, tvbuff_t *packet_buffer, const gint32 include_buffers, const gint32 include_columns, const gint32 include_representations, const gint32 include_strings, const gint32 include_bytes);
void ls_field_handle_proto_node(proto_node *node, gpointer data);
void ls_field_value_set_from_ftvalue(field_t *field, const field_info *field_info, const gint32 include_strings, const gint32 include_bytes);
buffer_t *ls_buffer_new_from_tvbuff(tvbuff_t *buffer);

static session_t *ls_session_new(void)
{
    session = g_malloc0(sizeof(session_t));
    session->cap_file = g_malloc0(sizeof(capture_file));

    return session;
}

static gint32 ls_init_infrastructure(const char *profile, char **error_message)
{
#ifdef _WIN32
    setlocale(LC_ALL, ".UTF-8");
#else
    setlocale(LC_ALL, "");
#endif

    cmdarg_err_init(ls_error_print, ls_error_print);
    ws_log_init(LS_APP_NAME, ls_error_print);

    ws_init_version_info(LS_APP_NAME, epan_gather_compile_info, epan_gather_runtime_info);

    configuration_init('\0', "wireshark");

    static const struct report_message_routines report_routines = {
        failure_message,
        failure_message,
        open_failure_message,
        read_failure_message,
        write_failure_message,
        cfile_open_failure_message,
        cfile_dump_open_failure_message,
        cfile_read_failure_message,
        cfile_write_failure_message,
        cfile_close_failure_message};

    init_report_message(LS_APP_NAME, &report_routines);

    init_process_policies();
    relinquish_special_privs_perm();

    timestamp_set_type(TS_RELATIVE);
    timestamp_set_precision(TS_PREC_AUTO);
    timestamp_set_seconds_type(TS_SECONDS_DEFAULT);

#ifdef HAVE_LIBPCAP
    capture_opts_init(&global_capture_opts, capture_opts_get_interface_list);
#endif

    wtap_init(TRUE);

    gboolean epan_init_result = epan_init(NULL, NULL, TRUE);
    if (epan_init_result == FALSE)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Initialization of epan failed.");
        }
        return LS_ERROR;
    }

    codecs_init();

    if (profile != NULL && strlen(profile) > 0)
    {
        if (profile_exists(profile, false))
        {
            set_profile_name(profile);
        }
        else
        {
            *error_message = g_strdup("Unknown profile.");
            return LS_ERROR;
        }
    }

    preferences = epan_load_settings();

    prefs_apply_all();

    infrastructure_is_initialized = TRUE;

    return LS_OK;
}

static gint32 ls_session_init(const char *profile, char **error_message)
{
    if (infrastructure_is_initialized == FALSE)
    {
        gint32 init_infrastructure_result = ls_init_infrastructure(profile, error_message);
        if (init_infrastructure_result == LS_ERROR)
        {
            return LS_ERROR;
        }
    }

    if (session != NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("There can only be one session at a time.");
        }

        return LS_ERROR;
    }

    session = ls_session_new();

    if (session == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Initialization of session failed.");
        }

        return LS_ERROR;
    }

    build_column_format_array(&session->cap_file->cinfo, preferences->num_cols, TRUE);

    return LS_OK;
}

static const nstime_t *ls_get_frame_ts(struct packet_provider_data *prov, guint32 frame_num)
{
    const frame_data *fdata = NULL;
    if (prov->ref && prov->ref->num == frame_num)
    {
        fdata = prov->ref;
    }
    else if (prov->prev_dis && prov->prev_dis->num == frame_num)
    {
        fdata = prov->prev_dis;
    }
    else if (prov->prev_cap && prov->prev_cap->num == frame_num)
    {
        fdata = prov->prev_cap;
    }
    else if (prov->frames)
    {
        fdata = frame_data_sequence_find(prov->frames, frame_num);
    }

    const nstime_t *timestamp = (fdata && fdata->has_ts) ? &fdata->abs_ts : NULL;

    return timestamp;
}

static const char *ls_get_interface_name(struct packet_provider_data *prov, uint32_t interface_id, unsigned section_number)
{
    wtapng_iface_descriptions_t *idb_info;
    wtap_block_t wtapng_if_descr = NULL;
    char *interface_name;

    idb_info = wtap_file_get_idb_info(prov->wth);

    guint32 global_interface_id = wtap_file_get_shb_global_interface_id(prov->wth, section_number, interface_id);

    if (global_interface_id < idb_info->interface_data->len)
    {
        wtapng_if_descr = g_array_index(idb_info->interface_data, wtap_block_t, global_interface_id);
    }

    g_free(idb_info);

    if (wtapng_if_descr)
    {
        if (wtap_block_get_string_option_value(wtapng_if_descr, OPT_IDB_NAME, &interface_name) == WTAP_OPTTYPE_SUCCESS)
        {
            return interface_name;
        }
        if (wtap_block_get_string_option_value(wtapng_if_descr, OPT_IDB_DESCRIPTION, &interface_name) == WTAP_OPTTYPE_SUCCESS)
        {
            return interface_name;
        }
        if (wtap_block_get_string_option_value(wtapng_if_descr, OPT_IDB_HARDWARE, &interface_name) == WTAP_OPTTYPE_SUCCESS)
        {
            return interface_name;
        }
    }

    return "unknown";
}

static const char *ls_get_interface_description(struct packet_provider_data *prov, uint32_t interface_id, unsigned section_number)
{
    wtapng_iface_descriptions_t *idb_info;
    wtap_block_t wtapng_if_descr = NULL;
    char *interface_name;

    idb_info = wtap_file_get_idb_info(prov->wth);

    guint32 global_interface_id = wtap_file_get_shb_global_interface_id(prov->wth, section_number, interface_id);

    if (global_interface_id < idb_info->interface_data->len)
    {
        wtapng_if_descr = g_array_index(idb_info->interface_data, wtap_block_t, global_interface_id);
    }

    g_free(idb_info);

    if (wtapng_if_descr)
    {
        if (wtap_block_get_string_option_value(wtapng_if_descr, OPT_IDB_DESCRIPTION, &interface_name) == WTAP_OPTTYPE_SUCCESS)
        {
            return interface_name;
        }
    }
    return NULL;
}

static wtap_block_t ls_get_modified_block(struct packet_provider_data *prov, const frame_data *fdata)
{
    if (prov->frames_modified_blocks)
    {
        return (wtap_block_t)g_tree_lookup(prov->frames_modified_blocks, fdata);
    }

    return NULL;
}

gint32 ls_session_create_from_file(const char *file_path, const char *read_filter, const char *profile, char **error_message)
{
    if (session != NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("There can only be one session at a time.");
        }

        return LS_ERROR;
    }

    if(file_path == NULL || strlen(file_path) == 0)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("File path is empty.");
        }
        return LS_ERROR;
	}

    gint32 init_result = ls_session_init(profile, error_message);
    if (init_result == LS_ERROR)
    {
        return LS_ERROR;
    }
    if (session == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Initialization of session failed.");
        }

        return LS_ERROR;
    }

    capture_file *cap_file = session->cap_file;

    if (read_filter != NULL)
    {
        df_error_t *df_error = NULL;
        gboolean filter_compilation_result = dfilter_compile(read_filter, &cap_file->rfcode, &df_error);
        if (filter_compilation_result == FALSE)
        {
            if (error_message != NULL)
            {
                *error_message = g_strdup_printf("Faulty read filter: %s", df_error->msg);
            }

            df_error_free(&df_error);

            return LS_ERROR;
        }

        session->read_filter = g_strdup(read_filter);

        if (df_error != NULL)
        {
            df_error_free(&df_error);
        }
    }

    int error = 0;
    wtap *wiretap_handle = wtap_open_offline(file_path, WTAP_TYPE_AUTO, &error, error_message, TRUE);

    if (wiretap_handle == NULL)
    {
        return LS_ERROR;
    }

    cap_file->provider.wth = wiretap_handle;

    wtap_rec_init(&cap_file->rec);
    ws_buffer_init(&cap_file->buf, 1514);
    cap_file->state = FILE_READ_IN_PROGRESS;

    cap_file->provider.frames = new_frame_data_sequence();

    cap_file->filename = g_strdup(file_path);
    cap_file->cd_t = wtap_file_type_subtype(cap_file->provider.wth);
    cap_file->open_type = WTAP_TYPE_AUTO;
    cap_file->snap = wtap_snapshot_length(cap_file->provider.wth);
    nstime_set_zero(&cap_file->elapsed_time);

    static const struct packet_provider_funcs funcs = {
        ls_get_frame_ts,
        ls_get_interface_name,
        ls_get_interface_description,
        ls_get_modified_block};

    epan_t* epan = epan_new(&cap_file->provider, &funcs);
    cap_file->epan = epan;

    cap_file->state = FILE_READ_IN_PROGRESS;

    wtap_set_cb_new_ipv4(cap_file->provider.wth, add_ipv4_name);
    wtap_set_cb_new_ipv6(cap_file->provider.wth, (wtap_new_ipv6_callback_t)add_ipv6_name);
    wtap_set_cb_new_secrets(cap_file->provider.wth, secrets_wtap_callback);

    reset_tap_listeners();

    return LS_OK;
}

gint32 ls_session_get_next_packet_id(char **error_message)
{
    if (session == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Invalid session");
        }

        return -1;
    }
    if (session->cap_file->count >= 0x7FFFFFFFU)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Maximum number of frames is exceeded");
        }

        return -1;
    }

    capture_file *cap_file = session->cap_file;
    wtap_rec *rec = &cap_file->rec;
    epan_dissect_t epan_dissect;
    epan_dissect_init(&epan_dissect, cap_file->epan, TRUE, FALSE);
    Buffer *buffer = &cap_file->buf;

    wtap_rec_reset(rec);

    gint64 file_offset = 0;
    int error = 0;
    gboolean read_result = wtap_read(cap_file->provider.wth, rec, buffer, &error, error_message, &file_offset);

    if (read_result == FALSE)
    {
        return -1;
    }

    frame_data current_frame_data;
    frame_data_init(&current_frame_data, cap_file->count + 1, rec, file_offset, cap_file->cum_bytes);

    gboolean include_columns = FALSE;
    if (cap_file->rfcode != NULL)
    {
        epan_dissect_prime_with_dfilter(&epan_dissect, cap_file->rfcode);
        include_columns = dfilter_requires_columns(cap_file->rfcode);
    }

    prime_epan_dissect_with_postdissector_wanted_hfids(&epan_dissect);

    frame_data_set_before_dissect(&current_frame_data, &cap_file->elapsed_time, &cap_file->provider.ref, cap_file->provider.prev_dis);
    if (cap_file->provider.ref == &current_frame_data)
    {
        session->ref_frame = current_frame_data;
        cap_file->provider.ref = &session->ref_frame;
    }

    tvbuff_t *tvbuffer = frame_tvbuff_new_buffer(&cap_file->provider, &current_frame_data, buffer);

    column_info *cinfo = include_columns ? &cap_file->cinfo : NULL;
    epan_dissect_run_with_taps(&epan_dissect, cap_file->cd_t, rec, tvbuffer, &current_frame_data, cinfo);

    gboolean passed = TRUE;

    if (cap_file->rfcode != NULL)
    {
        passed = dfilter_apply_edt(cap_file->rfcode, &epan_dissect);
    }

    epan_dissect_cleanup(&epan_dissect);

    gint32 packet_id = 0;
    if (passed == FALSE)
    {
        frame_data_destroy(&current_frame_data);
    }
    else
    {
        frame_data_set_after_dissect(&current_frame_data, &session->cum_bytes);
        cap_file->provider.prev_cap = cap_file->provider.prev_dis = frame_data_sequence_add(cap_file->provider.frames, &current_frame_data);
        cap_file->count++;

        packet_id = (gint32)cap_file->count;
    }

    return packet_id;
}

packet_t *ls_session_get_packet(gint32 packet_id, const gint32 include_buffers, const gint32 include_columns, const gint32 include_representations, const gint32 include_strings, const gint32 include_bytes, const gint32 *requested_field_ids, const gint32 requested_field_id_count, char **error_message)
{
    if (session == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Invalid session");
        }

        return NULL;
    }

    capture_file *cap_file = session->cap_file;
    wtap_rec *rec = &cap_file->rec;
    epan_dissect_t *epan_dissect = NULL;

    if (requested_field_ids != NULL && requested_field_id_count > 0)
    {
        epan_dissect = epan_dissect_new(cap_file->epan, TRUE, FALSE);
        for (gint32 i = 0; i < requested_field_id_count; i++)
        {
            gint32 requested_field_id = requested_field_ids[i];
            epan_dissect_prime_with_hfid(epan_dissect, requested_field_id);
            gint32 parent_id = proto_registrar_get_parent(requested_field_id);
            if (parent_id >= 0)
            {
                epan_dissect_prime_with_hfid(&session->current_epan_dissect, parent_id);
            }
        }
    }
    else
    {
        epan_dissect = epan_dissect_new(cap_file->epan, TRUE, TRUE);
    }

    Buffer *buffer = &cap_file->buf;

    frame_data *current_frame_data = frame_data_sequence_find(cap_file->provider.frames, packet_id);

    if (current_frame_data == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Unknown frame id");
        }

        return NULL;
    }

    wtap_rec_reset(rec);

    gint64 file_offset = current_frame_data->file_off;
    int error = 0;
    gboolean read_result = wtap_seek_read(cap_file->provider.wth, file_offset, rec, buffer, &error, error_message);

    if (read_result == FALSE)
    {
        return NULL;
    }

    column_info *cinfo = include_columns ? &cap_file->cinfo : NULL;

    tvbuff_t *tvbuffer = frame_tvbuff_new_buffer(&cap_file->provider, current_frame_data, buffer);

    prime_epan_dissect_with_postdissector_wanted_hfids(epan_dissect);

    epan_dissect_run_with_taps(epan_dissect, cap_file->cd_t, rec, tvbuffer, current_frame_data, cinfo);

    packet_info *current_packet_info = &epan_dissect->pi;
    proto_tree *tree = epan_dissect->tree;
    tvbuff_t *packet_buffer = epan_dissect->tvb;

    packet_t *packet = ls_packet_new_from_packet_info(current_packet_info, tree, packet_buffer, include_buffers, include_columns, include_representations, include_strings, include_bytes);

    epan_dissect_free(epan_dissect);

    return packet;
}

typedef struct handle_proto_node_data
{
    packet_info *packet_info;
    field_t *parent_field;
    GPtrArray *buffers;
    const gint32 include_buffers;
    const gint32 include_representations;
    const gint32 include_strings;
    const gint32 include_bytes;
} handle_proto_node_data_t;

packet_t *ls_packet_new_from_packet_info(packet_info *current_packet_info, proto_tree *tree, tvbuff_t *packet_buffer, const gint32 include_buffers, const gint32 include_columns, const gint32 include_representations, const gint32 include_strings, const gint32 include_bytes)
{
    packet_t *packet = ls_packet_new();

    ls_packet_id_set(packet, current_packet_info->num);
    ls_packet_timestamp_seconds_set(packet, current_packet_info->abs_ts.secs);
    ls_packet_timestamp_nanoseconds_set(packet, current_packet_info->abs_ts.nsecs);
    ls_packet_length_set(packet, current_packet_info->fd->pkt_len);
    ls_packet_interface_id_set(packet, current_packet_info->link_number);

    if (include_columns)
    {
        if (current_packet_info->cinfo != NULL)
        {
            ls_packet_protocol_column_set(packet, col_get_text(current_packet_info->cinfo, COL_PROTOCOL));
            ls_packet_info_column_set(packet, col_get_text(current_packet_info->cinfo, COL_INFO));
        }
    }

    ls_packet_visited_set(packet, current_packet_info->fd->visited);
    ls_packet_visible_set(packet, tree != NULL && tree->tree_data->visible != FALSE);
    ls_packet_ignored_set(packet, current_packet_info->fd->ignored);

    ls_packet_packet_buffer_id_set(packet, -1);

    GPtrArray *buffers = NULL;

    if (include_buffers)
    {
        buffers = g_ptr_array_new();
        g_ptr_array_add(buffers, packet_buffer);
        ls_packet_packet_buffer_id_set(packet, 0);
    }

    handle_proto_node_data_t handle_proto_node_data =
        {
            .packet_info = current_packet_info,
            .parent_field = packet->root_field,
            .buffers = buffers,
            .include_buffers = include_buffers,
            .include_representations = include_representations,
            .include_strings = include_strings,
            .include_bytes = include_bytes};

    proto_tree_children_foreach(tree, ls_field_handle_proto_node, &handle_proto_node_data);

    if (include_buffers)
    {
        for (guint i = 0; i < g_ptr_array_len(buffers); i++)
        {
            tvbuff_t *tvbuff = (tvbuff_t *)g_ptr_array_index(buffers, i);
            buffer_t *buffer = ls_buffer_new_from_tvbuff(tvbuff);
            ls_packet_buffers_add(packet, buffer);
        }

        g_ptr_array_free(buffers, TRUE);
    }

    return packet;
}

void ls_field_handle_proto_node(proto_node *node, gpointer data)
{
    handle_proto_node_data_t *handle_proto_node_data = (handle_proto_node_data_t *)data;
    const field_info *current_field_info = node->finfo;
    const header_field_info *current_header_field_info = current_field_info->hfinfo;

    field_t *field = ls_field_new();

    if (handle_proto_node_data->include_representations)
    {
        ls_field_representation_set(field, current_field_info->rep->representation);
    }

    ls_field_id_set(field, current_header_field_info->id);

    ls_field_buffer_id_set(field, -1);
    ls_field_offset_set(field, -1);

    if (handle_proto_node_data->include_buffers)
    {
        tvbuff_t *buffer = current_field_info->ds_tvb;
        if (buffer != NULL)
        {
            ls_field_offset_set(field, current_field_info->start);

            gboolean buffer_found = g_ptr_array_find(handle_proto_node_data->buffers, buffer, &field->buffer_id);
            if (buffer_found == FALSE)
            {
                ls_field_buffer_id_set(field, (gint32)g_ptr_array_len(handle_proto_node_data->buffers));
                g_ptr_array_add(handle_proto_node_data->buffers, buffer);
            }
        }
    }

    ls_field_hidden_set(field, current_field_info->flags & FI_HIDDEN);
    ls_field_generated_set(field, current_field_info->flags & FI_GENERATED);

    ls_field_encoding_set(field, ENC_NA);

    if (current_field_info->flags & FI_BIG_ENDIAN)
    {
        ls_field_encoding_set(field, ENC_BIG_ENDIAN);
    }
    else if (current_field_info->flags & FI_LITTLE_ENDIAN)
    {
        ls_field_encoding_set(field, ENC_LITTLE_ENDIAN);
    }

    ls_field_value_set_from_ftvalue(field, current_field_info, handle_proto_node_data->include_strings, handle_proto_node_data->include_bytes);

    char *value_representation = NULL;
    switch (current_header_field_info->type)
    {
    case FT_INT8:
    case FT_INT16:
    case FT_INT24:
    case FT_INT32:
    case FT_INT40:
    case FT_INT48:
    case FT_INT56:
    case FT_INT64:
    case FT_UINT8:
    case FT_UINT16:
    case FT_UINT24:
    case FT_UINT32:
    case FT_UINT40:
    case FT_UINT48:
    case FT_UINT56:
    case FT_UINT64:
    case FT_FLOAT:
    case FT_DOUBLE:
    case FT_STRING:
    case FT_STRINGZ:
    case FT_STRINGZPAD:
    case FT_STRINGZTRUNC:
    case FT_UINT_STRING:
    case FT_BYTES:
    case FT_UINT_BYTES:
    {
        value_representation = NULL;
    }
    break;
    default:
    {
        if (handle_proto_node_data->include_representations)
        {
            value_representation = fvalue_to_string_repr(handle_proto_node_data->packet_info->pool, current_field_info->value, FTREPR_DISPLAY, BASE_NONE);
        }
    }
    break;
    }

    ls_field_value_representation_set(field, value_representation);

    ls_field_children_add(handle_proto_node_data->parent_field, field);

    field_t *previous_parent = handle_proto_node_data->parent_field;
    handle_proto_node_data->parent_field = field;
    proto_tree_children_foreach(node, ls_field_handle_proto_node, handle_proto_node_data);
    handle_proto_node_data->parent_field = previous_parent;
}

void ls_field_value_set_from_ftvalue(field_t *field, const field_info *current_field_info, const gint32 include_strings, const gint32 include_bytes)
{
    enum ftenum type = current_field_info->hfinfo->type;
    switch (type)
    {
    case FT_INT8:
    case FT_INT16:
    case FT_INT24:
    case FT_INT32:
    {
        gint64 value = fvalue_get_sinteger(current_field_info->value);
        ls_field_value_set_int64(field, value, type);
    }
    break;
    case FT_INT40:
    case FT_INT48:
    case FT_INT56:
    case FT_INT64:
    {
        gint64 value = fvalue_get_sinteger64(current_field_info->value);
        ls_field_value_set_int64(field, value, type);
    }
    break;
    case FT_UINT8:
    case FT_UINT16:
    case FT_UINT24:
    case FT_UINT32:
    case FT_CHAR:
    case FT_IEEE_11073_SFLOAT:
    case FT_IEEE_11073_FLOAT:
    case FT_IPXNET:
    case FT_FRAMENUM:
    {
        guint64 value = fvalue_get_uinteger(current_field_info->value);
        ls_field_value_set_uint64(field, value, type);
    }
    break;
    case FT_UINT40:
    case FT_UINT48:
    case FT_UINT56:
    case FT_UINT64:
    case FT_BOOLEAN:
    case FT_EUI64:
    {
        guint64 value = fvalue_get_uinteger64(current_field_info->value);
        ls_field_value_set_uint64(field, value, type);
    }
    break;
    case FT_FLOAT:
    case FT_DOUBLE:
    {
        double value = fvalue_get_floating(current_field_info->value);
        ls_field_value_set_double(field, value, type);
    }
    break;
    case FT_ABSOLUTE_TIME:
    case FT_RELATIVE_TIME:
    {
        const nstime_t *timestamp = fvalue_get_time(current_field_info->value);
        double value = (double)(timestamp->secs) + (double)(timestamp->nsecs) / 1000000000.0;
        ls_field_value_set_double(field, value, type);
    }
    break;
    case FT_STRING:
    case FT_STRINGZ:
    case FT_STRINGZPAD:
    case FT_STRINGZTRUNC:
    case FT_UINT_STRING:
    case FT_AX25:
    {
        if (include_strings)
        {
            const wmem_strbuf_t *string_buffer = fvalue_get_strbuf(current_field_info->value);
            const char *value = wmem_strbuf_get_str(string_buffer);
            ls_field_value_set_string(field, value, type);
        }
    }
    break;
    case FT_ETHER:
    case FT_BYTES:
    case FT_UINT_BYTES:
    {
        if (include_bytes)
        {
            gint32 length = (gint32)(fvalue_get_bytes_size(current_field_info->value) & 0x7FFFFFFF);
            const void *data = fvalue_get_bytes_data(current_field_info->value);
            ls_field_value_set_bytes(field, data, length, type);
        }
    }
    break;
    case FT_IPv4:
    {
        const ipv4_addr_and_mask *current_ipv4_addr_and_mask = fvalue_get_ipv4(current_field_info->value);
        guint32 value = (guint32)current_ipv4_addr_and_mask->addr;
        ls_field_value_set_uint64(field, value, type);
    }
    break;
    case FT_IPv6:
    {
        if (include_bytes)
        {
            const ipv6_addr_and_prefix *current_ipv6_addr_and_prefix = fvalue_get_ipv6(current_field_info->value);
            ls_field_value_set_bytes(field, current_ipv6_addr_and_prefix->addr.bytes, 16, type);
        }
    }
    break;
    case FT_GUID:
    {
        if (include_bytes)
        {
            const e_guid_t *guid = fvalue_get_guid(current_field_info->value);

            guint8 *value = (guint8 *)g_malloc(16);

            ((guint32 *)value)[0] = (guint32)guid->data1;
            ((guint16 *)value)[2] = (guint16)guid->data2;
            ((guint16 *)value)[3] = (guint16)guid->data3;
            ((guint64 *)value)[1] = ((guint64 *)guid->data4)[0];

            ls_field_value_take_bytes(field, value, 16, type);
        }
    }
    break;
    case FT_OID:
    case FT_VINES:
    case FT_REL_OID:
    case FT_SYSTEM_ID:
    case FT_FCWWN:
    {
        if (include_bytes)
        {
            gint32 length = (gint32)(fvalue_get_bytes_size(current_field_info->value) & 0x7FFFFFFF);
            const void *data = fvalue_get_bytes_data(current_field_info->value);
            ls_field_value_set_bytes(field, data, length, type);
        }
    }
    break;
    case FT_PROTOCOL:
    {
        if (include_bytes)
        {
            gint32 length = (gint32)(current_field_info->length & 0x7FFFFFFF);
            if (length > 0)
            {
                tvbuff_t *buffer = fvalue_get_protocol(current_field_info->value);
                guint8 *value = g_malloc(length);
                tvb_memcpy(buffer, value, 0, length);

                ls_field_value_take_bytes(field, value, length, type);
            }
            else
            {
                ls_field_value_take_bytes(field, NULL, 0, type);
            }
        }
    }
    break;
    case FT_NONE:
    {
        if (include_strings)
        {
            ls_field_value_set_string(field, current_field_info->rep->representation, type);
        }
    }
    break;
    case FT_NUM_TYPES: // Special value without actual value
    case FT_SCALAR:    // Special value without actual value
    default:
    {
        ls_field_value_set_uint64(field, 0, type);
    }
    break;
    }
}

buffer_t *ls_buffer_new_from_tvbuff(tvbuff_t *tvbuff)
{
    gint32 buffer_length = (gint32)(tvb_captured_length(tvbuff) & 0x7FFFFFFF);

    guint8 *data = g_malloc(buffer_length);
    tvb_memcpy(tvbuff, data, 0, buffer_length);

    buffer_t *buffer = ls_buffer_new();
    ls_buffer_data_take(buffer, (guint8 *)data, buffer_length);

    return buffer;
}

static void ls_current_epan_packet_free(void)
{
    if (session->current_epan_packet != NULL)
    {
        ls_epan_packet_invalidate(session->current_epan_packet);
        ls_epan_packet_external_ref_count_add(session->current_epan_packet, -1);
        ls_epan_packet_free(session->current_epan_packet);
        session->current_epan_packet = NULL;

        epan_dissect_cleanup(&session->current_epan_dissect);
    }
}

epan_packet_t *ls_session_get_epan_packet(gint32 packet_id, const gint32 include_columns, const gint32 *requested_field_ids, const gint32 requested_field_id_count, char **error_message)
{
    if (session == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Invalid session");
        }

        return NULL;
    }

    capture_file *cap_file = session->cap_file;
    wtap_rec *rec = &cap_file->rec;
    Buffer *buffer = &cap_file->buf;

    ls_current_epan_packet_free();

    if (requested_field_ids != NULL && requested_field_id_count > 0)
    {
        epan_dissect_init(&session->current_epan_dissect, cap_file->epan, TRUE, FALSE);
        for (gint32 i = 0; i < requested_field_id_count; i++)
        {
            gint32 requested_field_id = requested_field_ids[i];
            epan_dissect_prime_with_hfid(&session->current_epan_dissect, requested_field_id);
            gint32 parent_id = proto_registrar_get_parent(requested_field_id);
            if (parent_id >= 0)
            {
                epan_dissect_prime_with_hfid(&session->current_epan_dissect, parent_id);
            }
        }
    }
    else
    {
        epan_dissect_init(&session->current_epan_dissect, cap_file->epan, TRUE, TRUE);
    }

    frame_data *current_frame_data = frame_data_sequence_find(cap_file->provider.frames, packet_id);

    if (current_frame_data == NULL)
    {
        if (error_message != NULL)
        {
            *error_message = g_strdup("Unknown frame id");
        }

        return NULL;
    }

    wtap_rec_reset(rec);

    gint64 file_offset = current_frame_data->file_off;
    int error = 0;
    gboolean read_result = wtap_seek_read(cap_file->provider.wth, file_offset, rec, buffer, &error, error_message);

    if (read_result == FALSE)
    {
        return NULL;
    }

    column_info *cinfo = include_columns ? &cap_file->cinfo : NULL;

    tvbuff_t *tvbuffer = frame_tvbuff_new_buffer(&cap_file->provider, current_frame_data, buffer);

    prime_epan_dissect_with_postdissector_wanted_hfids(&session->current_epan_dissect);

    epan_dissect_run_with_taps(&session->current_epan_dissect, cap_file->cd_t, rec, tvbuffer, current_frame_data, cinfo);

    session->current_epan_packet = ls_epan_packet_new();
    ls_epan_packet_init(session->current_epan_packet, &session->current_epan_dissect);
    ls_epan_packet_external_ref_count_add(session->current_epan_packet, 1);

    return session->current_epan_packet;
}

void ls_session_close(void)
{
    if (session == NULL)
    {
        return;
    }

    ls_current_epan_packet_free();

    postseq_cleanup_all_protocols();

    capture_file *cap_file = session->cap_file;

    if (cap_file->provider.wth != NULL)
    {
        wtap_close(cap_file->provider.wth);
        cap_file->provider.wth = NULL;
    }

    if (cap_file->dfilter != NULL)
    {
        g_free(cap_file->dfilter);
        cap_file->dfilter = NULL;
    }

    if (cap_file->rfcode != NULL)
    {
        dfilter_free(cap_file->rfcode);
        cap_file->rfcode = NULL;
    }

    if (cap_file->dfcode != NULL)
    {
        dfilter_free(cap_file->dfcode);
        cap_file->rfcode = NULL;
    }

    if (cap_file->provider.frames != NULL)
    {
        free_frame_data_sequence(cap_file->provider.frames);
        cap_file->provider.frames = NULL;
    }

    if (cap_file->provider.frames_modified_blocks != NULL)
    {
        g_tree_destroy(cap_file->provider.frames_modified_blocks);
        cap_file->provider.frames_modified_blocks = NULL;
    }

    if (cap_file->linktypes != NULL)
    {
        g_array_free(cap_file->linktypes, TRUE);
        cap_file->linktypes = NULL;
    }

    if (cap_file->provider.frames_modified_blocks)
    {
        g_tree_destroy(cap_file->provider.frames_modified_blocks);
        cap_file->provider.frames_modified_blocks = NULL;
    }

    if (cap_file->filename != NULL)
    {
        g_free(cap_file->filename);
    }

    wtap_rec_cleanup(&cap_file->rec);

    ws_buffer_free(&cap_file->buf);

    col_cleanup(&cap_file->cinfo);

    reset_tap_listeners();

    epan_free(cap_file->epan);
    cap_file->epan = NULL;

    g_free(cap_file);
    session->cap_file = NULL;

    if (session->read_filter != NULL)
    {
        g_free(session->read_filter);
        session->read_filter = NULL;
    }

    g_free(session);
    session = NULL;

    return;
}