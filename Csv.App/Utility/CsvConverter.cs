using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Csv.App.Utility
{
    public abstract class CsvConverter
    {
        protected readonly string ValueSeparator;
        protected readonly string InnerChildSeparator;
        protected string[] Headers;
        public CsvConverter(string valueSeparator = ",", string innerChildSeparator = "_")
        {
            ValueSeparator = valueSeparator;
            InnerChildSeparator = innerChildSeparator;
        }

        
        public async Task<string> ConvertAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Could not find file '{filePath}'.");
            }

            var fileData = await File.ReadAllLinesAsync(filePath);

            if (fileData.Length <= 0)
            {
                throw new ArgumentException($"The file '{filePath}' does not contain any data.");
            }

            
            Headers = fileData[0].Split(ValueSeparator);

            var dataSource = fileData.Skip(1)
                                                         .Take(fileData.Length - 1)
                                                         .ToArray();

            return ConvertInternal(dataSource);
        }

        protected abstract string ConvertInternal(string[] dataSource);


    }
}
