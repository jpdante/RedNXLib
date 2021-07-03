using System;

namespace RedNX.Net.Protocol.ProtoRed {
    [AttributeUsage(AttributeTargets.Property)] 
    public class ProtoField : Attribute {

        public ushort Index { get; init; }

        public ProtoField(ushort index) {
            Index = index;
        }

    }
}