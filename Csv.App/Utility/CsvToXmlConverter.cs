using System.Xml.Linq;

namespace Csv.App.Utility
{
    public class CsvToXmlConverter : CsvConverter
    {
        protected override string ConvertInternal(string[] dataSource)
        {
            var root = new XElement("root");
            foreach (var rowData in dataSource)
            {
                var values = rowData.Split(ValueSeparator);
                var element = new XElement("element");
                for (var j = 0; j < values.Length; j++)
                {
                    var currentHeader = Headers[j];
                    var value = values[j];
                    var splitHeader = currentHeader.Split('_');
                    if (splitHeader.Length > 1)
                    {
                        AddHeaderChildren(element, splitHeader, value);
                        continue;
                    }
                    element.Add(new XElement(currentHeader, string.IsNullOrEmpty(value) ? null : value));
                }

                root.Add(element);
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
