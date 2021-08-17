using System.Collections.Generic;

namespace RedNX.Application {
    public readonly struct ArgsValues {

        public readonly List<string> _values;

        public ArgsValues(string value) {
            _values = new List<string> { value };
        }

        internal void AddValue(string value) {
            _values.Add(value);
        }

        public string this[int index] => _values[index];

        public int Count => _values.Count;

        public int IndexOf(string value) => _values.IndexOf(value);

        public bool Contains(string value) => _values.Contains(value);

        public string[] ToArray() => _values.ToArray();

        public override string ToString() {
            return _values.Count switch {
                0 => null,
                1 => _values[0],
                _ => string.Join(",", _values)
            };
        }
    }
}