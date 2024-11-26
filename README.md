# lemonshark

![icon](https://raw.githubusercontent.com/CodeDevAM/lemonshark/main/icon.png)

lemonshark allows to use Wireshark as a library.

`liblemonshark` offers an FFI friendly C-API and bindings for C# (.NET) and Python.

See `lemonshark_demo` for a simple example in `C`.

## Build

### Windows

Building `liblemonshark` on Windows requires Git, Python, CMake and Visual Studio 2022 and a number of Python packages.

Run `build.py` within a `x64 Native Tools Command Prompt for VS 2022`.

### Linux
Building `liblemonshark` on Linux requires several tools Git, Python, CMake, gcc and number of Python packages.

In addition to that Wireshark itself has a number of dependencies. On debian based systems these can be installed with a script provided called [debian-setup.sh](https://gitlab.com/wireshark/wireshark/-/blob/master/tools/debian-setup.sh) in the Wireshark repo.

Having all dependencies installed run `build.py`.

## Contribution

Contributions are welcome.

## License

Copyright (c) 2024 DevAM

[GPL-2.0](https://www.gnu.org/licenses/old-licenses/gpl-2.0.txt)