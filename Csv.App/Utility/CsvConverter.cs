using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Csv.App.Models;

namespace Csv.App.Utility
{
    public abstract class CsvConverter : BaseConverter<string[]>
    {
        protected readonly CsvConverterConfiguration ConverterConfiguration;
        protected string[] Headers;

        protected CsvConverter(CsvConverterConfiguration? converterConfiguration = null)
        {
            if (converterConfiguration is null)
            {
                ConverterConfiguration = new CsvConverterConfiguration();
                return;
            }
            ConverterConfiguration = converterConfiguration;
        }

        protected override string[] PreProcessDataSource(string fileData)
        {
            //get data as string array (per line)
            var data = fileData.Replace("\r", "").Split("\n");
            // process headers
            Headers = data[0].Split(ConverterConfiguration.Separator);
            // remove headers from dataSource
            var dataSource = data.Skip(1)
                                     .Take(fileData.Length - 1)
                                     .ToArray();
            return dataSource;
        }
    }
}
