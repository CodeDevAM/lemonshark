# Copyright (c) 2024 DevAM. All Rights Reserved.

# SPDX-License-Identifier: GPL-2.0-only

cmake_minimum_required(VERSION 3.15)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

project(
    field_description_demo
    VERSION ${LEMONSHARK_MAJOR_VERSION}.${LEMONSHARK_MINOR_VERSION}.${LEMONSHARK_PATCH_VERSION}
    LANGUAGES CXX)

file(
    GLOB
    SRC_FILES
    "*.c"
    "*.cpp"
    "include/*.h"
    "include/*.hpp")

add_executable(field_description_demo ${SRC_FILES})

target_include_directories(
    field_description_demo
    PUBLIC ${LEMONSHARK_SRC_DIRECTORY}/include
    PUBLIC ${WIRESHARK_BASE_DIRECTORY})

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    target_link_libraries(
        field_description_demo
        debug ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.lib
        optimized ${WIRESHARK_BINARY_BASE_DIRECTORY}/$<CONFIG>/liblemonshark.lib)
    target_link_libraries(
        field_description_demo
        debug ${GLIB2_DLL_DIR_DEBUG}/../lib/glib-2.0.lib optimized
        ${GLIB2_DLL_DIR_RELEASE}/../lib/glib-2.0.lib)

elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    target_link_libraries(
        field_description_demo
        ${WIRESHARK_BINARY_BASE_DIRECTORY}/liblemonshark.so)
    target_link_libraries(
        field_description_demo 
        libglib-2.0.so)
endif()

target_compile_definitions(
    field_description_demo
    PUBLIC 
    LEMONSHARK_MAJOR_VERSION=${LEMONSHARK_MAJOR_VERSION}
    LEMONSHARK_MINOR_VERSION=${LEMONSHARK_MINOR_VERSION}
    LEMONSHARK_PATCH_VERSION=${LEMONSHARK_PATCH_VERSION}
    WIRESHARK_MAJOR_VERSION=${WIRESHARK_MAJOR_VERSION}
    WIRESHARK_MINOR_VERSION=${WIRESHARK_MINOR_VERSION}
    WIRESHARK_PATCH_VERSION=${WIRESHARK_PATCH_VERSION}
    LEMONSHARK_SYSTEM=${LEMONSHARK_SYSTEM}
    LEMONSHARK_ARCHITECTURE=${LEMONSHARK_ARCHITECTURE})

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    set_target_properties(
        field_description_demo PROPERTIES MSVC_RUNTIME_LIBRARY
        "MultiThreaded$<$<CONFIG:Debug>:Debug>")

    target_compile_options(field_description_demo PUBLIC /wd26812) # disable enum range warning

endif()
