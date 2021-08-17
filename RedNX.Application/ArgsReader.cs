using RedNX.System;
using System.Collections.Generic;

namespace RedNX.Application {
    public class ArgsReader {

        private readonly Dictionary<string, ArgsValues> _argsDictionary = new();
        private readonly List<string> _extraArgs = new();

        public readonly string KeyIdentifier = RedEnvironment.OperatingSystem == SystemOS.Windows ? "/" : "-";

        public IReadOnlyDictionary<string, ArgsValues> ArgsDictionary => _argsDictionary;
        public IReadOnlyList<string> ExtraArgs => _extraArgs;

        public ArgsReader() { }

        public ArgsReader(IReadOnlyList<string> args) {
            Load(args);
        }

        public void Load(IReadOnlyList<string> args) {
            var position = 0;
            while (position < args.Count) {
                string current = args[position];
                if (current.StartsWith(KeyIdentifier)) {
                    if (position + 1 < args.Count) {
                        string key = current.Remove(current.Length - 1, 1);
                        string value = args[position + 1];
                        if (!value.StartsWith(KeyIdentifier)) {
                            if (_argsDictionary.ContainsKey(key)) _argsDictionary[key].AddValue(value);
                            else _argsDictionary.Add(current.Remove(current.Length - 1, 1), new ArgsValues(value));
                            position++;
                        } else {
                            if (_argsDictionary.ContainsKey(key)) _argsDictionary[key].AddValue("1");
                            else _argsDictionary.Add(current.Remove(current.Length - 1, 1), new ArgsValues("1"));
                        }
                    }
                } else {
                    _extraArgs.Add(current);
                }
                position++;
            }
        }

        public void Clear() {
            _argsDictionary.Clear();
            _extraArgs.Clear();
        }

        public ArgsValues? Get(string key) {
            if (_argsDictionary.TryGetValue(key, out var value)) return value;
            return null;
        }

        public ArgsValues GetOrDefault(string key, ArgsValues defaultValue) {
            return _argsDictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public bool TryGet(string key, out ArgsValues value) {
            return _argsDictionary.TryGetValue(key, out value);
        }
    }
}