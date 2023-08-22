using System;

namespace ApiTests
{
    internal class TestConfig : IDisposable
    {
        public TestConfig()
        {
            Environment.SetEnvironmentVariable("PASSWORD_HASH", "TesteHash");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("PASSWORD_HASH", null);
        }
    }
}
