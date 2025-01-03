import os

original_current_working_dir: str = os.getcwd()

os.system(f"""python build.py""")

os.system(f"""wsl python3 ./build.py""")

os.chdir(original_current_working_dir)