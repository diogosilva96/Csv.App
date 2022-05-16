using System.Xml.Linq;

namespace Csv.App.Utility
{
    public class CsvToXmlConverter : CsvConverter
    {
        protected override string ConvertInternal(string[] dataSource)
        {
            var root = new XElement("data");
            for (var i = 0; i < dataSource.Length; i++)
            {
                var values = dataSource[i].Split(ValueSeparator);
                var rowElement = new XElement("row");
                rowElement.Add(new XAttribute("number", i));
                for (var j = 0; j < values.Length; j++)
                {
                    var currentHeader = Headers[j];
                    var value = values[j];
                    var splitHeader = currentHeader.Split(InnerChildSeparator);
                    if (splitHeader.Length > 1)
                    {
                        AddHeaderChildren(rowElement, splitHeader, value);
                        continue;
                    }
                    rowElement.Add(new XElement(currentHeader, string.IsNullOrEmpty(value) ? null : value));
                    
                }
                root.Add(rowElement);
            }

            return root.ToString();
        }

        protected void AddHeaderChildren(XElement root, string[] splitHeaders, string? value)
        {
            var currentRoot = root;
            for (var i = 0; i < splitHeaders.Length; i++)
            {
                var header = splitHeaders[i];

                if (i == splitHeaders.Length - 1)
                {
                    currentRoot.Add(new XElement(header, string.IsNullOrEmpty(value) ? null : value));
                    return;
                }
                if (currentRoot.Elements().Any(e => e.Name == header))
                {
                    currentRoot = currentRoot.Elements().First(e => e.Name == header);
                    continue;
                }
                var elementData = new XElement(header);
                currentRoot.Add(elementData);
                currentRoot = elementData;

            }
        }
    }
}
