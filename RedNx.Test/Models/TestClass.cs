using RedNX.Net.Protocol.ProtoRed;
using RedNx.Test.Models;

namespace RedNx.Test {
    public class TestClass {

        [ProtoField(0)]
        public string Test0 { get; private set; }

        [ProtoField(1)]
        public TestSubClass Test1 { get; private set; }

        public TestClass() {
            Test0 = "Test 0";
            Test1 = new TestSubClass();
        }

    }
}