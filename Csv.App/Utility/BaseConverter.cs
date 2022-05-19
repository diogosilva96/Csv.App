using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Csv.App.Utility
{
    public abstract class BaseConverter<TDataSource> : IFileConverter
    {
        protected Regex? InputFilePathRegex;

        public BaseConverter(Regex? inputFilePathRegex = null)
        {
            InputFilePathRegex = inputFilePathRegex;
        }
        private async Task<string> ValidateFileAndReadDataAsync(string filePath)
        {
            if (InputFilePathRegex is not null)
            {
                var match = InputFilePathRegex.Match(filePath);
                if (!match.Success)
                {
                    throw new ArgumentException($"The file extension is invalid for the path '{filePath}'");
                }
            }
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

        public async Task<string> ConvertFromFileAsync(string sourceFilePath)
        {
            var fileData = await ValidateFileAndReadDataAsync(sourceFilePath);
            return Convert(fileData);
        }

        public string Convert(string fileData)
        {
            var dataSource = PreProcessDataSource(fileData);
            return ConvertData(dataSource);
        }

        public async Task ConvertAndWriteToFileAsync(string sourceFilePath, string destinationFilePath)
        {
            var convertedData = await ConvertFromFileAsync(sourceFilePath);
            await File.WriteAllTextAsync(destinationFilePath, convertedData);
        }

        protected abstract string ConvertData(TDataSource dataSource);
    }
}
