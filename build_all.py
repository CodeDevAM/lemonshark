import os

original_current_working_dir: str = os.getcwd()

os.system(f"""python build.py""")

os.system(f"""wsl python3 ./build.py""")

os.chdir(f"""./dotnet/LemonShark""")
os.system(f"""dotnet build -c Release -o ../../build/dotnet/LemonShark/Release""")
os.chdir(original_current_working_dir)

os.chdir(f"""./python/lemonshark""")
os.system(f"""python build.py""")
os.chdir(original_current_working_dir)