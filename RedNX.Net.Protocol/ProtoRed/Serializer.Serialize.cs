using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedNX.Net.Protocol.ProtoRed {
    partial class Serializer {

        private void InitSerializer() {
            _typeSerializerDictionary.Add(typeof(byte), SerializeByte);
            _typeSerializerDictionary.Add(typeof(sbyte), SerializeSByte);
            _typeSerializerDictionary.Add(typeof(char), SerializeChar);
            _typeSerializerDictionary.Add(typeof(short), SerializeInt16);
            _typeSerializerDictionary.Add(typeof(ushort), SerializeUInt16);
            _typeSerializerDictionary.Add(typeof(int), SerializeInt32);
            _typeSerializerDictionary.Add(typeof(uint), SerializeUInt32);
            _typeSerializerDictionary.Add(typeof(long), SerializeInt64);
            _typeSerializerDictionary.Add(typeof(ulong), SerializeUInt64);
            _typeSerializerDictionary.Add(typeof(float), SerializeFloat);
            _typeSerializerDictionary.Add(typeof(double), SerializeDouble);
            _typeSerializerDictionary.Add(typeof(decimal), SerializeDecimal);
            _typeSerializerDictionary.Add(typeof(bool), SerializeBool);
            _typeSerializerDictionary.Add(typeof(string), SerializeString);
        }

        public async Task<byte[]> Serialize(object obj) {
            await using var memoryStream = new MemoryStream();
            await Serialize(obj, memoryStream);
            return memoryStream.ToArray();
        }

        public async Task Serialize(object obj, Stream stream) {
            if (!_typeDictionary.Reverse.TryGetValue(obj.GetType(), out uint classId)) {
                throw new KeyNotFoundException($"Type '{obj.GetType().FullName}' not found.");
            }
            await using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true);
            await SerializeClass(obj, binaryWriter);
        }

        private async Task SerializeClass(object obj, BinaryWriter binaryWriter) {
            if (!_typeDictionary.Reverse.TryGetValue(obj.GetType(), out uint classId)) {
                throw new KeyNotFoundException($"Type '{obj.GetType().FullName}' not found.");
            }
            binaryWriter.Write(classId);
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (var prop in props) {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs) {
                    if (attr is not ProtoField protoField) continue;
                    await SerializeField(obj, protoField.Index, prop, binaryWriter);
                }
            }
            binaryWriter.Write(ushort.MaxValue);
        }

        private async Task SerializeField(object obj, ushort fieldIndex, PropertyInfo propertyInfo, BinaryWriter binaryWriter) {
            binaryWriter.Write(fieldIndex);
            if (propertyInfo.PropertyType.IsArray) {
                binaryWriter.Write((byte) 0x1);
                await SerializeArray((object[]) propertyInfo.GetValue(obj), binaryWriter);
            } else {
                if (!_typeSerializerDictionary.TryGetValue(propertyInfo.PropertyType, out Func<object, BinaryWriter, Task> func)) {
                    throw new KeyNotFoundException($"Type '{propertyInfo.PropertyType.FullName}' not registered.");
                }
                binaryWriter.Write((byte) 0x0);
                await func(propertyInfo.GetValue(obj), binaryWriter);
            }
        }

        public Func<object, BinaryWriter, Task> GetSerializationFunc(object obj) {
            if (!_typeSerializerDictionary.TryGetValue(obj.GetType(), out Func<object, BinaryWriter, Task> func)) {
                throw new KeyNotFoundException($"Type '{obj.GetType().FullName}' not registered.");
            }
            return func;
        }

        private static Task SerializeByte(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((byte) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeSByte(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((sbyte) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeChar(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((char) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeInt16(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((short) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeUInt16(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((ushort) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeInt32(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((int) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeUInt32(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((uint) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeInt64(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((long) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeUInt64(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((ulong) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeFloat(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((float) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeDouble(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((double) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeDecimal(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((decimal) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeBool(object obj, BinaryWriter binaryWriter) {
            binaryWriter.Write((bool) obj);
            return Task.CompletedTask;
        }

        private static Task SerializeString(object obj, BinaryWriter binaryWriter) {
            byte[] buffer = Encoding.UTF8.GetBytes((string) obj);
            binaryWriter.Write(buffer.Length);
            binaryWriter.Write(buffer);
            return Task.CompletedTask;
        }

        private async Task SerializeArray(IReadOnlyCollection<object> obj, BinaryWriter binaryWriter) {
            binaryWriter.Write(obj.Count);
            foreach (object row in obj) {
                await GetSerializationFunc(row)(row, binaryWriter);
            }
        }
    }
}
