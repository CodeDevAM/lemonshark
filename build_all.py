import os

original_current_working_dir: str = os.getcwd()

os.system(f"""python build_wireshark.py""")

os.chdir(original_current_working_dir)

os.system(f"""python build_dotnet.py""")

os.chdir(original_current_working_dir)

os.system(f"""python build_python.py""")

os.chdir(original_current_working_dir)