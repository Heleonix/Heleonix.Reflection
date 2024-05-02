using Heleonix.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Test
{
#pragma warning disable CS1591

    public class TestData
    {
        public static string[] ArrayTest => ["One", "Two"];

        public static Collection<string> CollectionTest = ["One", "Two"];

        public static Dictionary<int, string> IntDictionaryTest = new()
        {
            { 3, "Three" },
            { 4, "Four" }
        };

        public static Dictionary<string, string> StringDictionaryTest = new()
        {
            { "Three", "Three" },
            { "Four", "Four" }
        };

    }

#pragma warning restore CS1591

    class Program
    {
        static void Main()
        {
            TestData testData = new();            

            if (Reflector.Get(testData, null, "ArrayTest[0]", out string arrayValue0)) Console.WriteLine("Array Index 0: " + arrayValue0);
            if (Reflector.Get(testData, null, "ArrayTest[1]", out string arrayValue1)) Console.WriteLine("Array Index 1: " + arrayValue1);
            if (Reflector.Get(testData, null, "ArrayTest[2]", out string arrayValue2)) Console.WriteLine("Array Index 2: " + arrayValue2);

            if (Reflector.Get(testData, null, "CollectionTest[0]", out string collectionValue0)) Console.WriteLine("Collection Index 0: " + collectionValue0);
            if (Reflector.Get(testData, null, "CollectionTest[1]", out string collectionValue1)) Console.WriteLine("Collection Index 1: " + collectionValue1);
            
            if (Reflector.Get(testData, null, "IntDictionaryTest[0]", out string intDictionaryValue0)) Console.WriteLine("Dictionary Int Index 0: " + intDictionaryValue0);
            if (Reflector.Get(testData, null, "IntDictionaryTest[1]", out string intDictionaryValue1)) Console.WriteLine("Dictionary Int Index 1: " + intDictionaryValue1);
            if (Reflector.Get(testData, null, "IntDictionaryTest[2]", out string intDictionaryValue2)) Console.WriteLine("Dictionary Int Index 2: " + intDictionaryValue2);
            if (Reflector.Get(testData, null, "IntDictionaryTest[3]", out string intDictionaryValue3)) Console.WriteLine("Dictionary Int Index 3: " + intDictionaryValue3);
            if (Reflector.Get(testData, null, "IntDictionaryTest[4]", out string intDictionaryValue4)) Console.WriteLine("Dictionary Int Index 4: " + intDictionaryValue4);

            if (Reflector.Get(testData, null, "StringDictionaryTest[Zero]", out string stringDictionaryValue0)) Console.WriteLine("Dictionary String Index 0: " + stringDictionaryValue0);
            if (Reflector.Get(testData, null, "StringDictionaryTest[One]", out string stringDictionaryValue1)) Console.WriteLine("Dictionary String Index 1: " + stringDictionaryValue1);
            if (Reflector.Get(testData, null, "StringDictionaryTest[Two]", out string stringDictionaryValue2)) Console.WriteLine("Dictionary String Index 2: " + stringDictionaryValue2);
            if (Reflector.Get(testData, null, "StringDictionaryTest[Three]", out string stringDictionaryValue3)) Console.WriteLine("Dictionary String Index 3: " + stringDictionaryValue3);
            if (Reflector.Get(testData, null, "StringDictionaryTest[Four]", out string stringDictionaryValue4)) Console.WriteLine("Dictionary String Index 4: " + stringDictionaryValue4);
        }
    }
}

