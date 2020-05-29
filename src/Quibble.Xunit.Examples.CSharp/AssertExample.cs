using System;

namespace Quibble.Xunit.Examples.CSharp
{
    static class AssertExample
    {
        public static void JsonEqual(string exampleName, string expectedJsonString, string actualJsonString)
        {
            Console.WriteLine(exampleName);
            Assert.JsonEqual(expectedJsonString, actualJsonString);
        }
    }
}