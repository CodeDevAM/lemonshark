"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""

import sys
import os
import platform
from typing import *

system: str = platform.system().lower()
architecture: str = platform.machine().lower()

lemonshark_directory: str = os.path.dirname(os.path.realpath(__file__)) + f"/../../lemonshark"
lemonshark_directory = os.path.realpath(lemonshark_directory)
sys.path.append(lemonshark_directory)

from lemonshark import LemonShark, Session, Filter

def main():
    wireshark_directory: str = None
    if system == "windows":
        if architecture == "amd64":
            wireshark_directory = os.path.dirname(os.path.realpath(__file__)) + f"/../../../build/wireshark/windows/amd64/run/RelWithDebInfo"
            wireshark_directory = os.path.realpath(wireshark_directory)

    LemonShark.init([wireshark_directory])

    trace_file_path: str = os.environ["LS_EXAMPLE_FILE"]

    session: Session = Session.create_from_file(trace_file_path, "")

    # Test a valid filter
    filter:str = "frame.len < 150"
    (is_valid, error_message) = Filter.is_valid(filter)
    if(is_valid):
        print(f"'{filter}' is valid.")
    else:
        print(f"'{filter}' is invalid: {error_message}")
        
    # Test an invalid filter
    filter = "frame.len ! 150"
    (is_valid, error_message) = Filter.is_valid(filter)
    if(is_valid):
        print(f"'{filter}' is valid.")
    else:
        print(f"'{filter}' is invalid: {error_message}")

    session.close()
