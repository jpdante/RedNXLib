using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RedNX.Application;
using RedNX.Caching;

namespace RedNx.Test.Tests {
    public class TestFileCache : ConsoleApplication {
        public void Run() {
            Run(Array.Empty<string>());
        }

        private FileCache _fileCache;

        protected override Task OnLoad() {
            var path = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FileCache"));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var config = new FileCacheConfig {
                CachePath = Path.Combine(path, "files"),
                DatabaseFileName = Path.Combine(path, "caching.db"),
                MaxAvailableSpace = 30 * 1024 * 1024,
                RemoveMissingFilesAtInit = true,
                DeleteUnknownFilesAtInit = true
            };
            _fileCache = new FileCache(config);
            return Task.CompletedTask;
        }

        protected override Task OnStart() {
            while (true) {
                Console.WriteLine("\n1- Add file");
                Console.WriteLine("2- Get file");
                Console.WriteLine("3- Remove file");
                Console.WriteLine("4- Contains file");
                Console.WriteLine("5- Release");
                Console.WriteLine("6- Clear");
                Console.WriteLine("7- Info");
                Console.WriteLine("9- Exit");
                Console.Write(">");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                string id;
                switch (line) {
                    case "1": {
                        Console.Write("id >");
                        id = Console.ReadLine();
                        if (string.IsNullOrEmpty(id)) continue;
                        Console.Write("file >");
                        var file = Console.ReadLine();
                        if (string.IsNullOrEmpty(file)) continue;
                        file = file.Replace("\"", "");
                        if (string.IsNullOrEmpty(file)) continue;
                        using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Console.WriteLine(_fileCache.AddFile(id, 2, WritableCacheStream => {
                            fileStream.CopyToAsync(WritableCacheStream);
                        }) ? $"{id} added." : $"Failed to add {id}.");
                        break;
                    }
                    case "2":
                        Console.Write("id >");
                        id = Console.ReadLine();
                        if (string.IsNullOrEmpty(id)) continue;
                        if (!_fileCache.GetFile(id, stream => { Console.WriteLine($"Size: {stream.Length}"); }, out var ex)) {
                            Console.WriteLine($"Failed to get {id}");
                            if (ex != null) throw ex;
                        }

                        break;
                    case "3":
                        Console.Write("id >");
                        id = Console.ReadLine();
                        if (string.IsNullOrEmpty(id)) continue;
                        Console.WriteLine(_fileCache.RemoveFile(id) ? $"{id} removed." : $"Failed to remove {id}.");
                        break;
                    case "4":
                        Console.Write("id >");
                        id = Console.ReadLine();
                        if (string.IsNullOrEmpty(id)) continue;
                        Console.WriteLine(_fileCache.ContainsFile(id) ? $"{id} exists." : $"{id} don't exists.");
                        break;
                    case "5":
                        //_fileCache.RequestSpaceRelease( 0 * 1024 * 1024, ReleaseMode.LargestSize);
                        _fileCache.RequestSpaceRelease(1, ReleaseMode.LargestSize);
                        break;
                    case "6":
                        Console.WriteLine("Cleaning...");
                        _fileCache.Clear();
                        Console.WriteLine("Cleared");
                        break;
                    case "7":
                        Console.WriteLine($"Used: {SizeSuffix(_fileCache.UsedSpace)} / {SizeSuffix(_fileCache.MaxAvailableSpace)}");
                        Console.WriteLine($"Free: {SizeSuffix(_fileCache.AvailableSpace)}");
                        break;
                    case "9":
                        Stop();
                        return Task.CompletedTask;
                }
            }
        }

        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        private static string SizeSuffix(long value, int decimalPlaces = 1) {
            if (decimalPlaces < 0) {
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }

            if (value < 0) {
                return "-" + SizeSuffix(-value, decimalPlaces);
            }

            if (value == 0) {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int) Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal) value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000) {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }
    }
}