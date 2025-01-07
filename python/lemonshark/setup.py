"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

from setuptools import setup

from os import path
working_directory = path.abspath(path.dirname(__file__))

with open(path.join(working_directory, "README.md"), encoding="utf-8") as f:
    long_description = f.read()

setup(
    name="lemonshark",
    version="0.5.0",
    description="lemonshark allows to use Wireshark as a library in an python application",
    long_description=long_description,
    long_description_content_type="text/markdown",
    license="GPL-2.0-only",
    packages=["lemonshark"],
    author="DevAM",
    author_email="email@example.com",
    keywords=["lemonshark", "shark", "Wireshark"],
    url="https://github.com/CodeDevAM/lemonshark",
    include_package_data=True,
    package_data={"": ["native/windows/amd64/*.*", "native/linux/x86_64/*.*"]},
)