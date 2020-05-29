using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Quibble.Xunit.Examples.CSharp
{
    static class CompareNumbers
    {
        public static void Compare1And2()
        {
            AssertExample.JsonEqual("Number example: 1 != 2", "1", "2");
        }
        
        public static void CompareDouble1AndInteger1()
        {
            AssertExample.JsonEqual("Number example: 1.0 == 1", "1.0", "1");
        }

        public static IEnumerable<Action> Enumerate()
        {
            yield return Compare1And2;
            yield return CompareDouble1AndInteger1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            foreach (var example in CompareNumbers.Enumerate())
            {
                try
                {
                    Console.WriteLine("Running example.");
                    example();
                    Console.WriteLine("Completed successfully.");
                }
                catch (XunitException ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine();
            }
        }
    }
}