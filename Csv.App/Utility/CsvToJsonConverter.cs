using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Csv.App.Models;

namespace Csv.App.Utility
{
    public class CsvToJsonConverter : CsvConverter
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public CsvToJsonConverter(JsonSerializerOptions? serializerOptions = null, CsvConverterConfiguration? converterConfiguration = null) : base(converterConfiguration)
        {
            if (serializerOptions is null)
            {
                _serializerOptions = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };
                return;
            }
            _serializerOptions = serializerOptions; 
        }

        protected override string ConvertData(string[] dataSource)
        {
            var dictionaryList = new List<Dictionary<string, object?>>();
            foreach (var rowData in dataSource)
            {
                var values = rowData.Split(ConverterConfiguration.Separator);
                var currentObject = new Dictionary<string, object?>();
                for (var j = 0; j < values.Length; j++)
                {
                    var currentHeader = Headers[j];
                    var value = values[j];
                    var splitHeader = currentHeader.Split(ConverterConfiguration.InnerChildSeparator);
                    if (splitHeader.Length > 1)
                    {
                        AddHeaderChildren(currentObject, splitHeader, value);
                        continue;
                    }
                    currentObject.Add(currentHeader, string.IsNullOrEmpty(value) ? null : value);
                }
                dictionaryList.Add(currentObject);
            }

            return JsonSerializer.Serialize(dictionaryList, _serializerOptions);
        }

        protected void AddHeaderChildren(Dictionary<string,object?> root, string[] splitHeaders, string? value)
        {
            var currentRoot = root;
            for (var i = 0; i < splitHeaders.Length; i++)
            {
                var header = splitHeaders[i];
                if (i == splitHeaders.Length - 1)
                {
                    if (currentRoot != null) currentRoot[header] = string.IsNullOrEmpty(value) ? null : value;
                    return;
                }
                if (currentRoot != null && currentRoot.ContainsKey(header))
                {
                    currentRoot = (Dictionary<string, object?>)currentRoot[header];
                    continue;
                }
                var headerData = new Dictionary<string, object?>();
                currentRoot.Add(splitHeaders[i], headerData);
                currentRoot = headerData;

            }
        }
    }
}
