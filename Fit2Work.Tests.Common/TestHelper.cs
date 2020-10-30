using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;

namespace Fit2Work.Tests.Common {
    public static class TestHelper {
        public static string EncryptionKey = "68566D597133743677397A2443264629";
        public static string TestFileName => "file.csv";
        public static void ClearTestFilesAndFolders() {
            // Ensure watch and archive folders are empty of test files
            var archiveFolder = GetFullPath("Watch\\Archive");
            if (Directory.Exists(archiveFolder)) {
                Directory.Delete(archiveFolder, true);
            }
            var testFile = GetFullPath($"Watch\\{TestFileName}");
            if (File.Exists(testFile)) {
                File.Delete(testFile);
            }
        }

        public static string GetFullPath(string path) {
            var root = GetPathToCurrentTest();
            var fullPath = Path.Combine(root, path);

            var workingDirectory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(Path.GetFullPath(workingDirectory))) {
                Directory.CreateDirectory(Path.GetFullPath(workingDirectory));
            }

            return fullPath;
        }

        /// <summary>
        /// Get the path to the current unit testing project as this can change on build server
        /// </summary>
        public static string GetPathToCurrentTest() {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);
            if (!folderAssembly.EndsWith("\\")) {
                folderAssembly += "\\";
            }
            return folderAssembly;
        }

        public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(dbSet.Object);
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
            return dbSet.Object;
        }

        public static List<string> MockResourcePlaceHolders {
            get {
                return new List<string>() {
                    "{header}",
                    "{footer}",
                    "{clientName}",
                    "{memberCode}",
                    "{firstName}",
                    "{lastName}",
                    "{phoneNumber}"};
            }
        }

        public static object GetReflectedProperty(object obj, string propertyName) {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property == null) {
                return null;
            }
            return property.GetValue(obj, null);
        }
    }
}
