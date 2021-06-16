using System;
using System.Collections.Generic;
using System.IO;

namespace RedNX.IO {
    public static class PathExt {

        private static readonly string TempPath;
        private static readonly string ProgramDataPath;
        private static readonly string LogPath;
        private static readonly string ConfigPath;
        private static readonly string LockFilePath;
        private static readonly string CachePath;

        static PathExt() {
            TempPath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "tmp") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "tmp");
            ProgramDataPath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "var", "share") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "var", "share");
            LogPath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "var", "log") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "var", "log");
            ConfigPath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "etc") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "etc");
            LockFilePath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "var", "lock") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "var", "lock");
            CachePath = Environment.OSVersion.Platform == PlatformID.Unix ? Path.Combine("/", "var", "cache") : Path.Combine(Directory.GetCurrentDirectory(), "fs_str", "var", "cache");
            EnsureFolders();
        }

        private static void EnsureFolders() {
            if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
            if (!Directory.Exists(ProgramDataPath)) Directory.CreateDirectory(ProgramDataPath);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!Directory.Exists(LockFilePath)) Directory.CreateDirectory(LockFilePath);
            if (!Directory.Exists(CachePath)) Directory.CreateDirectory(CachePath);
        }

        public static string GetTempPath() => TempPath;
        public static string GetTempPath(params string[] paths) => GetPath(TempPath, false, paths);
        public static string GetTempPath(bool create, params string[] paths) => GetPath(TempPath, create, paths);

        public static string GetProgramDataPath() => ProgramDataPath;
        public static string GetProgramDataPath(params string[] paths) => GetPath(ProgramDataPath, false, paths);
        public static string GetProgramDataPath(bool create, params string[] paths) => GetPath(ProgramDataPath, create, paths);

        public static string GetLogPath() => LogPath;
        public static string GetLogPath(params string[] paths) => GetPath(LogPath, false, paths);
        public static string GetLogPath(bool create, params string[] paths) => GetPath(LogPath, create, paths);

        public static string GetConfigPath() => ConfigPath;
        public static string GetConfigPath(params string[] paths) => GetPath(ConfigPath, false, paths);
        public static string GetConfigPath(bool create, params string[] paths) => GetPath(ConfigPath, create, paths);

        public static string GetLockFilePath() => LockFilePath;
        public static string GetLockFilePath(params string[] paths) => GetPath(LockFilePath, false, paths);
        public static string GetLockFilePath(bool create, params string[] paths) => GetPath(LockFilePath, create, paths);

        public static string GetCachePath() => CachePath;
        public static string GetCachePath(params string[] paths) => GetPath(CachePath, false, paths);
        public static string GetCachePath(bool create, params string[] paths) => GetPath(CachePath, create, paths);

        public static string GetPath(string basePath, bool create = false, params string[] paths) {
            var tmpPaths = new List<string>(paths);
            tmpPaths.Insert(0, basePath);
            string path = Path.Combine(tmpPaths.ToArray());
            if (create && !Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
