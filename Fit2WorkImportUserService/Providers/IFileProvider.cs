using System;
using System.IO;

namespace Fit2WorkImportUserService.Providers {
    public interface IFileProvider {

        void Archive(string file, string archiveFolder);

        IDisposable Open(string path, FileMode mode, FileAccess access, FileShare share);

        void DeleteFile(string path);
    }
}