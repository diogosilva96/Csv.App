using System.Xml.Linq;
using Csv.App.Models;

namespace Csv.App.Utility
{
    public class CsvToXmlConverter : CsvConverter
    {

        public CsvToXmlConverter(CsvConverterConfiguration? converterConfiguration = null) : base(converterConfiguration)
        {
            //in case we may want to override ConverterConfig
        }
        protected override string ConvertData(string[] dataSource)
        {
            XElement root = new ("data");
            for (var i = 0; i < dataSource.Length; i++)
            {
                if (string.IsNullOrEmpty(dataSource[i]))
                {
                    continue;
                }
                var values = dataSource[i].Split(ConverterConfiguration.Separator);
                XElement rowElement = new ("row");
                rowElement.Add(new XAttribute("number", i));
                for (var j = 0; j < values.Length; j++)
                {
                    var currentHeader = Headers[j];
                    var value = values[j];
                    var splitHeader = currentHeader.Split(ConverterConfiguration.InnerChildSeparator);
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
                XElement elementData = new (header);
                currentRoot.Add(elementData);
                currentRoot = elementData;

            }
        }

    }
}
