
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

// Get all field descriptions
FieldDescription[] fieldDescriptions = FieldDescription.GetAll();
foreach (FieldDescription fieldDescription in fieldDescriptions)
{
    PrintFieldDescription(fieldDescription);
}

Console.WriteLine();

// Get a field description by name
FieldDescription frameFieldDescription = FieldDescription.GetByName("frame");
PrintFieldDescription(frameFieldDescription);

static void PrintFieldDescription(FieldDescription fieldDescription)
{
    string displayName = fieldDescription.DisplayName;
    string name = fieldDescription.Name;
    int type = fieldDescription.Type;
    int id = fieldDescription.Id;
    string type_name = FieldType.GetName(type);

    Console.WriteLine($"""{displayName} (Name: {name}, Type: {type_name}, Id: {id})""");
}