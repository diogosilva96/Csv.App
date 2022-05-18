using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Csv.App.Utility;

public class XmlToJsonConverter : BaseConverter<IEnumerable<XElement?>>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public XmlToJsonConverter(JsonSerializerOptions? serializerOptions = null)
    {
        if (serializerOptions is null)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            return;
        }

        _serializerOptions = serializerOptions;
    }

    protected override IEnumerable<XElement?> PreProcessDataSource(string fileData)
    {
        var rootElement = XElement.Parse(fileData);
        return rootElement.Elements();
    }

    protected override string ConvertData(IEnumerable<XElement?> rowData)
    {
        var dictionaryList = rowData.Select(FindPropertiesForRowElement).ToList();
        return JsonSerializer.Serialize(dictionaryList, _serializerOptions);
    }

    private Dictionary<string, object?> FindPropertiesForRowElement(XElement? rootElement)
    {
        Dictionary<string, object?> dictionary = new();

        if (!rootElement.HasElements) return dictionary;

        foreach (var rowElement in rootElement.Elements())
        {
            if (!rowElement.HasElements)
            {
                dictionary[rowElement.Name.ToString()] = rowElement.Value;
                continue;
            }

            appendChildrenData(rowElement, dictionary);
        }

        return dictionary;

        void appendChildrenData(XElement? currentElement, Dictionary<string, object?> currentDictionary)
        {
            if (!currentDictionary.ContainsKey(currentElement.Name.ToString()))
                currentDictionary[currentElement.Name.ToString()] = new Dictionary<string, object?>();

            currentDictionary = (Dictionary<string, object?>)currentDictionary[currentElement.Name.ToString()];
            foreach (var element in currentElement.Elements())
            {
                if (!element.HasElements)
                {
                    currentDictionary[element.Name.ToString()] = element.Value;

                    continue;
                }

                appendChildrenData(element, currentDictionary);
            }
        }
    }
}