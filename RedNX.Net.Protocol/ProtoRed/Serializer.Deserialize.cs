using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedNX.Net.Protocol.ProtoRed {
    partial class Serializer {

        private void InitDeserializer() {
            _typeDeserializerDictionary.Add(typeof(byte), DeserializeByte);
            _typeDeserializerDictionary.Add(typeof(sbyte), DeserializeSByte);
            _typeDeserializerDictionary.Add(typeof(char), DeserializeChar);
            _typeDeserializerDictionary.Add(typeof(short), DeserializeInt16);
            _typeDeserializerDictionary.Add(typeof(ushort), DeserializeUInt16);
            _typeDeserializerDictionary.Add(typeof(int), DeserializeInt32);
            _typeDeserializerDictionary.Add(typeof(uint), DeserializeUInt32);
            _typeDeserializerDictionary.Add(typeof(long), DeserializeInt64);
            _typeDeserializerDictionary.Add(typeof(ulong), DeserializeUInt64);
            _typeDeserializerDictionary.Add(typeof(float), DeserializeFloat);
            _typeDeserializerDictionary.Add(typeof(double), DeserializeDouble);
            _typeDeserializerDictionary.Add(typeof(decimal), DeserializeDecimal);
            _typeDeserializerDictionary.Add(typeof(bool), DeserializeBool);
            _typeDeserializerDictionary.Add(typeof(string), DeserializeString);
        }

        public async Task<T> Deserialize<T>(byte[] buffer, CancellationToken cancellation = default(CancellationToken)) {
            await using var memoryStream = new MemoryStream(buffer);
            return await Deserialize<T>(memoryStream, cancellation);
        }

        public async Task<T> Deserialize<T>(Stream stream, CancellationToken cancellation = default(CancellationToken)) {
            using var binaryReader = new BinaryReader(stream, Encoding.UTF8, true);
            uint classId = binaryReader.ReadUInt32();
            if (!_typeDictionary.Forward.TryGetValue(classId, out var classType)) {
                throw new KeyNotFoundException($"Class id '{classId}' not found.");
            }
            if (classType != typeof(T)) {
                throw new InvalidCastException($"Failed to convert '{classType.FullName}' to '{typeof(T).FullName}'");
            }
            return (T) await DeserializeClass(classType, binaryReader, cancellation);
        }

        private async Task<object> DeserializeClass(Type type, BinaryReader binaryReader, CancellationToken cancellation) {
            var typeDictionary = new Dictionary<ushort, Tuple<Type, PropertyInfo>>();
            PropertyInfo[] props = type.GetProperties();
            foreach (var prop in props) {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs) {
                    if (attr is not ProtoField protoField) continue;
                    var propertyType = prop.PropertyType.IsArray ? prop.PropertyType.GetElementType() : prop.PropertyType;
                    typeDictionary.Add( protoField.Index, new Tuple<Type, PropertyInfo>(propertyType, prop));
                }
            }
            object classInstance = Activator.CreateInstance(type);
            while (!cancellation.IsCancellationRequested) {
                ushort fieldIndex = binaryReader.ReadUInt16();
                // Finished reading parameters
                if (fieldIndex == ushort.MaxValue) return classInstance;

                byte fieldType = binaryReader.ReadByte();
                var (propertyType, propertyInfo) = typeDictionary[fieldIndex];

                Func<BinaryReader, Task<object>> fieldDeserializer;

                if (_typeDeserializerDictionary.ContainsKey(propertyType)) {
                    fieldDeserializer = _typeDeserializerDictionary[propertyType];
                } else {
                    uint classId = binaryReader.ReadUInt32();
                    if (!_typeDictionary.Forward.TryGetValue(classId, out var classType)) {
                        throw new KeyNotFoundException($"Class id '{classId}' not found.");
                    }
                    fieldDeserializer = (reader) => DeserializeClass(classType, reader, cancellation);
                }

                object data = null;
                if (fieldType == 0x1) {
                    data = await DeserializeArray(propertyType, binaryReader, fieldDeserializer);
                } else {
                    data = await fieldDeserializer(binaryReader);
                }
                propertyInfo.SetValue(classInstance, data);
            }
            return classInstance;
        }

        private static Task<object> DeserializeByte(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadByte());
        }

        private static Task<object> DeserializeSByte(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadSByte());
        }

        private static Task<object> DeserializeChar(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadChar());
        }

        private static Task<object> DeserializeInt16(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadInt16());
        }

        private static Task<object> DeserializeUInt16(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadUInt16());
        }

        private static Task<object> DeserializeInt32(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadInt32());
        }

        private static Task<object> DeserializeUInt32(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadUInt32());
        }

        private static Task<object> DeserializeInt64(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadInt64());
        }

        private static Task<object> DeserializeUInt64(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadUInt64());
        }

        private static Task<object> DeserializeFloat(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadSingle());
        }

        private static Task<object> DeserializeDouble(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadDouble());
        }

        private static Task<object> DeserializeDecimal(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadDecimal());
        }

        private static Task<object> DeserializeBool(BinaryReader binaryReader) {
            return Task.FromResult((object) binaryReader.ReadBoolean());
        }

        private static Task<object> DeserializeString(BinaryReader binaryReader) {
            int length = binaryReader.ReadInt32();
            byte[] buffer = binaryReader.ReadBytes(length);
            return Task.FromResult((object) Encoding.UTF8.GetString(buffer, 0, length));
        }

        private static async Task<Array> DeserializeArray(Type type, BinaryReader binaryReader, Func<BinaryReader, Task<object>> fieldDeserializer) {
            int count = binaryReader.ReadInt32();
            var array = Array.CreateInstance(type, count);
            for (var i = 0; i < count; i++) {
                array.SetValue(await fieldDeserializer(binaryReader), i);
            }
            return array;
        }
    }
}
