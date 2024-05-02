using System;

class Tester
{
    static void Main(string[] args)
    {
        string[] arrayTest = new string[] { "One", "Two" }

        Collection<string> collectionTest = new Collection<string>() { "One", "Two"}

        Dictionary<uint, string> dictionaryTest = new Dictionary<uint, string>();
        dictionaryTest.Add(1, "One");
        dictionaryTest.Add(2, "Two");

        Reflector.Get(arrayTest, null, "[1]", out int value);
        Console.WriteLine(value);
    }
}