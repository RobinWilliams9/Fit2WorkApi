using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilGroup.Services.Fit2Work.Services {
    public interface IFileSystemService {
        StreamReader GetCSVReaderFromPath(string path);
    }
}
