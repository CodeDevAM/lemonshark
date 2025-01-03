import os

original_current_working_dir: str = os.getcwd()

os.chdir(f"""./python/lemonshark""")
os.system(f"""python build.py""")

os.chdir(original_current_working_dir)