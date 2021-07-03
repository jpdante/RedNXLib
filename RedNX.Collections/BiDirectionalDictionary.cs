﻿using System.Collections.Generic;

namespace RedNX.Collections {
    public class BiDirectionalDictionary<T1, T2> {
        
        private readonly Dictionary<T1, T2> _forward;
        private readonly Dictionary<T2, T1> _reverse;

        public BiDirectionalDictionary() {
            _forward = new Dictionary<T1, T2>();
            _reverse = new Dictionary<T2, T1>();
        }

        public void Add(T1 t1, T2 t2) {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1) {
            var t2 = _forward[t1];
            _forward.Remove(t1);
            _reverse.Remove(t2);
        }

        public void Remove(T2 t2) {
            var t1 = _reverse[t2];
            _reverse.Remove(t2);
            _forward.Remove(t1);
        }

        public void Clear() {
            _forward.Clear();
            _reverse.Clear();
        }

        public IReadOnlyDictionary<T1, T2> Forward => _forward;
        public IReadOnlyDictionary<T2, T1> Reverse => _reverse;

    }
}