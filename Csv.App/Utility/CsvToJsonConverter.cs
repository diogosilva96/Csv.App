using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Csv.App.Utility
{
    public class CsvToJsonConverter : CsvConverter
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public CsvToJsonConverter()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
        }
        public CsvToJsonConverter(JsonSerializerOptions serializerOptions)
        {
            // in case we may want to override serializer default behaviour
            _serializerOptions = serializerOptions; 
        }
        protected override string ConvertInternal(string[] dataSource)
        {
            var keyValues = new List<Dictionary<string, object>>();
            foreach (var rowData in dataSource)
            {
                var values = rowData.Split();
                var dictionary = new Dictionary<string, object?>();
                for (var j = 0; j < values.Length; j++)
                {
                    var currentHeader = Headers[j];
                    var value = values[j];
                    var splitHeader = currentHeader.Split(InnerChildSeparator);
                    if (splitHeader.Length > 1)
                    {
                        AddHeaderChildren(dictionary, splitHeader, value);
                        continue;
                    }
                    dictionary.Add(currentHeader, string.IsNullOrEmpty(value) ? null : value);
                }
                keyValues.Add(dictionary);
            }

            return JsonSerializer.Serialize(keyValues, _serializerOptions);
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
