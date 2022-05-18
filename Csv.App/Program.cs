// See https://aka.ms/new-console-template for more information

using Csv.App.Utility;


var baseOutputFilePath = "C:\\Users\\diogo\\source\\repos\\Csv.App\\Csv.App\\Files\\Outputs";
var baseInputFilePath = "C:\\Users\\diogo\\source\\repos\\Csv.App\\Csv.App\\Files\\Inputs";
var sourceCsvFile = $"{baseInputFilePath}\\TestCsv.csv";
var destinationJsonFile = $"{baseOutputFilePath}\\TestJson.json";

var destinationCsvFile = $"{baseOutputFilePath}\\TestCsv.csv";
var destinationXmlFile = $"{baseOutputFilePath}\\TestXml.xml";



//var csvToJsonConverter = new CsvToJsonConverter();
//var jsonResult = await csvToJsonConverter.ConvertAsync(sourceCsvFile);
//await csvToJsonConverter.ConvertAndWriteToFileAsync(sourceCsvFile, destinationJsonFile);
//Console.WriteLine(jsonResult);

var jsonToCsvConverter = new JsonToCsvConverter();
var csvResult = await jsonToCsvConverter.ConvertAsync(destinationJsonFile);
await jsonToCsvConverter.ConvertAndWriteToFileAsync(destinationJsonFile, destinationCsvFile);
Console.Write(csvResult);




//var csvToXmlConverter = new CsvToXmlConverter();
//var xmlResult = await csvToXmlConverter.ConvertAsync(sourceCsvFile);
//await csvToXmlConverter.ConvertAndWriteToFileAsync(sourceCsvFile, destinationXmlFile);
//Console.Write(xmlResult);

var xmlToCsvConverter = new XmlToCsvConverter();
var xmlToCsvConverterResult = await xmlToCsvConverter.ConvertAsync(destinationXmlFile);
await xmlToCsvConverter.ConvertAndWriteToFileAsync(destinationXmlFile,destinationCsvFile);
Console.WriteLine(xmlToCsvConverterResult);
Console.Read();