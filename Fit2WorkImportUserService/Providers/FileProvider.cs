using System;
using System.IO;

namespace Fit2WorkImportUserService.Providers {
    public class FileProvider : IFileProvider {
        public void Archive(string file, string archiveFolder) {
            if (!File.Exists(file)) {
                return;
            }
            // Generate name and path for archived file
            string ext = Path.GetExtension(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string newFileName = $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HH_mm_ss}.{ext}";
            string archiveFile = Path.Combine(archiveFolder, newFileName);
            // Create archive folder if it does not exist
            if (!Directory.Exists(archiveFolder)) {
                Directory.CreateDirectory(archiveFolder);
            }

            File.Copy(file, archiveFile);

        }

        public void DeleteFile(string path) {
            if (!File.Exists(path)) {
                return;
            }

            File.Delete(path);
        }

        public IDisposable Open(string path, FileMode mode, FileAccess access, FileShare share) {
            return File.Open(path, mode, access, share);
        }
    }
}
