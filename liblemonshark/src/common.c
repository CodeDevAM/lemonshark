/*
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
*/

#define WS_BUILD_DLL

// wireshark includes
#include "epan/epan.h"

// lemonshark includes
#include "common.h"

gint32 ls_version_get_major(void)
{
    return LEMONSHARK_MAJOR_VERSION;
}

gint32 ls_version_get_minor(void)
{
    return LEMONSHARK_MINOR_VERSION;
}

gint32 ls_version_get_patch(void)
{
    return LEMONSHARK_PATCH_VERSION;
}

gint32 ls_version_get_wireshark_major(void)
{
    gint32 major_version = 0;
    gint32 minor_version = 0;
    gint32 patch_version = 0;
    epan_get_version_number(&major_version, &minor_version, &patch_version);

    return (gint32)major_version;
}

gint32 ls_version_get_wireshark_minor(void)
{
    gint32 major_version = 0;
    gint32 minor_version = 0;
    gint32 patch_version = 0;
    epan_get_version_number(&major_version, &minor_version, &patch_version);

    return (gint32)minor_version;
}

gint32 ls_version_get_wireshark_patch(void)
{
    gint32 major_version = 0;
    gint32 minor_version = 0;
    gint32 patch_version = 0;
    epan_get_version_number(&major_version, &minor_version, &patch_version);

    return (gint32)patch_version;
}

void ls_memory_free(void *memory)
{
    if (memory == NULL)
    {
        return;
    }

    g_free(memory);
}

gint32 ls_ok(void)
{
    return LS_OK;
}

gint32 ls_error(void)
{
    return LS_ERROR;
}