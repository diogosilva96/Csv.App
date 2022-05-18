using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Csv.App.Models;

namespace Csv.App.Utility
{
    public class XmlToCsvConverter : BaseConverter<IEnumerable<XElement?>>
    {
        private HashSet<string> _headers;
        protected CsvConverterConfiguration ConverterConfiguration;
        public XmlToCsvConverter(CsvConverterConfiguration? converterConfiguration = null)
        {
            if (converterConfiguration is null)
            {
                ConverterConfiguration = new CsvConverterConfiguration();
                return;
            }
            ConverterConfiguration = converterConfiguration;
        }
        protected override IEnumerable<XElement?> PreProcessDataSource(string fileData)
        {
            _headers = new HashSet<string>();
            var rootElement = XElement.Parse(fileData);
            foreach (var element in rootElement.Elements())
            {
                // iterate over every element/row to ensure all properties were found
                // Note: we could just iterate over first row, but there may be some null properties that are not written in the json for example
                foreach (var rowElement in element.Elements())
                {
                    var propertyNames = FindPropertyNamesForElement(rowElement);
                    foreach (var propertyName in propertyNames)
                    {
                        appendPropertyNameIfNotExists(propertyName);
                    }
                }
            }

            return rootElement.Elements();

            void appendPropertyNameIfNotExists(string propertyName)
            {
                if (_headers.Any(p => p.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))) return;
                _headers.Add(propertyName);
            }
        }
        private string[] FindPropertyNamesForElement(XElement? rootElement)
        {
            if (!rootElement.HasElements)
            {
                return new []{ rootElement.Name.ToString()};
            }
            var propertiesFound = new HashSet<string>();
            buildPropertyNames(rootElement);
            return propertiesFound.ToArray();

            void buildPropertyNames(XElement? currentElement, string currentStr = "")
            {
                if (string.IsNullOrEmpty(currentStr)) currentStr = rootElement.Name.ToString();
                foreach (var element in currentElement.Elements())
                {
                    if (element.HasElements)
                    {
                        if (!string.IsNullOrEmpty(currentStr)) currentStr += ConverterConfiguration.InnerChildSeparator;
                        currentStr += element.Name.ToString();
                        buildPropertyNames(element,currentStr);
                        continue;
                    }
                    var propertyName = $"{currentStr}{ConverterConfiguration.InnerChildSeparator}{element.Name}";
                    currentStr = rootElement.Name.ToString();
                    if (propertiesFound.Any(p => p.Equals(propertyName))) continue;
                    propertiesFound.Add(propertyName);
                }
            }
        }
        protected override string ConvertData(IEnumerable<XElement?> rowData)
        {
            if (!_headers.Any())
            {
                return string.Empty;
            }
            StringBuilder builder = new ();
            builder.AppendLine(string.Join(ConverterConfiguration.Separator, _headers));
            bool alreadyInitialized;
            foreach (var rowElement in rowData)
            {
                var row = string.Empty;
                alreadyInitialized = false;
                foreach (var header in _headers)
                {
                    var propertyPath = string.Join("/", header.Split(ConverterConfiguration.InnerChildSeparator));
                    var headerValue = rowElement?.XPathSelectElement(propertyPath)?.Value;
                    row += getPropertyValue(headerValue);
                    alreadyInitialized = true;
                }
                builder.AppendLine(row);
            }

            return builder.ToString();

            string getPropertyValue(string? propertyValue) => !alreadyInitialized ? $"{propertyValue}" : $"{ConverterConfiguration.Separator}{propertyValue}";
        }
    }
}
