using System.Text;
using System.Text.Json.Nodes;
using Csv.App.Models;

namespace Csv.App.Utility;

public class JsonToCsvConverter : BaseConverter<JsonArray?>
{
    protected readonly CsvConverterConfiguration ConverterConfiguration;
    private HashSet<string> _headers;
    public JsonToCsvConverter(CsvConverterConfiguration? converterConfiguration = null)
    {
        if (converterConfiguration is null)
        {
            ConverterConfiguration = new CsvConverterConfiguration();
            return;
        }
        ConverterConfiguration = converterConfiguration;
    }

    protected override JsonArray? PreProcessDataSource(string fileData)
    {
        _headers = new HashSet<string>();
        var jsonData = JsonNode.Parse(fileData)?.AsArray();
        //get all json property names
        foreach (var node in jsonData)
        {
            // iterate over every row to ensure all properties were found
            // Note: we could just iterate over first row, but there may be some null properties that are not written in the json for example
            foreach (var keyValue in node.AsObject())
                switch (keyValue.Value)
                {
                    case JsonObject obj:
                        var childPropertyNames = GetChildPropertyNames(obj, keyValue.Key);
                        if (childPropertyNames.All(cp => _headers.All(p => p.Equals(cp, StringComparison.InvariantCultureIgnoreCase))))
                            break;
                        foreach (var propName in childPropertyNames) appendPropertyNameIfNotExists(propName);
                        break;
                    default:
                        appendPropertyNameIfNotExists(keyValue.Key);
                        break;
                }
        }

        return jsonData;

        void appendPropertyNameIfNotExists(string propertyName)
        {
            if (_headers.Any(p => p.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))) return;
            _headers.Add(propertyName);
        }
    }
    
    protected override string ConvertData(JsonArray? jsonData)
    {
        if (!_headers.Any())
        {
            return string.Empty;
        }
        StringBuilder builder = new ();
        var initializedRow = false;
        //append headers
        builder.AppendLine(string.Join(ConverterConfiguration.Separator, _headers));
        // build actual row data
        foreach (var node in jsonData)
        {
            var currentRow = string.Empty;
            initializedRow = false;
            foreach (var property in _headers)
            {
                // this means property has no children
                if (!property.Contains(ConverterConfiguration.InnerChildSeparator))
                {
                    var jNode = node.AsObject().FirstOrDefault(n => n.Key == property);
                    if (jNode.Key is null) continue;
                    currentRow += getPropertyValue(jNode.Value.ToString());
                    initializedRow = true;
                    continue;
                }

                var value = FindPropertyValueInChildren(node.AsObject(), property);
                currentRow += getPropertyValue(value);
                initializedRow = true;
            }

            builder.AppendLine(currentRow);
        }

        return builder.ToString();

        string getPropertyValue(string? propertyValue) =>!initializedRow ? $"{propertyValue}" : $"{ConverterConfiguration.Separator}{propertyValue}";
    }

    private string? FindPropertyValueInChildren(JsonObject rootObject, string propertyPath)
    {
        var paths = propertyPath.Split(ConverterConfiguration.InnerChildSeparator).ToList();
        string? foundValue = null;
        findValueForPath(rootObject, paths);
        return foundValue;

        void findValueForPath(JsonObject currentObject, List<string> pathsToVisit)
        {
            if (!pathsToVisit.Any() || !string.IsNullOrEmpty(foundValue)) return;
            var currentPath = pathsToVisit[0];
            for (var i = 0; i < pathsToVisit.Count; i++)
            {
                if (!currentObject.TryGetPropertyValue(currentPath, out var node)) return;
                pathsToVisit.RemoveRange(0, 1);
                switch (node)
                {
                    case JsonValue value:
                        if (pathsToVisit.Count == 0) foundValue = value.ToString();
                        break;
                    case JsonObject:
                        findValueForPath(node.AsObject(), pathsToVisit);
                        break;
                }
            }
        }
    }

    private string[] GetChildPropertyNames(JsonObject rootObject, string baseName)
    {
        var propertiesFound = new HashSet<string>();
        buildPropertyNames(rootObject);
        return propertiesFound.ToArray();

        void buildPropertyNames(JsonObject rootObject, string currentStr = "")
        {
            if (string.IsNullOrEmpty(currentStr)) currentStr = baseName;
            foreach (var node in rootObject)
                // build property names for each node in the root object
                switch (node.Value)
                {
                    case JsonValue:
                    {
                        var propertyName = $"{currentStr}{ConverterConfiguration.InnerChildSeparator}{node.Key}";
                        currentStr = baseName;
                        if (propertiesFound.Any(p => p.Equals(propertyName))) continue;
                        propertiesFound.Add(propertyName);
                        
                        break;
                    }
                    case JsonObject obj:
                    {
                        if (!string.IsNullOrEmpty(currentStr)) currentStr += ConverterConfiguration.InnerChildSeparator;
                        currentStr += node.Key;

                        buildPropertyNames(obj, currentStr);
                        break;
                    }
                }
        }
    }
}