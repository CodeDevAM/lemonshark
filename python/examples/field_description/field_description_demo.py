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

from lemonshark import LemonShark, Session, FieldDescription, FieldType


def print_field_description(field_description: FieldDescription) -> None:
    display_name: str = field_description.get_display_name()
    name: str = field_description.get_name()
    type: int = field_description.get_type()
    id: int = field_description.get_id()
    parent_id: int = field_description.get_parent_id()
    type_name: str = FieldType.get_name(type)

    print(f"""{display_name} (Name: {name}, Type: {type_name}, Id: {id})""", end="")
    if parent_id >= 0:
        print(f""", Parent: {parent_id}""", end="")
    print(")")


def main():
    wireshark_directory: str = None
    if system == "windows":
        if architecture == "amd64":
            wireshark_directory = os.path.dirname(os.path.realpath(__file__)) + f"/../../../build/wireshark/windows/amd64/run/RelWithDebInfo"
            wireshark_directory = os.path.realpath(wireshark_directory)

    LemonShark.init([wireshark_directory])

    trace_file_path: str = os.environ["LS_EXAMPLE_FILE"]

    session: Session = Session.create_from_file(trace_file_path, "")

    try:
        # Get all field descriptions
        field_descriptions: List[FieldDescription] = FieldDescription.get_all()

        for field_description in field_descriptions:
            print_field_description(field_description)

        print("")
        
        # Get a field description by name        
        frame_field_description: FieldDescription = FieldDescription.get_by_name("frame")
        print_field_description(frame_field_description)

    except Exception as ex:
        print(ex)
    finally:
        session.close()


if __name__ == "__main__":
    main()
