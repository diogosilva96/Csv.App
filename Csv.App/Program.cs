// See https://aka.ms/new-console-template for more information

using Csv.App.Utility;



var filePath = "C:\\Users\\diogo\\source\\repos\\Csv.App\\Csv.App\\Files\\TestCsv.csv";

var converter = new CsvToJsonConverter();
var res = await converter.ConvertAsync(filePath);

Console.WriteLine(res);
var xmlConverter = new CsvToXmlConverter();
var res2 = await xmlConverter.ConvertAsync(filePath);
Console.Write(res2);
Console.Read();