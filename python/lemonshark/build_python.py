"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""
import os
os.system(f"""pip install --upgrade setuptools twine wheel packaging build""")
os.system(f"""python -m build --wheel --outdir ../../build/python .""")