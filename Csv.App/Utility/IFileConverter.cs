using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv.App.Utility
{
    public interface IFileConverter
    {
        public Task ConvertAndWriteToFileAsync(string sourceFilePath, string destinationFilePath);
        public Task<string> ConvertFromFileAsync(string sourceFilePath);
        public string Convert(string fileData);
    }
}
