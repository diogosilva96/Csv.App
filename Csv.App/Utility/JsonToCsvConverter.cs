using System.Text;
using System.Text.Json.Nodes;
using Csv.App.Models;

namespace Csv.App.Utility;

public class JsonToCsvConverter : BaseConverter<string>
{
    protected readonly CsvConverterConfiguration ConverterConfiguration;
    public JsonToCsvConverter(CsvConverterConfiguration? converterConfiguration = null)
    {
        if (converterConfiguration is null)
        {
            ConverterConfiguration = new CsvConverterConfiguration();
            return;
        }
        ConverterConfiguration = converterConfiguration;
    }

    protected override string DoDataSourceProcessing(string fileData)
    {
        return fileData;
    }

    protected override string ConvertData(string dataSource)
    {
        var jsonData = JsonNode.Parse(dataSource)?.AsArray();

        var propertyNames = new List<string>();
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
                        if (childPropertyNames.All(cp =>
                                                       propertyNames.All(p =>
                                                                             p.Equals(cp,
                                                                                 StringComparison
                                                                                     .InvariantCultureIgnoreCase))))
                            break;
                        foreach (var propName in childPropertyNames) appendPropertyNameIfNotExists(propName);
                        break;
                    default:
                        appendPropertyNameIfNotExists(keyValue.Key);
                        break;
                }
        }

        var stringBuilder = new StringBuilder();
        //append headers
        stringBuilder.AppendLine(string.Join(",", propertyNames));
        // build actual row data
        foreach (var node in jsonData)
        {
            var currentRow = string.Empty;
            var alreadyInitialized = false;
            foreach (var property in propertyNames)
            {
                // this means property has no children
                if (!property.Contains('_'))
                {
                    var jNode = node.AsObject().FirstOrDefault(n => n.Key == property);
                    if (jNode.Key is null) continue;
                    currentRow += getPropertyValueStr(jNode.Value.ToString(), alreadyInitialized);
                    alreadyInitialized = true;
                    continue;
                }

                var value = FindPropertyValueInChildren(node.AsObject(), property);
                currentRow += getPropertyValueStr(value, alreadyInitialized);
                alreadyInitialized = true;
            }

            stringBuilder.AppendLine(currentRow);
        }

        return stringBuilder.ToString();

        string getPropertyValueStr(string? propertyValue, bool alreadyInitialized)
        {
            return !alreadyInitialized ? $"{propertyValue}" : $",{propertyValue}";
        }

        void appendPropertyNameIfNotExists(string propertyName)
        {
            if (propertyNames.Any(p => p.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))) return;
            propertyNames.Add(propertyName);
        }
    }

    public string? FindPropertyValueInChildren(JsonObject rootObject, string propertyPath)
    {
        var paths = propertyPath.Split("_").ToList();
        string? foundValue = null;
        findValueForPath(rootObject, paths);
        return foundValue;

        void findValueForPath(JsonObject currentObject, List<string> pathsToVisit)
        {
            if (pathsToVisit.Count() <= 0 || !string.IsNullOrEmpty(foundValue)) return;
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

    public string[] GetChildPropertyNames(JsonObject rootObject, string baseName)
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
                        var propertyName = $"{currentStr}_{node.Key}";
                        if (propertiesFound.Any(p => p.Equals(propertyName))) continue;
                        propertiesFound.Add(propertyName);
                        currentStr = baseName;
                        break;
                    }
                    case JsonObject obj:
                    {
                        if (!string.IsNullOrEmpty(currentStr)) currentStr += "_";
                        currentStr += node.Key;

                        buildPropertyNames(obj, currentStr);
                        break;
                    }
                }
        }
    }
}