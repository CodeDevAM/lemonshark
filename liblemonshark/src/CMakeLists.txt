# Copyright (c) 2024 DevAM. All Rights Reserved.

# SPDX-License-Identifier: GPL-2.0-only

cmake_minimum_required(VERSION 3.15)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

project(
    liblemonshark
    VERSION ${LEMONSHARK_MAJOR_VERSION}.${LEMONSHARK_MINOR_VERSION}.${LEMONSHARK_PATCH_VERSION}
    LANGUAGES CXX)

file(
    GLOB
    LEMONSHARK_SRC_FILES
    "*.c"
    "*.cpp"
    "include/*.h"
    "include/*.hpp"
    "${WIRESHARK_BASE_DIRECTORY}/frame_tvbuff.c"
    "${WIRESHARK_BASE_DIRECTORY}/ui/failure_message.c")

add_library(liblemonshark SHARED ${LEMONSHARK_SRC_FILES})

if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    set_target_properties(liblemonshark PROPERTIES OUTPUT_NAME "lemonshark")
endif()

target_include_directories(
    liblemonshark
    PUBLIC ${LEMONSHARK_SRC_DIRECTORY}/include
    PUBLIC ${WIRESHARK_BASE_DIRECTORY})

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    target_link_libraries(
        liblemonshark
        debug ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wireshark.lib
        optimized ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wireshark.lib)
    target_link_libraries(
        liblemonshark
        debug ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wsutil.lib
        optimized ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wsutil.lib)
    target_link_libraries(
        liblemonshark 
        debug ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wiretap.lib
        optimized ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/wiretap.lib)
    target_link_libraries(
        liblemonshark
        debug ${GLIB2_DLL_DIR_DEBUG}/../lib/glib-2.0.lib optimized
        ${GLIB2_DLL_DIR_RELEASE}/../lib/glib-2.0.lib)
    target_link_libraries(
        liblemonshark
        debug ${GLIB2_DLL_DIR_DEBUG}/../lib/gmodule-2.0.lib optimized
        ${GLIB2_DLL_DIR_RELEASE}/../lib/gmodule-2.0.lib)

elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    target_link_libraries(
        liblemonshark
        ${WIRESHARK_BINARY_BASE_DIRECTORY}/libwireshark.so)
    target_link_libraries(
        liblemonshark
        ${WIRESHARK_BINARY_BASE_DIRECTORY}/libwsutil.so)
    target_link_libraries(
        liblemonshark
        ${WIRESHARK_BINARY_BASE_DIRECTORY}/libwiretap.so)
    target_link_libraries(
        liblemonshark 
        libglib-2.0.so)
    target_link_libraries(
        liblemonshark 
        libgmodule-2.0.so)
endif()

target_compile_definitions(
    liblemonshark
    PUBLIC 
    LEMONSHARK_MAJOR_VERSION=${LEMONSHARK_MAJOR_VERSION}
    LEMONSHARK_MINOR_VERSION=${LEMONSHARK_MINOR_VERSION}
    LEMONSHARK_PATCH_VERSION=${LEMONSHARK_PATCH_VERSION}
    WIRESHARK_MAJOR_VERSION=${WIRESHARK_MAJOR_VERSION}
    WIRESHARK_MINOR_VERSION=${WIRESHARK_MINOR_VERSION}
    WIRESHARK_PATCH_VERSION=${WIRESHARK_PATCH_VERSION}
    LEMONSHARK_SYSTEM=${LEMONSHARK_SYSTEM}
    LEMONSHARK_ARCHITECTURE=${LEMONSHARK_ARCHITECTURE})

target_compile_definitions(
    liblemonshark
    PUBLIC 
    WIRESHARK_MAJOR_VERSION=${WIRESHARK_MAJOR_VERSION}
    WIRESHARK_MINOR_VERSION=${WIRESHARK_MINOR_VERSION}
    WIRESHARK_PATCH_VERSION=${WIRESHARK_PATCH_VERSION})

    if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.dll
            ${LEMONSHARK_BINARY_BASE_DIRECTORY}/liblemonshark.dll)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.lib
            ${LEMONSHARK_BINARY_BASE_DIRECTORY}/liblemonshark.lib)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.pdb
            ${LEMONSHARK_BINARY_BASE_DIRECTORY}/liblemonshark.pdb)
    
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.dll
            ${PYTHON_BASE_DIRECTORY}/lemonshark/lemonshark/native/${LEMONSHARK_SYSTEM}/${LEMONSHARK_ARCHITECTURE}/liblemonshark.dll)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.lib
            ${PYTHON_BASE_DIRECTORY}/lemonshark/lemonshark/native/${LEMONSHARK_SYSTEM}/${LEMONSHARK_ARCHITECTURE}/liblemonshark.lib)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.pdb
            ${PYTHON_BASE_DIRECTORY}/lemonshark/lemonshark/native/${LEMONSHARK_SYSTEM}/${LEMONSHARK_ARCHITECTURE}/liblemonshark.pdb)
    
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.dll
            ${DOTNET_BUILD_DIRECTORY}/runtimes/win-x64/native/liblemonshark.dll)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.lib
            ${DOTNET_BUILD_DIRECTORY}/runtimes/win-x64/native/liblemonshark.lib)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.pdb
            ${DOTNET_BUILD_DIRECTORY}/runtimes/win-x64/native/liblemonshark.pdb)

elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/liblemonshark.so
            ${LEMONSHARK_BINARY_BASE_DIRECTORY}/liblemonshark.so)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/liblemonshark.so
            ${PYTHON_BASE_DIRECTORY}/lemonshark/lemonshark/native/${LEMONSHARK_SYSTEM}/${LEMONSHARK_ARCHITECTURE}/liblemonshark.so)
    add_custom_command(
        TARGET liblemonshark
        POST_BUILD
        COMMAND
            ${CMAKE_COMMAND} -E copy
            ${WIRESHARK_BINARY_BASE_DIRECTORY}/liblemonshark.so
            ${DOTNET_BUILD_DIRECTORY}/runtimes/linux-x64/native/liblemonshark.so)

endif()


if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    set_target_properties(
        liblemonshark PROPERTIES MSVC_RUNTIME_LIBRARY
        "MultiThreaded$<$<CONFIG:Debug>:Debug>")

    target_compile_options(liblemonshark PUBLIC /wd26812) # disable enum range warning

endif()
