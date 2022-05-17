using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv.App.Models
{
    public class CsvConverterConfiguration
    {
        public string Separator { get; set; }
        public string InnerChildSeparator { get; set; }

        public CsvConverterConfiguration()
        {
            //default separators
            Separator = ",";
            InnerChildSeparator = "_";
        }
    }
}
