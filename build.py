"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from io import BytesIO
import os
import platform
import shutil
import subprocess
import sys
import traceback
from typing import *
from time import time
from datetime import timedelta
from zipfile import ZipFile
import requests
from tqdm import tqdm

system: str = platform.system().lower()
architecture: str = platform.machine().lower()

base_build_directory: str = f"""{os.getcwd()}/build"""
lemonshark_src_directory: str = f"""{os.getcwd()}/liblemonshark/src"""
lemonshark_build_directory: str = f"""{base_build_directory}/liblemonshark"""
lemonshark_binary_base_directory: str = f"""{lemonshark_build_directory}/bin"""

lemonshark_demo_src_directory: str = f"""{os.getcwd()}/liblemonshark/lemonshark_demo"""
lemonshark_demo_build_directory: str = f"""{base_build_directory}/lemonshark_demo"""

lemonshark_major_version: int = 0
lemonshark_minor_version: int = 1
lemonshark_patch_version: int = 0

wireshark_git_url: str = "https://gitlab.com/wireshark/wireshark.git"
wireshark_major_version: int = 4
wireshark_minor_version: int = 4
wireshark_patch_version: int = 1
wireshark_suffix_version: str = ""

wireshark_base_directory: str = f"""{os.getcwd()}/wireshark"""
wireshark_build_directory: str = f"""{base_build_directory}/wireshark/{system}/{architecture}"""
wireshark_binary_base_directory: str = f"""{wireshark_build_directory}/run"""
wireshark_libs_directory: str = f"""{base_build_directory}/wireshark-win64-libs-{wireshark_major_version}.{wireshark_minor_version}"""

python_base_directory: str = f"""{os.getcwd()}/python"""
dotnet_base_directory: str = f"""{os.getcwd()}/dotnet"""

win_flex_bison_url: str = "https://github.com/lexxmark/winflexbison/releases/download/v2.5.25/win_flex_bison-2.5.25.zip"
strawberry_perl_url: str = "https://github.com/StrawberryPerl/Perl-Dist-Strawberry/releases/download/SP_54001_64bit_UCRT/strawberry-perl-5.40.0.1-64bit-portable.zip"

def get_msbuild_path():
    path: str = ""
    if system == "windows":
        command: str = f"""\"%ProgramFiles(x86)%\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe"""
        print(f"""call '{command}'""")
        raw_path: bytes = subprocess.check_output(command, shell=True)
        path: str = raw_path.decode("UTF-8")
        path = path.replace("\n", "").replace("\r", "")
        if " " in path:
            path = f"""\"{path}\""""

    return path


def copy_file(source_path: str, target_path: str):
    try:
        if os.path.exists(source_path):
            print(f"""Copy '{source_path}' to '{target_path}'""")
            with open(source_path, "r+b") as input_file:
                content: bytes = input_file.read()
                input_file.close()
                os.makedirs(os.path.dirname(target_path), exist_ok=True)
                with open(target_path, "w+b") as output_file:
                    output_file.write(content)
                    output_file.flush()
                    output_file.close()
    except Exception:
        print(f"""### Error: {traceback.format_exc()}""")
        abort: str = input(f"Continue anyway? (y/n)")
        if abort.lower() != "y":
            sys.exit()


def copy_directory(
    source_directory: str,
    target_directories: Union[List[str], str],
    allowed_file_extensions: Union[List[str], None] = None,
    recursively=True):
    
    if isinstance(target_directories, str):
        target_directories = [target_directories]

    if os.path.exists(source_directory):
        for base_directory, sub_directories, file_names in os.walk(source_directory):
            if not recursively:
                if base_directory != source_directory:
                    continue
            for file_name in file_names:
                if allowed_file_extensions is None or any([file_name.endswith(extension)for extension in allowed_file_extensions]):

                    relative_base_directory_path = os.path.relpath(base_directory, source_directory)
                    source_path: str = os.path.join(base_directory, file_name)
                    for target_directory in target_directories:
                        target_path: str = os.path.join(target_directory, relative_base_directory_path, file_name)
                        copy_file(source_path, target_path)


def clear_directory(directory: str):
    if os.path.exists(directory):
        shutil.rmtree(directory, ignore_errors=True)


def clone_git(git_url: str, target_directory: str, version: Union[str, None]):
    start_time: float = time()
    currend_working_directory: str = os.getcwd()

    print(f"""### Get {git_url} source""")
    try:
        if not os.path.exists(target_directory):
            os.makedirs(target_directory, exist_ok=True)
            command: str = f"""git clone {git_url} {target_directory}"""
            print(f"""call '{command}'""")
            os.system(command)

        if version is not None:
            os.chdir(target_directory)
            os.system(f"""git fetch --all --tags""")
            os.system(f"""git checkout tags/{version}""")
            os.system(f"""git submodule update --init --recursive""")
    except Exception:
        print(f"""### Error: {traceback.format_exc()}""")
        abort: str = input(f"Continue anyway? (y/n)")
        if abort.lower() != "y":
            sys.exit()

    os.chdir(currend_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")


def get_wireshark_dependencies():
    start_time: float = time()
    currend_working_directory: str = os.getcwd()

    print(f"""### Get wireshark dependencies""")
    if system == "windows":
        try:
            os.environ["PLATFORM"] = "Win64"
            os.environ["WIRESHARK_BASE_DIR"] = wireshark_base_directory
            os.environ["WIRESHARK_LIB_DIR"] = wireshark_libs_directory

            # win_flex_bison
            if not os.path.exists(f"""{wireshark_libs_directory}/win_flex_bison"""):
                print(f"""### Get win flex bison""")
                with requests.get(win_flex_bison_url, stream=True) as win_flex_bison_request_response:
                    size = int(win_flex_bison_request_response.headers.get('content-length', 0))
                    with BytesIO() as file:
                        with tqdm(desc=win_flex_bison_url, total=size, unit='iB', unit_scale=True, unit_divisor=4096) as bar:
                            for data in win_flex_bison_request_response.iter_content(chunk_size=4096):
                                size = file.write(data)
                                bar.update(size)
                        with ZipFile(file) as win_flex_bison_zip_file:
                            win_flex_bison_zip_file.extractall(f"""{wireshark_libs_directory}/win_flex_bison""")

            os.environ["PATH"] += f""";{wireshark_libs_directory}/win_flex_bison"""

            # strawberry_perl
            if not os.path.exists(f"""{wireshark_libs_directory}/strawberry_perl"""):
                print(f"""### Get strawberry perl""")
                with requests.get(strawberry_perl_url, stream=True) as strawberry_perl_request_response:
                    size = int(strawberry_perl_request_response.headers.get('content-length', 0))
                    with BytesIO() as file:
                        with tqdm(desc=strawberry_perl_url, total=size, unit='iB', unit_scale=True, unit_divisor=4096) as bar:
                            for data in strawberry_perl_request_response.iter_content(chunk_size=4096):
                                size = file.write(data)
                                bar.update(size)
                        with ZipFile(file) as strawberry_perl_zip_file:
                            strawberry_perl_zip_file.extractall(f"""{wireshark_libs_directory}/strawberry_perl""")
                        

            os.environ["PATH"] += f""";{wireshark_libs_directory}/strawberry_perl/perl/bin"""

        except Exception:
            print(f"""### Error: {traceback.format_exc()}""")
            abort: str = input(f"Continue anyway? (y/n)")
            if abort.lower() != "y":
                sys.exit()

    os.chdir(currend_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")


def integrate_lemonshark():
    start_time: float = time()
    currend_working_directory: str = os.getcwd()

    print(f"""### Integrate lemonshark""")
    try:
        cmake_file_content: str = ""
        cmake_file_content += f"""
            # BEGIN LEMONSHARK
            set(LEMONSHARK_MAJOR_VERSION {lemonshark_major_version})
            set(LEMONSHARK_MINOR_VERSION {lemonshark_minor_version})
            set(LEMONSHARK_PATCH_VERSION {lemonshark_patch_version})
            set(LEMONSHARK_SRC_DIRECTORY {lemonshark_src_directory})
            set(LEMONSHARK_BUILD_DIRECTORY {lemonshark_build_directory})
            set(LEMONSHARK_BINARY_BASE_DIRECTORY {lemonshark_binary_base_directory})

            set(LEMONSHARK_DEMO_SRC_DIRECTORY {lemonshark_demo_src_directory})
            set(LEMONSHARK_DEMO_BUILD_DIRECTORY {lemonshark_demo_build_directory})

            set(WIRESHARK_MAJOR_VERSION {wireshark_major_version})
            set(WIRESHARK_MINOR_VERSION {wireshark_minor_version})
            set(WIRESHARK_PATCH_VERSION {wireshark_patch_version})
            set(WIRESHARK_BASE_DIRECTORY {wireshark_base_directory})
            set(WIRESHARK_BINARY_BASE_DIRECTORY {wireshark_binary_base_directory})

            set(PYTHON_BASE_DIRECTORY {python_base_directory})

            set(DOTNET_BASE_DIRECTORY {dotnet_base_directory})

            set(LEMONSHARK_SYSTEM {system})
            set(LEMONSHARK_ARCHITECTURE {architecture})
            """

        if system == "windows":
            cmake_file_content += f"""
                set(ENV{{PLATFORM}} "Win64")
                set(ENV{{WIRESHARK_BASE_DIR}} {wireshark_base_directory})
                set(ENV{{WIRESHARK_LIB_DIR}} {wireshark_libs_directory})
                """

            if "WIRESHARK_QT6_PREFIX_PATH" in os.environ and os.environ["WIRESHARK_QT6_PREFIX_PATH"] != "":
                cmake_file_content += f"""
                    set(BUILD_wireshark ON)
                    """
            else:
                cmake_file_content += f"""set(BUILD_wireshark OFF)\n"""

            cmake_file_content = cmake_file_content.replace("\\", "/")

        cmake_file_content = cmake_file_content.replace("    ", "")
        cmake_file_content += f"""# END LEMONSHARK\n"""

        if os.path.exists(f"""{wireshark_base_directory}/CMakeLists.backup"""):
            with open(f"""{wireshark_base_directory}/CMakeLists.backup""", "r", encoding="UTF-8",) as cmake_file:
                cmake_file_content += cmake_file.read()
                cmake_file.close()
        else:
            with open(f"""{wireshark_base_directory}/CMakeLists.txt""", "r", encoding="UTF-8", ) as cmake_file:
                cmake_file_content_backup: str = cmake_file.read()
                cmake_file.close()

            with open(f"""{wireshark_base_directory}/CMakeLists.backup""", "w", encoding="UTF-8", ) as cmake_file:
                cmake_file.write(cmake_file_content_backup)
                cmake_file.flush()
                cmake_file.close()

            cmake_file_content += cmake_file_content_backup

        cmake_file_content += """
            # BEGIN LEMONSHARK
            
            add_subdirectory(${LEMONSHARK_SRC_DIRECTORY} ${LEMONSHARK_BUILD_DIRECTORY})
            add_dependencies(liblemonshark epan)
            add_dependencies(liblemonshark wsutil)
            add_dependencies(liblemonshark wiretap)

            add_subdirectory(${LEMONSHARK_DEMO_SRC_DIRECTORY} ${LEMONSHARK_DEMO_BUILD_DIRECTORY})
            add_dependencies(lemonshark_demo liblemonshark)

            # END LEMONSHARK
            """.replace(
            "    ", ""
        )

        with open(f"""{wireshark_base_directory}/CMakeLists.txt""", "w", encoding="UTF-8") as cmake_file:
            cmake_file.write(cmake_file_content)
            cmake_file.flush()
            cmake_file.close()

    except Exception:
        print(f"""### Error: {traceback.format_exc()}""")
        abort: str = input(f"Continue anyway? (y/n)")
        if abort.lower() != "y":
            sys.exit()

    os.chdir(currend_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")


def generate_wireshark():
    start_time: float = time()
    currend_working_directory: str = os.getcwd()

    print(f"""### Generate wireshark""")
    try:
        os.makedirs(wireshark_build_directory, exist_ok=True)
        os.chdir(wireshark_build_directory)

        cmake_command: str = f"""cmake {wireshark_base_directory} """
        if system == "windows":
            cmake_command += """ -G "Visual Studio 17 2022" -A x64"""
            cmake_command = cmake_command.replace("\\", "/")

        cmake_command += " -DCMAKE_BUILD_TYPE=RelWithDebInfo"
        print(f"""call '{cmake_command}'""")
        os.system(cmake_command)

    except Exception:
        print(f"""### Error: {traceback.format_exc()}""")
        abort: str = input(f"Continue anyway? (y/n)")
        if abort.lower() != "y":
            sys.exit()

    os.chdir(currend_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")


def build_wireshark():
    start_time: float = time()
    currend_working_directory: str = os.getcwd()

    print(f"""### Build wireshark""")
    try:
        os.chdir(wireshark_build_directory)
        if system == "windows":
            msbuild_path: str = get_msbuild_path()
            command: str = f"""{msbuild_path} /m /p:Configuration=RelWithDebInfo /p:Platform=x64 Wireshark.sln"""
            print(f"""call '{command}'""")
            os.system(command)

        elif system == "linux":
            command: str = f"""make -j{os.cpu_count()} CXXFLAGS='-fPIC'"""
            print(f"""call '{command}'""")
            os.system(command)

    except Exception:
        print(f"""### Error: {traceback.format_exc()}""")
        abort: str = input(f"Continue anyway? (y/n)")
        if abort.lower() != "y":
            sys.exit()

    os.chdir(currend_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")


def main():
    if " " in os.getcwd():
        raise Exception(f"Path of current working directory must not contain whitespaces. ({os.getcwd()})")

    start_time: float = time()

    current_working_directory: str = os.getcwd()

    wireshark_tag: str = f"v{wireshark_major_version}.{wireshark_minor_version}.{wireshark_patch_version}{wireshark_suffix_version}"

    clone_git(wireshark_git_url, wireshark_base_directory, wireshark_tag)

    get_wireshark_dependencies()

    integrate_lemonshark()

    generate_wireshark()

    build_wireshark()

    os.chdir(current_working_directory)

    duration: float = time() - start_time
    print(f"""### Done in {timedelta(seconds=duration)} s""")

if __name__ == "__main__":
    main()