// See https://aka.ms/new-console-template for more information

using Csv.App.Utility;


var baseOutputFilePath = "C:\\Users\\diogo\\source\\repos\\Csv.App\\Csv.App\\Files\\Outputs";
var baseInputFilePath = "C:\\Users\\diogo\\source\\repos\\Csv.App\\Csv.App\\Files\\Inputs";
var sourceCsvFile = $"{baseInputFilePath}\\TestCsv.csv";
var destinationJsonFile = $"{baseOutputFilePath}\\TestJson.json";

var destinationCsvFile = $"{baseOutputFilePath}\\TestCsv.csv";
var destinationXmlFile = $"{baseOutputFilePath}\\TestXml.xml";



//IFileConverter csvToJsonConverter = new CsvToJsonConverter();
//var jsonResult = await csvToJsonConverter.ConvertFromFileAsync(sourceCsvFile);
//await csvToJsonConverter.ConvertAndWriteToFileAsync(sourceCsvFile, destinationJsonFile);
//Console.WriteLine(jsonResult);

IFileConverter jsonToXmlConverter = new JsonToXmlConverter();
var jsonToXmlResult = await jsonToXmlConverter.ConvertFromFileAsync(destinationJsonFile);
Console.Write(jsonToXmlResult);

IFileConverter xmlToJsonConverter = new XmlToJsonConverter();
var xmlToJsonResult = await xmlToJsonConverter.ConvertFromFileAsync(destinationXmlFile);
Console.WriteLine(xmlToJsonResult);

IFileConverter jsonToCsvConverter = new JsonToCsvConverter();
var csvResult = await jsonToCsvConverter.ConvertFromFileAsync(destinationJsonFile);
await jsonToCsvConverter.ConvertAndWriteToFileAsync(destinationJsonFile, destinationCsvFile);
Console.Write(csvResult);




//IFileConverter csvToXmlConverter = new CsvToXmlConverter();
//var xmlResult = await csvToXmlConverter.ConvertFromFileAsync(sourceCsvFile);
//await csvToXmlConverter.ConvertAndWriteToFileAsync(sourceCsvFile, destinationXmlFile);
//Console.Write(xmlResult);

IFileConverter xmlToCsvConverter = new XmlToCsvConverter();
var xmlToCsvConverterResult = await xmlToCsvConverter.ConvertFromFileAsync(destinationXmlFile);
await xmlToCsvConverter.ConvertAndWriteToFileAsync(destinationXmlFile,destinationCsvFile);
Console.WriteLine(xmlToCsvConverterResult);
Console.Read();