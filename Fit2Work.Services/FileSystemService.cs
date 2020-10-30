using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilGroup.Services.Fit2Work.Services {
    public class FileSystemService : IFileSystemService {

        /// <summary>
        /// Helper method to get a stream from a path for parsing csvs
        /// </summary>
        /// <param name="path">The path to the text file, must have csv extension</param>
        /// <returns>A csv stream</returns>
        /// <remarks>Don't forget to wrap this in a using statement</remarks>
        public StreamReader GetCSVReaderFromPath(string path) {

            // Validate the file exists and the extension is CSV
            if (!File.Exists(path) || !Path.GetExtension(path).ToUpper().Equals(".CSV")) {
                throw new ApplicationException($"File '{path}' does not exist or is not a CSV.");
            }

            return new StreamReader(path);

        }
    }
}
