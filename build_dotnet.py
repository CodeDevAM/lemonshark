import os

original_current_working_dir: str = os.getcwd()

os.chdir(f"""./dotnet/LemonShark""")
os.system(f"""dotnet build -c Release -p:PlatformTarget=AnyCPU -o ../../build/dotnet/LemonShark/Release""")

os.chdir(original_current_working_dir)
