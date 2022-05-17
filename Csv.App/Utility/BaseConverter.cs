using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv.App.Utility
{
    public abstract class BaseConverter<TDataSource>
    {

        private async Task<string> ValidateFileAndReadDataAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Could not find file '{filePath}'.");
            }

            var fileData = await File.ReadAllTextAsync(filePath);

            if (fileData.Length <= 0)
            {
                throw new ArgumentException($"The file '{filePath}' does not contain any data.");
            }

            return fileData;
        }

        protected abstract TDataSource PreProcessDataSource(string fileData);
        public async Task<string> ConvertAsync(string sourceFilePath)
        {
            var fileData = await ValidateFileAndReadDataAsync(sourceFilePath);

            var dataSource = PreProcessDataSource(fileData);
            
            return ConvertData(dataSource);
        }

        public async Task ConvertAndWriteToFileAsync(string sourceFilePath, string destinationFilePath)
        {
            var convertedData = await ConvertAsync(sourceFilePath);
            await File.WriteAllTextAsync(destinationFilePath, convertedData);
        }

        protected abstract string ConvertData(TDataSource dataSource);
    }
}
