using System.ComponentModel;
using Csv.App.Enums;
using Csv.App.Utility;

const string Menu = "1 - Convert CSV to JSON \n2 - Convert CSV To XML \n3 - Convert JSON to CSV \n4 - Convert XML to CSV\n5 - Convert JSON to XML\n6 - Convert XML to JSON\n7 - Exit\n";
IReadOnlyDictionary<int,IConverter> menuConverters = new Dictionary<int,IConverter>()
{
    {1,new CsvToJsonConverter()},
    {2,new CsvToXmlConverter()},
    {3,new JsonToCsvConverter()},
    {4,new XmlToCsvConverter()},
    {5,new JsonToXmlConverter()},
    {6,new XmlToJsonConverter()}
};

while (false)
{
    await presentMenu();
}

async Task presentMenu()
{
    Console.WriteLine("Please select one of the options below:");
    var selectedOption = -1;
    while (true)
    {
        
        Console.Write(Menu);
        var option = Console.ReadLine();
        if (!int.TryParse(option, out var optionNumber) || optionNumber is < 0 or > 8)
        {
            Console.WriteLine();
            Console.WriteLine("Invalid option, please select a valid option");
            continue;
        }

        selectedOption = optionNumber;
        break;
    }

    if (selectedOption == 7)
    {
        Environment.Exit(0);
    }

    var sourcePath = readFilePath(EntryFileType.Source);
    var destinationPath = readFilePath(EntryFileType.Destination);
    try
    {
        Console.WriteLine($"Converting file from '{sourcePath}' to '{destinationPath}'...");
        var converter = menuConverters[selectedOption];
        await converter.ConvertAndWriteToFileAsync(sourcePath, destinationPath);
        Console.WriteLine("File conversion complete.");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred during file conversion. Error details: {ex}");
        Console.WriteLine("");
    }

    string readFilePath(EntryFileType type)
    {
        var filePath = string.Empty;
        var fileType = type.ToString().ToLowerInvariant();
        while (true)
        {
            Console.WriteLine($"Please type the path to the {fileType} file:");
            filePath = Console.ReadLine();
            filePath = filePath.Trim();
            if (type == EntryFileType.Destination || File.Exists(filePath))
            {
                return filePath;
            }
            Console.WriteLine();
            Console.WriteLine($"The file '{filePath}' does not exist, please specify a valid path for the {fileType} file.");
        }
    }
}






var inputsFolder = $"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}\\Files\\Inputs";
var outputsFolder = $"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}\\Files\\Outputs";

checkIfFolderExists(inputsFolder);
checkIfFolderExists(outputsFolder);
cleanUpFolder(outputsFolder);
var csvFiles = Directory.EnumerateFiles(inputsFolder, "*.csv");

var number = 1;
var testConverters = new List<IConverter>()
{
    new CsvToJsonConverter(),
    new CsvToXmlConverter(),
    new JsonToCsvConverter(),
    new XmlToCsvConverter(),
    new JsonToXmlConverter(),
    new XmlToJsonConverter()
};
foreach (var file in csvFiles)
{
    var testDirectory = $"{outputsFolder}\\Test-{number}";
    Directory.CreateDirectory(testDirectory);
    foreach (var converter in testConverters)
    {
        switch (converter)
        {
            case CsvToJsonConverter:
                await converter.ConvertAndWriteToFileAsync(file, $"{testDirectory}\\{nameof(CsvToJsonConverter)}.json");
                break;
            case CsvToXmlConverter:
                await converter.ConvertAndWriteToFileAsync(file, $"{testDirectory}\\{nameof(CsvToXmlConverter)}.xml");
                break;
            case JsonToCsvConverter:
                await converter.ConvertAndWriteToFileAsync($"{testDirectory}\\{nameof(CsvToJsonConverter)}.json", $"{testDirectory}\\{nameof(JsonToCsvConverter)}.csv");
                break;
            case XmlToCsvConverter:
                await converter.ConvertAndWriteToFileAsync($"{testDirectory}\\{nameof(CsvToXmlConverter)}.xml", $"{testDirectory}\\{nameof(XmlToCsvConverter)}.csv");
                break;
            case XmlToJsonConverter:
                await converter.ConvertAndWriteToFileAsync($"{testDirectory}\\{nameof(CsvToXmlConverter)}.xml", $"{testDirectory}\\{nameof(XmlToJsonConverter)}.json");
                break;
            case JsonToXmlConverter:
                await converter.ConvertAndWriteToFileAsync($"{testDirectory}\\{nameof(CsvToJsonConverter)}.json", $"{testDirectory}\\{nameof(JsonToXmlConverter)}.json");
                break;
        }
    }
    number++;
}

void cleanUpFolder(string path)
{
    var directoryInfo = new DirectoryInfo(path);
    foreach (var file in directoryInfo.GetFiles())
    {
     file.Delete();   
    }
    foreach (var directory in directoryInfo.GetDirectories())
    {
        directory.Delete(true);
    }
}
void checkIfFolderExists(string path)
{
    if (!Directory.Exists(path))
    {
        throw new ArgumentException($"Folder with path '{path}' does not exist.");
    }
}