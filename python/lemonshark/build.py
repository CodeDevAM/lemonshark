"""
Copyright (c) 2024 DevAM. All Rights Reserved.

SPDX-License-Identifier: GPL-2.0-only
"""
import os

os.system(f"""pip install --upgrade setuptools twine wheel""")
os.system(f"""python setup.py build --build-base=../../build/python/temp egg_info --egg-base=../../build/python/temp bdist_wheel --dist-dir=../../build/python --bdist-dir=../../build/python/temp""")