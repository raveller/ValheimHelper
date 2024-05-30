using Xunit.Abstractions;

namespace ValheimHelper.Tests
{
    public class EndianTest
    {
        private readonly ITestOutputHelper _output;

        public EndianTest(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void DisplayByteEndianness()
        {
            int x = 0x00000223;

            var bytes = BitConverter.GetBytes(x);

            _output.WriteLine(BitConverter.ToString(bytes));
        }


    }
}