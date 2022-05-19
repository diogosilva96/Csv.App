using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Csv.App.Utility
{
    public class JsonToXmlConverter : BaseConverter<JsonArray?> 
    {
        public JsonToXmlConverter() : base(new (@"^.*\.([Jj][Ss][Oo][Nn])$"))
        {

        }
        protected override JsonArray? PreProcessDataSource(string fileData)
        {
            return JsonNode.Parse(fileData)?.AsArray();
        }

        protected override string ConvertData(JsonArray? jsonData)
        {
            
            XElement root = new ("data");
            var index = 0;
            foreach (var node in jsonData)
            {
                XElement row = new ("row");
                row.Add(new XAttribute("number",index++));
                foreach (var (key, value) in node?.AsObject())
                {
                    var propertyPath = value switch
                                            {
                                                JsonObject jsonObject => BuildChildPropertyValues(jsonObject),
                                                _ => new(key, value?.ToString() ?? string.Empty)
                                            };
                    row.Add(propertyPath);
                }
                root.Add(row);
            }

            return root.ToString();
        }

        private XElement BuildChildPropertyValues(JsonObject jsonObject)
        {
            XElement root = new (jsonObject.GetPath().Split(".").Last());
            return findProperties(jsonObject, root);

            XElement findProperties(JsonObject jsonObject, XElement currentElement)
            {
                foreach (var node in jsonObject)
                {
                    XElement element;
                    switch (node.Value)
                    {
                        case JsonValue:
                            element = new (node.Key, node.Value.ToString());
                            currentElement.Add(element);
                            break;
                        case JsonObject:
                            element = new(node.Key);
                            currentElement.Add(element);
                            findProperties(node.Value.AsObject(),element); 
                            break;
                    }
                }
                return currentElement;
            }
        }
    }
}
