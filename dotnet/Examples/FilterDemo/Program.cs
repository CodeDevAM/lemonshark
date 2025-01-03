using LemonShark;
using System.Reflection;
using System.Runtime.InteropServices;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string traceFilePath = Environment.GetEnvironmentVariable("LS_EXAMPLE_FILE", EnvironmentVariableTarget.Process);

string assemblyDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
string wiresharkDirectory = null;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
    {
        wiresharkDirectory = Path.GetFullPath($"{assemblyDirectory}/../../../../../../build/wireshark/windows/amd64/run/RelWithDebInfo");
    }
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
    {
        wiresharkDirectory = Path.GetFullPath($"{assemblyDirectory}/../../../../../../build/wireshark/linux/x86_64/run");
    }
}

LemonShark.LemonShark.Init([wiresharkDirectory]);

using Session session = Session.CreateFromFile(traceFilePath, "");

// Test a valid filter
string filter = "frame.len < 150";
bool isValidFilter = Filter.IsValid(filter, out string errorMessage);
if (isValidFilter)
{
    Console.WriteLine($"'{filter}' is valid");
}
else
{
    Console.WriteLine($"'{filter}' is invalid: {errorMessage}");
}

// Test an invalid filter
filter = "frame.len ! 150";
isValidFilter = Filter.IsValid("frame.len ! 150", out errorMessage);
if (isValidFilter)
{
    Console.WriteLine($"'{filter}' is valid");
}
else
{
    Console.WriteLine($"'{filter}' is invalid: {errorMessage}");
}