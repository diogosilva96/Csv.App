using System.ComponentModel;
using Csv.App.Enums;
using Csv.App.Utility;


var option = printSelectOptionMenuAndReadOption("1 - Show file conversion menu\n2 - Run tests\n", 1, 2);
switch (option)
{
    case 1:
        await fileConversionMenu();
        break;
    case 2:
        await runTests();
        break;
}

async Task fileConversionMenu()
{
    var menu = "1 - Convert CSV to JSON \n2 - Convert CSV To XML \n3 - Convert JSON to CSV \n4 - Convert XML to CSV\n5 - Convert JSON to XML\n6 - Convert XML to JSON\n7 - Exit\n";
    IReadOnlyDictionary<int, IConverter> menuConverters = new Dictionary<int, IConverter>()
{
    {1,new CsvToJsonConverter()},
    {2,new CsvToXmlConverter()},
    {3,new JsonToCsvConverter()},
    {4,new XmlToCsvConverter()},
    {5,new JsonToXmlConverter()},
    {6,new XmlToJsonConverter()}
};

    while (true)
    {
        await presentMenu();
    }

    async Task presentMenu()
    {
        var selectedOption = printSelectOptionMenuAndReadOption(menu, 1, 7);

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
}

int printSelectOptionMenuAndReadOption(string menu, int minOptionNumber, int maxOptionNumber)
{
    Console.WriteLine("Please select one of the options below:");
    while (true)
    {

        Console.Write(menu);
        var option = Console.ReadLine();
        if (int.TryParse(option, out var optionNumber) &&
            (optionNumber >= minOptionNumber || optionNumber <= maxOptionNumber)) return optionNumber;
        Console.WriteLine();
        Console.WriteLine("Invalid option, please select a valid option");
    }
}



async Task runTests()
{
    // put all csvs for tests in inputs folder 
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
        Console.WriteLine($"Creating test directory '{testDirectory}'...");
        Directory.CreateDirectory(testDirectory);
        foreach (var converter in testConverters)
        {
            Console.WriteLine($"[Test-{number}] Running test for converter {converter.GetType().Name}");
            var sourcePath = string.Empty;
            var targetPath = string.Empty;
            switch (converter)
            {
                case CsvToJsonConverter:
                    sourcePath = file;
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.json";
                    break;
                case CsvToXmlConverter:
                    sourcePath = file;
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.xml";
                    break;
                case JsonToCsvConverter:
                    sourcePath = $"{testDirectory}\\{nameof(CsvToJsonConverter)}.json";
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.csv";
                    break;
                case XmlToCsvConverter:
                    sourcePath = $"{testDirectory}\\{nameof(CsvToXmlConverter)}.xml";
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.csv";
                    break;
                case XmlToJsonConverter:
                    sourcePath = $"{testDirectory}\\{nameof(CsvToXmlConverter)}.xml";
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.json";
                    break;
                case JsonToXmlConverter:
                    sourcePath = $"{testDirectory}\\{nameof(CsvToJsonConverter)}.json";
                    targetPath = $"{testDirectory}\\{converter.GetType().Name}.xml";
                    break;
                default:
                    throw new ArgumentException($"Invalid implementation type {converter.GetType().Name} for {nameof(IConverter)} interface.");
            }
            Console.WriteLine($"[Test-{number}] Converting file from path '{sourcePath}' to path '{targetPath}'...");
            await converter.ConvertAndWriteToFileAsync(sourcePath, targetPath);
            Console.WriteLine($"[Test-{number}] Test run complete.");
        }

        number++;
    }

    void cleanUpFolder(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        Console.WriteLine($"Cleaning up folder '{path}'...");
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

}