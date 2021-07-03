using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RedNX.Collections;

namespace RedNX.Net.Protocol.ProtoRed {
    public partial class Serializer {

        private readonly BiDirectionalDictionary<uint, Type> _typeDictionary;
        private readonly Dictionary<Type, Func<object, BinaryWriter, Task>> _typeSerializerDictionary;
        private readonly Dictionary<Type, Func<BinaryReader, Task<object>>> _typeDeserializerDictionary;
        private uint _count;

        public Serializer() {
            _typeDictionary = new BiDirectionalDictionary<uint, Type>();
            _typeSerializerDictionary = new Dictionary<Type, Func<object, BinaryWriter, Task>>();
            _typeDeserializerDictionary = new Dictionary<Type, Func<BinaryReader, Task<object>>>();
            _count = 0;
            InitSerializer();
            InitDeserializer();
        }

        public void AddType(Type type) {
            _typeDictionary.Add(_count, type);
            _typeSerializerDictionary.Add(type, SerializeClass);
            _count++;
        }

        public void ResetTypes() {
            _typeDictionary.Clear();
            _typeSerializerDictionary.Clear();
            _typeDeserializerDictionary.Clear();
            _count = 0;
            InitSerializer();
            InitDeserializer();
        }

    }
}