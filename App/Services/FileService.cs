namespace fvm.Services
{
    public static class FileService
    {
        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                return;

            foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(sourceDir, targetDir));

            foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(sourceDir, targetDir), true);
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static List<string> GetAllFiles(string directory)
        {
            if (!Directory.Exists(directory))
                return new List<string>();

            return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(directory, f))
                .ToList();
        }

        public static List<string> GetDirectories(string directory)
        {
            if (!Directory.Exists(directory))
                return new List<string>();

            return Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)
                .Select(d => Path.GetFileName(d))
                .ToList();
        }
    }
}
