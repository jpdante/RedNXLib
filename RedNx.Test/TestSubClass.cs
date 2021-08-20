using RedNX.Net.Protocol.ProtoRed;

namespace RedNx.Test {
    public class TestSubClass {
        
        [ProtoField(0)]
        public string Test0 { get; private set; }

        [ProtoField(1)]
        public string[] Test1 { get; private set; }

        public TestSubClass() {
            Test0 = "Test 0";
            Test1 = new [] {
                "Test 1",
                "Test 2",
                "Test 3",
                "Test 4",
                "Test 5"
            };
        }

    }
}